// app/plugins/api.ts
// Creates a pre-configured $fetch instance with JWT auth header.
// All stores use $api instead of raw $fetch so the token is injected automatically.
//
// To switch to the .NET backend: change BASE_URL below.

import { useAuthStore } from '~/stores/auth'

const BASE_URL = '/api' // Change to 'https://your-dotnet-api.com/api' when ready

export default defineNuxtPlugin(() => {
  const auth = useAuthStore()

  const api = $fetch.create({
    baseURL: BASE_URL,
    onRequest({ options }) {
      if (auth.token) {
        options.headers = {
          ...options.headers,
          Authorization: `Bearer ${auth.token}`,
        }
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
