<!-- app/pages/configuracion.vue -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'
import { useTheme } from '~/composables/use-theme'
import { useToast } from '~/composables/use-toast'

definePageMeta({ middleware: 'auth' })
useHead({ title: 'Configuración - FinanzasApp' })

const { t, locale, availableLocales, setLocale } = useI18n()
const auth = useAuthStore()
const { isDark, toggle: toggleTheme } = useTheme()
const { ok: toastOk, error: toastError } = useToast()
const { $api } = useNuxtApp()

// ── 2FA state machine ─────────────────────────────────────────────────────────
// 'idle'     : initial state; user can attempt setup or disable
// 'qr'       : setup started; showing QR + confirm form
// 'confirm'  : confirming TOTP code after scanning QR
// 'disable'  : user wants to disable 2FA; showing TOTP input
type TwoFaState = 'idle' | 'qr' | 'disable'

const twoFaState    = shallowRef<TwoFaState>('idle')
const twoFaEnabled  = shallowRef<boolean | null>(null)  // null = unknown
const qrUri         = shallowRef('')
const totpCode      = shallowRef('')
const twoFaLoading  = shallowRef(false)
const twoFaError    = shallowRef('')

async function handleSetup2Fa() {
  twoFaError.value = ''
  twoFaLoading.value = true
  try {
    const res = await ($api as typeof $fetch)('/auth/setup-2fa', { method: 'POST' }) as {
      secret: string
      provisioningUri: string
    }
    qrUri.value = res.provisioningUri
    twoFaState.value = 'qr'
  } catch (e: any) {
    // 409 → already enabled
    if (e?.status === 409 || e?.statusCode === 409) {
      twoFaEnabled.value = true
      toastError(t('configuracion.seguridad.yaActivo'))
    } else {
      twoFaError.value = t('configuracion.seguridad.errorSetup')
    }
  } finally {
    twoFaLoading.value = false
  }
}

async function handleConfirm2Fa() {
  if (totpCode.value.length < 6) return
  twoFaError.value = ''
  twoFaLoading.value = true
  try {
    await ($api as typeof $fetch)('/auth/confirm-2fa', {
      method: 'POST',
      body: { code: totpCode.value },
    })
    twoFaEnabled.value = true
    twoFaState.value = 'idle'
    totpCode.value = ''
    qrUri.value = ''
    toastOk(t('configuracion.seguridad.activado'))
  } catch {
    twoFaError.value = t('configuracion.seguridad.codigoIncorrecto')
  } finally {
    twoFaLoading.value = false
  }
}

async function handleDisable2Fa() {
  if (totpCode.value.length < 6) return
  twoFaError.value = ''
  twoFaLoading.value = true
  try {
    await ($api as typeof $fetch)('/auth/disable-2fa', {
      method: 'DELETE',
      body: { code: totpCode.value },
    })
    twoFaEnabled.value = false
    twoFaState.value = 'idle'
    totpCode.value = ''
    toastOk(t('configuracion.seguridad.desactivado'))
  } catch {
    twoFaError.value = t('configuracion.seguridad.codigoIncorrecto')
  } finally {
    twoFaLoading.value = false
  }
}

function cancelTwoFa() {
  twoFaState.value = 'idle'
  totpCode.value = ''
  qrUri.value = ''
  twoFaError.value = ''
}
</script>

