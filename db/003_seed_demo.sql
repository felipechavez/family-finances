-- =============================================================================
-- 003_seed_demo.sql
-- Demo data for local development — DO NOT run in production
-- =============================================================================

DO $$
DECLARE
    v_user_id    UUID := gen_random_uuid();
    v_family_id  UUID := gen_random_uuid();
    v_acc_bank   UUID := gen_random_uuid();
    v_acc_cash   UUID := gen_random_uuid();
    v_acc_saving UUID := gen_random_uuid();

    v_cat_alim   UUID;
    v_cat_trans  UUID;
    v_cat_hogar  UUID;
    v_cat_educ   UUID;
    v_cat_entret UUID;
    v_cat_sueldo UUID;
BEGIN

-- User
INSERT INTO users (id, name, email, password_hash)
VALUES (v_user_id, 'Demo User', 'demo@familia.cl',
        '$2a$12$PlaceholderBcryptHashReplaceMe...');

-- Family
INSERT INTO families (id, name, owner_user_id)
VALUES (v_family_id, 'Mi Familia', v_user_id);

INSERT INTO family_members (family_id, user_id, role)
VALUES (v_family_id, v_user_id, 'owner');

-- Accounts
INSERT INTO accounts (id, family_id, name, type, balance) VALUES
    (v_acc_bank,   v_family_id, 'Cuenta Corriente', 'bank',    1500000),
    (v_acc_cash,   v_family_id, 'Efectivo',          'cash',      50000),
    (v_acc_saving, v_family_id, 'Ahorro',            'savings',  800000);

-- Resolve global category IDs
SELECT id INTO v_cat_alim   FROM categories WHERE family_id IS NULL AND name = 'alimentacion';
SELECT id INTO v_cat_trans  FROM categories WHERE family_id IS NULL AND name = 'transporte';
SELECT id INTO v_cat_hogar  FROM categories WHERE family_id IS NULL AND name = 'hogar';
SELECT id INTO v_cat_educ   FROM categories WHERE family_id IS NULL AND name = 'educacion';
SELECT id INTO v_cat_entret FROM categories WHERE family_id IS NULL AND name = 'entretenimiento';
SELECT id INTO v_cat_sueldo FROM categories WHERE family_id IS NULL AND name = 'sueldo';

-- Transactions (current month)
INSERT INTO transactions
    (family_id, account_id, user_id, category_id, type, amount, description, transaction_date)
VALUES
    (v_family_id, v_acc_bank, v_user_id, v_cat_sueldo, 'income',  1200000, 'Sueldo Mamá',          DATE_TRUNC('month', NOW())),
    (v_family_id, v_acc_bank, v_user_id, v_cat_sueldo, 'income',   950000, 'Sueldo Papá',          DATE_TRUNC('month', NOW())),
    (v_family_id, v_acc_bank, v_user_id, v_cat_alim,   'expense',  180000, 'Supermercado',         DATE_TRUNC('month', NOW()) + 2),
    (v_family_id, v_acc_cash, v_user_id, v_cat_trans,  'expense',   60000, 'Bencina',              DATE_TRUNC('month', NOW()) + 4),
    (v_family_id, v_acc_bank, v_user_id, v_cat_educ,   'expense',  120000, 'Mensualidad colegio',  DATE_TRUNC('month', NOW()) + 4),
    (v_family_id, v_acc_bank, v_user_id, v_cat_hogar,  'expense',   85000, 'Cuenta de luz',        DATE_TRUNC('month', NOW()) + 6),
    (v_family_id, v_acc_bank, v_user_id, v_cat_entret, 'expense',   25000, 'Streaming',            DATE_TRUNC('month', NOW()) + 7);

-- Budgets
INSERT INTO budgets (family_id, category_id, monthly_limit) VALUES
    (v_family_id, v_cat_alim,   300000),
    (v_family_id, v_cat_trans,  100000),
    (v_family_id, v_cat_hogar,  150000),
    (v_family_id, v_cat_educ,   150000),
    (v_family_id, v_cat_entret,  50000);

END $$;
