<!-- app/pages/presupuestos.vue -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useTransaccionesStore } from '~/stores/transacciones'
import { usePresupuestosStore } from '~/stores/presupuestos'
import { useCategorias } from '~/composables/use-categorias'
import { useToast } from '~/composables/use-toast'

definePageMeta({ middleware: 'auth' })
useHead({ title: 'Presupuestos - FinanzasApp' })

const txStore = useTransaccionesStore()
const presupStore = usePresupuestosStore()
const { categoriasGasto } = useCategorias()
const { ok: toastOk, error: toastError } = useToast()

const { gastosPorCategoria } = storeToRefs(txStore)
const { presupuestoPorCategoria, status, error: presupError } = storeToRefs(presupStore)

async function handleGuardarLimite(categoria: string, limite: number) {
  try {
    await presupStore.guardarLimite(categoria, limite)
    toastOk('Límite guardado')
  } catch {
    toastError('No se pudo guardar el límite')
  }
}
</script>

<template>
  <div>
    <header class="header">
      <div>
        <h1 class="header-titulo">Presupuestos</h1>
        <p class="header-sub">Define cuánto quieres gastar por categoría cada mes</p>
      </div>
    </header>

    <main class="main">
      <UiSpinner v-if="status === 'pending'">Cargando...</UiSpinner>
      <div v-else-if="presupError" class="error-state"><p>Error al cargar presupuestos</p></div>
      <template v-else>
        <div class="presupuesto-grid">
          <PresupuestosCard
            v-for="cat in categoriasGasto"
            :key="cat.id"
            :categoria="cat"
            :gastado="gastosPorCategoria[cat.id] ?? 0"
            :limite="presupuestoPorCategoria[cat.id] ?? 0"
            @guardar-limite="handleGuardarLimite"
          />
        </div>
      </template>
    </main>

    <UiToast />
  </div>
</template>

<style scoped>
.header {
  padding: 20px 20px 0;
  border-bottom: 1px solid #1a1a2e;
  padding-bottom: 16px;
  margin-bottom: 4px;
}
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 0 0 4px; }
.header-sub { font-size: 13px; color: #6b6b8a; margin: 0; }

.main { padding: 16px 16px 100px; flex: 1; }

.presupuesto-grid { display: grid; grid-template-columns: 1fr; gap: 0; }

.error-state { text-align: center; color: #f87171; padding: 40px 20px; }

@media (min-width: 768px) {
  .main { padding: 24px 32px 40px; max-width: 1100px; width: 100%; }
  .presupuesto-grid { grid-template-columns: repeat(2, 1fr); gap: 12px; }
}
@media (min-width: 1280px) {
  .presupuesto-grid { grid-template-columns: repeat(3, 1fr); }
}
</style>
