// app/plugins/theme.client.ts
// Runs before first paint (client-only) to apply the stored theme immediately,
// preventing a flash of dark theme when the user prefers light mode.

export default defineNuxtPlugin(() => {
  const stored = localStorage.getItem('app-theme')
  const dark = stored !== 'light'
  document.documentElement.setAttribute('data-theme', dark ? 'dark' : 'light')
})
