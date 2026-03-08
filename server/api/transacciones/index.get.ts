// server/api/transacciones/index.get.ts

import { getTransaccionesByMes, getTransacciones } from '~~/server/utils/storage'
import { mesQuerySchema } from '~~/server/utils/schemas'

export default defineEventHandler(async (event) => {
  const query = await getValidatedQuery(event, mesQuerySchema.parse)

  const transacciones = query.mes
    ? getTransaccionesByMes(query.mes)
    : getTransacciones()

  return { data: transacciones, ok: true }
})
