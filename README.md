# CleanArchitecture.Full — Accounts API

API de gestión de cuentas bancarias construida en .NET 10 siguiendo **Clean Architecture** + **CQRS** (MediatR), con autenticación **JWT** basada en roles, persistencia en PostgreSQL, logging estructurado (Serilog + Seq), contenedores Docker y despliegue a Kubernetes (Minikube) vía Helm, con pipeline de CI/CD en GitHub Actions.

## Arquitectura

Solución (`CleanArchitecture.Full.slnx`) con 4 proyectos en `src/`:

| Proyecto | Responsabilidad |
|---|---|
| `CleanArchitecture.Full.Domain` | Entidades puras (`Account`, `Customer`, `Transaction`, `User`) e interfaces (repositorios, `IPasswordHasher`, `IJwtTokenGenerator`). Sin dependencias externas. |
| `CleanArchitecture.Full.Application` | Casos de uso con MediatR (Commands/Queries + Handlers), validaciones con FluentValidation, DTOs y mapeos. Autorización por dueño de recurso vía `ICurrentUser`. |
| `CleanArchitecture.Full.Infrastructure` | Acceso a datos con EF Core sobre PostgreSQL (Npgsql, snake_case), migraciones, repositorios, hash de contraseñas (PBKDF2). |
| `CleanArchitecture.Full.Api` | Minimal API, autenticación/emisión de JWT, middleware de manejo de errores, logging de requests, health checks, OpenAPI/Scalar. |

Flujo de una request: `Endpoint (Minimal API)` → `ISender.Send(Command/Query)` → `ValidationBehavior` (FluentValidation) → `Handler` (valida ownership vía `ICurrentUser` cuando aplica) → `Repository` → `AppDbContext` (PostgreSQL).

## Modelo de datos

- **`customers`**: cliente del banco (`document_type` + `document_number` únicos, `full_name`).
- **`accounts`**: pertenece a un `customer_id`; `account_number` se **genera automáticamente** al crear la cuenta (no lo elige el cliente); `currency` (`PYG` por defecto; también admite `ARS`/`USD`/`EUR`); `status` (`Active` / `Inactive` / `Closed`, con `closed_at` cuando se cierra).
- **`transactions`**: historial inmutable de movimientos por cuenta (`Deposit`, `Withdrawal`, `TransferOut`, `TransferIn`), con `balance_after` y, en transferencias, `related_account_id`.
- **`users`**: login de la API, con rol `Admin` o `Cliente`; un usuario `Cliente` está vinculado a un `customer_id` y solo puede operar sus propias cuentas.

## Autenticación y autorización (JWT)

- `POST /api/v1/auth/login` — con las credenciales de administrador (config `Auth:AdminUsername`/`Auth:AdminPassword`) devuelve un JWT con rol `Admin`; con un usuario registrado devuelve un JWT con rol `Cliente` y el `customerId` asociado como claim.
- `POST /api/v1/auth/register` — autoregistro de un cliente ya existente: requiere `customerId` + `documentNumber` (debe coincidir con el cliente) para crear su usuario. Devuelve un JWT igual que el login.
- Todos los endpoints de `accounts`, `customers` y `transfers` requieren `Authorization: Bearer <token>`.
- **Rol `Admin`**: acceso total (crear clientes/cuentas, actualizar, cerrar, activar/desactivar).
- **Rol `Cliente`**: solo puede leer, depositar, retirar y transferir sobre las cuentas que pertenecen a su propio `customerId`; cualquier intento sobre una cuenta ajena responde `403 Forbidden`.

## Endpoints

### Auth (`api/v1/auth`, públicos)

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/v1/auth/login` | `{ username, password }` → `{ token, username, role, customerId, expiresAtUtc }` (401 si las credenciales no son válidas) |
| `POST` | `/api/v1/auth/register` | `{ username, password, customerId, documentNumber }` → crea un usuario `Cliente` y devuelve su token (400 si el documento no coincide o el username ya existe) |

### Customers (`api/v1/customers`, requieren token)

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/v1/customers` | **Admin.** Crea un cliente (`documentType`: `DNI`/`Pasaporte`/`CUIT`, `documentNumber`, `fullName`). 409 si el documento ya existe |
| `GET` | `/api/v1/customers/{id}` | Admin, o el `Cliente` dueño de ese id (403 en otro caso) |

