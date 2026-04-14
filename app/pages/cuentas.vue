<!-- app/pages/cuentas.vue -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useCuentasStore } from '~/stores/cuentas'
import { useFormato } from '~/composables/use-formato'
import { useToast } from '~/composables/use-toast'
import { useApiError } from '~/composables/use-api-error'
import type { Account, AccountType } from '#shared/types'

definePageMeta({ middleware: 'auth' })

const { t } = useI18n()
useHead({ title: 'Cuentas - FinanzasApp' })

const cuentasStore = useCuentasStore()
const { formatCLP } = useFormato()
const { ok: toastOk, error: toastError } = useToast()

const { cuentas, balanceTotal, status, error: cuentasError } = storeToRefs(cuentasStore)
const { message: errorMessage } = useApiError(cuentasError)

const mostrarForm = ref(false)
const form = ref({ name: '', type: 'bank' as AccountType, initialBalance: 0 })

const cuentaEditando = shallowRef<Account | null>(null)
const formEdicion = ref({ name: '', balance: 0 })

function iniciarEdicion(cuenta: Account) {
  mostrarForm.value = false
  cuentaEditando.value = cuenta
  formEdicion.value = { name: cuenta.name, balance: cuenta.balance }
}

function cancelarEdicion() {
  cuentaEditando.value = null
}

const tiposCuenta = computed(() => [
  { value: 'cash' as AccountType,        label: t('cuentas.tipos.cash'),        emoji: '💵' },
  { value: 'bank' as AccountType,        label: t('cuentas.tipos.bank'),        emoji: '🏦' },
  { value: 'savings' as AccountType,     label: t('cuentas.tipos.savings'),     emoji: '🐷' },
  { value: 'credit_card' as AccountType, label: t('cuentas.tipos.credit_card'), emoji: '💳' },
])

function tipoInfo(type: AccountType) {
  return tiposCuenta.value.find(t => t.value === type) ?? tiposCuenta.value[0]!
}

async function handleCrear() {
  if (!form.value.name.trim()) { toastError(t('cuentas.ingresaNombre')); return }
  try {
    await cuentasStore.crear({ name: form.value.name.trim(), type: form.value.type, initialBalance: form.value.initialBalance })
    form.value = { name: '', type: 'bank', initialBalance: 0 }
    mostrarForm.value = false
    toastOk(t('cuentas.creada'))
  } catch (err) {
    const serverMessage = (err as { data?: { error?: string } }).data?.error
    toastError(serverMessage ?? t('cuentas.noSePudoCrear'))
  }
}

async function handleActualizar() {
  if (!formEdicion.value.name.trim()) { toastError(t('cuentas.ingresaNombre')); return }
  if (!cuentaEditando.value) return
  try {
    await cuentasStore.actualizar(cuentaEditando.value.id, {
      name: formEdicion.value.name.trim(),
      balance: formEdicion.value.balance,
    })
    cuentaEditando.value = null
    toastOk(t('cuentas.actualizada'))
  } catch (err) {
    const serverMessage = (err as { data?: { error?: string } }).data?.error
    toastError(serverMessage ?? t('cuentas.noSePudoActualizar'))
  }
}

async function handleEliminar(id: string) {
  try {
    await cuentasStore.eliminar(id)
    toastOk(t('cuentas.eliminada'))
  } catch {
    toastError(t('cuentas.noSePudoEliminar'))
  }
}
</script>

<template>
  <div>
    <header class="header">
      <h1 class="header-titulo">{{ $t('cuentas.title') }}</h1>
      <button class="btn-agregar" @click="mostrarForm = !mostrarForm">
        {{ mostrarForm ? $t('cuentas.cancelar') : $t('cuentas.nueva') }}
      </button>
    </header>

    <main class="main">
      <!-- Balance total -->
      <div class="balance-card">
        <p class="balance-label">{{ $t('cuentas.balanceTotal') }}</p>
        <p class="balance-monto">{{ formatCLP(balanceTotal) }}</p>
      </div>

      <!-- Form edición -->
      <div v-if="cuentaEditando" class="form-card form-card--edicion">
        <p class="form-titulo">{{ $t('cuentas.editarCuenta') }}: <strong>{{ cuentaEditando.name }}</strong></p>
        <div class="form">
          <label class="field-label">{{ $t('cuentas.nombre') }}</label>
          <input v-model="formEdicion.name" class="input" type="text" :placeholder="$t('cuentas.namePlaceholder')" />

          <label class="field-label">{{ $t('cuentas.balanceActual') }}</label>
          <input v-model.number="formEdicion.balance" class="input" type="number" step="0.01" placeholder="0" />

          <div class="form-acciones">
            <button class="btn-primary" @click="handleActualizar">{{ $t('cuentas.guardarCambios') }}</button>
            <button class="btn-cancelar" @click="cancelarEdicion">{{ $t('cuentas.cancelar') }}</button>
          </div>
        </div>
      </div>

      <!-- Form creación -->
      <div v-if="mostrarForm" class="form-card">
        <div class="form">
          <label class="field-label">{{ $t('cuentas.nombre') }}</label>
          <input v-model="form.name" class="input" type="text" :placeholder="$t('cuentas.namePlaceholder')" />

          <label class="field-label">{{ $t('cuentas.tipo') }}</label>
          <select v-model="form.type" class="input select">
            <option v-for="tp in tiposCuenta" :key="tp.value" :value="tp.value">
              {{ tp.emoji }} {{ tp.label }}
            </option>
          </select>

          <label class="field-label">{{ $t('cuentas.balanceInicial') }}</label>
          <input v-model.number="form.initialBalance" class="input" type="number" step="0.01" min="0" placeholder="0" />

          <button class="btn-primary" @click="handleCrear">{{ $t('cuentas.crearCuenta') }}</button>
        </div>
      </div>

      <!-- Lista -->
      <UiSpinner v-if="status === 'pending'">{{ $t('common.loading') }}</UiSpinner>
      <UiError404 v-else-if="cuentasError" :message="errorMessage" />
      <div v-else-if="cuentas.length === 0" class="empty-state">
        <span class="empty-icon">💳</span>
        <p>{{ $t('cuentas.empty') }}</p>
      </div>
      <div v-else class="cuentas-grid">
        <div v-for="cuenta in cuentas" :key="cuenta.id" class="cuenta-card">
          <div class="cuenta-header">
            <span class="cuenta-emoji">{{ tipoInfo(cuenta.type).emoji }}</span>
            <div class="cuenta-info">
              <p class="cuenta-nombre">{{ cuenta.name }}</p>
              <p class="cuenta-tipo">{{ tipoInfo(cuenta.type).label }}</p>
            </div>
            <button class="btn-editar" @click="iniciarEdicion(cuenta)">✏</button>
            <button class="btn-eliminar" @click="handleEliminar(cuenta.id)">✕</button>
          </div>
          <p class="cuenta-balance" :class="{ 'cuenta-balance--negativo': cuenta.balance < 0 }">
            {{ formatCLP(cuenta.balance) }}
          </p>
        </div>
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
  border-bottom: 1px solid #1a1a2e;
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

