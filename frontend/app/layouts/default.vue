<!-- app/layouts/default.vue -->
<!-- Layout for authenticated pages with sidebar/bottom nav -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

const auth = useAuthStore()
const route = useRoute()
const { t } = useI18n()

interface NavItem {
  to: string
  labelKey: string
  emoji: string
}

const NAV_ITEMS: NavItem[] = [
  { to: '/',              labelKey: 'nav.inicio',         emoji: '🏠' },
  { to: '/transacciones', labelKey: 'nav.historial',      emoji: '📋' },
  { to: '/cuentas',       labelKey: 'nav.cuentas',        emoji: '💳' },
  { to: '/presupuestos',  labelKey: 'nav.limites',        emoji: '📊' },
  { to: '/reportes',      labelKey: 'nav.reportes',       emoji: '📈' },
  { to: '/familia',       labelKey: 'nav.familia',        emoji: '👨‍👩‍👧' },
  { to: '/configuracion', labelKey: 'nav.configuracion',  emoji: '⚙️' },
]

function isActive(path: string): boolean {
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}
</script>

<template>
  <!-- No family: show setup screen, skip nav entirely -->
  <ClientOnly>
    <UiSinFamilia v-if="!auth.hasFamiliy" />
  </ClientOnly>

  <div v-if="auth.hasFamiliy" class="layout">
    <!-- ── SIDEBAR (desktop >= 768px) ── -->
    <aside class="sidebar">
      <div class="sidebar-logo">
        <img src="/logo.png" alt="DomusPay" class="sidebar-logo-img" />
      </div>

      <nav class="sidebar-nav">
        <NuxtLink
          v-for="item in NAV_ITEMS"
          :key="item.to"
          :to="item.to"
          class="sidebar-item"
          :class="{ 'sidebar-item--active': isActive(item.to) }"
        >
          <span class="sidebar-emoji">{{ item.emoji }}</span>
          <span class="sidebar-label">{{ t(item.labelKey) }}</span>
        </NuxtLink>
      </nav>

      <div class="sidebar-footer">
        <ClientOnly>
          <p class="sidebar-user">{{ auth.userName }}</p>
        </ClientOnly>
        <UiLocaleSwitcher class="sidebar-locale" />
        <button class="sidebar-logout" @click="auth.logout()">{{ t('nav.cerrarSesion') }}</button>
      </div>
    </aside>

    <!-- ── CONTENT AREA ── -->
    <div class="content-area">
      <!-- Topbar visible in all screen sizes — contains notification bell -->
      <div class="content-topbar">
        <ClientOnly>
          <UiNotificationBell />
        </ClientOnly>
      </div>
      <slot />
    </div>

    <!-- ── BOTTOM NAV (mobile < 768px) — icons only, no labels ── -->
    <nav class="bottomnav">
      <NuxtLink
        v-for="item in NAV_ITEMS"
        :key="item.to"
        :to="item.to"
        class="bottomnav-item"
        :class="{ 'bottomnav-item--active': isActive(item.to) }"
        :title="t(item.labelKey)"
      >
        <span class="bottomnav-emoji">{{ item.emoji }}</span>
        <span class="bottomnav-dot" />
      </NuxtLink>
    </nav>
  </div>
</template>

<style scoped>
.layout {
  display: flex;
  min-height: 100dvh;
  width: 100%;
  max-width: 100vw;
  overflow-x: hidden;
}

.content-area {
  flex: 1;
  min-width: 0;
  width: 0; /* forces flex child not to overflow past parent */
  display: flex;
  flex-direction: column;
  overflow-x: hidden;
}

/* Topbar — always visible, contains the notification bell */
.content-topbar {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding: 8px 16px;
  border-bottom: 1px solid var(--border);
  min-height: 44px;
}

/* ── SIDEBAR ── */
.sidebar { display: none; }

/* ── BOTTOM NAV ── */
.bottomnav {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: var(--bg-elevated);
  border-top: 1px solid var(--border);
  display: flex;
  align-items: center;
  padding: 6px 4px env(safe-area-inset-bottom, 6px);
  z-index: 100;
}

.bottomnav-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 4px;
  padding: 10px 2px 6px;
  min-width: 0;
  cursor: pointer;
  text-decoration: none;
  color: var(--text-muted);
  -webkit-tap-highlight-color: transparent;
  transition: color 0.15s;
  border-radius: 12px;
  position: relative;
}
.bottomnav-item--active { color: var(--accent-soft); }

.bottomnav-emoji { font-size: 22px; line-height: 1; }

/* Active indicator dot */
.bottomnav-dot {
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background: transparent;
  transition: background 0.15s;
  flex-shrink: 0;
}
.bottomnav-item--active .bottomnav-dot { background: var(--accent-soft); }

/* ── DESKTOP ── */
@media (min-width: 768px) {
  .sidebar {
    display: flex;
    flex-direction: column;
    width: 220px;
    min-height: 100dvh;
    background: var(--bg-card);
    border-right: 1px solid var(--border);
    padding: 28px 16px;
    position: sticky;
    top: 0;
    flex-shrink: 0;
  }

  .sidebar-logo {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0 8px 32px;
    border-bottom: 1px solid var(--border);
    margin-bottom: 24px;
  }
  .sidebar-logo-img { height: 97px; width: auto; display: block; }

  .sidebar-nav { display: flex; flex-direction: column; gap: 4px; flex: 1; }

  .sidebar-item {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 11px 14px;
    border-radius: 12px;
    text-decoration: none;
    color: var(--text-muted);
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.15s;
    width: 100%;
  }
  .sidebar-item:hover { background: var(--accent-hover); color: var(--accent-light); }
  .sidebar-item--active { background: var(--accent-bg); color: var(--accent-soft); }

  .sidebar-emoji { font-size: 18px; width: 24px; text-align: center; }
  .sidebar-label { font-size: 14px; }

  .sidebar-footer {
    padding-top: 24px;
    border-top: 1px solid var(--border);
    margin-top: auto;
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
  }
  .sidebar-user { font-size: 13px; color: var(--accent-light); font-weight: 600; margin: 0; }
  .sidebar-locale { margin-bottom: 0; }
  .sidebar-logout {
    background: none;
    border: 1px solid var(--border);
    color: var(--text-muted);
    border-radius: 10px;
    padding: 8px 16px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
    width: 100%;
    transition: all 0.15s;
  }
  .sidebar-logout:hover { border-color: var(--danger); color: var(--danger); }

  .bottomnav { display: none; }
}
</style>
