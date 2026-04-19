// app/composables/use-theme.ts
// Direct DOM approach — avoids VueUse's SSR indirection that can skip
// document.documentElement.setAttribute in Nuxt environments.
// The companion plugin (plugins/theme.client.ts) applies the stored theme
// immediately on load so there is no dark→light flash.

const STORAGE_KEY = 'app-theme'

function applyTheme(dark: boolean) {
  if (import.meta.client) {
    document.documentElement.setAttribute('data-theme', dark ? 'dark' : 'light')
  }
}

// Singleton compartido entre todos los componentes
const isDark = ref(true)

export function useTheme() {
  onMounted(() => {
    const stored = localStorage.getItem(STORAGE_KEY)
    isDark.value = stored !== 'light'
    applyTheme(isDark.value)
  })

  const toggle = () => {
    isDark.value = !isDark.value
    localStorage.setItem(STORAGE_KEY, isDark.value ? 'dark' : 'light')
    applyTheme(isDark.value)
  }

  return { isDark, toggle }
}
