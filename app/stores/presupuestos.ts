// app/stores/presupuestos.ts
import { defineStore, storeToRefs } from 'pinia'
import type { Presupuesto, CategoriaGastoId } from '#shared/types'
import { useTransaccionesStore } from './transacciones'

interface BudgetDto {
  id: string
  familyId: string
  categoryId: string
  categoryName: string
  monthlyLimit: number
}

export const usePresupuestosStore = defineStore('presupuestos', () => {
  const { $api } = useNuxtApp()
  const txStore = useTransaccionesStore()
  const { mes } = storeToRefs(txStore)

  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch<BudgetDto[]>('/budgets', {
    key: () => `presupuestos-${mes.value}`,
    $fetch: $api as typeof $fetch,
    default: (): BudgetDto[] => [],
  })

  const presupuestos = computed<Presupuesto[]>(() =>
    (response.value ?? []).map(b => ({
      id: b.id,
      familyId: b.familyId,
      categoria: b.categoryName as CategoriaGastoId,
      limite: Number(b.monthlyLimit),
      mes: mes.value,
    })),
  )

  const presupuestoPorCategoria = computed(() => {
    const map: Record<string, number> = {}
    presupuestos.value.forEach(p => { map[p.categoria] = p.limite })
    return map
  })

  async function guardarLimite(categoria: string, limite: number): Promise<void> {
    await ($api as typeof $fetch)('/budgets', {
      method: 'POST',
      body: { categoria, limite },
    })
    await refresh()
  }

  return {
    presupuestos,
    presupuestoPorCategoria,
    status,
    error,
    refresh,
    guardarLimite,
  }
})
