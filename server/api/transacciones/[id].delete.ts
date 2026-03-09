// server/api/transacciones/[id].delete.ts

import { deleteTransaccion, getTransaccionById } from '~~/server/utils/storage'
import { notFound, badRequest } from '~~/server/utils/errors'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')

  if (!id) badRequest('ID de transacción requerido')

  const existente = getTransaccionById(id!)
  if (!existente) notFound('Transacción', id!)

  deleteTransaccion(id!)
  return { data: { id }, ok: true }
})