### Accounts (`api/v1/accounts`, requieren token)

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/v1/accounts` | Admin ve todas; `Cliente` ve solo las propias |
| `GET` | `/api/v1/accounts/{id}` | 404 si no existe, 403 si no es dueño |
| `GET` | `/api/v1/accounts/{id}/balance` | Saldo puntual (`accountId`, `accountNumber`, `balance`, `currency`) |
| `GET` | `/api/v1/accounts/{id}/transactions` | Historial de movimientos de la cuenta, más recientes primero |
| `POST` | `/api/v1/accounts` | **Admin.** `{ customerId, balance, currency }` → el `accountNumber` se asigna automáticamente |
| `PUT` | `/api/v1/accounts/{id}` | **Admin.** Solo permite corregir `currency` |
| `DELETE` | `/api/v1/accounts/{id}` | **Admin.** Cierre lógico (soft-close): marca `status=Closed` y `closedAt`, nunca borra la fila |
| `POST` | `/api/v1/accounts/{id}/deposit` | `{ amount }`. 400 si el monto es inválido o la cuenta no está `Active` |
| `POST` | `/api/v1/accounts/{id}/withdraw` | `{ amount }`. 400 si el monto es inválido, la cuenta no está `Active` o no hay saldo suficiente |
| `POST` | `/api/v1/accounts/{id}/activate` | **Admin.** Vuelve la cuenta `Active` (400 si estaba `Closed`: una cuenta cerrada no se reabre) |
| `POST` | `/api/v1/accounts/{id}/deactivate` | **Admin.** Marca `Inactive` (400 si estaba `Closed`) |

### Transfers (`api/v1/transfers`, requiere token)

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/v1/transfers` | Ver formato abajo. 404 si el ordenante o el beneficiario no existen por número de cuenta; 400 si los documentos no coinciden, ambas cuentas son la misma, alguna no está `Active`, la moneda no coincide con la de las cuentas, o no hay saldo suficiente |

Formato del body — el ordenante se identifica con **número de cuenta + documento** (verifica que sea el dueño), el beneficiario con **número de cuenta + tipo y número de documento** (verifica que coincida con el titular real):

```json
{
  "sender": { "accountNumber": "ACC-000001", "documentNumber": "27134863" },
  "beneficiary": { "accountNumber": "ACC-000002", "documentType": "DNI", "documentNumber": "40123456" },
  "amount": 1000,
  "currency": "PYG"
}
```

Documentación interactiva (OpenAPI vía Scalar) disponible en `/scalar` cuando la API corre en `Development`. El documento OpenAPI declara el esquema `Bearer` y lo exige automáticamente en cada operación protegida (no en `login`/`register`/health checks); en Scalar, abrí el panel **Authentication** (arriba a la derecha), pegá el token obtenido en `/api/v1/auth/login` una sola vez (sin la palabra `Bearer`) y Scalar lo va a mandar solo en cada "Try it" mientras dure la sesión (persiste en el navegador).

Health checks (públicos, sin token):
- `GET /health/live` — liveness del proceso.
- `GET /health/ready` — readiness, valida conexión a PostgreSQL.

### Errores

Todas las respuestas de error siguen el mismo formato JSON (`title`, `status`, y `errors`/`detail` según el caso), generado por el middleware de manejo de excepciones (`Middleware/ValidationExceptionMiddleware.cs`):

- **400** — falla de validación (FluentValidation), incluye `errors` agrupados por campo.
- **401** — token ausente/ inválido, o credenciales de login incorrectas.
- **403** — usuario autenticado sin permiso sobre el recurso (rol o dueño incorrecto).
- **404** — recurso no encontrado.
- **409** — conflicto de datos (número de cuenta, documento o username duplicado).
- **500** — error no controlado (sin exponer stack trace ni detalles internos).

## Correr localmente

### Con Docker Compose (recomendado)

Levanta PostgreSQL, Seq y la API:

```bash
docker compose up --build
```

- API: `http://localhost:8080`
- Seq (UI de logs): `http://localhost:5341` — primer login: `admin` / `123456`. Seq va a pedir fijar una contraseña nueva en ese momento (mínimo 8 caracteres, es una regla propia de Seq).
- PostgreSQL: `localhost:5432` (db `accountsdb`, user/pass `postgres`/`postgres`)

La primera vez, aplicar el esquema y datos semilla contra la base:

```bash
docker exec -i accounts-postgres psql -U postgres -d accountsdb < database/02_create_table_and_seed.sql
```

Esto crea, con montos en **guaraníes (PYG)**:

| Cliente | Documento | Cuenta | Balance | Estado |
|---|---|---|---|---|
| Lionel Messi | DNI 27134863 | `ACC-000001` | 120.000.000 | Active (con 3 movimientos de historial) |
| Julian Alvarez | DNI 40123456 | `ACC-000002` | 65.000.000 | Active (con 2 movimientos de historial) |
| Sebas Caballero | DNI 35987654 | `ACC-000003` | 0 | Inactive |
| Miguel Almiron | DNI 41234567 | `ACC-000004` | 45.000.000 | Active |
| Gustavo Gomez | DNI 34567890 | `ACC-000005` | 30.750.000 | Active |
| Angel Romero | DNI 23456789 | `ACC-000006` | 18.200.000 | Active |
| Roberto Fernandez | DNI 56789012 | `ACC-000007` | 0 | **Closed** (para probar el bloqueo de reactivación/operaciones) |

Además crea un usuario `Cliente` ya registrado (vinculado a Miguel Almiron) para probar el rol sin pasar por `/register`: usuario **`malmiron`**, contraseña **`Cliente123!`**.

