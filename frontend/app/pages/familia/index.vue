<!-- app/pages/familia/index.vue -->
<!-- Family management: invite code, QR, members list, role badges -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'
import { useToast } from '~/composables/use-toast'

definePageMeta({ middleware: 'auth' })
useHead({ title: 'Mi Familia - DomusPay' })

const { t } = useI18n()
const auth = useAuthStore()
const { ok: toastOk, error: toastError } = useToast()
const { $api } = useNuxtApp()
const config = useRuntimeConfig()

// ── Fetch family info ──────────────────────────────────────────────────────
interface MemberDto {
  userId: string
  name: string
  role: 'owner' | 'admin' | 'member'
  joinedAt: string
}
interface FamilyInfo {
  familyId: string
  name: string
  inviteCode: string
  isOwner: boolean
  members: MemberDto[]
}

const { data: familia, status, error, refresh } = useFetch<FamilyInfo>('/families/me', {
  key: 'familia-me',
  server: false,
  $fetch: useNuxtApp().$api as typeof $fetch,
})

// ── Invite code UI ─────────────────────────────────────────────────────────
const inviteCode = ref('')
const copied = shallowRef(false)
const regenerating = shallowRef(false)

// Sync inviteCode from fetch response (shallowRef data → need explicit watch)
watch(
  () => familia.value?.inviteCode,
  (code) => { if (code) inviteCode.value = code },
  { immediate: true },
)

const appBase = computed(() => {
  if (import.meta.client) return window.location.origin
  return config.public.apiBase.replace('/api', '').replace(':58196', ':3000')
})

const joinUrl  = computed(() => `${appBase.value}/familia/setup`)
const qrData   = computed(() =>
  `${joinUrl.value}?code=${inviteCode.value}`,
)
const qrSrc    = computed(() =>
  `https://api.qrserver.com/v1/create-qr-code/?size=220x220&data=${encodeURIComponent(qrData.value)}`,
)

async function copyCode() {
  if (!inviteCode.value) return
  await navigator.clipboard.writeText(inviteCode.value)
  copied.value = true
  setTimeout(() => { copied.value = false }, 2000)
}

async function regenerateCode() {
  regenerating.value = true
  try {
    const res = await ($api as typeof $fetch)('/families/me/regenerate-code', { method: 'POST' }) as { inviteCode: string }
    inviteCode.value = res.inviteCode
    toastOk(t('familia.gestion.codigoRegenerado'))
  } catch {
    toastError(t('familia.gestion.errorRegenerado'))
  } finally {
    regenerating.value = false
  }
}

// ── Share ──────────────────────────────────────────────────────────────────
async function shareCode() {
  if (!inviteCode.value) return
  // qrData already contains the full URL with ?code= pre-filled
  const shareUrl = qrData.value
  const text = t('familia.gestion.shareText', { code: inviteCode.value, url: shareUrl })
  if (navigator.share) {
    try {
      await navigator.share({ title: 'DomusPay — Unirse a la familia', text, url: shareUrl })
    } catch { /* user cancelled */ }
  } else {
    // fallback: copy the full share text
    await navigator.clipboard.writeText(text)
    toastOk(t('familia.gestion.linkCopiado'))
  }
}

// ── Role helpers ───────────────────────────────────────────────────────────
const roleLabel: Record<string, string> = {
  owner:  '👑',
  admin:  '🛡️',
  member: '👤',
}
function roleBadgeClass(role: string) {
  return {
    'badge--owner':  role === 'owner',
    'badge--admin':  role === 'admin',
    'badge--member': role === 'member',
  }
}
</script>

<template>
  <div>
    <header class="header">
      <h1 class="header-titulo">{{ $t('familia.gestion.title') }}</h1>
    </header>

    <main class="main">
      <UiSpinner v-if="status === 'pending'">{{ $t('common.loading') }}</UiSpinner>
      <UiError404 v-else-if="error" />

      <template v-else-if="familia">

        <!-- ── Invite code card ──────────────────────────────────────────── -->
        <section class="seccion">
          <h2 class="seccion-titulo">{{ $t('familia.gestion.codigoInvitacion') }}</h2>
          <div class="invite-card">

            <!-- Code + copy -->
            <div class="code-row">
              <span class="code-display">{{ inviteCode }}</span>
              <button class="btn-icon" :title="$t('familia.gestion.copiar')" @click="copyCode">
                {{ copied ? '✓' : '📋' }}
              </button>
            </div>

            <!-- QR code -->
            <div class="qr-section">
              <img
                :src="qrSrc"
                alt="QR code"
                class="qr-img"
                width="220"
                height="220"
              />
            </div>

            <!-- Actions -->
            <div class="invite-actions">
              <button class="btn-share" @click="shareCode">
                {{ $t('familia.gestion.compartir') }}
              </button>
              <button
                v-if="familia.isOwner"
                class="btn-regenerar"
                :disabled="regenerating"
                @click="regenerateCode"
              >
                {{ regenerating ? '…' : $t('familia.gestion.regenerar') }}
              </button>
            </div>

            <p class="invite-hint">{{ $t('familia.gestion.inviteHint') }}</p>
          </div>
        </section>

        <!-- ── Members list ──────────────────────────────────────────────── -->
        <section class="seccion">
          <h2 class="seccion-titulo">
            {{ $t('familia.gestion.miembros') }}
            <span class="member-count">{{ familia.members.length }}</span>
          </h2>
          <div class="members-card">
            <div
              v-for="m in familia.members"
              :key="m.userId"
              class="member-row"
            >
              <div class="member-avatar">
                {{ m.name.charAt(0).toUpperCase() }}
              </div>
              <div class="member-info">
                <p class="member-name">
                  {{ m.name }}
                  <span v-if="m.userId === auth.user?.id" class="you-badge">{{ $t('familia.gestion.tu') }}</span>
                </p>
                <p class="member-date">
                  {{ $t('familia.gestion.desde') }}
                  {{ new Date(m.joinedAt).toLocaleDateString() }}
                </p>
              </div>
              <span class="badge" :class="roleBadgeClass(m.role)">
                {{ roleLabel[m.role] ?? '👤' }}
                {{ $t(`familia.roles.${m.role}`) }}
              </span>
            </div>
          </div>
        </section>

        <!-- ── Family name ───────────────────────────────────────────────── -->
        <section class="seccion">
          <h2 class="seccion-titulo">{{ $t('familia.gestion.infoFamilia') }}</h2>
          <div class="info-card">
            <p class="info-label">{{ $t('familia.gestion.nombre') }}</p>
            <p class="info-valor">{{ familia.name }}</p>
          </div>
        </section>

      </template>
    </main>

    <UiToast />
  </div>
