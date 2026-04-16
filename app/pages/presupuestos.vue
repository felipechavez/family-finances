<!-- app/pages/presupuestos.vue -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useTransaccionesStore } from '~/stores/transacciones'
import { usePresupuestosStore } from '~/stores/presupuestos'
import { useCategorias } from '~/composables/use-categorias'
import { useToast } from '~/composables/use-toast'
import { useApiError } from '~/composables/use-api-error'

definePageMeta({ middleware: 'auth' })

const { t } = useI18n()
useHead({ title: 'Presupuestos - DomusPay' })

const txStore = useTransaccionesStore()
const presupStore = usePresupuestosStore()
const { categoriasGasto } = useCategorias()
const { ok: toastOk, error: toastError } = useToast()

const { gastosPorCategoria } = storeToRefs(txStore)
const { presupuestoPorCategoria, status, error: presupError } = storeToRefs(presupStore)
const { message: errorMessage } = useApiError(presupError)

async function handleGuardarLimite(categoria: string, limite: number) {
  try {
    await presupStore.guardarLimite(categoria, limite)
    toastOk(t('presupuestos.limiteSaved'))
  } catch {
    toastError(t('presupuestos.noSePudoGuardar'))
  }
}
</script>

<template>
  <div>
    <header class="header">
      <div>
        <h1 class="header-titulo">{{ $t('presupuestos.title') }}</h1>
        <p class="header-sub">{{ $t('presupuestos.subtitle') }}</p>
      </div>
    </header>

    <main class="main">
      <UiSpinner v-if="status === 'pending'">{{ $t('common.loading') }}</UiSpinner>
      <UiError404 v-else-if="presupError" :message="errorMessage" />
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
  border-bottom: 1px solid var(--border-subtle);
  padding-bottom: 16px;
  margin-bottom: 4px;
}
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 0 0 4px; }
.header-sub { font-size: 13px; color: var(--text-muted); margin: 0; }

.main { padding: 16px 16px 100px; flex: 1; }

.presupuesto-grid { display: grid; grid-template-columns: 1fr; gap: 0; }

@media (min-width: 768px) {
  .header { max-width: 1100px; margin-inline: auto; padding-inline: 32px; width: 100%; }
  .main { padding: 24px 32px 40px; max-width: 1100px; width: 100%; margin-inline: auto; }
  .presupuesto-grid { grid-template-columns: repeat(2, 1fr); gap: 12px; }
}
@media (min-width: 1280px) {
  .presupuesto-grid { grid-template-columns: repeat(3, 1fr); }
}
</style>
