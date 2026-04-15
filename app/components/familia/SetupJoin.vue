<script setup lang="ts">
const emit = defineEmits<{
  success: [token: string, familyId: string]
}>()

const { t } = useI18n()
const { $api } = useNuxtApp()

const familyIdInput = shallowRef('')
const loading = shallowRef(false)
const errorMsg = shallowRef<string | null>(null)

const canSubmit = computed(() => familyIdInput.value.trim().length > 0 && !loading.value)

async function handleJoin() {
  errorMsg.value = null
  loading.value = true
  try {
    const res = await ($api as typeof $fetch)('/families/join', {
      method: 'POST',
      body: { familyId: familyIdInput.value.trim() },
    }) as { token: string; familyId: string }
    emit('success', res.token, res.familyId)
  } catch {
    errorMsg.value = t('familia.joinError')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="form">
    <label class="field-label">{{ $t('familia.joinLabel') }}</label>
    <input
      v-model="familyIdInput"
      class="input"
      type="text"
      :placeholder="$t('familia.joinPlaceholder')"
      :disabled="loading"
      @keydown.enter="canSubmit && handleJoin()"
    />
    <p class="hint">{{ $t('familia.joinHint') }}</p>
    <p v-if="errorMsg" class="error-msg">{{ errorMsg }}</p>
    <button class="btn-primary" :disabled="!canSubmit" @click="handleJoin">
      {{ loading ? $t('familia.joining') : $t('familia.joinBtn') }}
    </button>
  </div>
</template>

<style scoped>
.form { display: flex; flex-direction: column; gap: 14px; }
.field-label { font-size: 12px; color: var(--text-label); font-weight: 600; letter-spacing: 0.5px; }
.input {
  width: 100%; background: var(--bg-input); border: 1.5px solid var(--border); border-radius: 12px;
  padding: 14px 16px; font-size: 15px; color: var(--text-primary); outline: none;
  transition: border-color 0.15s;
}
.input:focus { border-color: var(--accent); }
.input:disabled { opacity: 0.5; cursor: not-allowed; }
.hint { font-size: 12px; color: var(--text-muted); margin: 0; line-height: 1.5; }
.error-msg { font-size: 13px; color: var(--danger); margin: 0; }
.btn-primary {
  background: linear-gradient(135deg, #7c3aed, #4f46e5); color: #fff; border: none;
  border-radius: 14px; padding: 15px 24px; font-size: 15px; font-weight: 600;
  cursor: pointer; width: 100%; transition: opacity 0.15s;
}
.btn-primary:disabled { opacity: 0.5; cursor: not-allowed; }
</style>
