// app/composables/transacciones/use-transacciones.ts
// Exports functions ONLY — no types (per imports-composable-exports rule)

import type { Transaccion } from '#shared/types'
import type { FormTransaccion } from '~/types/ui'

export function useTransacciones(mes: Ref<string>) {
  // useFetch with unique key that reacts to mes changes (data-key-unique rule)
  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch('/api/transacciones', {
    key: () => `transacciones-${mes.value}`,
    query: { mes },
    // Transform at fetch time, not in template (data-transform rule)
    transform: (res: { data: Transaccion[] }) =>
      [...res.data].sort((a, b) => b.fecha.localeCompare(a.fecha)),
    default: (): Transaccion[] => [],
  })

  const transacciones = computed(() => response.value ?? [])

  const totalIngresos = computed(() =>
    transacciones.value
      .filter(t => t.tipo === 'ingreso')
      .reduce((sum, t) => sum + t.monto, 0),
  )

  const totalGastos = computed(() =>
    transacciones.value
      .filter(t => t.tipo === 'gasto')
      .reduce((sum, t) => sum + t.monto, 0),
  )

  const balance = computed(() => totalIngresos.value - totalGastos.value)

  const gastosPorCategoria = computed(() => {
    const map: Record<string, number> = {}
    transacciones.value
      .filter(t => t.tipo === 'gasto')
      .forEach(t => { map[t.categoria] = (map[t.categoria] ?? 0) + t.monto })
    return map
  })

  async function crearTransaccion(form: FormTransaccion): Promise<Transaccion> {
    const body = {
      tipo: form.tipo,
      categoria: form.categoria,
      monto: Number(form.monto),
      descripcion: form.descripcion.trim(),
      fecha: form.fecha,
      miembro: form.miembro,
    }

    const res = await $fetch<{ data: Transaccion }>('/api/transacciones', {
      method: 'POST',
      body,
    })

    await refresh()
    return res.data
  }

  async function eliminarTransaccion(id: string): Promise<void> {
    await $fetch(`/api/transacciones/${id}`, { method: 'DELETE' })
    await refresh()
  }

  return {
    transacciones,
    status,
    error,
    refresh,
    totalIngresos,
    totalGastos,
    balance,
    gastosPorCategoria,
    crearTransaccion,
    eliminarTransaccion,
  }
}
