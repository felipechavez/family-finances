// app/composables/use-categorias.ts
import type { CategoriaInfo } from '~/types/ui'
import type { CategoriaGastoId, CategoriaIngresoId } from '#shared/types'

// Static base data (id, emoji, color) — labels come from i18n
const BASE_GASTO = [
  { id: 'alimentacion' as CategoriaGastoId,    emoji: '🛒', color: '#f97316' },
  { id: 'transporte' as CategoriaGastoId,      emoji: '🚗', color: '#3b82f6' },
  { id: 'salud' as CategoriaGastoId,           emoji: '💊', color: '#22c55e' },
  { id: 'educacion' as CategoriaGastoId,       emoji: '📚', color: '#a855f7' },
  { id: 'hogar' as CategoriaGastoId,           emoji: '🏠', color: '#ec4899' },
  { id: 'entretenimiento' as CategoriaGastoId, emoji: '🎬', color: '#eab308' },
  { id: 'ropa' as CategoriaGastoId,            emoji: '👕', color: '#06b6d4' },
  { id: 'otros' as CategoriaGastoId,           emoji: '📦', color: '#6b7280' },
]

const BASE_INGRESO = [
  { id: 'sueldo' as CategoriaIngresoId,       emoji: '💼', color: '#22c55e' },
  { id: 'freelance' as CategoriaIngresoId,    emoji: '💻', color: '#3b82f6' },
  { id: 'arriendo' as CategoriaIngresoId,     emoji: '🏠', color: '#a855f7' },
  { id: 'otros_ingreso' as CategoriaIngresoId, emoji: '💰', color: '#eab308' },
]

export function useCategorias() {
  const { t } = useI18n()

  const categoriasGasto = computed<CategoriaInfo[]>(() =>
    BASE_GASTO.map(c => ({ ...c, label: t(`categorias.${c.id}`) }))
  )

  const categoriasIngreso = computed<CategoriaInfo[]>(() =>
    BASE_INGRESO.map(c => ({ ...c, label: t(`categorias.${c.id}`) }))
  )

  function getCategoriaInfo(categoriaId: string): CategoriaInfo {
    const base = [...BASE_GASTO, ...BASE_INGRESO].find(c => c.id === categoriaId)
    if (!base) {
      return { id: categoriaId as CategoriaGastoId, emoji: '📦', label: categoriaId, color: '#6b7280' }
    }
    return { ...base, label: t(`categorias.${base.id}`) }
  }

  return { categoriasGasto, categoriasIngreso, getCategoriaInfo }
}
