// server/api/cuentas/[id].delete.ts
import { deleteAccount } from '~~/server/utils/storage'
import { notFound } from '~~/server/utils/errors'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  if (!id) throw createError({ statusCode: 400, message: 'ID requerido' })

  const deleted = deleteAccount(id)
  if (!deleted) notFound('Cuenta', id)

  return { data: { id }, ok: true }
})
