// nuxt.config.ts
import tailwindcss from '@tailwindcss/vite'

export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',

  future: {
    compatibilityVersion: 4,
  },

  vite: {
    plugins: [tailwindcss()],
  },

  typescript: {
    strict: true,
    typeCheck: false,
  },

  routeRules: {
    '/': { ssr: true },
    '/api/**': { cors: false },
  },

  alias: {
    '#shared/types': './shared/types/finanzas',
  },

  components: [
    { path: '~/components', pathPrefix: true },
  ],

  modules: [
    '@vueuse/nuxt',
    '@pinia/nuxt',
  ],

  css: ['~/assets/css/main.css'],

  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1',
      title: 'FinanzasApp Familiar',
      meta: [
        { name: 'description', content: 'Gestión de finanzas familiares' },
        { name: 'theme-color', content: '#0f0f14' },
        { name: 'mobile-web-app-capable', content: 'yes' },
        { name: 'apple-mobile-web-app-capable', content: 'yes' },
        { name: 'apple-mobile-web-app-status-bar-style', content: 'black-translucent' },
      ],
      link: [
        { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css2?family=DM+Sans:wght@300;400;500;600;700&display=swap' },
      ],
    },
  },
})
