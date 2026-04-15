<!-- app/pages/reportes.vue -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useTransaccionesStore } from '~/stores/transacciones'
import { useCategorias } from '~/composables/use-categorias'
import { useFormato } from '~/composables/use-formato'
import { useApiError } from '~/composables/use-api-error'

definePageMeta({ middleware: 'auth' })

const { t } = useI18n()
useHead({ title: 'Reportes - FinanzasApp' })

const txStore = useTransaccionesStore()
const { categoriasGasto } = useCategorias()
const { formatCLP, pct } = useFormato()

const {
  transacciones,
  totalIngresos,
  totalGastos,
  balance,
  gastosPorCategoria,
  status,
  error: txError,
} = storeToRefs(txStore)
const { message: errorMessage } = useApiError(txError)

// Distribución de gastos para chart/tabla
const distribucion = computed(() => {
  return categoriasGasto.value
    .filter(c => gastosPorCategoria.value[c.id] !== undefined)
    .map(c => ({
      ...c,
      monto: gastosPorCategoria.value[c.id] ?? 0,
      porcentaje: pct(gastosPorCategoria.value[c.id] ?? 0, totalGastos.value),
    }))
    .sort((a, b) => b.monto - a.monto)
})
</script>

<template>
  <div>
    <header class="header">
      <h1 class="header-titulo">{{ $t('reportes.title') }}</h1>
    </header>

    <main class="main">
      <UiSpinner v-if="status === 'pending'">{{ $t('common.loading') }}</UiSpinner>
      <UiError404 v-else-if="txError" :message="errorMessage" />
      <template v-else>
        <!-- Resumen mensual -->
        <section class="seccion">
          <h2 class="seccion-titulo">{{ $t('reportes.resumenMensual') }}</h2>
          <div class="resumen-cards">
            <div class="resumen-item resumen-item--ingreso">
              <p class="resumen-label">{{ $t('reportes.ingresos') }}</p>
              <p class="resumen-valor">{{ formatCLP(totalIngresos) }}</p>
            </div>
            <div class="resumen-item resumen-item--gasto">
              <p class="resumen-label">{{ $t('reportes.gastos') }}</p>
              <p class="resumen-valor">{{ formatCLP(totalGastos) }}</p>
            </div>
            <div class="resumen-item" :class="balance >= 0 ? 'resumen-item--positivo' : 'resumen-item--negativo'">
              <p class="resumen-label">{{ $t('reportes.balance') }}</p>
              <p class="resumen-valor">{{ formatCLP(balance) }}</p>
            </div>
          </div>
        </section>

        <!-- Distribución de gastos -->
        <section class="seccion">
          <h2 class="seccion-titulo">{{ $t('reportes.distribucion') }}</h2>
          <div v-if="distribucion.length === 0" class="empty-state">
            <p>{{ $t('reportes.sinGastos') }}</p>
          </div>
          <div v-else class="distribucion-list">
            <div v-for="item in distribucion" :key="item.id" class="dist-row">
              <div class="dist-header">
                <span class="dist-emoji">{{ item.emoji }}</span>
                <span class="dist-label">{{ item.label }}</span>
                <span class="dist-pct">{{ item.porcentaje }}%</span>
                <span class="dist-monto">{{ formatCLP(item.monto) }}</span>
              </div>
              <div class="dist-bar-track">
                <div
                  class="dist-bar-fill"
                  :style="{ width: `${item.porcentaje}%`, background: item.color }"
                />
              </div>
            </div>
          </div>
        </section>

        <!-- Estadísticas -->
        <section class="seccion">
          <h2 class="seccion-titulo">{{ $t('reportes.estadisticas') }}</h2>
          <div class="stats-grid">
            <div class="stat-card">
              <p class="stat-label">{{ $t('reportes.totalTransacciones') }}</p>
              <p class="stat-valor">{{ transacciones.length }}</p>
            </div>
            <div class="stat-card">
              <p class="stat-label">{{ $t('reportes.promedioPorGasto') }}</p>
              <p class="stat-valor">
                {{ formatCLP(transacciones.filter(tx => tx.tipo === 'gasto').length > 0
                  ? totalGastos / transacciones.filter(tx => tx.tipo === 'gasto').length
                  : 0) }}
              </p>
            </div>
            <div class="stat-card">
              <p class="stat-label">{{ $t('reportes.categoriaMasAlta') }}</p>
              <p class="stat-valor">{{ distribucion[0]?.label ?? '-' }}</p>
            </div>
          </div>
        </section>
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
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 0; }

.main { padding: 16px 16px 100px; flex: 1; }

.seccion { margin-bottom: 28px; }
.seccion-titulo { font-size: 16px; font-weight: 700; color: var(--accent-light); margin: 0 0 14px; }

.resumen-cards { display: grid; grid-template-columns: 1fr; gap: 12px; }
.resumen-item {
  background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 16px; padding: 16px;
  text-align: center;
}
.resumen-item--ingreso { border-color: rgba(34,197,94,0.3); }
.resumen-item--gasto { border-color: rgba(239,68,68,0.3); }
.resumen-item--positivo { border-color: rgba(34,197,94,0.3); }
.resumen-item--negativo { border-color: rgba(239,68,68,0.3); }
.resumen-label { font-size: 12px; color: var(--text-muted); font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; margin: 0 0 4px; }
.resumen-valor { font-size: 22px; font-weight: 700; color: var(--text-primary); margin: 0; }
.resumen-item--ingreso .resumen-valor { color: var(--success-bright); }
.resumen-item--gasto .resumen-valor { color: var(--danger); }
.resumen-item--positivo .resumen-valor { color: var(--success-bright); }
.resumen-item--negativo .resumen-valor { color: var(--danger); }

.distribucion-list { background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 18px; padding: 16px; }
.dist-row { margin-bottom: 14px; }
.dist-row:last-child { margin-bottom: 0; }
.dist-header { display: flex; align-items: center; gap: 8px; margin-bottom: 6px; }
.dist-emoji { font-size: 16px; }
.dist-label { font-size: 13px; font-weight: 500; color: var(--text-primary); flex: 1; }
.dist-pct { font-size: 12px; font-weight: 700; color: var(--accent-soft); }
.dist-monto { font-size: 13px; font-weight: 700; color: var(--text-primary); min-width: 90px; text-align: right; }
.dist-bar-track { height: 6px; background: var(--border); border-radius: 100px; overflow: hidden; }
.dist-bar-fill { height: 100%; border-radius: 100px; transition: width 0.6s cubic-bezier(.34,1.56,.64,1); }

.stats-grid { display: grid; grid-template-columns: 1fr; gap: 12px; }
.stat-card { background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 16px; padding: 16px; text-align: center; }
.stat-label { font-size: 12px; color: var(--text-muted); font-weight: 600; text-transform: uppercase; margin: 0 0 4px; }
.stat-valor { font-size: 20px; font-weight: 700; color: var(--accent-light); margin: 0; }

.error-state { text-align: center; color: var(--danger); padding: 40px 20px; }
.empty-state { text-align: center; color: var(--text-muted); padding: 40px 20px; }

@media (min-width: 768px) {
  .header { max-width: 1100px; margin-inline: auto; padding-inline: 32px; width: 100%; }
  .main { padding: 24px 32px 40px; max-width: 1100px; width: 100%; margin-inline: auto; }
  .resumen-cards { grid-template-columns: repeat(3, 1fr); }
  .stats-grid { grid-template-columns: repeat(3, 1fr); }
}
</style>
