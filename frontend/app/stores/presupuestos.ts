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
  function getApi() {
    return useNuxtApp().$api as typeof $fetch
  }
  const txStore = useTransaccionesStore()
  const { mes } = storeToRefs(txStore)

  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch<BudgetDto[]>('/budgets', {
    key: () => `presupuestos-${mes.value}`,
    server: false,
    $fetch: getApi(),
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
    const $api = getApi()
    await $api('/budgets', {
      method: 'POST',
      body: { category: categoria, limit: limite },
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
