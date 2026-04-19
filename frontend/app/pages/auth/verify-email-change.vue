<!-- app/pages/auth/verify-email-change.vue -->
<script setup lang="ts">
definePageMeta({ layout: 'auth' })
useHead({ title: 'Verificar nuevo correo - DomusPay' })

const { t } = useI18n()
const route = useRoute()
const { $api } = useNuxtApp()

type State = 'verifying' | 'success' | 'error'
const state = shallowRef<State>('verifying')
const errorKey = shallowRef<'invalid' | 'expired'>('invalid')

onMounted(async () => {
  const token = route.query.token as string | undefined
  if (!token) {
    errorKey.value = 'invalid'
    state.value = 'error'
    return
  }
  try {
    await ($api as typeof $fetch)(`/auth/confirm-email-change?token=${encodeURIComponent(token)}`)
    state.value = 'success'
  } catch (e: any) {
    const detail: string = e?.data?.detail ?? ''
    errorKey.value = detail.includes('Expired') ? 'expired' : 'invalid'
    state.value = 'error'
  }
})
</script>

<template>
  <div class="wrapper">
    <h1 class="titulo">{{ $t('auth.verifyEmailChange.title') }}</h1>

    <template v-if="state === 'verifying'">
      <p class="msg">{{ $t('auth.verifyEmailChange.verifying') }}</p>
    </template>

    <template v-else-if="state === 'success'">
      <p class="msg msg--ok">{{ $t('auth.verifyEmailChange.success') }}</p>
      <NuxtLink to="/configuracion" class="btn">
        {{ $t('auth.verifyEmailChange.goToSettings') }}
      </NuxtLink>
    </template>

    <template v-else>
      <p class="msg msg--err">
        {{ errorKey === 'expired'
          ? $t('auth.verifyEmailChange.expired')
          : $t('auth.verifyEmailChange.invalid') }}
      </p>
      <NuxtLink to="/configuracion" class="btn">
        {{ $t('auth.verifyEmailChange.goToSettings') }}
      </NuxtLink>
    </template>
  </div>
</template>

<style scoped>
.wrapper {
  max-width: 400px;
  margin: 80px auto;
  padding: 32px 24px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 20px;
  text-align: center;
}
.titulo {
  font-size: 20px;
  font-weight: 700;
  color: var(--accent-light);
  margin: 0 0 20px;
}
.msg {
  font-size: 15px;
  color: var(--text-muted);
  margin: 0 0 24px;
  line-height: 1.5;
}
.msg--ok  { color: var(--success, #34d399); }
.msg--err { color: var(--danger); }
.btn {
  display: inline-block;
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff;
  text-decoration: none;
  padding: 12px 24px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 14px;
}
</style>
