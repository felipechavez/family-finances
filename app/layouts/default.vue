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
        <span class="logo-icon">💜</span>
        <span class="logo-text">FinanzasApp</span>
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
      <slot />
    </div>

    <!-- ── BOTTOM NAV (mobile < 768px) ── -->
    <nav class="bottomnav">
      <NuxtLink
        v-for="item in NAV_ITEMS"
        :key="item.to"
        :to="item.to"
        class="bottomnav-item"
        :class="{ 'bottomnav-item--active': isActive(item.to) }"
      >
        <span class="bottomnav-emoji">{{ item.emoji }}</span>
        <span class="bottomnav-label">{{ t(item.labelKey) }}</span>
      </NuxtLink>
      <button class="bottomnav-item bottomnav-logout" @click="auth.logout()">
        <span class="bottomnav-emoji">🚪</span>
        <span class="bottomnav-label">{{ t('nav.cerrarSesion') }}</span>
      </button>
    </nav>
  </div>
</template>

<style scoped>
.layout {
  display: flex;
  min-height: 100dvh;
}

.content-area {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
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
  padding: 6px 8px;
  z-index: 100;
}

.bottomnav-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 3px;
  padding: 8px 4px;
  cursor: pointer;
  text-decoration: none;
  color: var(--text-muted);
  -webkit-tap-highlight-color: transparent;
  transition: color 0.2s;
}
.bottomnav-item--active { color: var(--accent-soft); }

.bottomnav-emoji { font-size: 18px; line-height: 1; }
.bottomnav-label { font-size: 10px; font-weight: 600; letter-spacing: 0.5px; text-transform: uppercase; }
.bottomnav-logout {
  background: none;
  border: none;
  cursor: pointer;
  -webkit-tap-highlight-color: transparent;
}
.bottomnav-logout:active { color: var(--danger); }

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
    gap: 10px;
    padding: 0 8px 32px;
    border-bottom: 1px solid var(--border);
    margin-bottom: 24px;
  }
  .logo-icon { font-size: 24px; }
  .logo-text { font-size: 16px; font-weight: 700; color: var(--text-primary); letter-spacing: -0.3px; }

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
