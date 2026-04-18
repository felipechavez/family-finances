<!-- app/components/ui/NotificationBell.vue -->
<!-- In-app notification bell with badge, dropdown, and polling every 60s -->
<script setup lang="ts">
import { useIntervalFn } from '@vueuse/core'

const { $api } = useNuxtApp()
const { t } = useI18n()

interface NotificationItem {
  id: string
  type: string
  title: string
  body: string | null
  isRead: boolean
  createdAt: string
}

const notifications = ref<NotificationItem[]>([])
const open          = ref(false)
const loading       = ref(false)

const unreadCount = computed(() => notifications.value.filter(n => !n.isRead).length)

async function fetchNotifications() {
  try {
    const data = await ($api as typeof $fetch)<NotificationItem[]>('/notifications')
    notifications.value = data ?? []
  } catch {
    // Silently fail — bell should never crash the app
  }
}

async function markRead(id: string) {
  try {
    await ($api as typeof $fetch)(`/notifications/${id}/read`, { method: 'PATCH' })
    const n = notifications.value.find(n => n.id === id)
    if (n) n.isRead = true
  } catch { /* silent */ }
}

async function markAllRead() {
  if (unreadCount.value === 0) return
  loading.value = true
  try {
    await ($api as typeof $fetch)('/notifications/read-all', { method: 'PATCH' })
    notifications.value.forEach(n => { n.isRead = true })
  } catch { /* silent */ }
  finally { loading.value = false }
}

function toggleOpen() {
  open.value = !open.value
}

function closeDropdown() {
  open.value = false
}

function formatTime(iso: string) {
  const d = new Date(iso)
  return d.toLocaleDateString(undefined, { day: '2-digit', month: '2-digit' })
    + ' ' + d.toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })
}

// Initial fetch + poll every 60s
onMounted(fetchNotifications)
useIntervalFn(fetchNotifications, 60_000)

// Close on outside click
function onDocClick(e: MouseEvent) {
  const el = document.getElementById('notif-bell-root')
  if (el && !el.contains(e.target as Node)) closeDropdown()
}
onMounted(() => document.addEventListener('click', onDocClick))
onUnmounted(() => document.removeEventListener('click', onDocClick))
</script>

<template>
  <div id="notif-bell-root" class="bell-root">
    <!-- Bell button -->
    <button class="bell-btn" :aria-label="t('notificaciones.titulo')" @click.stop="toggleOpen">
      <span class="bell-icon">🔔</span>
      <span v-if="unreadCount > 0" class="bell-badge">{{ unreadCount > 9 ? '9+' : unreadCount }}</span>
    </button>

    <!-- Dropdown -->
    <Transition name="dropdown">
      <div v-if="open" class="dropdown" @click.stop>
        <div class="dropdown-header">
          <span class="dropdown-title">{{ t('notificaciones.titulo') }}</span>
          <button
            v-if="unreadCount > 0"
            class="btn-mark-all"
            :disabled="loading"
            @click="markAllRead"
          >
            {{ t('notificaciones.marcarTodas') }}
          </button>
        </div>

        <div v-if="notifications.length === 0" class="empty-state">
          {{ t('notificaciones.sinNotificaciones') }}
        </div>

        <ul v-else class="notif-list">
          <li
            v-for="n in notifications"
            :key="n.id"
            class="notif-item"
            :class="{ 'notif-item--unread': !n.isRead }"
            @click="markRead(n.id)"
          >
            <div class="notif-body">
              <p class="notif-title">{{ n.title }}</p>
              <p v-if="n.body" class="notif-desc">{{ n.body }}</p>
              <p class="notif-time">{{ formatTime(n.createdAt) }}</p>
            </div>
            <span v-if="!n.isRead" class="notif-dot" />
          </li>
        </ul>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.bell-root {
  position: relative;
  display: inline-flex;
}

.bell-btn {
  background: none;
  border: none;
  cursor: pointer;
  padding: 6px;
  border-radius: 10px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s;
  -webkit-tap-highlight-color: transparent;
}
.bell-btn:hover { background: var(--accent-hover); }

.bell-icon { font-size: 20px; line-height: 1; }

.bell-badge {
  position: absolute;
  top: 1px;
  right: 1px;
  background: var(--danger);
  color: #fff;
  font-size: 10px;
  font-weight: 700;
  min-width: 16px;
  height: 16px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 3px;
  line-height: 1;
}

/* ── Dropdown ── */
.dropdown {
  position: absolute;
  top: calc(100% + 8px);
  right: 0;
  width: min(300px, calc(100vw - 24px));
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(0,0,0,0.35);
  z-index: 200;
  overflow: hidden;
  max-height: 420px;
  display: flex;
  flex-direction: column;
}

.dropdown-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 16px 10px;
  border-bottom: 1px solid var(--border);
  flex-shrink: 0;
}
.dropdown-title { font-size: 13px; font-weight: 700; color: var(--text-primary); }
.btn-mark-all {
  background: none;
  border: none;
  color: var(--accent-soft);
  font-size: 11px;
  font-weight: 600;
  cursor: pointer;
  padding: 0;
}
.btn-mark-all:disabled { opacity: 0.5; }

.empty-state {
  padding: 32px 16px;
  text-align: center;
  color: var(--text-muted);
  font-size: 13px;
}

.notif-list {
  list-style: none;
  margin: 0;
  padding: 0;
  overflow-y: auto;
  flex: 1;
}

.notif-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 16px;
  border-bottom: 1px solid var(--border);
  cursor: pointer;
  transition: background 0.1s;
}
.notif-item:last-child { border-bottom: none; }
.notif-item:hover { background: var(--bg-elevated); }
.notif-item--unread { background: var(--accent-bg); }
.notif-item--unread:hover { background: var(--accent-hover); }

.notif-body { flex: 1; min-width: 0; }
.notif-title { font-size: 13px; font-weight: 600; color: var(--text-primary); margin: 0 0 2px; }
.notif-desc { font-size: 12px; color: var(--text-muted); margin: 0 0 4px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.notif-time { font-size: 11px; color: var(--text-muted); margin: 0; }

.notif-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--accent-soft);
  flex-shrink: 0;
}

/* Transition */
.dropdown-enter-active,
.dropdown-leave-active { transition: opacity 0.15s, transform 0.15s; }
.dropdown-enter-from,
.dropdown-leave-to { opacity: 0; transform: translateY(-6px); }
</style>
