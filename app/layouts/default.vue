<!-- app/layouts/default.vue -->
<!-- Layout for authenticated pages with sidebar/bottom nav -->
<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

const auth = useAuthStore()
const route = useRoute()

interface NavItem {
  to: string
  label: string
  emoji: string
}

const items: NavItem[] = [
  { to: '/',             label: 'Inicio',     emoji: '🏠' },
  { to: '/transacciones', label: 'Historial', emoji: '📋' },
  { to: '/cuentas',      label: 'Cuentas',    emoji: '💳' },
  { to: '/presupuestos', label: 'Límites',    emoji: '📊' },
  { to: '/reportes',     label: 'Reportes',   emoji: '📈' },
]

function isActive(path: string): boolean {
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}
</script>

<template>
  <div class="layout">
    <!-- ── SIDEBAR (desktop >= 768px) ── -->
    <aside class="sidebar">
      <div class="sidebar-logo">
        <span class="logo-icon">💜</span>
        <span class="logo-text">FinanzasApp</span>
      </div>

      <nav class="sidebar-nav">
        <NuxtLink
          v-for="item in items"
          :key="item.to"
          :to="item.to"
          class="sidebar-item"
          :class="{ 'sidebar-item--active': isActive(item.to) }"
        >
          <span class="sidebar-emoji">{{ item.emoji }}</span>
          <span class="sidebar-label">{{ item.label }}</span>
        </NuxtLink>
      </nav>

      <div class="sidebar-footer">
        <p class="sidebar-user">{{ auth.userName }}</p>
        <button class="sidebar-logout" @click="auth.logout()">Cerrar sesión</button>
      </div>
    </aside>

    <!-- ── CONTENT AREA ── -->
    <div class="content-area">
      <slot />
    </div>

    <!-- ── BOTTOM NAV (mobile < 768px) ── -->
    <nav class="bottomnav">
      <NuxtLink
        v-for="item in items"
        :key="item.to"
        :to="item.to"
        class="bottomnav-item"
        :class="{ 'bottomnav-item--active': isActive(item.to) }"
      >
        <span class="bottomnav-emoji">{{ item.emoji }}</span>
        <span class="bottomnav-label">{{ item.label }}</span>
      </NuxtLink>
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
  background: #18182a;
  border-top: 1px solid #2a2a40;
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
  color: #6b6b8a;
  -webkit-tap-highlight-color: transparent;
  transition: color 0.2s;
}
.bottomnav-item--active { color: #a78bfa; }

.bottomnav-emoji { font-size: 18px; line-height: 1; }
.bottomnav-label { font-size: 10px; font-weight: 600; letter-spacing: 0.5px; text-transform: uppercase; }

/* ── DESKTOP ── */
@media (min-width: 768px) {
  .sidebar {
    display: flex;
    flex-direction: column;
    width: 220px;
    min-height: 100dvh;
    background: #13131f;
    border-right: 1px solid #2a2a40;
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
    border-bottom: 1px solid #2a2a40;
    margin-bottom: 24px;
  }
  .logo-icon { font-size: 24px; }
  .logo-text { font-size: 16px; font-weight: 700; color: #f0eeff; letter-spacing: -0.3px; }

  .sidebar-nav { display: flex; flex-direction: column; gap: 4px; flex: 1; }

  .sidebar-item {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 11px 14px;
    border-radius: 12px;
    text-decoration: none;
    color: #6b6b8a;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.15s;
    width: 100%;
  }
  .sidebar-item:hover { background: #1e1e30; color: #c4b5fd; }
  .sidebar-item--active { background: #1e1230; color: #a78bfa; }

  .sidebar-emoji { font-size: 18px; width: 24px; text-align: center; }
  .sidebar-label { font-size: 14px; }

  .sidebar-footer {
    padding-top: 24px;
    border-top: 1px solid #2a2a40;
    margin-top: auto;
    text-align: center;
  }
  .sidebar-user { font-size: 13px; color: #c4b5fd; font-weight: 600; margin: 0 0 8px; }
  .sidebar-logout {
    background: none;
    border: 1px solid #2a2a40;
    color: #6b6b8a;
    border-radius: 10px;
    padding: 8px 16px;
    font-size: 12px;
    font-weight: 600;
    cursor: pointer;
    width: 100%;
    transition: all 0.15s;
  }
  .sidebar-logout:hover { border-color: #f87171; color: #f87171; }

  .bottomnav { display: none; }
}
</style>
