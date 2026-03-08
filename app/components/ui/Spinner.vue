<!-- app/components/ui/Spinner.vue -->
<!-- Auto-imported as <UiSpinner /> -->
<template>
  <div class="spinner-wrap">
    <div class="spinner" />
    <p v-if="$slots.default" class="spinner-text"><slot /></p>
  </div>
</template>

<style scoped>
.spinner-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
  padding: 40px 20px;
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid #2a2a40;
  border-top-color: #7c3aed;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

.spinner-text {
  color: #6b6b8a;
  font-size: 14px;
}

@keyframes spin { to { transform: rotate(360deg); } }
</style>
