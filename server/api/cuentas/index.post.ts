// server/api/cuentas/index.post.ts
import { addAccount } from '~~/server/utils/storage'
import { z } from 'zod'

const accountCreateSchema = z.object({
  name: z.string().min(1, 'El nombre es requerido'),
  type: z.enum(['cash', 'bank', 'savings', 'credit_card']),
})

export default defineEventHandler(async (event) => {
  const body = await readValidatedBody(event, accountCreateSchema.parse)
  const account = addAccount(body)
  return { data: account, ok: true }
})
