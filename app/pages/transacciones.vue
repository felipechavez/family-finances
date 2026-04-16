<!-- app/pages/transacciones.vue -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useTransaccionesStore } from '~/stores/transacciones'
import { useCuentasStore } from '~/stores/cuentas'
import { useCategorias } from '~/composables/use-categorias'
import { useToast } from '~/composables/use-toast'
import { useApiError } from '~/composables/use-api-error'
import type { TransaccionCreateInput, TipoMovimiento, CategoriaId } from '#shared/types'

definePageMeta({ middleware: 'auth' })

const { t } = useI18n()
useHead({ title: 'Transacciones - DomusPay' })

const txStore = useTransaccionesStore()
const cuentasStore = useCuentasStore()
const { categoriasGasto, categoriasIngreso } = useCategorias()
const { ok: toastOk, error: toastError } = useToast()

const { transacciones, status, error: txError } = storeToRefs(txStore)
const { message: errorMessage } = useApiError(txError)
const { cuentas } = storeToRefs(cuentasStore)

const mostrarForm = ref(false)

const form = ref({
  tipo: 'gasto' as TipoMovimiento,
  categoria: 'alimentacion' as string,
  monto: '',
  descripcion: '',
  fecha: new Date().toISOString().slice(0, 10),
  accountId: '',
})

const categoriasPorTipo = computed(() =>
  form.value.tipo === 'gasto' ? categoriasGasto.value : categoriasIngreso.value,
)

const descripcionPlaceholder = computed(() =>
  form.value.tipo === 'gasto'
    ? t('transacciones.descripcionPlaceholder')
    : t('transacciones.descripcionPlaceholderIngreso'),
)

function handleTipoChange(tipo: TipoMovimiento) {
  form.value.tipo = tipo
  form.value.categoria = tipo === 'gasto' ? 'alimentacion' : 'sueldo'
}

async function handleCrear() {
  const monto = Number(form.value.monto)
  if (!monto || monto <= 0) { toastError(t('transacciones.montoInvalido')); return }
  if (!form.value.descripcion.trim()) { toastError(t('transacciones.descripcionRequerida')); return }

  const input: TransaccionCreateInput = {
    accountId: form.value.accountId || (cuentas.value[0]?.id ?? ''),
    tipo: form.value.tipo,
    categoria: form.value.categoria as CategoriaId,
    monto,
    descripcion: form.value.descripcion.trim(),
    fecha: form.value.fecha,
  }

  try {
    await txStore.crear(input)
    form.value = { ...form.value, monto: '', descripcion: '' }
    mostrarForm.value = false
    toastOk(form.value.tipo === 'gasto'
      ? t('transacciones.gastoRegistrado')
      : t('transacciones.ingresoRegistrado'))
  } catch {
    toastError(t('transacciones.noSePudoGuardar'))
  }
}

async function handleEliminar(id: string) {
  try {
    await txStore.eliminar(id)
    toastOk(t('transacciones.eliminado'))
  } catch {
    toastError(t('transacciones.noSePudoEliminar'))
  }
}
</script>

