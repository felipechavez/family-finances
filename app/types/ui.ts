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
