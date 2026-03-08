<!-- app/components/layout/Navbar.vue -->
<script setup lang="ts">
type TabId = 'resumen' | 'movimientos' | 'agregar' | 'presupuesto'

interface NavItem {
  id: TabId
  label: string
  emoji: string
}

const emit = defineEmits<{
  'tab-change': [tab: TabId]
}>()

defineProps<{
  tabActiva: TabId
}>()

const items: NavItem[] = [
  { id: 'resumen',      label: 'Inicio',    emoji: '🏠' },
  { id: 'movimientos',  label: 'Historial', emoji: '📋' },
  { id: 'agregar',      label: 'Agregar',   emoji: '➕' },
  { id: 'presupuesto',  label: 'Límites',   emoji: '📊' },
]

function handleTabChange(tab: TabId): void {
  emit('tab-change', tab)
}
</script>

<template>
  <!-- ── SIDEBAR (desktop ≥ 768px) ── -->
  <aside class="sidebar">
    <div class="sidebar-logo">
      <span class="logo-icon">💜</span>
      <span class="logo-text">FinanzasApp</span>
    </div>

    <nav class="sidebar-nav">
      <button
        v-for="item in items"
        :key="item.id"
        class="sidebar-item"
        :class="{ 'sidebar-item--active': tabActiva === item.id }"
        @click="handleTabChange(item.id)"
      >
        <span class="sidebar-emoji">{{ item.emoji }}</span>
        <span class="sidebar-label">{{ item.label }}</span>
      </button>
    </nav>

    <div class="sidebar-footer">
      <p class="sidebar-footer-text">Gestión familiar</p>
    </div>
  </aside>

  <!-- ── BOTTOM NAV (móvil < 768px) ── -->
  <nav class="bottomnav">
    <button
      v-for="item in items"
      :key="item.id"
      class="bottomnav-item"
      :class="{ 'bottomnav-item--active': tabActiva === item.id, 'bottomnav-item--add': item.id === 'agregar' }"
      @click="handleTabChange(item.id)"
    >
      <span v-if="item.id === 'agregar'" class="add-circle" :class="{ 'add-circle--active': tabActiva === 'agregar' }">
        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
          <line x1="12" y1="8" x2="12" y2="16" /><line x1="8" y1="12" x2="16" y2="12" />
        </svg>
      </span>
      <template v-else>
        <span class="bottomnav-emoji">{{ item.emoji }}</span>
        <span class="bottomnav-label">{{ item.label }}</span>
      </template>
    </button>
  </nav>
</template>

<style scoped>
/* ── SIDEBAR ── */
.sidebar {
  display: none;
}

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
  border: none;
  background: none;
  color: #6b6b8a;
  -webkit-tap-highlight-color: transparent;
  transition: color 0.2s;
}
.bottomnav-item--active { color: #a78bfa; }

.bottomnav-emoji { font-size: 18px; line-height: 1; }
.bottomnav-label { font-size: 10px; font-weight: 600; letter-spacing: 0.5px; text-transform: uppercase; }

.add-circle {
  width: 44px; height: 44px; border-radius: 50%;
  background: #2a2a40;
  display: flex; align-items: center; justify-content: center;
  color: #fff; transition: background 0.2s; margin-bottom: -2px;
}
.add-circle--active { background: linear-gradient(135deg, #7c3aed, #4f46e5); }

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
    border: none;
    background: transparent;
    color: #6b6b8a;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    text-align: left;
    transition: all 0.15s;
    width: 100%;
  }
  .sidebar-item:hover { background: #1e1e30; color: #c4b5fd; }
  .sidebar-item--active { background: #1e1230; color: #a78bfa; }
  .sidebar-item--active .sidebar-emoji { filter: none; }

  .sidebar-emoji { font-size: 18px; width: 24px; text-align: center; }
  .sidebar-label { font-size: 14px; }

  .sidebar-footer { padding-top: 24px; border-top: 1px solid #2a2a40; margin-top: auto; }
  .sidebar-footer-text { font-size: 11px; color: #3a3a55; text-align: center; margin: 0; }

  .bottomnav { display: none; }
}
</style>
