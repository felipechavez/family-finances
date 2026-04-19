-- Feature 3: Email change flow
-- Adds columns to store a pending email change and its one-time token.

ALTER TABLE finances.users
    ADD COLUMN IF NOT EXISTS pending_email        VARCHAR(254),
    ADD COLUMN IF NOT EXISTS email_change_token   VARCHAR(128),
    ADD COLUMN IF NOT EXISTS email_change_token_exp TIMESTAMPTZ;