Para probar la API, primero pedí un token (admin o el cliente de arriba):

```bash
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'
```

y usalo en el resto de las llamadas con `-H "Authorization: Bearer <token>"` (o pegalo una vez en el panel de autenticación de Scalar, ver más abajo).

### Sin Docker

```bash
dotnet restore CleanArchitecture.Full.slnx
dotnet build CleanArchitecture.Full.slnx
dotnet run --project src/CleanArchitecture.Full.Api
```

Requiere una instancia de PostgreSQL accesible según `ConnectionStrings:DefaultConnection` en `src/CleanArchitecture.Full.Api/appsettings.Development.json`, y opcionalmente Seq en `http://localhost:5341` para centralizar logs.

## Logging

Serilog escribe de forma asíncrona a consola y a Seq (`Seq:ServerUrl`, configurable por entorno). Se usa `Information` para operaciones normales, `Warning` para reglas de negocio "blandas", errores de cliente (validación, 4xx) e intentos de login fallidos, y `Error` para excepciones no controladas (5xx). Cada request queda enriquecida con `RequestId`, `ClientIp`, `RequestHost` y `UserAgent`.

## Despliegue a Kubernetes (Minikube)

1. Namespace + PostgreSQL + Seq (manifiestos crudos):

   ```bash
   kubectl apply -f k8s/00-namespace.yaml
   kubectl apply -f k8s
   ```

2. Esquema y datos semilla:

   ```bash
   kubectl exec --stdin deployment/postgres --namespace accounts -- \
     psql -v ON_ERROR_STOP=1 -U postgres -d accountsdb < database/02_create_table_and_seed.sql
   ```

3. API vía Helm:

   - Si la imagen ya está en Docker Hub:

     ```bash
     helm upgrade --install accounts-api ./helm/accounts-api \
       --namespace accounts --create-namespace \
       --set image.repository=docker.io/<tu-usuario>/accounts-api \
       --set image.tag=latest
     ```

   - Para probar en local sin publicar la imagen (lo que se usó para verificar este despliegue): construir con `docker compose build accounts-api`, cargarla al cluster con `minikube image load bootcampclt2026-main-accounts-api:latest` y desplegar apuntando a esa imagen con `pullPolicy=Never`:

     ```bash
     helm upgrade --install accounts-api ./helm/accounts-api \
       --namespace accounts --create-namespace \
       --set image.repository=docker.io/library/bootcampclt2026-main-accounts-api \
       --set image.tag=latest \
       --set image.pullPolicy=Never \
       --wait --timeout 3m
     ```

El chart (`helm/accounts-api`) separa configuración no sensible (`ConfigMap`: `APPLICATION_NAME`, `ASPNETCORE_ENVIRONMENT`, `Seq__ServerUrl`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpirationMinutes`) de datos sensibles (`Secret`: cadena de conexión a PostgreSQL, `Jwt__SigningKey`, credenciales de admin), incluye un init-container que espera a que PostgreSQL esté listo, y expone la API como `NodePort` en el puerto `30080`.

**Verificado en Minikube real** (driver Docker, Windows): namespace + Postgres + Seq + API con 0 reinicios, health checks, login, listado de cuentas y depósito respondiendo 200 contra la base sembrada dentro del clúster. En Windows con el driver Docker, `minikube ip`/el NodePort no son alcanzables directamente desde el host — para probar hay que usar `kubectl port-forward service/accounts-api 18080:8080 --namespace accounts` (o `minikube service accounts-api -n accounts`) y pegarle a `http://localhost:18080`.

## CI/CD

Definido en `.github/workflows/ci.yml`:

- **CI** (`ubuntu-latest`): restore, build, test de la solución, lint del chart de Helm, build y push de la imagen a Docker Hub (tag = SHA corto + `latest`).
- **CD** (`self-hosted, Windows`): despliega los manifiestos de PostgreSQL/Seq y la API (vía Helm) a un clúster Minikube local, aplica el esquema/seed de la base, y verifica el rollout.

## Variables de entorno relevantes

| Variable | Uso |
|---|---|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a PostgreSQL |
| `Seq__ServerUrl` | URL del servidor Seq para centralizar logs |
| `APPLICATION_NAME` | Nombre de aplicación usado como propiedad de log |
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución (`Development`/`Production`) |
| `Jwt__Issuer` / `Jwt__Audience` | Issuer/audience validados en cada token JWT |
| `Jwt__SigningKey` | Clave simétrica para firmar/validar los JWT (sensible) |
| `Jwt__ExpirationMinutes` | Minutos de validez del token (por defecto 60) |
| `Auth__AdminUsername` / `Auth__AdminPassword` | Credenciales del usuario administrador (sensible) |

En Docker Compose y en Kubernetes estas variables se inyectan por entorno/Secret/ConfigMap — nunca se espera que las credenciales reales de producción vivan en `appsettings.json`. Los valores de `appsettings.json`/`docker-compose.yml` de este repo son únicamente para desarrollo local.
