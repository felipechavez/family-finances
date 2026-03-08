// app/composables/use-categorias.ts
// Exports functions ONLY

import type { CategoriaInfo } from '~/types/ui'
import type { CategoriaGastoId, CategoriaIngresoId } from '#shared/types'

const CATEGORIAS_GASTO: CategoriaInfo[] = [
  { id: 'alimentacion' as CategoriaGastoId, emoji: '🛒', label: 'Alimentación', color: '#f97316' },
  { id: 'transporte' as CategoriaGastoId, emoji: '🚗', label: 'Transporte', color: '#3b82f6' },
  { id: 'salud' as CategoriaGastoId, emoji: '💊', label: 'Salud', color: '#22c55e' },
  { id: 'educacion' as CategoriaGastoId, emoji: '📚', label: 'Educación', color: '#a855f7' },
  { id: 'hogar' as CategoriaGastoId, emoji: '🏠', label: 'Hogar', color: '#ec4899' },
  { id: 'entretenimiento' as CategoriaGastoId, emoji: '🎬', label: 'Entretención', color: '#eab308' },
  { id: 'ropa' as CategoriaGastoId, emoji: '👕', label: 'Ropa', color: '#06b6d4' },
  { id: 'otros' as CategoriaGastoId, emoji: '📦', label: 'Otros', color: '#6b7280' },
]

const CATEGORIAS_INGRESO: CategoriaInfo[] = [
  { id: 'sueldo' as CategoriaIngresoId, emoji: '💼', label: 'Sueldo', color: '#22c55e' },
  { id: 'freelance' as CategoriaIngresoId, emoji: '💻', label: 'Freelance', color: '#3b82f6' },
  { id: 'arriendo' as CategoriaIngresoId, emoji: '🏠', label: 'Arriendo', color: '#a855f7' },
  { id: 'otros_ingreso' as CategoriaIngresoId, emoji: '💰', label: 'Otros', color: '#eab308' },
]

export function useCategorias() {
  function getCategoriaInfo(categoriaId: string): CategoriaInfo {
    return (
      [...CATEGORIAS_GASTO, ...CATEGORIAS_INGRESO].find(c => c.id === categoriaId) ?? {
        id: categoriaId as CategoriaGastoId,
        emoji: '📦',
        label: categoriaId,
        color: '#6b7280',
      }
    )
  }

  return {
    categoriasGasto: CATEGORIAS_GASTO,
    categoriasIngreso: CATEGORIAS_INGRESO,
    getCategoriaInfo,
  }
}
