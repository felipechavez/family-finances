// app/composables/index.ts
// Barrel ROOT requerido por Nuxt para auto-importar composables en subdirectorios.
// SOLO incluir composables que estén en subcarpetas — los que están en la raíz
// de composables/ ya son escaneados directamente por Nuxt y NO deben ir aquí
// (causaría "Duplicated imports"). Ver: imports-no-barrel-autoimport rule.

export { useTransacciones } from './transacciones/use-transacciones'
export { usePresupuestos } from './presupuestos/use-presupuestos'
