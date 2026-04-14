// app/stores/cuentas.ts
import { defineStore } from 'pinia'
import type { Account, AccountCreateInput, AccountUpdateInput } from '#shared/types'

export const useCuentasStore = defineStore('cuentas', () => {
  function getApi() {
    return useNuxtApp().$api as typeof $fetch
  }

  const {
    data: response,
    status,
    error,
    refresh,
  } = useFetch('/accounts', {
    key: 'cuentas',
    server: false,
    $fetch: getApi(),
    transform: (res: Account[]) => res,
    default: (): Account[] => [],
  })

  const cuentas = computed(() => response.value ?? [])

  const balanceTotal = computed(() =>
    cuentas.value.reduce((sum, c) => sum + c.balance, 0),
  )

  async function crear(input: AccountCreateInput): Promise<Account> {
    const $api = getApi()
    const res = await $api<Account>('/accounts', {
      method: 'POST',
      body: input,
    })
    await refresh()
    return res
  }

  async function actualizar(id: string, input: AccountUpdateInput): Promise<void> {
    const $api = getApi()
    await $api(`/accounts/${id}`, {
      method: 'PATCH',
      body: input,
    })
    await refresh()
  }

  async function eliminar(id: string): Promise<void> {
    const $api = getApi()
    await $api(`/accounts/${id}`, { method: 'DELETE' })
    await refresh()
  }

  return {
    cuentas,
    balanceTotal,
    status,
    error,
    refresh,
    crear,
    actualizar,
    eliminar,
  }
})
