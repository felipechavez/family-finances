<script setup lang="ts">
const { isDark, toggle } = useTheme()
const { t } = useI18n()

const NAV_LINKS = [
  { href: '#inicio',         key: 'landing.nav.inicio' },
  { href: '#funcionalidades', key: 'landing.nav.funcionalidades' },
  { href: '#como-funciona',  key: 'landing.nav.comoFunciona' },
  { href: '#empezar',        key: 'landing.nav.empezar' },
]

function scrollTo(href: string) {
  const el = document.querySelector(href)
  if (el) el.scrollIntoView({ behavior: 'smooth' })
  menuOpen.value = false
}

const menuOpen = ref(false)
</script>

<template>
  <div class="landing-layout">
    <header class="navbar">
      <div class="navbar-inner">
        <!-- Logo -->
        <a href="#inicio" class="navbar-logo" @click.prevent="scrollTo('#inicio')">
          <img src="/logo.png" alt="DomusPay" class="navbar-logo-img" />
          <span class="navbar-logo-text">DomusPay</span>
        </a>

        <!-- Links desktop -->
        <nav class="navbar-links">
          <a
            v-for="link in NAV_LINKS"
            :key="link.href"
            :href="link.href"
            class="navbar-link"
            @click.prevent="scrollTo(link.href)"
          >
            {{ t(link.key) }}
          </a>
        </nav>

        <!-- Controles derecha -->
        <div class="navbar-controls">
          <ClientOnly>
            <button class="theme-toggle" :title="isDark ? 'Modo claro' : 'Modo oscuro'" @click="toggle">
              {{ isDark ? '☀️' : '🌙' }}
            </button>
            <UiLocaleSwitcher />
          </ClientOnly>
          <NuxtLink to="/auth/login" class="btn-login">{{ t('landing.hero.ctaSecondary') }}</NuxtLink>
          <NuxtLink to="/auth/register" class="btn-register">{{ t('landing.hero.ctaPrimary') }}</NuxtLink>

          <!-- Hamburger móvil -->
          <button class="hamburger" @click="menuOpen = !menuOpen" :aria-expanded="menuOpen">
            <span /><span /><span />
          </button>
        </div>
      </div>

      <!-- Menú móvil -->
      <div v-if="menuOpen" class="mobile-menu">
        <a
          v-for="link in NAV_LINKS"
          :key="link.href"
          :href="link.href"
          class="mobile-link"
          @click.prevent="scrollTo(link.href)"
        >
          {{ t(link.key) }}
        </a>
        <div class="mobile-ctas">
          <NuxtLink to="/auth/login" class="btn-login" @click="menuOpen = false">{{ t('landing.hero.ctaSecondary') }}</NuxtLink>
          <NuxtLink to="/auth/register" class="btn-register" @click="menuOpen = false">{{ t('landing.hero.ctaPrimary') }}</NuxtLink>
        </div>
      </div>
    </header>

    <main class="landing-content">
      <slot />
    </main>
  </div>
</template>

<style scoped>
.landing-layout {
  min-height: 100dvh;
  background: var(--bg-base);
}

/* ── NAVBAR ── */
.navbar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 100;
  background: color-mix(in srgb, var(--bg-base) 85%, transparent);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-bottom: 1px solid var(--border-subtle);
}

.navbar-inner {
  max-width: 1100px;
  margin-inline: auto;
  padding: 0 24px;
  height: 64px;
  display: flex;
  align-items: center;
  gap: 32px;
}

.navbar-logo {
  display: flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
  flex-shrink: 0;
}
.navbar-logo-img { height: 36px; width: 36px; object-fit: contain; }
.navbar-logo-text { font-size: 17px; font-weight: 800; color: var(--accent-soft); letter-spacing: -0.3px; }

.navbar-links {
  display: none;
  gap: 4px;
  flex: 1;
}

.navbar-link {
  padding: 8px 14px;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 600;
  color: var(--text-muted);
  text-decoration: none;
  transition: color 0.15s, background 0.15s;
}
.navbar-link:hover { color: var(--text-primary); background: var(--accent-hover); }

.navbar-controls {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-left: auto;
}

.theme-toggle {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 34px;
  height: 34px;
  border-radius: 9px;
  border: none;
  background: none;
  font-size: 17px;
  cursor: pointer;
  transition: background 0.15s;
  -webkit-tap-highlight-color: transparent;
  flex-shrink: 0;
}
.theme-toggle:hover { background: var(--accent-hover); }

.btn-login {
  display: none;
  padding: 8px 16px;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 700;
  text-decoration: none;
  border: 1.5px solid var(--border);
  color: var(--text-muted);
  transition: border-color 0.15s, color 0.15s;
  white-space: nowrap;
}
.btn-login:hover { border-color: var(--accent-soft); color: var(--accent-soft); }

.btn-register {
  display: none;
  padding: 8px 16px;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 700;
  text-decoration: none;
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff;
  transition: opacity 0.15s;
  white-space: nowrap;
}
.btn-register:hover { opacity: 0.88; }

/* Hamburger */
.hamburger {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 5px;
  width: 34px;
  height: 34px;
  border: none;
  background: none;
  cursor: pointer;
  padding: 4px;
  border-radius: 8px;
  -webkit-tap-highlight-color: transparent;
}
.hamburger span {
  display: block;
  height: 2px;
  background: var(--text-muted);
  border-radius: 2px;
  transition: background 0.15s;
}
.hamburger:hover span { background: var(--text-primary); }

/* Mobile menu */
.mobile-menu {
  border-top: 1px solid var(--border-subtle);
  padding: 12px 24px 16px;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.mobile-link {
  padding: 12px 14px;
  border-radius: 10px;
  font-size: 15px;
  font-weight: 600;
  color: var(--text-muted);
  text-decoration: none;
  transition: color 0.15s, background 0.15s;
}
.mobile-link:hover { color: var(--text-primary); background: var(--accent-hover); }

.mobile-ctas {
  display: flex;
  gap: 8px;
  margin-top: 10px;
}
.mobile-ctas .btn-login,
.mobile-ctas .btn-register { display: flex; }

/* Content offset for fixed navbar */
.landing-content { padding-top: 64px; }

/* ── DESKTOP ── */
@media (min-width: 768px) {
  .navbar-links { display: flex; }
  .btn-login, .btn-register { display: inline-flex; }
  .hamburger { display: none; }
}
</style>
