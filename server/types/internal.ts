// server/types/internal.ts
// Only used within server/ — never imported by app/

import type { Transaccion, Presupuesto, Account, Family } from '#shared/types'

export interface StorageState {
  users: Array<{
    id: string
    name: string
    email: string
    passwordHash: string
    createdAt: string
  }>
  families: Family[]
  accounts: Account[]
  transacciones: Transaccion[]
  presupuestos: Presupuesto[]
}
