// app/plugins/auth.ts
// Initializes auth state from localStorage on app start

import { useAuthStore } from '~/stores/auth'

export default defineNuxtPlugin(() => {
  const auth = useAuthStore()
  auth.initFromStorage()
})
