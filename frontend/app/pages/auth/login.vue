<!-- app/pages/auth/login.vue -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

definePageMeta({ layout: 'auth' })

const { t } = useI18n()
const auth = useAuthStore()
const { $api } = useNuxtApp()

// 'credentials' | '2fa'
const step = shallowRef<'credentials' | '2fa'>('credentials')

const form = ref({ email: '', password: '' })
const totpCode = shallowRef('')
const challengeToken = shallowRef('')

const errorMsg = shallowRef('')
const loading = shallowRef(false)
const showResend = shallowRef(false)
const resendMsg = shallowRef('')

// ── Step 1: credentials ───────────────────────────────────────────────────

async function handleLogin() {
  errorMsg.value = ''
  resendMsg.value = ''
  showResend.value = false

  if (!form.value.email || !form.value.password) {
    errorMsg.value = t('auth.login.fillFields')
    return
  }

  loading.value = true
  try {
    const outcome = await auth.login(form.value)

    if (outcome.requiresTwoFactor) {
      challengeToken.value = outcome.challengeToken!
      step.value = '2fa'
      return
    }

    await navigateTo('/')
  } catch (e: any) {
    if (e?.status === 403 || e?.statusCode === 403) {
      errorMsg.value = t('auth.login.emailNotVerified')
      showResend.value = true
    } else {
      errorMsg.value = t('auth.login.invalidCredentials')
    }
  } finally {
    loading.value = false
  }
}

async function handleResend() {
  resendMsg.value = ''
  loading.value = true
  try {
    await ($api as typeof $fetch)('/auth/resend-verification', {
      method: 'POST',
      body: { email: form.value.email },
    })
    resendMsg.value = t('auth.login.resendSuccess')
    showResend.value = false
  } catch {
    resendMsg.value = t('auth.login.resendError')
  } finally {
    loading.value = false
  }
}

// ── Step 2: 2FA ───────────────────────────────────────────────────────────

async function handleVerify2Fa() {
  errorMsg.value = ''
  if (!totpCode.value) return

  loading.value = true
  try {
    await auth.verify2Fa(challengeToken.value, totpCode.value)
    await navigateTo('/')
  } catch {
    errorMsg.value = t('auth.login.totpError')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-card">
    <!-- ── Step 1: Email + Password ──────────────────────────────────────── -->
    <template v-if="step === 'credentials'">
      <div class="auth-header">
        <img src="/logo.png" alt="DomusPay" class="auth-logo" />
        <p class="auth-subtitle">{{ $t('auth.login.subtitle') }}</p>
      </div>

      <form class="auth-form" @submit.prevent="handleLogin">
        <div class="field">
          <label class="field-label">{{ $t('auth.login.email') }}</label>
          <input
            v-model="form.email"
            type="email"
            class="input"
            :placeholder="$t('auth.login.emailPlaceholder')"
            autocomplete="email"
          />
        </div>

        <div class="field">
          <label class="field-label">{{ $t('auth.login.password') }}</label>
          <input
            v-model="form.password"
            type="password"
            class="input"
            placeholder="••••••••"
            autocomplete="current-password"
          />
        </div>

        <p v-if="errorMsg" class="error">{{ errorMsg }}</p>

        <button
          v-if="showResend"
          type="button"
          class="btn-secondary"
          :disabled="loading"
          @click="handleResend"
        >
          {{ $t('auth.login.resendVerification') }}
        </button>

        <p v-if="resendMsg" class="info-msg">{{ resendMsg }}</p>

        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? $t('auth.login.submitting') : $t('auth.login.submit') }}
        </button>
      </form>

      <p class="auth-footer">
        {{ $t('auth.login.noAccount') }}
        <NuxtLink to="/auth/register" class="auth-link">{{ $t('auth.login.registerLink') }}</NuxtLink>
      </p>

      <div class="locale-row">
        <UiLocaleSwitcher />
      </div>
    </template>

    <!-- ── Step 2: TOTP ──────────────────────────────────────────────────── -->
    <template v-else>
      <div class="auth-header">
        <span class="auth-icon">🔐</span>
        <h1 class="auth-title">{{ $t('auth.login.totpTitle') }}</h1>
        <p class="auth-subtitle">{{ $t('auth.login.totpSubtitle') }}</p>
      </div>

      <form class="auth-form" @submit.prevent="handleVerify2Fa">
        <div class="field">
          <label class="field-label">{{ $t('auth.login.totpCode') }}</label>
          <input
            v-model="totpCode"
            type="text"
            inputmode="numeric"
            maxlength="6"
            class="input input--center"
            :placeholder="$t('auth.login.totpCodePlaceholder')"
            autocomplete="one-time-code"
          />
        </div>

        <p v-if="errorMsg" class="error">{{ errorMsg }}</p>

        <button type="submit" class="btn-primary" :disabled="loading || totpCode.length < 6">
          {{ loading ? $t('auth.login.totpSubmitting') : $t('auth.login.totpSubmit') }}
        </button>
      </form>
    </template>
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
.auth-logo { height: 138px; width: auto; display: block; margin: 0 auto 12px; }
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
.input--center { text-align: center; font-size: 22px; letter-spacing: 6px; font-weight: 700; }

.error { color: var(--danger); font-size: 13px; margin: 0; text-align: center; }
.info-msg { color: var(--success); font-size: 13px; margin: 0; text-align: center; }

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

.btn-secondary {
  background: none;
  border: 1.5px solid var(--border);
  color: var(--accent-soft);
  border-radius: 12px;
  padding: 11px 16px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  width: 100%;
  transition: border-color 0.15s;
}
.btn-secondary:hover { border-color: var(--accent); }
.btn-secondary:disabled { opacity: 0.5; cursor: not-allowed; }

.auth-footer { text-align: center; font-size: 14px; color: var(--text-muted); margin: 20px 0 0; }
.auth-link { color: var(--accent-soft); text-decoration: none; font-weight: 600; }
.auth-link:hover { text-decoration: underline; }

.locale-row { display: flex; justify-content: center; margin-top: 16px; }
</style>
