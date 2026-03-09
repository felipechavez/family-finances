// server/api/auth/register.post.ts
import { addUser, getUserByEmail } from '~~/server/utils/storage'
import { z } from 'zod'

const registerSchema = z.object({
  name: z.string().min(1, 'El nombre es requerido'),
  email: z.string().email('Email inválido'),
  password: z.string().min(6, 'La contraseña debe tener al menos 6 caracteres'),
})

export default defineEventHandler(async (event) => {
  const body = await readValidatedBody(event, registerSchema.parse)

  const existing = getUserByEmail(body.email)
  if (existing) {
    throw createError({ statusCode: 409, message: 'El email ya está registrado' })
  }

  const user = addUser(body)
  return { data: { id: user.id, name: user.name, email: user.email }, ok: true }
})
