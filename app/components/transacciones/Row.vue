<!-- app/components/transacciones/Row.vue -->
<!-- Auto-imported as <TransaccionesRow /> -->
<script setup lang="ts">
import type { Transaccion } from '#shared/types'
import { useFormato } from '~/composables/use-formato'
import { useCategorias } from '~/composables/use-categorias'

defineProps<{
  transaccion: Transaccion
  mostrarEliminar?: boolean
}>()

const emit = defineEmits<{
  'eliminar': [id: string]
}>()

const { formatCLP } = useFormato()
const { getCategoriaInfo } = useCategorias()

function handleEliminar(id: string): void {
  emit('eliminar', id)
}
</script>

<template>
  <div class="tx-row">
    <div
      class="icono"
      :style="{ background: getCategoriaInfo(transaccion.categoria).color + '22' }"
    >
      {{ getCategoriaInfo(transaccion.categoria).emoji }}
    </div>

    <div class="info">
      <p class="descripcion">{{ transaccion.descripcion }}</p>
      <p class="meta">
        {{ getCategoriaInfo(transaccion.categoria).label }} · {{ transaccion.fecha }}
      </p>
    </div>

    <div class="derecha">
      <span class="monto" :class="transaccion.tipo === 'ingreso' ? 'monto--verde' : 'monto--rojo'">
        {{ transaccion.tipo === 'ingreso' ? '+' : '-' }}{{ formatCLP(transaccion.monto) }}
      </span>
      <button
        v-if="mostrarEliminar"
        class="btn-eliminar"
        @click="handleEliminar(transaccion.id)"
      >
        ✕
      </button>
    </div>
  </div>
</template>

<style scoped>
.tx-row { display: flex; align-items: center; gap: 12px; padding: 13px 0; border-bottom: 1px solid #1e1e2e; }
.tx-row:last-child { border-bottom: none; }

.icono { width: 42px; height: 42px; border-radius: 13px; display: flex; align-items: center; justify-content: center; font-size: 18px; flex-shrink: 0; }

.info { flex: 1; min-width: 0; }
.descripcion { font-size: 14px; font-weight: 500; color: #f0eeff; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; margin: 0 0 2px; }
.meta { font-size: 11px; color: #6b6b8a; margin: 0; }

.derecha { display: flex; flex-direction: column; align-items: flex-end; gap: 6px; flex-shrink: 0; }

.monto { font-size: 15px; font-weight: 700; }
.monto--verde { color: #4ade80; }
.monto--rojo { color: #f87171; }

.btn-eliminar {
  background: rgba(239,68,68,0.1); border: 1px solid rgba(239,68,68,0.2);
  color: #f87171; border-radius: 8px; padding: 3px 9px; font-size: 11px; cursor: pointer;
  -webkit-tap-highlight-color: transparent;
}
</style>
