<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'
import { useToast } from '~/composables/use-toast'

definePageMeta({ middleware: 'auth' })
useHead({ title: 'Configurar familia - FinanzasApp' })

const { t } = useI18n()
const auth = useAuthStore()
const { ok: toastOk } = useToast()

const tab = shallowRef<'create' | 'join'>('create')

async function handleSuccess(token: string, familyId: string) {
  auth.updateToken(token, familyId)
  toastOk(tab.value === 'create' ? t('familia.createSuccess') : t('familia.joinSuccess'))
  await navigateTo('/')
}
</script>

<template>
  <div class="page">
    <div class="card">
      <div class="card-icon">💜</div>
      <h1 class="card-title">{{ $t('familia.setupTitle') }}</h1>
      <p class="card-subtitle">{{ $t('familia.setupSubtitle') }}</p>

      <!-- Tab switcher -->
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

      <!-- Form panels -->
      <FamiliaSetupCreate v-if="tab === 'create'" @success="handleSuccess" />
      <FamiliaSetupJoin v-else @success="handleSuccess" />
    </div>

    <UiToast />
  </div>
</template>

<style scoped>
.page {
  min-height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px 16px 40px;
  background: #0c0c18;
}

.card {
  width: 100%;
  max-width: 440px;
  background: #13131f;
  border: 1px solid #2a2a40;
  border-radius: 24px;
  padding: 36px 28px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.card-icon { font-size: 36px; text-align: center; }
.card-title { font-size: 22px; font-weight: 700; color: #f0eeff; text-align: center; margin: 0; }
.card-subtitle { font-size: 14px; color: #6b6b8a; text-align: center; margin: 0; line-height: 1.5; }

.tabs {
  display: flex;
  background: #0f0f18;
  border: 1px solid #2a2a40;
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
  color: #6b6b8a;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.15s;
}
.tab-btn--active {
  background: #1e1230;
  color: #a78bfa;
}

@media (min-width: 480px) {
  .card { padding: 40px 36px; }
}
</style>