<template>
  <div>
    <header class="header">
      <h1 class="header-titulo">{{ $t('configuracion.title') }}</h1>
    </header>

    <main class="main">

      <!-- ── Apariencia ──────────────────────────────────────────────────── -->
      <section class="seccion">
        <h2 class="seccion-titulo">{{ $t('configuracion.apariencia.title') }}</h2>
        <div class="card row-card">
          <div class="row-info">
            <p class="row-label">{{ $t('configuracion.apariencia.tema') }}</p>
            <p class="row-desc">{{ isDark ? $t('configuracion.apariencia.oscuro') : $t('configuracion.apariencia.claro') }}</p>
          </div>
          <button class="toggle-btn" :class="{ 'toggle-btn--on': !isDark }" @click="toggleTheme">
            <span class="toggle-thumb" />
          </button>
        </div>
      </section>

      <!-- ── Idioma ──────────────────────────────────────────────────────── -->
      <section class="seccion">
        <h2 class="seccion-titulo">{{ $t('configuracion.idioma.title') }}</h2>
        <div class="card">
          <div class="lang-grid">
            <button
              v-for="loc in availableLocales"
              :key="loc"
              class="lang-btn"
              :class="{ 'lang-btn--active': locale === loc }"
              @click="setLocale(loc as 'es' | 'en')"
            >
              {{ loc === 'es' ? '🇨🇱 Español' : '🇺🇸 English' }}
            </button>
          </div>
        </div>
      </section>

      <!-- ── Seguridad (2FA) ─────────────────────────────────────────────── -->
      <section class="seccion">
        <h2 class="seccion-titulo">{{ $t('configuracion.seguridad.title') }}</h2>

        <!-- Idle: show activate / deactivate buttons -->
        <template v-if="twoFaState === 'idle'">
          <div class="card row-card">
            <div class="row-info">
              <p class="row-label">{{ $t('configuracion.seguridad.dosFa') }}</p>
              <p class="row-desc">
                {{ twoFaEnabled === true
                  ? $t('configuracion.seguridad.activo')
                  : twoFaEnabled === false
                    ? $t('configuracion.seguridad.inactivo')
                    : $t('configuracion.seguridad.estado') }}
              </p>
            </div>
            <div class="row-actions">
              <button
                v-if="twoFaEnabled !== true"
                class="btn-small btn-small--accent"
                :disabled="twoFaLoading"
                @click="handleSetup2Fa"
              >
                {{ twoFaLoading ? '…' : $t('configuracion.seguridad.activar') }}
              </button>
              <button
                v-if="twoFaEnabled === true"
                class="btn-small btn-small--danger"
                :disabled="twoFaLoading"
                @click="twoFaState = 'disable'"
              >
                {{ $t('configuracion.seguridad.desactivar') }}
              </button>
            </div>
          </div>
          <p v-if="twoFaError" class="error-msg">{{ twoFaError }}</p>
        </template>

        <!-- QR: setup started, show QR code and confirm form -->
        <template v-else-if="twoFaState === 'qr'">
          <div class="card">
            <p class="qr-instruccion">{{ $t('configuracion.seguridad.escanea') }}</p>
            <div class="qr-wrapper">
              <!-- Uses a free QR code API via img src — data is the provisioning URI, not sensitive -->
              <img
                :src="`https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(qrUri)}`"
                alt="QR Code 2FA"
                class="qr-img"
                width="200"
                height="200"
              />
            </div>
            <label class="field-label">{{ $t('configuracion.seguridad.codigoLabel') }}</label>
            <input
              v-model="totpCode"
              class="input input--center"
              type="text"
              inputmode="numeric"
              maxlength="6"
              :placeholder="$t('configuracion.seguridad.codigoPlaceholder')"
              @keydown.enter="handleConfirm2Fa"
            />
            <p v-if="twoFaError" class="error-msg">{{ twoFaError }}</p>
            <div class="form-acciones">
              <button
                class="btn-primary"
                :disabled="twoFaLoading || totpCode.length < 6"
                @click="handleConfirm2Fa"
              >
                {{ twoFaLoading ? '…' : $t('configuracion.seguridad.confirmar') }}
              </button>
              <button class="btn-cancelar" @click="cancelTwoFa">
                {{ $t('configuracion.cancelar') }}
              </button>
            </div>
          </div>
        </template>

        <!-- Disable: TOTP code required to disable -->
        <template v-else>
          <div class="card">
            <p class="qr-instruccion">{{ $t('configuracion.seguridad.ingresaCodigoDesactivar') }}</p>
            <label class="field-label">{{ $t('configuracion.seguridad.codigoLabel') }}</label>
            <input
              v-model="totpCode"
              class="input input--center"
              type="text"
              inputmode="numeric"
              maxlength="6"
              :placeholder="$t('configuracion.seguridad.codigoPlaceholder')"
              @keydown.enter="handleDisable2Fa"
            />
            <p v-if="twoFaError" class="error-msg">{{ twoFaError }}</p>
            <div class="form-acciones">
              <button
                class="btn-danger"
                :disabled="twoFaLoading || totpCode.length < 6"
                @click="handleDisable2Fa"
              >
                {{ twoFaLoading ? '…' : $t('configuracion.seguridad.desactivar') }}
              </button>
              <button class="btn-cancelar" @click="cancelTwoFa">
                {{ $t('configuracion.cancelar') }}
              </button>
            </div>
          </div>
        </template>
      </section>

      <!-- ── Cuenta ──────────────────────────────────────────────────────── -->
      <section class="seccion">
        <h2 class="seccion-titulo">{{ $t('configuracion.cuenta.title') }}</h2>
        <div class="card row-card">
          <div class="row-info">
            <p class="row-label">{{ auth.userName }}</p>
            <p class="row-desc">{{ $t('configuracion.cuenta.sesionActiva') }}</p>
          </div>
          <button class="btn-small btn-small--danger" @click="auth.logout()">
            {{ $t('nav.cerrarSesion') }}
          </button>
        </div>
      </section>

    </main>

    <UiToast />
  </div>
