// app/stores/auth.ts
import { defineStore } from 'pinia'
import type { User, Family, LoginInput, RegisterInput, AuthResponse } from '#shared/types'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const family = ref<Family | null>(null)
  const token = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)
  const userName = computed(() => user.value?.name ?? '')

  function setAuth(response: AuthResponse) {
    token.value = response.token
    user.value = response.user
    family.value = response.family
    if (import.meta.client) {
      localStorage.setItem('auth_token', response.token)
    }
  }

  function clearAuth() {
    token.value = null
    user.value = null
    family.value = null
    if (import.meta.client) {
      localStorage.removeItem('auth_token')
    }
  }

  async function login(input: LoginInput): Promise<void> {
    const res = await $fetch<{ data: AuthResponse }>('/api/auth/login', {
      method: 'POST',
      body: input,
    })
    setAuth(res.data)
  }

  async function register(input: RegisterInput): Promise<void> {
    await $fetch('/api/auth/register', {
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
    family,
    token,
    isAuthenticated,
    userName,
    login,
    register,
    logout,
    initFromStorage,
    setAuth,
  }
})
