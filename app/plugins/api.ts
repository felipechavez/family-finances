// app/plugins/api.ts
// Creates a pre-configured $fetch instance with JWT auth header.
// All stores use $api instead of raw $fetch so the token is injected automatically.

import { useAuthStore } from '~/stores/auth'

export default defineNuxtPlugin(() => {
  const auth = useAuthStore()
  const config = useRuntimeConfig()

  const api = $fetch.create({
    baseURL: config.public.apiBase,
    onRequest({ options }) {
      if (auth.token) {
        const headers = new Headers(options.headers as HeadersInit)
        headers.set('Authorization', `Bearer ${auth.token}`)
        options.headers = headers
      }
    },
    onResponseError({ response }) {
      if (response.status === 401) {
        auth.logout()
      }
    },
  })

  return {
    provide: { api },
  }
})
