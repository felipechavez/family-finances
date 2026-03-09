// server/api/cuentas/index.get.ts
import { getAccounts } from '~~/server/utils/storage'

export default defineEventHandler(() => {
  const accounts = getAccounts()
  return { data: accounts, ok: true }
})
