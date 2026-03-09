// app/stores/transacciones.ts
import { defineStore } from 'pinia'
import type { Transaccion, TransaccionCreateInput } from '#shared/types'
export const useTransaccionesStore = defineStore('transacciones', () => {
  const { $api } = useNuxtApp()
  const mes = ref(new Date().toISOString().slice(0, 7))

  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch('/api/transacciones', {
    key: () => `transacciones-${mes.value}`,
    query: { mes },
    $fetch: $api as typeof $fetch,
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

  async function crear(input: TransaccionCreateInput): Promise<Transaccion> {
    const res = await ($api as typeof $fetch)<{ data: Transaccion }>('/api/transacciones', {
      method: 'POST',
      body: input,
    })
    await refresh()
    return res.data
  }

  async function eliminar(id: string): Promise<void> {
    await ($api as typeof $fetch)(`/api/transacciones/${id}`, { method: 'DELETE' })
    await refresh()
  }

  function setMes(nuevoMes: string) {
    mes.value = nuevoMes
  }

  return {
    mes,
    transacciones,
    status,
    error,
    refresh,
    totalIngresos,
    totalGastos,
    balance,
    gastosPorCategoria,
    crear,
    eliminar,
    setMes,
  }
})
