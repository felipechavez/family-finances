// app/stores/presupuestos.ts
import { defineStore, storeToRefs } from 'pinia'
import type { Presupuesto } from '#shared/types'
import { useTransaccionesStore } from './transacciones'

export const usePresupuestosStore = defineStore('presupuestos', () => {
  const { $api } = useNuxtApp()
  const txStore = useTransaccionesStore()
  const { mes } = storeToRefs(txStore)

  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch('/api/presupuestos', {
    key: () => `presupuestos-${mes.value}`,
    query: { mes },
    $fetch: $api as typeof $fetch,
    transform: (res: { data: Presupuesto[] }) => res.data,
    default: (): Presupuesto[] => [],
  })

  const presupuestos = computed(() => response.value ?? [])

  const presupuestoPorCategoria = computed(() => {
    const map: Record<string, number> = {}
    presupuestos.value.forEach(p => { map[p.categoria] = p.limite })
    return map
  })

  async function guardarLimite(categoria: string, limite: number): Promise<void> {
    await ($api as typeof $fetch)('/api/presupuestos', {
      method: 'PUT',
      body: { categoria, limite, mes: mes.value },
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
