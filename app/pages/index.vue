<!-- app/pages/index.vue -->
<!-- Dashboard page — protected by auth middleware -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useTransaccionesStore } from '~/stores/transacciones'
import { usePresupuestosStore } from '~/stores/presupuestos'
import { useFormato } from '~/composables/use-formato'
import { useApiError } from '~/composables/use-api-error'

definePageMeta({ middleware: 'auth' })

const { t } = useI18n()
useHead({ title: 'Dashboard - DomusPay' })

const txStore = useTransaccionesStore()
const presupStore = usePresupuestosStore()
const { formatMesLabel } = useFormato()

const {
  transacciones,
  status: statusTx,
  error: errorTx,
  totalIngresos,
  totalGastos,
  balance,
  gastosPorCategoria,
  mes,
} = storeToRefs(txStore)  


const { message: errorMessage } = useApiError(errorTx)

const { presupuestoPorCategoria } = storeToRefs(presupStore)

const mesActual = new Date().toISOString().slice(0, 7)

const mesesDisponibles = computed(() => {
  const meses = new Set(transacciones.value.map(trx => trx.fecha.slice(0, 7)))
  meses.add(mesActual)
  return Array.from(meses).sort().reverse()
})
</script>

<template>
  <div>
    <!-- Header -->
    <header class="header">
      <div>
        <p class="header-sub">{{ $t('dashboard.family') }}</p>
        <h1 class="header-titulo">{{ $t('dashboard.title') }}</h1>
      </div>
      <select v-model="mes" class="select-mes">
        <option v-for="m in mesesDisponibles" :key="m" :value="m">
          {{ formatMesLabel(m) }}
        </option>
      </select>
    </header>

    <main class="main">
      <UiSpinner v-if="statusTx === 'pending'">{{ $t('common.loading') }}</UiSpinner>
      <UiError404 v-else-if="errorTx" :message="errorMessage" />     
      <template v-else>
        <div class="resumen-grid">
          <div class="resumen-col-izq">
            <DashboardResumenBalance
              :total-ingresos="totalIngresos"
              :total-gastos="totalGastos"
              :balance="balance"
            />
            <DashboardGastosCategorias
              :gastos-por-categoria="gastosPorCategoria"
              :presupuesto-por-categoria="presupuestoPorCategoria"
              :total-gastos="totalGastos"
            />
          </div>
          <div class="resumen-col-der">
            <div v-if="transacciones.length" class="card">
              <div class="card-header">
                <h3 class="card-titulo">{{ $t('dashboard.recentMovements') }}</h3>
                <NuxtLink to="/transacciones" class="btn-ver-todos">{{ $t('dashboard.seeAll') }}</NuxtLink>
              </div>
              <TransaccionesRow
                v-for="tx in transacciones.slice(0, 6)"
                :key="tx.id"
                :transaccion="tx"
              />
            </div>
          </div>
        </div>
      </template>
    </main>

    <UiToast />
  </div>
</template>

<style scoped>
.header {
  padding: 20px 20px 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid var(--border-subtle);
  padding-bottom: 16px;
  margin-bottom: 4px;
}
.header-sub { font-size: 11px; color: var(--text-muted); font-weight: 600; letter-spacing: 1px; text-transform: uppercase; margin: 0; }
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 4px 0 0; }

.select-mes {
  background: var(--bg-input); border: 1.5px solid var(--border-accent); border-radius: 10px;
  color: var(--accent-soft); padding: 8px 12px; font-size: 13px; font-weight: 600; outline: none;
  -webkit-appearance: none; cursor: pointer;
}

.main { padding: 16px 16px 100px; flex: 1; }

.card { background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 18px; padding: 16px; margin-bottom: 14px; }
.card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
.card-titulo { font-size: 14px; font-weight: 700; color: var(--accent-light); margin: 0; }
.btn-ver-todos { background: none; border: none; color: var(--accent); font-size: 13px; font-weight: 600; cursor: pointer; padding: 0; text-decoration: none; }

.error-state { text-align: center; color: var(--danger); padding: 40px 20px; }

.resumen-grid { display: flex; flex-direction: column; }

@media (min-width: 768px) {
  .header { max-width: 1100px; margin-inline: auto; padding-inline: 32px; width: 100%; }
  .main { padding: 24px 32px 40px; max-width: 1100px; width: 100%; margin-inline: auto; }
  .resumen-grid { flex-direction: row; gap: 24px; align-items: flex-start; }
  .resumen-col-izq { flex: 1; min-width: 0; }
  .resumen-col-der { width: 360px; flex-shrink: 0; }
}
@media (min-width: 1280px) {
  .main { padding: 32px 48px 40px; }
  .resumen-col-der { width: 400px; }
}
</style>
