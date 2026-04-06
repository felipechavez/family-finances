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
  } = useFetch('/accounts', {
    key: 'cuentas',
    $fetch: $api as typeof $fetch,
    transform: (res: Account[]) => res,
    default: (): Account[] => [],
  })

  const cuentas = computed(() => response.value ?? [])

  const balanceTotal = computed(() =>
    cuentas.value.reduce((sum, c) => sum + c.balance, 0),
  )

  async function crear(input: AccountCreateInput): Promise<Account> {
    const res = await ($api as typeof $fetch)<Account>('/accounts', {
      method: 'POST',
      body: input,
    })
    await refresh()
    return res
  }

  async function eliminar(id: string): Promise<void> {
    await ($api as typeof $fetch)(`/accounts/${id}`, { method: 'DELETE' })
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
