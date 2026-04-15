<!-- app/components/ui/Error404.vue -->
<!-- Auto-imported as <UiError404 /> -->
<script setup lang="ts">
import errorImg from '~/assets/error404.png'

defineProps<{
  message?: string | null
}>()
</script>

<template>
  <div class="error404">
    <img :src="errorImg" alt="404" class="error404-img" />
    <p class="error404-code">404</p>
    <p v-if="message" class="error404-msg">{{ message }}</p>
  </div>
</template>

<style scoped>
.error404 {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 48px 24px;
  text-align: center;
}

.error404-img {
  width: 100%;
  max-width: 420px;
  height: auto;
}

.error404-code {
  font-size: 48px;
  font-weight: 800;
  color: var(--accent-light);
  letter-spacing: -2px;
  margin: 16px 0 8px;
  line-height: 1;
}

.error404-msg {
  font-size: 15px;
  color: var(--text-muted);
  margin: 0;
  max-width: 360px;
  line-height: 1.5;
}
</style>