<template>
  <div>
    <header class="header">
      <h1 class="header-titulo">{{ $t('transacciones.title') }}</h1>
      <button class="btn-agregar" @click="mostrarForm = !mostrarForm">
        {{ mostrarForm ? $t('transacciones.cancelar') : $t('transacciones.nuevo') }}
      </button>
    </header>

    <main class="main">
      <!-- Form de creación -->
      <div v-if="mostrarForm" class="form-card">
        <div class="segmento">
          <button class="seg-btn" :class="{ 'seg-btn--gasto': form.tipo === 'gasto' }" @click="handleTipoChange('gasto')">
            {{ $t('transacciones.gasto') }}
          </button>
          <button class="seg-btn" :class="{ 'seg-btn--ingreso': form.tipo === 'ingreso' }" @click="handleTipoChange('ingreso')">
            {{ $t('transacciones.ingreso') }}
          </button>
        </div>

        <div class="form">
          <label class="field-label">{{ $t('transacciones.monto') }}</label>
          <input v-model="form.monto" class="input" type="number" placeholder="0" inputmode="numeric" />

          <label class="field-label">{{ $t('transacciones.descripcion') }}</label>
          <input v-model="form.descripcion" class="input" type="text" :placeholder="descripcionPlaceholder" />

          <label class="field-label">{{ $t('transacciones.categoria') }}</label>
          <select v-model="form.categoria" class="input select">
            <option v-for="cat in categoriasPorTipo" :key="cat.id" :value="cat.id">
              {{ cat.emoji }} {{ cat.label }}
            </option>
          </select>

          <div class="form-row">
            <div class="form-col">
              <label class="field-label">{{ $t('transacciones.cuenta') }}</label>
              <select v-model="form.accountId" class="input select">
                <option v-for="c in cuentas" :key="c.id" :value="c.id">{{ c.name }}</option>
              </select>
            </div>
            <div class="form-col">
              <label class="field-label">{{ $t('transacciones.fecha') }}</label>
              <input v-model="form.fecha" class="input" type="date" />
            </div>
          </div>

          <button class="btn-primary" @click="handleCrear">
            {{ form.tipo === 'gasto' ? $t('transacciones.registrarGasto') : $t('transacciones.registrarIngreso') }}
          </button>
        </div>
      </div>

      <!-- Lista -->
      <UiSpinner v-if="status === 'pending'">{{ $t('common.loading') }}</UiSpinner>
      <UiError404 v-else-if="txError" :message="errorMessage" />
      <div v-else-if="transacciones.length === 0" class="empty-state">
        <span class="empty-icon">📭</span>
        <p>{{ $t('transacciones.empty') }}</p>
      </div>
      <div v-else class="card card--sin-padding">
        <TransaccionesRow
          v-for="tx in transacciones"
          :key="tx.id"
          :transaccion="tx"
          :mostrar-eliminar="true"
          @eliminar="handleEliminar"
        />
      </div>
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
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 0; }

.btn-agregar {
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff; border: none; border-radius: 12px;
  padding: 10px 20px; font-size: 14px; font-weight: 600; cursor: pointer;
}

.main { padding: 16px 16px 100px; flex: 1; }

.form-card { max-width: 560px; margin-bottom: 24px; background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 18px; padding: 20px; }
.segmento { display: flex; gap: 8px; margin-bottom: 16px; }
.seg-btn {
  flex: 1; padding: 10px; border: 1.5px solid var(--border); background: transparent;
  color: var(--text-label); border-radius: 12px; font-size: 14px; font-weight: 600; cursor: pointer;
}
.seg-btn--gasto { background: #2d1a1a; border-color: #ef4444; color: #f87171; }
.seg-btn--ingreso { background: #1a2d1a; border-color: #22c55e; color: #4ade80; }
.form { display: flex; flex-direction: column; gap: 10px; }
.field-label { font-size: 12px; color: var(--text-label); font-weight: 600; letter-spacing: 0.5px; display: block; margin-bottom: -4px; }
.input {
  width: 100%; background: var(--bg-input); border: 1.5px solid var(--border); border-radius: 12px;
  padding: 13px 16px; font-size: 15px; color: var(--text-primary); outline: none;
}
.input:focus { border-color: var(--accent); }
.select { -webkit-appearance: none; cursor: pointer; }
.form-row { display: flex; gap: 10px; }
.form-col { flex: 1; display: flex; flex-direction: column; gap: 6px; }
.btn-primary {
  background: linear-gradient(135deg, #7c3aed, #4f46e5); color: #fff; border: none;
  border-radius: 14px; padding: 14px 24px; font-size: 15px; font-weight: 600;
  cursor: pointer; width: 100%; margin-top: 8px;
}

.card { background: var(--bg-elevated); border: 1px solid var(--border); border-radius: 18px; padding: 16px; margin-bottom: 14px; }
.card--sin-padding { padding: 0 16px; }
.error-state { text-align: center; color: var(--danger); padding: 40px 20px; }
.empty-state { text-align: center; color: var(--text-muted); padding: 60px 20px; }
.empty-icon { font-size: 48px; display: block; margin-bottom: 12px; }

@media (min-width: 768px) {
  .header { max-width: 1100px; margin-inline: auto; padding-inline: 32px; width: 100%; }
  .main { padding: 24px 32px 40px; max-width: 1100px; width: 100%; margin-inline: auto; }
}
</style>
