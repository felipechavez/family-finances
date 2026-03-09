// server/api/presupuestos/index.put.ts

import { upsertPresupuesto } from '~~/server/utils/storage'
import { presupuestoUpsertSchema } from '~~/server/utils/schemas'

export default defineEventHandler(async (event) => {
  const body = await readValidatedBody(event, presupuestoUpsertSchema.parse)
  const resultado = upsertPresupuesto(body)
  return { data: resultado, ok: true }
})
