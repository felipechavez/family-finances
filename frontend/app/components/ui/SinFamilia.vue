<!-- app/components/ui/SinFamilia.vue -->
<!-- Auto-imported as <UiSinFamilia /> -->
<!-- Shown in the default layout when the user is authenticated but has no family. -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'
import { useToast } from '~/composables/use-toast'

const auth = useAuthStore()
const { t } = useI18n()
const { ok: toastOk } = useToast()

const tab = shallowRef<'create' | 'join'>('create')

async function handleSuccess(token: string, familyId: string) {
  auth.updateToken(token, familyId)
  toastOk(tab.value === 'create' ? t('familia.createSuccess') : t('familia.joinSuccess'))
  await navigateTo('/')
}
</script>

<template>
  <div class="sin-familia">
    <div class="card">
      <div class="card-icon">💜</div>
      <h1 class="card-title">{{ $t('familia.setupTitle') }}</h1>
      <p class="card-subtitle">{{ $t('familia.setupSubtitle') }}</p>

      <div class="tabs">
        <button
          class="tab-btn"
          :class="{ 'tab-btn--active': tab === 'create' }"
          @click="tab = 'create'"
        >
          {{ $t('familia.tabCreate') }}
        </button>
        <button
          class="tab-btn"
          :class="{ 'tab-btn--active': tab === 'join' }"
          @click="tab = 'join'"
        >
          {{ $t('familia.tabJoin') }}
        </button>
      </div>

      <FamiliaSetupCreate v-if="tab === 'create'" @success="handleSuccess" />
      <FamiliaSetupJoin v-else @success="handleSuccess" />

      <button class="btn-logout" @click="auth.logout()">
        {{ $t('nav.cerrarSesion') }}
      </button>
    </div>

    <UiToast />
  </div>
</template>

<style scoped>
.sin-familia {
  min-height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px 16px 40px;
  background: var(--bg-base);
}

.card {
  width: 100%;
  max-width: 440px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 24px;
  padding: 36px 28px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.card-icon { font-size: 36px; text-align: center; }
.card-title { font-size: 22px; font-weight: 700; color: var(--text-primary); text-align: center; margin: 0; }
.card-subtitle { font-size: 14px; color: var(--text-muted); text-align: center; margin: 0; line-height: 1.5; }

.tabs {
  display: flex;
  background: var(--bg-input);
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 4px;
  gap: 4px;
}

.tab-btn {
  flex: 1;
  padding: 10px 12px;
  border: none;
  border-radius: 9px;
  background: transparent;
  color: var(--text-muted);
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
}
.tab-btn--active {
  background: var(--accent-bg);
  color: var(--accent-soft);
}

.btn-logout {
  background: none;
  border: 1px solid var(--border);
  color: var(--text-muted);
  border-radius: 10px;
  padding: 10px 16px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  width: 100%;
  transition: all 0.15s;
}
.btn-logout:hover { border-color: var(--danger); color: var(--danger); }

@media (min-width: 480px) {
  .card { padding: 40px 36px; }
}
</style>
