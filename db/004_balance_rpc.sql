-- =============================================================================
-- 004_balance_rpc.sql
-- Atomic account balance adjustment via RPC.
-- Using an SQL function avoids read-modify-write races in the application layer.
-- =============================================================================

-- Public schema so Supabase can route /rest/v1/rpc/adjust_account_balance
CREATE OR REPLACE FUNCTION finances.adjust_account_balance(
    p_account_id UUID,
    p_delta      NUMERIC
)
RETURNS void
LANGUAGE sql
SECURITY DEFINER
AS $$
    UPDATE finances.accounts
    SET    balance = balance + p_delta
    WHERE  id = p_account_id;
$$;

GRANT EXECUTE ON FUNCTION public.adjust_account_balance(UUID, NUMERIC) TO authenticated;
