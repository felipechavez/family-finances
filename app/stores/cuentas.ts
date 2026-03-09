// app/stores/cuentas.ts
import { defineStore } from 'pinia'
import type { Account, AccountCreateInput } from '#shared/types'

export const useCuentasStore = defineStore('cuentas', () => {
  const { $api } = useNuxtApp()

  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch('/api/cuentas', {
    key: 'cuentas',
    $fetch: $api as typeof $fetch,
    transform: (res: { data: Account[] }) => res.data,
    default: (): Account[] => [],
  })

  const cuentas = computed(() => response.value ?? [])

  const balanceTotal = computed(() =>
    cuentas.value.reduce((sum, c) => sum + c.balance, 0),
  )

  async function crear(input: AccountCreateInput): Promise<Account> {
    const res = await ($api as typeof $fetch)<{ data: Account }>('/api/cuentas', {
      method: 'POST',
      body: input,
    })
    await refresh()
    return res.data
  }

  async function eliminar(id: string): Promise<void> {
    await ($api as typeof $fetch)(`/api/cuentas/${id}`, { method: 'DELETE' })
    await refresh()
  }

  return {
    cuentas,
    balanceTotal,
    status,
    error,
    refresh,
    crear,
    eliminar,
  }
})
