// shared/types/finanzas.ts
// Used by both client (app/) and server (server/)

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

export type MiembroFamiliar = 'Todos' | 'Mamá' | 'Papá' | 'Hijo/a 1' | 'Hijo/a 2'

export interface Transaccion {
  id: string
  tipo: TipoMovimiento
  categoria: CategoriaId
  monto: number
  descripcion: string
  fecha: string // ISO date string YYYY-MM-DD
  miembro: MiembroFamiliar
  creadoEn: string // ISO datetime
}

export interface Presupuesto {
  categoria: CategoriaGastoId
  limite: number
  mes: string // YYYY-MM
}

export interface ResumenMes {
  mes: string
  totalIngresos: number
  totalGastos: number
  balance: number
  gastosPorCategoria: Record<CategoriaGastoId, number>
}

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
