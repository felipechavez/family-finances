// server/types/internal.ts
// Only used within server/ — never imported by app/

import type { Transaccion, Presupuesto } from '#shared/types'

export interface StorageState {
  transacciones: Transaccion[]
  presupuestos: Presupuesto[]
}

export interface TransaccionCreateInput {
  tipo: string
  categoria: string
  monto: number
  descripcion: string
  fecha: string
  miembro: string
}

export interface PresupuestoUpsertInput {
  categoria: string
  limite: number
  mes: string
}
