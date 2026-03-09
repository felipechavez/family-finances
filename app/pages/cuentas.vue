<!-- app/pages/cuentas.vue -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useCuentasStore } from '~/stores/cuentas'
import { useFormato } from '~/composables/use-formato'
import { useToast } from '~/composables/use-toast'
import type { AccountType } from '#shared/types'

definePageMeta({ middleware: 'auth' })
useHead({ title: 'Cuentas - FinanzasApp' })

const cuentasStore = useCuentasStore()
const { formatCLP } = useFormato()
const { ok: toastOk, error: toastError } = useToast()

const { cuentas, balanceTotal, status, error: cuentasError } = storeToRefs(cuentasStore)

const mostrarForm = ref(false)
const form = ref({ name: '', type: 'bank' as AccountType })

const tiposCuenta: { value: AccountType; label: string; emoji: string }[] = [
  { value: 'cash', label: 'Efectivo', emoji: '💵' },
  { value: 'bank', label: 'Cuenta Bancaria', emoji: '🏦' },
  { value: 'savings', label: 'Ahorro', emoji: '🐷' },
  { value: 'credit_card', label: 'Tarjeta de Crédito', emoji: '💳' },
]

function tipoInfo(type: AccountType) {
  return tiposCuenta.find(t => t.value === type) ?? tiposCuenta[0]
}

async function handleCrear() {
  if (!form.value.name.trim()) { toastError('Ingresa un nombre'); return }
  try {
    await cuentasStore.crear({ name: form.value.name.trim(), type: form.value.type })
    form.value = { name: '', type: 'bank' }
    mostrarForm.value = false
    toastOk('Cuenta creada')
  } catch {
    toastError('No se pudo crear la cuenta')
  }
}

async function handleEliminar(id: string) {
  try {
    await cuentasStore.eliminar(id)
    toastOk('Cuenta eliminada')
  } catch {
    toastError('No se pudo eliminar')
  }
}
</script>

<template>
  <div>
    <header class="header">
      <h1 class="header-titulo">Cuentas</h1>
      <button class="btn-agregar" @click="mostrarForm = !mostrarForm">
        {{ mostrarForm ? 'Cancelar' : '+ Nueva' }}
      </button>
    </header>

    <main class="main">
      <!-- Balance total -->
      <div class="balance-card">
        <p class="balance-label">Balance total</p>
        <p class="balance-monto">{{ formatCLP(balanceTotal) }}</p>
      </div>

      <!-- Form -->
      <div v-if="mostrarForm" class="form-card">
        <div class="form">
          <label class="field-label">NOMBRE</label>
          <input v-model="form.name" class="input" type="text" placeholder="Ej: Cuenta corriente BCI" />

          <label class="field-label">TIPO</label>
          <select v-model="form.type" class="input select">
            <option v-for="t in tiposCuenta" :key="t.value" :value="t.value">
              {{ t.emoji }} {{ t.label }}
            </option>
          </select>

          <button class="btn-primary" @click="handleCrear">Crear cuenta</button>
        </div>
      </div>

      <!-- Lista -->
      <UiSpinner v-if="status === 'pending'">Cargando...</UiSpinner>
      <div v-else-if="cuentasError" class="error-state"><p>Error al cargar cuentas</p></div>
      <div v-else-if="cuentas.length === 0" class="empty-state">
        <span class="empty-icon">💳</span>
        <p>No hay cuentas registradas</p>
      </div>
      <div v-else class="cuentas-grid">
        <div v-for="cuenta in cuentas" :key="cuenta.id" class="cuenta-card">
          <div class="cuenta-header">
            <span class="cuenta-emoji">{{ tipoInfo(cuenta.type).emoji }}</span>
            <div class="cuenta-info">
              <p class="cuenta-nombre">{{ cuenta.name }}</p>
              <p class="cuenta-tipo">{{ tipoInfo(cuenta.type).label }}</p>
            </div>
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

.btn-eliminar {
  background: rgba(239,68,68,0.1); border: 1px solid rgba(239,68,68,0.2);
  color: #f87171; border-radius: 8px; padding: 4px 10px; font-size: 12px; cursor: pointer;
}

.error-state { text-align: center; color: #f87171; padding: 40px 20px; }
.empty-state { text-align: center; color: #6b6b8a; padding: 60px 20px; }
.empty-icon { font-size: 48px; display: block; margin-bottom: 12px; }

@media (min-width: 768px) {
  .main { padding: 24px 32px 40px; max-width: 1100px; width: 100%; }
  .cuentas-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (min-width: 1280px) {
  .cuentas-grid { grid-template-columns: repeat(3, 1fr); }
}
</style>
