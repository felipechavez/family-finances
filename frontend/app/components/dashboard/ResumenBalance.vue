<!-- app/components/dashboard/ResumenBalance.vue -->
<!-- Auto-imported as <DashboardResumenBalance /> -->
<script setup lang="ts">
// Direct imports between composables
import { useFormato } from '~/composables/use-formato'

const { t } = useI18n()

const props = defineProps<{
  totalIngresos: number
  totalGastos: number
  balance: number
}>()

const { formatCLP, pct } = useFormato()

const porcentajeUso = computed(() => pct(props.totalGastos, props.totalIngresos))
const enRojo = computed(() => props.balance < 0)
const barraRoja = computed(() => porcentajeUso.value > 90)
</script>

<template>
  <div class="balance-card" :class="{ 'balance-card--negativo': enRojo }">
    <p class="label">{{ t('dashboard.balanceMes') }}</p>
    <p class="monto" :class="enRojo ? 'monto--rojo' : 'monto--morado'">
      {{ formatCLP(balance) }}
    </p>

    <div class="fila-resumen">
      <div class="resumen-chip resumen-chip--ingreso">
        <span class="chip-label">▲ {{ t('dashboard.ingresos') }}</span>
        <span class="chip-monto">{{ formatCLP(totalIngresos) }}</span>
      </div>
      <div class="resumen-chip resumen-chip--gasto">
        <span class="chip-label">▼ {{ t('dashboard.gastos') }}</span>
        <span class="chip-monto">{{ formatCLP(totalGastos) }}</span>
      </div>
    </div>

    <template v-if="totalIngresos > 0">
      <div class="barra-info">
        <span class="barra-texto">{{ t('dashboard.usoPresupuesto') }}</span>
        <span class="barra-pct" :class="{ 'barra-pct--rojo': barraRoja }">{{ porcentajeUso }}%</span>
      </div>
      <div class="barra-track">
        <div
          class="barra-fill"
          :class="{ 'barra-fill--rojo': barraRoja }"
          :style="{ width: `${porcentajeUso}%` }"
        />
      </div>
      <p class="disponible">{{ formatCLP(totalIngresos - totalGastos) }} {{ t('dashboard.disponible') }}</p>
    </template>
  </div>
</template>

<style scoped>
.balance-card {
  border-radius: 22px;
  padding: 20px;
  background: var(--hero-bg);
  border: 1px solid var(--hero-border);
  margin-bottom: 14px;
  overflow: hidden;
  width: 100%;
  box-sizing: border-box;
}
.balance-card--negativo {
  background: linear-gradient(135deg, #2d0a0a 0%, #1a0a1a 100%);
  border-color: #5a2a2a;
}

.label { font-size: 12px; color: var(--accent-soft); font-weight: 600; letter-spacing: 1px; text-transform: uppercase; margin: 0 0 4px; }

.monto { font-size: clamp(22px, 6vw, 38px); font-weight: 700; letter-spacing: -1.5px; margin: 0 0 16px; word-break: break-word; }
.monto--morado { color: var(--accent-light); }
.monto--rojo { color: var(--danger); }

.fila-resumen { display: flex; gap: 12px; margin-bottom: 16px; }

.resumen-chip { flex: 1; min-width: 0; border-radius: 12px; padding: 10px 14px; }
.resumen-chip--ingreso { background: rgba(34,197,94,0.1); border: 1px solid rgba(34,197,94,0.2); }
.resumen-chip--gasto { background: rgba(239,68,68,0.1); border: 1px solid rgba(239,68,68,0.2); }

.chip-label { display: block; font-size: 11px; font-weight: 600; margin-bottom: 2px; }
.resumen-chip--ingreso .chip-label { color: #4ade80; }
.resumen-chip--gasto .chip-label { color: #f87171; }

.chip-monto { font-size: clamp(12px, 3.5vw, 17px); font-weight: 700; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; display: block; }
.resumen-chip--ingreso .chip-monto { color: #4ade80; }
.resumen-chip--gasto .chip-monto { color: #f87171; }

.barra-info { display: flex; justify-content: space-between; margin-bottom: 8px; }
.barra-texto { font-size: 13px; font-weight: 600; color: var(--text-label); }
.barra-pct { font-size: 13px; font-weight: 700; color: var(--accent-soft); }
.barra-pct--rojo { color: var(--danger); }

.barra-track { height: 8px; background: var(--border); border-radius: 100px; overflow: hidden; }
.barra-fill { height: 100%; border-radius: 100px; background: linear-gradient(90deg,#7c3aed,#4f46e5); transition: width 0.6s cubic-bezier(.34,1.56,.64,1); }
.barra-fill--rojo { background: linear-gradient(90deg,#ef4444,#dc2626); }

.disponible { font-size: 11px; color: var(--text-muted); margin: 6px 0 0; }
</style>
