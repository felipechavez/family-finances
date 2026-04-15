<!-- app/components/presupuestos/Card.vue -->
<!-- Auto-imported as <PresupuestosCard /> -->
<script setup lang="ts">
import type { CategoriaInfo } from '~/types/ui'
import { useFormato } from '~/composables/use-formato'

const { t } = useI18n()

const props = defineProps<{
  categoria: CategoriaInfo
  gastado: number
  limite: number
}>()

const emit = defineEmits<{
  'guardar-limite': [categoria: string, limite: number]
}>()

const { formatCLP, pct } = useFormato()

const inputLimite = ref<string>(props.limite > 0 ? String(props.limite) : '')
const editando = ref(false)

const sobre = computed(() => props.limite > 0 && props.gastado > props.limite)
const porcentaje = computed(() => pct(props.gastado, props.limite))

function handleGuardar(): void {
  const val = Number(inputLimite.value)
  if (!isNaN(val) && val >= 0) {
    emit('guardar-limite', props.categoria.id, val)
    editando.value = false
  }
}
</script>

<template>
  <div class="card" :class="{ 'card--sobre': sobre }">
    <div class="header">
      <div class="cat-nombre">
        <span class="cat-emoji">{{ categoria.emoji }}</span>
        <span class="cat-label">{{ categoria.label }}</span>
        <span v-if="sobre" class="badge-excedido">{{ $t('presupuestos.excedido') }}</span>
      </div>
    </div>

    <div class="stats">
      <span>{{ $t('presupuestos.gastado') }}: <strong :class="{ 'texto-rojo': sobre }">{{ formatCLP(gastado) }}</strong></span>
      <span v-if="limite > 0">{{ $t('presupuestos.limite') }}: <strong class="texto-morado">{{ formatCLP(limite) }}</strong></span>
    </div>

    <div v-if="limite > 0" class="barra-track">
      <div
        class="barra-fill"
        :style="{ width: `${porcentaje}%`, background: sobre ? '#ef4444' : categoria.color }"
      />
    </div>

    <div class="input-row">
      <input
        v-model="inputLimite"
        type="number"
        class="input-limite"
        :placeholder="t('presupuestos.sinLimite')"
        @focus="editando = true"
      />
      <button
        v-if="editando"
        class="btn-guardar"
        @click="handleGuardar"
      >
        {{ $t('presupuestos.guardar') }}
      </button>
    </div>
  </div>
</template>

<style scoped>
.card { background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 18px; padding: 14px 16px; margin-bottom: 10px; }
.card--sobre { border-color: #5a2a2a; }

.header { margin-bottom: 8px; }
.cat-nombre { display: flex; align-items: center; gap: 8px; }
.cat-emoji { font-size: 20px; }
.cat-label { font-size: 14px; font-weight: 600; color: var(--text-primary); }
.badge-excedido { background: rgba(239,68,68,0.15); color: var(--danger); border-radius: 100px; padding: 2px 8px; font-size: 10px; font-weight: 600; }

.stats { display: flex; justify-content: space-between; font-size: 12px; color: var(--text-muted); margin-bottom: 8px; }
.texto-rojo { color: var(--danger); }
.texto-morado { color: var(--accent-soft); }

.barra-track { height: 6px; background: var(--border); border-radius: 100px; overflow: hidden; margin-bottom: 10px; }
.barra-fill { height: 100%; border-radius: 100px; transition: width 0.6s cubic-bezier(.34,1.56,.64,1); }

.input-row { display: flex; gap: 8px; align-items: center; }

.input-limite {
  flex: 1; background: var(--bg-input); border: 1.5px solid var(--border); border-radius: 12px;
  padding: 9px 12px; font-size: 14px; color: var(--text-primary); outline: none;
}
.input-limite:focus { border-color: var(--accent); }

.btn-guardar {
  background: linear-gradient(135deg, #7c3aed, #4f46e5); border: none; border-radius: 10px;
  padding: 9px 16px; color: #fff; font-weight: 700; cursor: pointer; font-size: 13px;
  -webkit-tap-highlight-color: transparent;
}
</style>
