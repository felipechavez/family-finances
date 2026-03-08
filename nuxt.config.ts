// nuxt.config.ts
import tailwindcss from '@tailwindcss/vite'

export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',

  // Habilitar estructura de carpetas de Nuxt 4 (app/ como directorio raíz de la app)
  future: {
    compatibilityVersion: 4,
  },

  // Tailwind v4 via Vite plugin
  vite: {
    plugins: [tailwindcss()],
  },

  // Strict TypeScript (types-no-any rule)
  typescript: {
    strict: true,
    typeCheck: true,
  },

  // Route-level rendering rules (rendering-route-rules rule)
  routeRules: {
    '/': { ssr: true },
    '/api/**': { cors: false },
  },

  // Alias para tipos compartidos (types-import-paths rule)
  // '#shared/types' apunta directamente al archivo, sin barrel index.ts
  // para evitar "Duplicated imports" warnings de Nuxt
  alias: {
    '#shared/types': './shared/types/finanzas',
  },

  // Auto-import de componentes (imports-component-naming rule)
  components: [
    { path: '~/components', pathPrefix: true },
  ],

  // Módulos
  modules: [
    '@vueuse/nuxt',
  ],

  // CSS global
  css: ['~/assets/css/main.css'],

  // App head
  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1, maximum-scale=1',
      title: 'FinanzasApp Familiar',
      meta: [
        { name: 'description', content: 'Gestión de finanzas familiares' },
        { name: 'theme-color', content: '#0f0f14' },
        { name: 'mobile-web-app-capable', content: 'yes' },
        { name: 'apple-mobile-web-app-capable', content: 'yes' },
        { name: 'apple-mobile-web-app-status-bar-style', content: 'black-translucent' },
      ],
    },
  },
})
