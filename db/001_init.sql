-- =============================================================================
-- 001_init.sql
-- Initial schema for family-finances
-- PostgreSQL 16+
-- =============================================================================

-- Extensions
CREATE EXTENSION IF NOT EXISTS "pgcrypto";  -- gen_random_uuid()

-- =============================================================================
-- USERS
-- =============================================================================
CREATE TABLE users (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    name          VARCHAR(120) NOT NULL,
    email         VARCHAR(254) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_users_email ON users (email);

-- =============================================================================
-- FAMILIES
-- Each family is an isolated tenant
-- =============================================================================
CREATE TABLE families (
    id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    name          VARCHAR(120) NOT NULL,
    owner_user_id UUID        NOT NULL REFERENCES users (id) ON DELETE RESTRICT,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

-- =============================================================================
-- FAMILY MEMBERS
-- Many-to-many: users <-> families with a role
-- =============================================================================
CREATE TYPE family_role AS ENUM ('owner', 'admin', 'member');

CREATE TABLE family_members (
    family_id  UUID        NOT NULL REFERENCES families (id) ON DELETE CASCADE,
    user_id    UUID        NOT NULL REFERENCES users    (id) ON DELETE CASCADE,
    role       family_role NOT NULL DEFAULT 'member',
    joined_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (family_id, user_id)
);

CREATE INDEX idx_family_members_user ON family_members (user_id);

-- =============================================================================
-- CATEGORIES
-- Seeded with defaults; families can create custom ones
-- =============================================================================
CREATE TYPE transaction_type AS ENUM ('income', 'expense');

CREATE TABLE categories (
    id         UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    family_id  UUID             REFERENCES families (id) ON DELETE CASCADE,  -- NULL = global seed
    name       VARCHAR(80)      NOT NULL,
    type       transaction_type NOT NULL,
    created_at TIMESTAMPTZ      NOT NULL DEFAULT NOW(),
    UNIQUE (family_id, name)
);

-- Global seed categories (family_id = NULL)
INSERT INTO categories (id, family_id, name, type) VALUES
    -- Expense
    (gen_random_uuid(), NULL, 'alimentacion',    'expense'),
    (gen_random_uuid(), NULL, 'transporte',      'expense'),
    (gen_random_uuid(), NULL, 'salud',           'expense'),
    (gen_random_uuid(), NULL, 'educacion',       'expense'),
    (gen_random_uuid(), NULL, 'hogar',           'expense'),
    (gen_random_uuid(), NULL, 'entretenimiento', 'expense'),
    (gen_random_uuid(), NULL, 'ropa',            'expense'),
    (gen_random_uuid(), NULL, 'otros',           'expense'),
    -- Income
    (gen_random_uuid(), NULL, 'sueldo',          'income'),
    (gen_random_uuid(), NULL, 'freelance',       'income'),
    (gen_random_uuid(), NULL, 'arriendo',        'income'),
    (gen_random_uuid(), NULL, 'otros_ingreso',   'income');

-- =============================================================================
-- ACCOUNTS
-- Each family can have multiple accounts (cash, bank, savings, credit card)
-- =============================================================================
CREATE TYPE account_type AS ENUM ('cash', 'bank', 'savings', 'credit_card');

CREATE TABLE accounts (
    id         UUID         PRIMARY KEY DEFAULT gen_random_uuid(),
    family_id  UUID         NOT NULL REFERENCES families (id) ON DELETE CASCADE,
    name       VARCHAR(120) NOT NULL,
    type       account_type NOT NULL,
    balance    NUMERIC(15,2) NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    UNIQUE (family_id, name)
);

CREATE INDEX idx_accounts_family ON accounts (family_id);

-- =============================================================================
-- TRANSACTIONS
-- Core financial records (income / expense)
-- =============================================================================
CREATE TABLE transactions (
    id               UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    family_id        UUID             NOT NULL REFERENCES families     (id) ON DELETE CASCADE,
    account_id       UUID             NOT NULL REFERENCES accounts     (id) ON DELETE RESTRICT,
    user_id          UUID             NOT NULL REFERENCES users        (id) ON DELETE RESTRICT,
    category_id      UUID             NOT NULL REFERENCES categories   (id) ON DELETE RESTRICT,
    type             transaction_type NOT NULL,
    amount           NUMERIC(15,2)    NOT NULL CHECK (amount > 0),
    currency         CHAR(3)          NOT NULL DEFAULT 'CLP',
    description      VARCHAR(200)     NOT NULL,
    transaction_date DATE             NOT NULL,
    created_at       TIMESTAMPTZ      NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_transactions_family       ON transactions (family_id);
CREATE INDEX idx_transactions_account      ON transactions (account_id);
CREATE INDEX idx_transactions_date         ON transactions (transaction_date DESC);
CREATE INDEX idx_transactions_family_month
ON transactions (
  family_id,
  ( (EXTRACT(YEAR FROM transaction_date)::int * 12)
    + (EXTRACT(MONTH FROM transaction_date)::int - 1)
  )
);

-- =============================================================================
-- BUDGETS
-- Monthly spending limit per category per family
-- =============================================================================
CREATE TABLE budgets (
    id            UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
    family_id     UUID          NOT NULL REFERENCES families   (id) ON DELETE CASCADE,
    category_id   UUID          NOT NULL REFERENCES categories (id) ON DELETE CASCADE,
    monthly_limit NUMERIC(15,2) NOT NULL CHECK (monthly_limit >= 0),
    UNIQUE (family_id, category_id)
);

CREATE INDEX idx_budgets_family ON budgets (family_id);

-- =============================================================================
-- RECURRING TRANSACTIONS
-- Templates for auto-generated transactions (rent, salary, subscriptions)
-- =============================================================================
CREATE TYPE recurrence_type AS ENUM ('daily', 'weekly', 'monthly', 'yearly');

CREATE TABLE recurring_transactions (
    id                  UUID             PRIMARY KEY DEFAULT gen_random_uuid(),
    family_id           UUID             NOT NULL REFERENCES families     (id) ON DELETE CASCADE,
    account_id          UUID             NOT NULL REFERENCES accounts     (id) ON DELETE RESTRICT,
    category_id         UUID             NOT NULL REFERENCES categories   (id) ON DELETE RESTRICT,
    type                transaction_type NOT NULL,
    amount              NUMERIC(15,2)    NOT NULL CHECK (amount > 0),
    currency            CHAR(3)          NOT NULL DEFAULT 'CLP',
    description         VARCHAR(200)     NOT NULL,
    recurrence_type     recurrence_type  NOT NULL,
    next_execution_date DATE             NOT NULL,
    created_at          TIMESTAMPTZ      NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_recurring_next_exec ON recurring_transactions (next_execution_date);
