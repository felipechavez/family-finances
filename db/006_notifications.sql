-- =============================================================================
-- 006_notifications.sql
-- In-app notifications for budget alerts and daily summaries
-- Run against: finances schema
-- =============================================================================

SET search_path TO finances, public;

CREATE TABLE notifications (
    id         UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id    UUID        NOT NULL REFERENCES users(id)    ON DELETE CASCADE,
    family_id  UUID        NOT NULL REFERENCES families(id) ON DELETE CASCADE,
    type       VARCHAR(50) NOT NULL,   -- 'budget_exceeded' | 'daily_summary'
    title      VARCHAR(255) NOT NULL,
    body       TEXT,
    is_read    BOOLEAN     NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_notifications_user ON notifications(user_id, is_read, created_at DESC);
