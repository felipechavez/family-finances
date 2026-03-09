<!-- app/pages/auth/login.vue -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

definePageMeta({ layout: 'auth' })

const auth = useAuthStore()

const form = ref({ email: '', password: '' })
const errorMsg = ref('')
const loading = ref(false)

async function handleLogin() {
  errorMsg.value = ''
  if (!form.value.email || !form.value.password) {
    errorMsg.value = 'Completa todos los campos'
    return
  }
  loading.value = true
  try {
    await auth.login(form.value)
    await navigateTo('/')
  } catch {
    errorMsg.value = 'Credenciales incorrectas'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-card">
    <div class="auth-header">
      <span class="auth-icon">💜</span>
      <h1 class="auth-title">FinanzasApp</h1>
      <p class="auth-subtitle">Inicia sesión para continuar</p>
    </div>

    <form class="auth-form" @submit.prevent="handleLogin">
      <div class="field">
        <label class="field-label">EMAIL</label>
        <input
          v-model="form.email"
          type="email"
          class="input"
          placeholder="tu@email.com"
          autocomplete="email"
        />
      </div>

      <div class="field">
        <label class="field-label">CONTRASEÑA</label>
        <input
          v-model="form.password"
          type="password"
          class="input"
          placeholder="••••••••"
          autocomplete="current-password"
        />
      </div>

      <p v-if="errorMsg" class="error">{{ errorMsg }}</p>

      <button type="submit" class="btn-primary" :disabled="loading">
        {{ loading ? 'Ingresando...' : 'Iniciar sesión' }}
      </button>
    </form>

    <p class="auth-footer">
      ¿No tienes cuenta?
      <NuxtLink to="/auth/register" class="auth-link">Regístrate</NuxtLink>
    </p>
  </div>
</template>

<style scoped>
.auth-card {
  width: 100%;
  max-width: 400px;
  background: #18182a;
  border: 1px solid #2a2a40;
  border-radius: 22px;
  padding: 32px 28px;
}

.auth-header { text-align: center; margin-bottom: 28px; }
.auth-icon { font-size: 40px; display: block; margin-bottom: 8px; }
.auth-title { font-size: 24px; font-weight: 700; color: #c4b5fd; margin: 0 0 4px; }
.auth-subtitle { font-size: 14px; color: #6b6b8a; margin: 0; }

.auth-form { display: flex; flex-direction: column; gap: 16px; }

.field { display: flex; flex-direction: column; gap: 6px; }
.field-label { font-size: 12px; color: #8888aa; font-weight: 600; letter-spacing: 0.5px; }

.input {
  width: 100%;
  background: #0f0f18;
  border: 1.5px solid #2a2a40;
  border-radius: 12px;
  padding: 13px 16px;
  font-size: 15px;
  color: #f0eeff;
  outline: none;
}
.input:focus { border-color: #7c3aed; }

.error { color: #f87171; font-size: 13px; margin: 0; text-align: center; }

.btn-primary {
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff;
  border: none;
  border-radius: 14px;
  padding: 14px 24px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  width: 100%;
  margin-top: 4px;
}
.btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-primary:active:not(:disabled) { opacity: 0.85; transform: scale(0.98); }

.auth-footer { text-align: center; font-size: 14px; color: #6b6b8a; margin: 20px 0 0; }
.auth-link { color: #a78bfa; text-decoration: none; font-weight: 600; }
.auth-link:hover { text-decoration: underline; }
</style>
