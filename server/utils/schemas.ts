// server/utils/schemas.ts
// Zod schemas for runtime validation

import { z } from 'zod'

const categoriaGastoEnum = z.enum([
  'alimentacion',
  'transporte',
  'salud',
  'educacion',
  'hogar',
  'entretenimiento',
  'ropa',
  'otros',
])

const categoriaIngresoEnum = z.enum([
  'sueldo',
  'freelance',
  'arriendo',
  'otros_ingreso',
])

const categoriaIdEnum = z.union([categoriaGastoEnum, categoriaIngresoEnum])

export const transaccionCreateSchema = z.object({
  accountId: z.string().min(1, 'La cuenta es requerida'),
  tipo: z.enum(['gasto', 'ingreso']),
  categoria: categoriaIdEnum,
  monto: z.number().positive('El monto debe ser positivo'),
  descripcion: z.string().min(1, 'La descripción es requerida').max(200),
  fecha: z.string().regex(/^\d{4}-\d{2}-\d{2}$/, 'Formato de fecha inválido'),
})

export const presupuestoUpsertSchema = z.object({
  categoria: categoriaGastoEnum,
  limite: z.number().nonnegative('El límite debe ser 0 o mayor'),
  mes: z.string().regex(/^\d{4}-\d{2}$/, 'Formato de mes inválido (YYYY-MM)'),
})

export const mesQuerySchema = z.object({
  mes: z.string().regex(/^\d{4}-\d{2}$/).optional(),
})

export type PresupuestoUpsertInput = z.infer<typeof presupuestoUpsertSchema>
