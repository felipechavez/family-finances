// server/api/auth/login.post.ts
import { getUserByEmail, getFamilyByUserId } from '~~/server/utils/storage'
import { z } from 'zod'

const loginSchema = z.object({
  email: z.string().email(),
  password: z.string().min(1),
})

export default defineEventHandler(async (event) => {
  const body = await readValidatedBody(event, loginSchema.parse)

  const user = getUserByEmail(body.email)
  if (!user || user.passwordHash !== body.password) {
    throw createError({ statusCode: 401, message: 'Credenciales incorrectas' })
  }

  const family = getFamilyByUserId(user.id)

  // Simulated JWT token (in production, use proper JWT signing)
  const token = `mock-jwt-${user.id}-${Date.now()}`

  return {
    data: {
      token,
      user: {
        id: user.id,
        name: user.name,
        email: user.email,
        createdAt: user.createdAt,
      },
      family,
    },
    ok: true,
  }
})
