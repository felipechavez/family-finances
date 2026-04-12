-- =============================================================================
-- 002_views.sql
-- Reporting views used by GET /reports/monthly and /reports/trend
-- =============================================================================
SET search_path TO finances, public;

-- Monthly summary per family
CREATE OR REPLACE VIEW v_monthly_summary AS
SELECT
    t.family_id,
    DATE_TRUNC('month', t.transaction_date)::DATE AS month,
    SUM(t.amount) FILTER (WHERE t.type = 'income')  AS total_income,
    SUM(t.amount) FILTER (WHERE t.type = 'expense') AS total_expense,
    SUM(t.amount) FILTER (WHERE t.type = 'income')
        - SUM(t.amount) FILTER (WHERE t.type = 'expense') AS balance
FROM transactions t
GROUP BY t.family_id, DATE_TRUNC('month', t.transaction_date);

-- Expense distribution by category per family per month
CREATE OR REPLACE VIEW v_expense_by_category AS
SELECT
    t.family_id,
    DATE_TRUNC('month', t.transaction_date)::DATE AS month,
    c.name  AS category_name,
    c.id    AS category_id,
    SUM(t.amount) AS total
FROM transactions t
JOIN categories c ON c.id = t.category_id
WHERE t.type = 'expense'
GROUP BY t.family_id, DATE_TRUNC('month', t.transaction_date), c.id, c.name;

-- Budget consumption (current month)
CREATE OR REPLACE VIEW v_budget_consumption AS
SELECT
    b.family_id,
    b.category_id,
    c.name            AS category_name,
    b.monthly_limit,
    COALESCE(SUM(t.amount), 0) AS spent,
    b.monthly_limit - COALESCE(SUM(t.amount), 0) AS remaining,
    ROUND(
        COALESCE(SUM(t.amount), 0) / NULLIF(b.monthly_limit, 0) * 100,
        1
    ) AS pct_used
FROM budgets b
JOIN categories c ON c.id = b.category_id
LEFT JOIN transactions t
    ON  t.family_id   = b.family_id
    AND t.category_id = b.category_id
    AND t.type        = 'expense'
    AND DATE_TRUNC('month', t.transaction_date) = DATE_TRUNC('month', NOW())
GROUP BY b.family_id, b.category_id, c.name, b.monthly_limit;

-- Account balances per family
CREATE OR REPLACE VIEW v_account_balances AS
SELECT
    a.family_id,
    a.id          AS account_id,
    a.name        AS account_name,
    a.type        AS account_type,
    a.balance     AS initial_balance,
    COALESCE(SUM(t.amount) FILTER (WHERE t.type = 'income'),  0) AS total_income,
    COALESCE(SUM(t.amount) FILTER (WHERE t.type = 'expense'), 0) AS total_expense,
    a.balance
        + COALESCE(SUM(t.amount) FILTER (WHERE t.type = 'income'),  0)
        - COALESCE(SUM(t.amount) FILTER (WHERE t.type = 'expense'), 0) AS current_balance
FROM accounts a
LEFT JOIN transactions t ON t.account_id = a.id
GROUP BY a.family_id, a.id, a.name, a.type, a.balance;
