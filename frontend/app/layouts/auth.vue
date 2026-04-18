<!-- app/layouts/auth.vue -->
<!-- Minimal layout for login/register pages (no sidebar) -->
<template>
  <div class="auth-layout">
    <slot />
  </div>
</template>

<style scoped>
.auth-layout {
  min-height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-base);
  padding: 20px;
}
</style>
