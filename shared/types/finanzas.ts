// shared/types/finanzas.ts
// Used by both client (app/) and server (server/)

// ── Auth & Users ──

export interface User {
  id: string
  name: string
  email: string
  createdAt: string
}

export interface AuthResponse {
  token: string
  user: User
  family: Family | null
}

export interface LoginInput {
  email: string
  password: string
}

export interface RegisterInput {
  name: string
  email: string
  password: string
}

// ── Family ──

export type FamilyRole = 'owner' | 'admin' | 'member'

export interface Family {
  id: string
  name: string
  ownerUserId: string
  createdAt: string
}

export interface FamilyMember {
  userId: string
  userName: string
  role: FamilyRole
}

// ── Accounts ──

export type AccountType = 'cash' | 'bank' | 'savings' | 'credit_card'

export interface Account {
  id: string
  familyId: string
  name: string
  type: AccountType
  balance: number
  createdAt: string
}

export interface AccountCreateInput {
  name: string
  type: AccountType
}

// ── Categories ──

export type TipoMovimiento = 'gasto' | 'ingreso'

export type CategoriaGastoId =
  | 'alimentacion'
  | 'transporte'
  | 'salud'
  | 'educacion'
  | 'hogar'
  | 'entretenimiento'
  | 'ropa'
  | 'otros'

export type CategoriaIngresoId =
  | 'sueldo'
  | 'freelance'
  | 'arriendo'
  | 'otros_ingreso'

export type CategoriaId = CategoriaGastoId | CategoriaIngresoId

// ── Transactions ──

export interface Transaccion {
  id: string
  familyId: string
  accountId: string
  userId: string
  tipo: TipoMovimiento
  categoria: CategoriaId
  monto: number
  currency: string
  descripcion: string
  fecha: string   // ISO date string YYYY-MM-DD
  creadoEn: string // ISO datetime
}

export interface TransaccionCreateInput {
  accountId: string
  tipo: TipoMovimiento
  categoria: CategoriaId
  monto: number
  descripcion: string
  fecha: string
}

// ── Budgets ──

export interface Presupuesto {
  id: string
  familyId: string
  categoria: CategoriaGastoId
  limite: number
  mes: string // YYYY-MM
}

// ── Recurring Transactions ──

export type RecurrenceType = 'daily' | 'weekly' | 'monthly' | 'yearly'

export interface RecurringTransaction {
  id: string
  familyId: string
  template: TransaccionCreateInput
  recurrenceType: RecurrenceType
  nextExecutionDate: string
}

// ── Reports ──

export interface ResumenMes {
  mes: string
  totalIngresos: number
  totalGastos: number
  balance: number
  gastosPorCategoria: Record<string, number>
}

export interface TendenciaMensual {
  mes: string
  ingresos: number
  gastos: number
}

// ── API Response ──

export interface ApiResponse<T> {
  data: T
  ok: boolean
}

export interface PaginatedResponse<T> {
  data: T[]
  total: number
  page: number
  pageSize: number
}
