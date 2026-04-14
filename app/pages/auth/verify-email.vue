<!-- app/pages/auth/verify-email.vue -->
<!-- Reads ?token= from the URL, calls POST /auth/verify-email, and shows result. -->
<script setup lang="ts">
definePageMeta({ layout: 'auth' })
useHead({ title: 'Verificar correo - FinanzasApp' })

const { t } = useI18n()
const route = useRoute()
const { $api } = useNuxtApp()

type Status = 'verifying' | 'success' | 'expired' | 'invalid'
const status = shallowRef<Status>('verifying')

onMounted(async () => {
  const token = route.query.token as string | undefined

  if (!token) {
    status.value = 'invalid'
    return
  }

  try {
    await ($api as typeof $fetch)('/auth/verify-email', {
      method: 'POST',
      body: { token },
    })
    status.value = 'success'
  } catch (e: any) {
    const statusCode = e?.status ?? e?.statusCode
    status.value = statusCode === 409 ? 'success' : // already verified → treat as success
                   statusCode === 400 ? 'expired' :
                   'invalid'
  }
})
</script>

<template>
  <div class="auth-card">
    <div class="auth-header">
      <span class="auth-icon">
        {{ status === 'verifying' ? '⏳' : status === 'success' ? '✅' : '❌' }}
      </span>
      <h1 class="auth-title">{{ $t('auth.verifyEmail.title') }}</h1>
    </div>

    <div class="status-body">
      <p v-if="status === 'verifying'" class="msg msg--muted">
        {{ $t('auth.verifyEmail.verifying') }}
      </p>
      <p v-else-if="status === 'success'" class="msg msg--success">
        {{ $t('auth.verifyEmail.success') }}
      </p>
      <p v-else-if="status === 'expired'" class="msg msg--error">
        {{ $t('auth.verifyEmail.expired') }}
      </p>
      <p v-else class="msg msg--error">
        {{ $t('auth.verifyEmail.invalid') }}
      </p>

      <NuxtLink v-if="status !== 'verifying'" to="/auth/login" class="btn-primary">
        {{ $t('auth.verifyEmail.goToLogin') }}
      </NuxtLink>
    </div>
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
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.auth-header { text-align: center; }
.auth-icon { font-size: 48px; display: block; margin-bottom: 12px; }
.auth-title { font-size: 20px; font-weight: 700; color: #c4b5fd; margin: 0; }

.status-body { display: flex; flex-direction: column; gap: 20px; align-items: center; }

.msg { font-size: 14px; line-height: 1.6; text-align: center; margin: 0; }
.msg--muted  { color: #6b6b8a; }
.msg--success { color: #34d399; }
.msg--error   { color: #f87171; }

.btn-primary {
  display: inline-block;
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff;
  text-decoration: none;
  border-radius: 14px;
  padding: 13px 28px;
  font-size: 14px;
  font-weight: 600;
}
</style>
