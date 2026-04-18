<script setup lang="ts">
const { t } = useI18n()

const props = defineProps<{ password: string }>()

const criteria = computed(() => [
  /[A-Z]/.test(props.password),
  /[a-z]/.test(props.password),
  /[0-9]/.test(props.password),
  /[!@#$%^&*()_\-+=\[\]{};':"\\|,.<>\/?`~]/.test(props.password),
])

const score = computed(() => criteria.value.filter(Boolean).length)

const label = computed(() => {
  if (props.password.length === 0) return ''
  if (score.value <= 1) return t('auth.register.passwordStrength.weak')
  if (score.value <= 3) return t('auth.register.passwordStrength.medium')
  return t('auth.register.passwordStrength.strong')
})

const segmentColor = (index: number) => {
  if (index >= score.value) return 'var(--border)'
  if (score.value <= 1) return 'var(--danger)'
  if (score.value <= 3) return '#f59e0b'
  return 'var(--success)'
}
</script>

<template>
  <div v-if="password.length > 0" class="strength-bar">
    <div class="segments">
      <div
        v-for="i in 4"
        :key="i"
        class="segment"
        :style="{ background: segmentColor(i - 1) }"
      />
    </div>
    <span class="label">{{ label }}</span>
  </div>
</template>

<style scoped>
.strength-bar {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-top: 4px;
}

.segments {
  display: flex;
  gap: 4px;
  flex: 1;
}

.segment {
  height: 4px;
  flex: 1;
  border-radius: 2px;
  transition: background 0.25s;
}

.label {
  font-size: 11px;
  color: var(--text-muted);
  white-space: nowrap;
}
</style>
