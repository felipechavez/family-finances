// app/types/ui.ts
// Frontend-only UI types — never imported in server code

import type { CategoriaGastoId, CategoriaIngresoId } from '#shared/types'

export interface CategoriaInfo {
  id: CategoriaGastoId | CategoriaIngresoId
  label: string
  emoji: string
  color: string
}

export interface ToastMessage {
  id: string
  texto: string
  tipo: 'ok' | 'error' | 'info'
}

export interface FormTransaccion {
  tipo: 'gasto' | 'ingreso'
  categoria: string
  monto: string
  descripcion: string
  fecha: string
  miembro: string
}

export interface FiltrosMes {
  mes: string // YYYY-MM
}
