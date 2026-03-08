// app/composables/presupuestos/use-presupuestos.ts
// Exports functions ONLY — no types (per imports-composable-exports rule)

import type { Presupuesto } from '#shared/types'

export function usePresupuestos(mes: Ref<string>) {
  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch('/api/presupuestos', {
    key: () => `presupuestos-${mes.value}`,
    query: { mes },
    // Transform at fetch time (data-transform rule)
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
    await $fetch('/api/presupuestos', {
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
}