</template>

<style scoped>
.header {
  padding: 20px 20px 0;
  border-bottom: 1px solid var(--border-subtle);
  padding-bottom: 16px;
  margin-bottom: 4px;
}
.header-titulo { font-size: 22px; font-weight: 700; letter-spacing: -0.5px; margin: 0; }

.main { padding: 16px 16px 100px; flex: 1; }

.seccion { margin-bottom: 28px; }
.seccion-titulo {
  font-size: 16px; font-weight: 700; color: var(--accent-light);
  margin: 0 0 12px; display: flex; align-items: center; gap: 8px;
}
.member-count {
  background: var(--accent-bg); color: var(--accent-soft);
  border-radius: 100px; padding: 2px 10px; font-size: 13px;
}

/* ── Invite card ── */
.invite-card {
  background: var(--bg-elevated); border: 1px solid var(--border);
  border-radius: 20px; padding: 20px;
  display: flex; flex-direction: column; align-items: center; gap: 16px;
}

.code-row { display: flex; align-items: center; gap: 12px; }
.code-display {
  font-size: 32px; font-weight: 800; letter-spacing: 8px;
  color: var(--accent-light); font-variant-numeric: tabular-nums;
}
.btn-icon {
  background: var(--accent-bg); border: none; border-radius: 10px;
  padding: 8px 12px; font-size: 16px; cursor: pointer; transition: opacity 0.15s;
  color: var(--accent-soft);
}
.btn-icon:hover { opacity: 0.8; }

.qr-section { display: flex; justify-content: center; }
.qr-img { border-radius: 14px; border: 1px solid var(--border); }

.invite-actions { display: flex; gap: 10px; width: 100%; }
.btn-share {
  flex: 1; background: linear-gradient(135deg, #7c3aed, #4f46e5); color: #fff;
  border: none; border-radius: 12px; padding: 13px 16px;
  font-size: 14px; font-weight: 600; cursor: pointer;
}
.btn-regenerar {
  flex: 1; background: var(--bg-input); border: 1.5px solid var(--border);
  color: var(--text-muted); border-radius: 12px; padding: 13px 16px;
  font-size: 13px; font-weight: 600; cursor: pointer; transition: border-color 0.15s;
}
.btn-regenerar:hover:not(:disabled) { border-color: var(--accent); color: var(--accent-soft); }
.btn-regenerar:disabled { opacity: 0.5; cursor: not-allowed; }
.invite-hint {
  font-size: 12px; color: var(--text-muted); text-align: center; margin: 0; line-height: 1.5;
}

/* ── Members card ── */
.members-card {
  background: var(--bg-elevated); border: 1px solid var(--border);
  border-radius: 18px; overflow: hidden;
}
.member-row {
  display: flex; align-items: center; gap: 12px;
  padding: 14px 16px; border-bottom: 1px solid var(--border);
}
.member-row:last-child { border-bottom: none; }
.member-avatar {
  width: 40px; height: 40px; border-radius: 50%;
  background: var(--accent-bg); color: var(--accent-soft);
  display: flex; align-items: center; justify-content: center;
  font-size: 16px; font-weight: 700; flex-shrink: 0;
}
.member-info { flex: 1; min-width: 0; }
.member-name {
  font-size: 14px; font-weight: 600; color: var(--text-primary);
  margin: 0 0 2px; display: flex; align-items: center; gap: 6px;
}
.you-badge {
  background: var(--accent-bg); color: var(--accent-soft);
  border-radius: 100px; padding: 1px 8px; font-size: 10px; font-weight: 700;
}
.member-date { font-size: 11px; color: var(--text-muted); margin: 0; }

.badge {
  flex-shrink: 0; font-size: 11px; font-weight: 700; border-radius: 100px;
  padding: 4px 10px; display: flex; align-items: center; gap: 4px;
}
.badge--owner  { background: rgba(250,204,21,0.15); color: #ca8a04; }
.badge--admin  { background: rgba(99,102,241,0.15); color: var(--accent-soft); }
.badge--member { background: var(--bg-input); color: var(--text-muted); }

/* ── Info card ── */
.info-card {
  background: var(--bg-elevated); border: 1px solid var(--border);
  border-radius: 16px; padding: 16px;
}
.info-label { font-size: 11px; color: var(--text-muted); font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; margin: 0 0 4px; }
.info-valor  { font-size: 18px; font-weight: 700; color: var(--text-primary); margin: 0; }

@media (min-width: 768px) {
  .header { max-width: 1100px; margin-inline: auto; padding-inline: 32px; width: 100%; }
  .main { padding: 24px 32px 40px; max-width: 700px; width: 100%; margin-inline: auto; }
}
</style>
