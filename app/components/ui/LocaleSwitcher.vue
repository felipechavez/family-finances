<script setup lang="ts">
const { locale, locales, setLocale } = useI18n()

const other = computed(() =>
  locales.value.find(l => l.code !== locale.value)!
)

function toggle() {
  setLocale(other.value.code as 'es' | 'en')
}
</script>

<template>
  <button class="locale-btn" :aria-label="`Switch to ${other.name}`" @click="toggle">
    <span class="locale-active">{{ locale.toUpperCase() }}</span>
    <span class="locale-sep">·</span>
    <span class="locale-other">{{ other.code.toUpperCase() }}</span>
  </button>
</template>

<style scoped>
.locale-btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  background: var(--bg-elevated);
  border: 1px solid var(--border);
  border-radius: 20px;
  padding: 5px 12px;
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.5px;
  cursor: pointer;
  transition: border-color 0.15s;
  color: var(--text-muted);
}
.locale-btn:hover { border-color: var(--accent); }

.locale-active { color: var(--accent-soft); }
.locale-sep { color: var(--border); }
.locale-other { color: var(--text-muted); }
</style>
