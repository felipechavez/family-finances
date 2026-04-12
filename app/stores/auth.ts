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
  const user = ref<User | null>(null)
  const token = shallowRef<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)
  const userName = computed(() => user.value?.name ?? '')

  function setAuth(response: LoginResult) {
    const userData = {
      id: response.userId,
      name: response.name,
      email: response.email,
      createdAt: '',
    }
    token.value = response.token
    user.value = userData
    if (import.meta.client) {
      localStorage.setItem('auth_token', response.token)
      localStorage.setItem('auth_user', JSON.stringify(userData))
    }
  }

  function clearAuth() {
    token.value = null
    user.value = null
    if (import.meta.client) {
      localStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')
    }
  }

  async function login(input: LoginInput): Promise<void> {
    const $api = useNuxtApp().$api as typeof $fetch
    const res = await $api('/auth/login', {
      method: 'POST',
      body: input,
    })
    setAuth(res as LoginResult)
  }

  async function register(input: RegisterInput): Promise<void> {
    const $api = useNuxtApp().$api as typeof $fetch
    await $api('/auth/register', {
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
      const storedToken = localStorage.getItem('auth_token')
      const storedUser = localStorage.getItem('auth_user')
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
