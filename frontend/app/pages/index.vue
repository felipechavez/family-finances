<!-- app/pages/index.vue -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useAuthStore } from '~/stores/auth'

const authStore = useAuthStore()
const { isAuthenticated, hasFamiliy } = storeToRefs(authStore)

useHead({
  title: computed(() =>
    isAuthenticated.value ? 'Dashboard - DomusPay' : 'DomusPay — Finanzas familiares'
  ),
})

definePageMeta({ layout: false })

watch(isAuthenticated, (authed) => {
  if (authed && !hasFamiliy.value) {
    navigateTo('/familia/setup')
  }
}, { immediate: true })
</script>

<template>
  <ClientOnly>
    <NuxtLayout :name="isAuthenticated ? 'default' : 'landing'">
      <LandingPage v-if="!isAuthenticated" />
      <DashboardView v-else />
    </NuxtLayout>
  </ClientOnly>
</template>
