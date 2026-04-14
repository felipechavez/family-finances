-- =============================================================================
-- 002_auth_email_2fa.sql
-- Adds email verification and 2FA (TOTP) support to the users table
-- Run against: finances schema
-- =============================================================================

SET search_path TO finances, public;

-- Email verification
ALTER TABLE users
    ADD COLUMN email_verified         BOOLEAN     NOT NULL DEFAULT FALSE,
    ADD COLUMN verification_token     VARCHAR(128),
    ADD COLUMN verification_token_exp TIMESTAMPTZ;

-- Two-factor authentication (TOTP)
ALTER TABLE users
    ADD COLUMN two_factor_enabled BOOLEAN    NOT NULL DEFAULT FALSE,
    ADD COLUMN totp_secret        VARCHAR(64);

-- Opt-in daily summary email (used in Phase 5)
ALTER TABLE users
    ADD COLUMN daily_summary_enabled BOOLEAN NOT NULL DEFAULT TRUE;
