<!-- app/components/ui/Toast.vue -->
<!-- Auto-imported as <UiToast /> -->
<script setup lang="ts">
// Direct import between composables — no auto-import to avoid circular deps
import { useToast } from '~/composables/use-toast'

const { toasts, quitar } = useToast()
</script>

<template>
  <Teleport to="body">
    <TransitionGroup name="toast" tag="div" class="toast-container">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        class="toast"
        :class="`toast--${toast.tipo}`"
        @click="quitar(toast.id)"
      >
        {{ toast.texto }}
      </div>
    </TransitionGroup>
  </Teleport>
</template>

<style scoped>
.toast-container {
  position: fixed;
  bottom: 90px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  flex-direction: column;
  gap: 8px;
  z-index: 999;
  pointer-events: none;
}

.toast {
  background: #18182a;
  border: 1px solid #3a3a55;
  padding: 12px 22px;
  border-radius: 100px;
  font-size: 14px;
  font-weight: 600;
  white-space: nowrap;
  pointer-events: all;
  cursor: pointer;
}

.toast--ok { color: #4ade80; border-color: #2a4a2a; }
.toast--error { color: #f87171; border-color: #5a2a2a; }
.toast--info { color: #a78bfa; border-color: #3a2a60; }

.toast-enter-active { transition: all 0.3s ease; }
.toast-leave-active { transition: all 0.25s ease; }
.toast-enter-from { opacity: 0; transform: translateY(10px); }
.toast-leave-to { opacity: 0; transform: translateY(-10px); }
</style>