</template>

<style scoped>
.header {
  padding: 20px 20px 0;
  border-bottom: 1px solid var(--border-subtle);
  padding-bottom: 16px;
  margin-bottom: 4px;
}
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 0; }

.main { padding: 16px 16px 100px; flex: 1; }

.seccion { margin-bottom: 28px; }
.seccion-titulo { font-size: 16px; font-weight: 700; color: var(--accent-light); margin: 0 0 12px; }

.card {
  background: var(--bg-elevated);
  border: 1px solid var(--border);
  border-radius: 18px;
  padding: 16px;
}

/* Row layout (label + action side by side) */
.row-card { display: flex; align-items: center; gap: 12px; }
.row-info { flex: 1; }
.row-label { font-size: 15px; font-weight: 600; color: var(--text-primary); margin: 0 0 2px; }
.row-desc { font-size: 12px; color: var(--text-muted); margin: 0; }
.row-actions { display: flex; gap: 8px; flex-shrink: 0; }

/* Theme toggle switch */
.toggle-btn {
  width: 52px;
  height: 28px;
  border-radius: 100px;
  background: var(--border);
  border: none;
  cursor: pointer;
  padding: 3px;
  transition: background 0.2s;
  flex-shrink: 0;
  position: relative;
}
.toggle-btn--on { background: var(--accent); }
.toggle-thumb {
  display: block;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: #fff;
  transition: transform 0.2s;
}
.toggle-btn--on .toggle-thumb { transform: translateX(24px); }

/* Language grid */
.lang-grid { display: flex; gap: 10px; flex-wrap: wrap; }
.lang-btn {
  flex: 1;
  min-width: 120px;
  padding: 12px 16px;
  border: 1.5px solid var(--border);
  border-radius: 12px;
  background: transparent;
  color: var(--text-muted);
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
}
.lang-btn--active {
  border-color: var(--accent);
  background: var(--accent-bg);
  color: var(--accent-soft);
}

/* Small action buttons */
.btn-small {
  padding: 8px 14px;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: opacity 0.15s;
}
.btn-small:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-small--accent { background: var(--accent-bg); color: var(--accent-soft); }
.btn-small--danger { background: rgba(239,68,68,0.1); color: var(--danger); }

/* QR / form styles */
.qr-instruccion { font-size: 14px; color: var(--text-muted); margin: 0 0 16px; line-height: 1.5; }
.qr-wrapper { display: flex; justify-content: center; margin-bottom: 20px; }
.qr-img { border-radius: 12px; border: 1px solid var(--border); }

.field-label { display: block; font-size: 12px; color: var(--text-label); font-weight: 600; letter-spacing: 0.5px; margin-bottom: 8px; }
.input {
  width: 100%; background: var(--bg-input); border: 1.5px solid var(--border); border-radius: 12px;
  padding: 13px 16px; font-size: 15px; color: var(--text-primary); outline: none; box-sizing: border-box;
}
.input:focus { border-color: var(--accent); }
.input--center { text-align: center; font-size: 22px; letter-spacing: 6px; font-weight: 700; }

.error-msg { font-size: 13px; color: var(--danger); margin: 8px 0 0; }

.form-acciones { display: flex; gap: 8px; margin-top: 16px; }
.btn-primary {
  flex: 1; background: linear-gradient(135deg, #7c3aed, #4f46e5); color: #fff; border: none;
  border-radius: 12px; padding: 13px 16px; font-size: 14px; font-weight: 600; cursor: pointer;
}
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-danger {
  flex: 1; background: rgba(239,68,68,0.15); color: var(--danger); border: 1px solid rgba(239,68,68,0.3);
  border-radius: 12px; padding: 13px 16px; font-size: 14px; font-weight: 600; cursor: pointer;
}
.btn-danger:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-cancelar {
  flex: 1; background: transparent; border: 1.5px solid var(--border); color: var(--text-label);
  border-radius: 12px; padding: 13px 16px; font-size: 14px; font-weight: 600; cursor: pointer;
}

@media (min-width: 768px) {
  .header { max-width: 1100px; margin-inline: auto; padding-inline: 32px; width: 100%; }
  .main { padding: 24px 32px 40px; max-width: 700px; width: 100%; margin-inline: auto; }
}
</style>
