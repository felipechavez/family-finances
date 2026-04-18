<!-- app/components/dashboard/GastosCategorias.vue -->
<!-- Auto-imported as <DashboardGastosCategorias /> -->
<script setup lang="ts">
import { useFormato } from '~/composables/use-formato'
import { useCategorias } from '~/composables/use-categorias'

const props = defineProps<{
  gastosPorCategoria: Record<string, number>
  presupuestoPorCategoria: Record<string, number>
  totalGastos: number
}>()

const { formatCLP, pct } = useFormato()
const { categoriasGasto } = useCategorias()

const categoriasConGasto = computed(() =>
  categoriasGasto.value
    .filter(c => props.gastosPorCategoria[c.id] !== undefined)
    .sort((a, b) => (props.gastosPorCategoria[b.id] ?? 0) - (props.gastosPorCategoria[a.id] ?? 0)),
)

// Resuelve los valores de cada categoría en computed para evitar
// narrowing problemático en el template (state-computed-over-watch rule)
interface FilaCat {
  gastado: number
  limite: number
  excedido: boolean
}

const filasPorCategoria = computed((): Record<string, FilaCat> => {
  const result: Record<string, FilaCat> = {}
  for (const cat of categoriasConGasto.value) {
    const gastado = props.gastosPorCategoria[cat.id] ?? 0
    const limite = props.presupuestoPorCategoria[cat.id] ?? 0
    result[cat.id] = {
      gastado,
      limite,
      excedido: limite > 0 && gastado > limite,
    }
  }
  return result
})
</script>

<template>
  <div v-if="categoriasConGasto.length" class="card">
    <h3 class="titulo">Gastos por categoría</h3>

    <div v-for="cat in categoriasConGasto" :key="cat.id" class="categoria-row">
      <div class="cat-header">
        <div class="cat-nombre">
          <span class="cat-emoji">{{ cat.emoji }}</span>
          <span class="cat-label">{{ cat.label }}</span>
          <span v-if="filasPorCategoria[cat.id]?.excedido" class="badge-excedido">⚠ Excedido</span>
        </div>
        <span class="cat-monto" :class="{ 'cat-monto--rojo': filasPorCategoria[cat.id]?.excedido }">
          {{ formatCLP(filasPorCategoria[cat.id]?.gastado ?? 0) }}
        </span>
      </div>

      <div class="barra-track">
        <div
          class="barra-fill"
          :style="{
            width: `${pct(filasPorCategoria[cat.id]?.gastado ?? 0, totalGastos)}%`,
            background: filasPorCategoria[cat.id]?.excedido ? '#ef4444' : cat.color,
          }"
        />
      </div>

      <p v-if="filasPorCategoria[cat.id]?.limite" class="limite-texto">
        Límite: {{ formatCLP(filasPorCategoria[cat.id]?.limite ?? 0) }} ·
        {{ pct(filasPorCategoria[cat.id]?.gastado ?? 0, filasPorCategoria[cat.id]?.limite ?? 1) }}% usado
      </p>
    </div>
  </div>
</template>

<style scoped>
.card { background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 18px; padding: 16px; margin-bottom: 14px; }
.titulo { font-size: 14px; font-weight: 700; color: var(--accent-light); margin: 0 0 14px; }

.categoria-row { margin-bottom: 12px; }
.categoria-row:last-child { margin-bottom: 0; }

.cat-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 5px; }
.cat-nombre { display: flex; align-items: center; gap: 7px; }
.cat-emoji { font-size: 16px; }
.cat-label { font-size: 13px; font-weight: 500; color: var(--text-primary); }

.badge-excedido {
  background: rgba(239,68,68,0.15); color: var(--danger);
  border-radius: 100px; padding: 2px 8px; font-size: 10px; font-weight: 600;
}

.cat-monto { font-size: 13px; font-weight: 700; color: var(--text-primary); }
.cat-monto--rojo { color: var(--danger); }

.barra-track { height: 6px; background: var(--border); border-radius: 100px; overflow: hidden; }
.barra-fill { height: 100%; border-radius: 100px; transition: width 0.6s cubic-bezier(.34,1.56,.64,1); }

.limite-texto { font-size: 10px; color: var(--text-muted); margin: 3px 0 0; }
</style>
