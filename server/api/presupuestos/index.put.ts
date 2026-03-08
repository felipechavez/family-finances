// server/api/presupuestos/index.put.ts

import { upsertPresupuesto } from '~~/server/utils/storage'
import { presupuestoUpsertSchema } from '~~/server/utils/schemas'
import type { Presupuesto } from '#shared/types'

export default defineEventHandler(async (event) => {
  const body = await readValidatedBody(event, presupuestoUpsertSchema.parse)

  const presupuesto: Presupuesto = {
    categoria: body.categoria,
    limite: body.limite,
    mes: body.mes,
  }

  const resultado = upsertPresupuesto(presupuesto)
  return { data: resultado, ok: true }
})