.balance-card {
  background: linear-gradient(135deg, #1e0a4a 0%, #0d1f4a 100%);
  border: 1px solid #3a2a60;
  border-radius: 18px;
  padding: 20px;
  margin-bottom: 20px;
  text-align: center;
}
.balance-label { font-size: 12px; color: #a78bfa; font-weight: 600; letter-spacing: 1px; text-transform: uppercase; margin: 0 0 4px; }
.balance-monto { font-size: 32px; font-weight: 700; color: #c4b5fd; margin: 0; }

.form-card { max-width: 500px; margin-bottom: 20px; background: #18182a; border: 1px solid #2a2a40; border-radius: 18px; padding: 20px; }
.form { display: flex; flex-direction: column; gap: 10px; }
.field-label { font-size: 12px; color: #8888aa; font-weight: 600; letter-spacing: 0.5px; }
.input {
  width: 100%; background: #0f0f18; border: 1.5px solid #2a2a40; border-radius: 12px;
  padding: 13px 16px; font-size: 15px; color: #f0eeff; outline: none;
}
.input:focus { border-color: #7c3aed; }
.select { -webkit-appearance: none; cursor: pointer; }
.btn-primary {
  background: linear-gradient(135deg, #7c3aed, #4f46e5); color: #fff; border: none;
  border-radius: 14px; padding: 14px 24px; font-size: 15px; font-weight: 600;
  cursor: pointer; width: 100%; margin-top: 8px;
}

.cuentas-grid { display: grid; grid-template-columns: 1fr; gap: 12px; }

.cuenta-card {
  background: #18182a; border: 1px solid #2a2a40; border-radius: 18px; padding: 16px;
}
.cuenta-header { display: flex; align-items: center; gap: 12px; margin-bottom: 10px; }
.cuenta-emoji { font-size: 28px; }
.cuenta-info { flex: 1; }
.cuenta-nombre { font-size: 15px; font-weight: 600; color: #f0eeff; margin: 0; }
.cuenta-tipo { font-size: 12px; color: #6b6b8a; margin: 2px 0 0; }
.cuenta-balance { font-size: 22px; font-weight: 700; color: #c4b5fd; margin: 0; }
.cuenta-balance--negativo { color: #f87171; }

.btn-editar {
  background: rgba(124,58,237,0.1); border: 1px solid rgba(124,58,237,0.2);
  color: #a78bfa; border-radius: 8px; padding: 4px 10px; font-size: 12px; cursor: pointer;
}
.btn-eliminar {
  background: rgba(239,68,68,0.1); border: 1px solid rgba(239,68,68,0.2);
  color: #f87171; border-radius: 8px; padding: 4px 10px; font-size: 12px; cursor: pointer;
}
.form-card--edicion { border-color: #4f46e5; }
.form-titulo { font-size: 13px; color: #a78bfa; font-weight: 600; margin: 0 0 12px; }
.form-titulo strong { color: #f0eeff; }
.form-acciones { display: flex; gap: 8px; margin-top: 8px; }
.form-acciones .btn-primary { flex: 1; margin-top: 0; }
.btn-cancelar {
  flex: 1; background: transparent; border: 1.5px solid #2a2a40; color: #8888aa;
  border-radius: 14px; padding: 14px 24px; font-size: 15px; font-weight: 600; cursor: pointer;
}
.empty-state { text-align: center; color: #6b6b8a; padding: 60px 20px; }
.empty-icon { font-size: 48px; display: block; margin-bottom: 12px; }

@media (min-width: 768px) {
  .header { max-width: 1100px; margin-inline: auto; padding-inline: 32px; width: 100%; }
  .main { padding: 24px 32px 40px; max-width: 1100px; width: 100%; margin-inline: auto; }
  .cuentas-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (min-width: 1280px) {
  .cuentas-grid { grid-template-columns: repeat(3, 1fr); }
}
</style>
