// app/stores/auth.ts
import { defineStore } from 'pinia'
import type { User, LoginInput, RegisterInput } from '#shared/types'

interface LoginResult {
  token: string
  userId: string
  name: string
  email: string
  familyId: string | null
  requiresTwoFactor?: boolean
  challengeToken?: string | null
}

export interface LoginOutcome {
  requiresTwoFactor: boolean
  challengeToken?: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const token = shallowRef<string | null>(null)
  const familyId = shallowRef<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)
  const userName = computed(() => user.value?.name ?? '')
  const hasFamiliy = computed(() => !!familyId.value)

  function setAuth(response: LoginResult) {
    const userData = {
      id: response.userId,
      name: response.name,
      email: response.email,
      createdAt: '',
    }
    token.value = response.token
    user.value = userData
    familyId.value = response.familyId ?? null
    if (import.meta.client) {
      localStorage.setItem('auth_token', response.token)
      localStorage.setItem('auth_user', JSON.stringify(userData))
      if (response.familyId) {
        localStorage.setItem('auth_family_id', response.familyId)
      } else {
        localStorage.removeItem('auth_family_id')
      }
    }
  }

  /** Called after creating or joining a family to refresh the token with the new family_id claim. */
  function updateToken(newToken: string, newFamilyId: string) {
    token.value = newToken
    familyId.value = newFamilyId
    if (import.meta.client) {
      localStorage.setItem('auth_token', newToken)
      localStorage.setItem('auth_family_id', newFamilyId)
    }
  }

  function clearAuth() {
    token.value = null
    user.value = null
    familyId.value = null
    if (import.meta.client) {
      localStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')
      localStorage.removeItem('auth_family_id')
    }
  }

  async function login(input: LoginInput): Promise<LoginOutcome> {
    const $api = useNuxtApp().$api as typeof $fetch
    const res = await $api('/auth/login', {
      method: 'POST',
      body: input,
    }) as LoginResult

    if (res.requiresTwoFactor) {
      return { requiresTwoFactor: true, challengeToken: res.challengeToken ?? undefined }
    }

    setAuth(res)
    return { requiresTwoFactor: false }
  }

  async function verify2Fa(challengeToken: string, code: string): Promise<void> {
    const $api = useNuxtApp().$api as typeof $fetch
    const res = await $api('/auth/verify-2fa', {
      method: 'POST',
      body: { challengeToken, code },
    }) as LoginResult
    setAuth(res)
  }

  async function register(input: RegisterInput): Promise<void> {
    const $api = useNuxtApp().$api as typeof $fetch
    await $api('/auth/register', {
      method: 'POST',
      body: input,
    })
  }

  async function changePassword(payload: { currentPassword: string; newPassword: string; confirmNewPassword: string }): Promise<void> {
    const $api = useNuxtApp().$api as typeof $fetch
    await $api('/auth/change-password', {
      method: 'PATCH',
      body: payload,
    })
  }

  async function initiateEmailChange(newEmail: string): Promise<void> {
    const $api = useNuxtApp().$api as typeof $fetch
    await $api('/auth/initiate-email-change', {
      method: 'POST',
      body: { newEmail },
    })
  }

  function logout() {
    clearAuth()
    navigateTo('/auth/login')
  }

  function initFromStorage() {
    if (import.meta.client) {
      const storedToken = localStorage.getItem('auth_token')
      const storedUser = localStorage.getItem('auth_user')
      const storedFamilyId = localStorage.getItem('auth_family_id')
      if (storedToken) {
        token.value = storedToken
      }
      if (storedUser) {
        try {
          user.value = JSON.parse(storedUser)
        } catch {
          localStorage.removeItem('auth_user')
        }
      }
      if (storedFamilyId) {
        familyId.value = storedFamilyId
      }
    }
  }

  return {
    user,
    token,
    familyId,
    isAuthenticated,
    userName,
    hasFamiliy,
    login,
    verify2Fa,
    register,
    changePassword,
    initiateEmailChange,
    logout,
    updateToken,
    initFromStorage,
  }
})
