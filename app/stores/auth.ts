// app/stores/auth.ts
import { defineStore } from 'pinia'
import type { User, LoginInput, RegisterInput } from '#shared/types'

interface LoginResult {
  token: string
  userId: string
  name: string
  email: string
  familyId: string | null
}

export const useAuthStore = defineStore('auth', () => {
  const { $api } = useNuxtApp()
  const user = ref<User | null>(null)
  const token = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)
  const userName = computed(() => user.value?.name ?? '')

  function setAuth(response: LoginResult) {
    token.value = response.token
    user.value = {
      id: response.userId,
      name: response.name,
      email: response.email,
      createdAt: '',
    }
    if (import.meta.client) {
      localStorage.setItem('auth_token', response.token)
    }
  }

  function clearAuth() {
    token.value = null
    user.value = null
    if (import.meta.client) {
      localStorage.removeItem('auth_token')
    }
  }

  async function login(input: LoginInput): Promise<void> {
    const res = await ($api as typeof $fetch)('/auth/login', {
      method: 'POST',
      body: input,
    })
    setAuth(res as LoginResult)
  }

  async function register(input: RegisterInput): Promise<void> {
    await ($api as typeof $fetch)('/auth/register', {
      method: 'POST',
      body: input,
    })
  }

  function logout() {
    clearAuth()
    navigateTo('/auth/login')
  }

  function initFromStorage() {
    if (import.meta.client) {
      const stored = localStorage.getItem('auth_token')
      if (stored) {
        token.value = stored
      }
    }
  }

  return {
    user,
    token,
    isAuthenticated,
    userName,
    login,
    register,
    logout,
    initFromStorage,
  }
})
