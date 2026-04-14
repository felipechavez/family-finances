// app/middleware/auth.ts
// Protects routes that require authentication and family membership.
// - No token → /auth/login
// - Token but no family (and not on /familia/*) → /familia/setup

export default defineNuxtRouteMiddleware((to) => {
  if (import.meta.server) return

  const token = localStorage.getItem('auth_token')

  if (!token) {
    return navigateTo('/auth/login')
  }

  if (!to.path.startsWith('/familia')) {
    const storedFamilyId = localStorage.getItem('auth_family_id')
    if (!storedFamilyId) {
      return navigateTo('/familia/setup')
    }
  }
})
