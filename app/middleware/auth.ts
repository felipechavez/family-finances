// app/middleware/auth.ts
// Protects routes that require authentication

export default defineNuxtRouteMiddleware((to) => {
  if (import.meta.server) return

  const token = localStorage.getItem('auth_token')

  if (!token) {
    return navigateTo('/auth/login')
  }
})
