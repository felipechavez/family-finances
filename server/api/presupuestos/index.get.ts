// server/api/presupuestos/index.get.ts

import { getPresupuestosByMes } from '~~/server/utils/storage'
import { mesQuerySchema } from '~~/server/utils/schemas'

const mesActual = new Date().toISOString().slice(0, 7)

export default defineEventHandler(async (event) => {
  const query = await getValidatedQuery(event, mesQuerySchema.parse)
  const mes = query.mes ?? mesActual

  const presupuestos = getPresupuestosByMes(mes)
  return { data: presupuestos, ok: true }
})
