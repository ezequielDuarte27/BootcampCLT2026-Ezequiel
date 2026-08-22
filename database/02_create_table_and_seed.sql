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
    currency       VARCHAR(3)    NOT NULL DEFAULT 'ARS',
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

-- Clientes de ejemplo (DNI ficticios)
INSERT INTO customers (id, document_type, document_number, full_name, created_at) VALUES
    ('b1b1b1b1-0001-0001-0001-000000000001', 'DNI', '27134863', 'Lionel Messi',    NOW()),
    ('b1b1b1b1-0002-0002-0002-000000000002', 'DNI', '40123456', 'Julian Alvarez',  NOW()),
    ('b1b1b1b1-0003-0003-0003-000000000003', 'DNI', '35987654', 'Sebas Caballero', NOW())
ON CONFLICT (document_type, document_number) DO NOTHING;

-- Una cuenta de ejemplo por cliente
INSERT INTO accounts (id, account_number, customer_id, balance, currency, status, created_at) VALUES
    ('a1a1a1a1-0001-0001-0001-000000000001', 'ACC-000001', 'b1b1b1b1-0001-0001-0001-000000000001', 15000.50, 'ARS', 'Active',   NOW()),
    ('a1a1a1a1-0002-0002-0002-000000000002', 'ACC-000002', 'b1b1b1b1-0002-0002-0002-000000000002',  8320.00, 'ARS', 'Active',   NOW()),
    ('a1a1a1a1-0003-0003-0003-000000000003', 'ACC-000003', 'b1b1b1b1-0003-0003-0003-000000000003',     0.00, 'ARS', 'Inactive', NOW())
ON CONFLICT (account_number) DO NOTHING;

SELECT * FROM accounts;
