// server/api/transacciones/index.post.ts

import { addTransaccion } from '~~/server/utils/storage'
import { transaccionCreateSchema } from '~~/server/utils/schemas'
import type { Transaccion } from '#shared/types'

export default defineEventHandler(async (event) => {
  const body = await readValidatedBody(event, transaccionCreateSchema.parse)

  const nueva: Transaccion = {
    ...body,
    id: crypto.randomUUID(),
    familyId: 'f1',
    userId: 'u1',
    currency: 'CLP',
    creadoEn: new Date().toISOString(),
  }

  const creada = addTransaccion(nueva)
  return { data: creada, ok: true }
})
