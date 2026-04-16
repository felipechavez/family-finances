-- 005_family_invite.sql
-- Adds a short random invite code to each family, and a family_role column
-- to the families table for quick owner lookups.
-- Run against Supabase SQL editor.

-- Add invite_code column (8-char uppercase alphanumeric, unique)
ALTER TABLE finances.families
  ADD COLUMN IF NOT EXISTS invite_code VARCHAR(10);

UPDATE finances.families
SET invite_code = upper(substring(replace(gen_random_uuid()::text, '-', '') from 1 for 8))
WHERE invite_code IS NULL;

ALTER TABLE finances.families
  ALTER COLUMN invite_code SET NOT NULL;

ALTER TABLE finances.families
  ADD CONSTRAINT families_invite_code_unique UNIQUE (invite_code);

CREATE INDEX IF NOT EXISTS idx_families_invite_code
ON finances.families (invite_code);