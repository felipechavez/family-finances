// app/stores/transacciones.ts
import { defineStore } from 'pinia'
import type { Transaccion, TransaccionCreateInput, CategoriaId, TipoMovimiento } from '#shared/types'

interface TransactionDto {
  id: string
  familyId: string
  accountId: string
  userId: string
  categoryId: string
  categoryName: string
  type: 'Income' | 'Expense'
  amount: number
  currency: string
  description: string
  transactionDate: string
  createdAt: string
}

export const useTransaccionesStore = defineStore('transacciones', () => {
  function getApi() {
    const nuxtApp = useNuxtApp()
    return nuxtApp.$api as typeof $fetch
  }
  const mes = shallowRef(new Date().toISOString().slice(0, 7))

  const query = computed(() => {
    const [year, month] = mes.value.split('-').map(Number)
    return {
      year,
      month,
    }
  })

  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch<TransactionDto[]>('/transactions', {
    key: () => `transacciones-${mes.value}`,
    server: false,
    query,
    $fetch: getApi(),
    default: (): TransactionDto[] => [],
  })

  const transacciones = computed<Transaccion[]>(() =>
    (response.value ?? [])
      .map(tx => ({
        id: tx.id,
        familyId: tx.familyId,
        accountId: tx.accountId,
        userId: tx.userId,
        tipo: tx.type === 'Expense' ? 'gasto' : 'ingreso' as TipoMovimiento,
        categoria: tx.categoryName as CategoriaId,
        monto: tx.amount,
        currency: tx.currency,
        descripcion: tx.description,
        fecha: tx.transactionDate,
        creadoEn: tx.createdAt,
      }))
      .sort((a, b) => b.fecha.localeCompare(a.fecha)),
  )

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
    const $api = getApi()
    const res = await $api<Transaccion>('/transactions', {
      method: 'POST',
      body: input,
    })
    await refresh()
    return res
  }

  async function eliminar(id: string): Promise<void> {
    const $api = getApi()
    await $api(`/transactions/${id}`, { method: 'DELETE' })
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
