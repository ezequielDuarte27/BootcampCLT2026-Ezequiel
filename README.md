# CleanArchitecture.Full — Accounts API

API de gestión de cuentas bancarias construida en .NET 10 siguiendo **Clean Architecture** + **CQRS** (MediatR), con persistencia en PostgreSQL, logging estructurado (Serilog + Seq), contenedores Docker y despliegue a Kubernetes (Minikube) vía Helm, con pipeline de CI/CD en GitHub Actions.

## Arquitectura

Solución (`CleanArchitecture.Full.slnx`) con 4 proyectos en `src/`:

| Proyecto | Responsabilidad |
|---|---|
| `CleanArchitecture.Full.Domain` | Entidades puras (`Account`) e interfaces de repositorio. Sin dependencias externas. |
| `CleanArchitecture.Full.Application` | Casos de uso con MediatR (Commands/Queries + Handlers), validaciones con FluentValidation, DTOs y mapeos. |
| `CleanArchitecture.Full.Infrastructure` | Acceso a datos con EF Core sobre PostgreSQL (Npgsql, snake_case), migraciones, repositorios. |
| `CleanArchitecture.Full.Api` | Minimal API, middleware de manejo de errores, logging de requests, health checks, OpenAPI/Scalar. |

Flujo de una request: `Endpoint (Minimal API)` → `ISender.Send(Command/Query)` → `ValidationBehavior` (FluentValidation) → `Handler` → `IAccountRepository` → `AppDbContext` (PostgreSQL).

## Endpoints

Base: `api/v1/accounts`

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/v1/accounts` | Lista todas las cuentas |
| `GET` | `/api/v1/accounts/{id}` | Obtiene una cuenta por id (404 si no existe) |
| `POST` | `/api/v1/accounts` | Crea una cuenta (400 si falla la validación, 409 si el número de cuenta ya existe) |
| `PUT` | `/api/v1/accounts/{id}` | Actualiza una cuenta (404 si no existe) |
| `DELETE` | `/api/v1/accounts/{id}` | Elimina una cuenta (404 si no existe) |
| `POST` | `/api/v1/accounts/{id}/deposit` | Deposita `{ "amount": number }` en la cuenta (400 si el monto es inválido o la cuenta no está `Active`) |
| `POST` | `/api/v1/accounts/{id}/withdraw` | Retira `{ "amount": number }` de la cuenta (400 si el monto es inválido, si no está `Active` o si no hay saldo suficiente) |

Documentación interactiva (OpenAPI vía Scalar) disponible en `/scalar` cuando la API corre en `Development`.

Health checks:
- `GET /health/live` — liveness del proceso.
- `GET /health/ready` — readiness, valida conexión a PostgreSQL.

### Errores

Todas las respuestas de error siguen el mismo formato JSON (`title`, `status`, y `errors`/`detail` según el caso), generado por el middleware de manejo de excepciones (`Middleware/ValidationExceptionMiddleware.cs`):

- **400** — falla de validación (FluentValidation), incluye `errors` agrupados por campo.
- **404** — recurso no encontrado.
- **409** — conflicto de datos (por ejemplo, número de cuenta duplicado).
- **500** — error no controlado (sin exponer stack trace ni detalles internos).

## Correr localmente

### Con Docker Compose (recomendado)

Levanta PostgreSQL, Seq y la API:

```bash
docker compose up --build
```

- API: `http://localhost:8080`
- Seq (UI de logs): `http://localhost:5341`
- PostgreSQL: `localhost:5432` (db `accountsdb`, user/pass `postgres`/`postgres`)

La primera vez, aplicar el esquema y datos semilla contra la base:

```bash
psql -h localhost -p 5432 -U postgres -d accountsdb -f database/01_create_database.sql
psql -h localhost -p 5432 -U postgres -d accountsdb -f database/02_create_table_and_seed.sql
```

### Sin Docker

```bash
dotnet restore CleanArchitecture.Full.slnx
dotnet build CleanArchitecture.Full.slnx
dotnet run --project src/CleanArchitecture.Full.Api
```

Requiere una instancia de PostgreSQL accesible según `ConnectionStrings:DefaultConnection` en `src/CleanArchitecture.Full.Api/appsettings.Development.json`, y opcionalmente Seq en `http://localhost:5341` para centralizar logs.

## Logging

Serilog escribe de forma asíncrona a consola y a Seq (`Seq:ServerUrl`, configurable por entorno). Se usa `Information` para operaciones normales, `Warning` para reglas de negocio "blandas" o errores de cliente (validación, 4xx), y `Error` para excepciones no controladas (5xx). Cada request queda enriquecida con `RequestId`, `ClientIp`, `RequestHost` y `UserAgent`.

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

   ```bash
   helm upgrade --install accounts-api ./helm/accounts-api \
     --namespace accounts --create-namespace \
     --set image.repository=docker.io/<tu-usuario>/accounts-api \
     --set image.tag=latest
   ```

El chart (`helm/accounts-api`) separa configuración no sensible (`ConfigMap`: `APPLICATION_NAME`, `ASPNETCORE_ENVIRONMENT`, `Seq__ServerUrl`) de datos sensibles (`Secret`: cadena de conexión a PostgreSQL), incluye un init-container que espera a que PostgreSQL esté listo, y expone la API como `NodePort` en el puerto `30080`.

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

En Docker Compose y en Kubernetes estas variables se inyectan por entorno/Secret/ConfigMap — nunca se espera que las credenciales reales de producción vivan en `appsettings.json`.
