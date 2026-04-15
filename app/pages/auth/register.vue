<!-- app/pages/auth/register.vue -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

definePageMeta({ layout: 'auth' })

const { t } = useI18n()
const auth = useAuthStore()

const form = ref({ name: '', email: '', password: '', confirmPassword: '' })
const errorMsg = shallowRef('')
const loading = shallowRef(false)
const registered = shallowRef(false)

async function handleRegister() {
  errorMsg.value = ''

  if (!form.value.name || !form.value.email || !form.value.password) {
    errorMsg.value = t('auth.register.fillFields')
    return
  }
  if (form.value.password !== form.value.confirmPassword) {
    errorMsg.value = t('auth.register.passwordMismatch')
    return
  }
  if (form.value.password.length < 6) {
    errorMsg.value = t('auth.register.passwordTooShort')
    return
  }

  loading.value = true
  try {
    await auth.register({
      name: form.value.name,
      email: form.value.email,
      password: form.value.password,
    })
    // Show "check your email" message instead of auto-login (email not yet verified).
    registered.value = true
  } catch(e) {
    console.error('Error al registrar:', e)
    errorMsg.value = t('auth.register.error')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-card">
    <div class="auth-header">
      <span class="auth-icon">💜</span>
      <h1 class="auth-title">{{ $t('auth.register.title') }}</h1>
      <p class="auth-subtitle">{{ $t('auth.register.subtitle') }}</p>
    </div>

    <form class="auth-form" @submit.prevent="handleRegister">
      <div class="field">
        <label class="field-label">{{ $t('auth.register.name') }}</label>
        <input
          v-model="form.name"
          type="text"
          class="input"
          :placeholder="$t('auth.register.namePlaceholder')"
          autocomplete="name"
        />
      </div>

      <div class="field">
        <label class="field-label">{{ $t('auth.register.email') }}</label>
        <input
          v-model="form.email"
          type="email"
          class="input"
          :placeholder="$t('auth.register.emailPlaceholder')"
          autocomplete="email"
        />
      </div>

      <div class="field">
        <label class="field-label">{{ $t('auth.register.password') }}</label>
        <input
          v-model="form.password"
          type="password"
          class="input"
          :placeholder="$t('auth.register.passwordPlaceholder')"
          autocomplete="new-password"
        />
      </div>

      <div class="field">
        <label class="field-label">{{ $t('auth.register.confirmPassword') }}</label>
        <input
          v-model="form.confirmPassword"
          type="password"
          class="input"
          :placeholder="$t('auth.register.confirmPasswordPlaceholder')"
          autocomplete="new-password"
        />
      </div>

      <p v-if="errorMsg" class="error">{{ errorMsg }}</p>

      <div v-if="registered" class="success-box">
        <p class="success-msg">{{ $t('auth.register.checkEmail') }}</p>
        <NuxtLink to="/auth/login" class="auth-link">{{ $t('auth.register.loginLink') }}</NuxtLink>
      </div>
      <template v-else>
        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? $t('auth.register.submitting') : $t('auth.register.submit') }}
        </button>
      </template>
    </form>

    <p v-if="!registered" class="auth-footer">
      {{ $t('auth.register.hasAccount') }}
      <NuxtLink to="/auth/login" class="auth-link">{{ $t('auth.register.loginLink') }}</NuxtLink>
    </p>

    <div class="locale-row">
      <UiLocaleSwitcher />
    </div>
  </div>
</template>

<style scoped>
.auth-card {
  width: 100%;
  max-width: 400px;
  background: var(--bg-elevated);
  border: 1px solid var(--border);
  border-radius: 22px;
  padding: 32px 28px;
}

.auth-header { text-align: center; margin-bottom: 28px; }
.auth-icon { font-size: 40px; display: block; margin-bottom: 8px; }
.auth-title { font-size: 24px; font-weight: 700; color: var(--accent-light); margin: 0 0 4px; }
.auth-subtitle { font-size: 14px; color: var(--text-muted); margin: 0; }

.auth-form { display: flex; flex-direction: column; gap: 16px; }

.field { display: flex; flex-direction: column; gap: 6px; }
.field-label { font-size: 12px; color: var(--text-label); font-weight: 600; letter-spacing: 0.5px; }

.input {
  width: 100%;
  background: var(--bg-input);
  border: 1.5px solid var(--border);
  border-radius: 12px;
  padding: 13px 16px;
  font-size: 15px;
  color: var(--text-primary);
  outline: none;
}
.input:focus { border-color: var(--accent); }

.error { color: var(--danger); font-size: 13px; margin: 0; text-align: center; }

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

.auth-footer { text-align: center; font-size: 14px; color: var(--text-muted); margin: 20px 0 0; }
.auth-link { color: var(--accent-soft); text-decoration: none; font-weight: 600; }
.auth-link:hover { text-decoration: underline; }

.success-box {
  background: var(--accent-bg);
  border: 1px solid var(--success);
  border-radius: 12px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
  text-align: center;
}
.success-msg { color: var(--success); font-size: 14px; line-height: 1.5; margin: 0; }

.locale-row { display: flex; justify-content: center; margin-top: 16px; }
</style>
