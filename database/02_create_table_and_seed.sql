-- Ejecutar conectado a la base "accountsdb" (creada con 01_create_database.sql).
-- psql: psql -h localhost -p 5432 -U postgres -d accountsdb -f 02_create_table_and_seed.sql

CREATE TABLE IF NOT EXISTS customers (
    id              UUID PRIMARY KEY,
    document_type   VARCHAR(20)  NOT NULL,
    document_number VARCHAR(20)  NOT NULL,
    full_name       VARCHAR(150) NOT NULL,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT ix_customers_document UNIQUE (document_type, document_number)
);

CREATE TABLE IF NOT EXISTS accounts (
    id             UUID PRIMARY KEY,
    account_number VARCHAR(20)   NOT NULL,
    customer_id    UUID          NOT NULL REFERENCES customers (id),
    balance        NUMERIC(18,2) NOT NULL DEFAULT 0,
    currency       VARCHAR(3)    NOT NULL DEFAULT 'PYG',
    status         VARCHAR(20)   NOT NULL,
    created_at     TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    closed_at      TIMESTAMPTZ   NULL,
    CONSTRAINT ix_accounts_account_number UNIQUE (account_number)
);

CREATE TABLE IF NOT EXISTS transactions (
    id                 UUID          PRIMARY KEY,
    account_id         UUID          NOT NULL REFERENCES accounts (id),
    type               VARCHAR(20)   NOT NULL,
    amount             NUMERIC(18,2) NOT NULL,
    currency           VARCHAR(3)    NOT NULL,
    balance_after      NUMERIC(18,2) NOT NULL,
    related_account_id UUID          NULL,
    description        VARCHAR(250)  NULL,
    created_at         TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS ix_transactions_account_id ON transactions (account_id);

CREATE TABLE IF NOT EXISTS users (
    id            UUID         PRIMARY KEY,
    username      VARCHAR(50)  NOT NULL,
    password_hash TEXT         NOT NULL,
    role          VARCHAR(20)  NOT NULL,
    customer_id   UUID         NULL REFERENCES customers (id),
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT ix_users_username UNIQUE (username)
);

-- Clientes de ejemplo (DNI ficticios, tematica futbolera Argentina/Paraguay)
INSERT INTO customers (id, document_type, document_number, full_name, created_at) VALUES
    ('b1b1b1b1-0001-0001-0001-000000000001', 'DNI', '27134863', 'Lionel Messi',      NOW()),
    ('b1b1b1b1-0002-0002-0002-000000000002', 'DNI', '40123456', 'Julian Alvarez',    NOW()),
    ('b1b1b1b1-0003-0003-0003-000000000003', 'DNI', '35987654', 'Sebas Caballero',   NOW()),
    ('b1b1b1b1-0004-0004-0004-000000000004', 'DNI', '41234567', 'Miguel Almiron',    NOW()),
    ('b1b1b1b1-0005-0005-0005-000000000005', 'DNI', '34567890', 'Gustavo Gomez',     NOW()),
    ('b1b1b1b1-0006-0006-0006-000000000006', 'DNI', '23456789', 'Angel Romero',      NOW()),
    ('b1b1b1b1-0007-0007-0007-000000000007', 'DNI', '56789012', 'Roberto Fernandez', NOW())
ON CONFLICT (document_type, document_number) DO NOTHING;

-- Una cuenta de ejemplo por cliente, montos en guaranies (PYG)
INSERT INTO accounts (id, account_number, customer_id, balance, currency, status, created_at, closed_at) VALUES
    ('a1a1a1a1-0001-0001-0001-000000000001', 'ACC-000001', 'b1b1b1b1-0001-0001-0001-000000000001', 120000000.00, 'PYG', 'Active',   NOW(), NULL),
    ('a1a1a1a1-0002-0002-0002-000000000002', 'ACC-000002', 'b1b1b1b1-0002-0002-0002-000000000002',  65000000.00, 'PYG', 'Active',   NOW(), NULL),
    ('a1a1a1a1-0003-0003-0003-000000000003', 'ACC-000003', 'b1b1b1b1-0003-0003-0003-000000000003',          0.00, 'PYG', 'Inactive', NOW(), NULL),
    ('a1a1a1a1-0004-0004-0004-000000000004', 'ACC-000004', 'b1b1b1b1-0004-0004-0004-000000000004',  45000000.00, 'PYG', 'Active',   NOW(), NULL),
    ('a1a1a1a1-0005-0005-0005-000000000005', 'ACC-000005', 'b1b1b1b1-0005-0005-0005-000000000005',  30750000.00, 'PYG', 'Active',   NOW(), NULL),
    ('a1a1a1a1-0006-0006-0006-000000000006', 'ACC-000006', 'b1b1b1b1-0006-0006-0006-000000000006',  18200000.00, 'PYG', 'Active',   NOW(), NULL),
    ('a1a1a1a1-0007-0007-0007-000000000007', 'ACC-000007', 'b1b1b1b1-0007-0007-0007-000000000007',          0.00, 'PYG', 'Closed',   NOW(), NOW())
ON CONFLICT (account_number) DO NOTHING;

-- Historial de movimientos de ejemplo (coherente con el balance final de cada cuenta)
INSERT INTO transactions (id, account_id, type, amount, currency, balance_after, related_account_id, description, created_at) VALUES
    ('c1c1c1c1-0001-0001-0001-000000000001', 'a1a1a1a1-0001-0001-0001-000000000001', 'Deposit',    100000000.00, 'PYG', 100000000.00, NULL, NULL, NOW() - INTERVAL '10 days'),
    ('c1c1c1c1-0001-0001-0001-000000000002', 'a1a1a1a1-0001-0001-0001-000000000001', 'Deposit',     25000000.00, 'PYG', 125000000.00, NULL, NULL, NOW() - INTERVAL '5 days'),
    ('c1c1c1c1-0001-0001-0001-000000000003', 'a1a1a1a1-0001-0001-0001-000000000001', 'Withdrawal',   5000000.00, 'PYG', 120000000.00, NULL, NULL, NOW() - INTERVAL '2 days'),
    ('c1c1c1c1-0002-0002-0002-000000000001', 'a1a1a1a1-0002-0002-0002-000000000002', 'Deposit',     70000000.00, 'PYG',  70000000.00, NULL, NULL, NOW() - INTERVAL '7 days'),
    ('c1c1c1c1-0002-0002-0002-000000000002', 'a1a1a1a1-0002-0002-0002-000000000002', 'Withdrawal',   5000000.00, 'PYG',  65000000.00, NULL, NULL, NOW() - INTERVAL '1 day')
ON CONFLICT (id) DO NOTHING;

-- Usuario Cliente de ejemplo (rol Cliente vinculado a Miguel Almiron) para probar login sin registrarse primero.
-- Usuario: malmiron / Contrasena: Cliente123!  (hash real, generado con el mismo PBKDF2 de la app)
INSERT INTO users (id, username, password_hash, role, customer_id, created_at) VALUES
    ('8edb8602-3d9c-4686-bafb-d4d2b093edd3', 'malmiron', 'lK8DMQ9qyN7ocuMEYH+Pqw==.2i8hwpsDGy53gBN/RkmluSzX6zuuUwz4QF5PYgLfztA=', 'Cliente', 'b1b1b1b1-0004-0004-0004-000000000004', NOW())
ON CONFLICT (username) DO NOTHING;

SELECT * FROM accounts;
