<script setup lang="ts">
const { t } = useI18n()
</script>

<template>
  <section id="inicio" class="hero">
    <div class="hero-inner">
      <div class="hero-logo">
        <img src="/logo.png" alt="DomusPay" class="hero-logo-img" />
      </div>
      <h1 class="hero-tagline">{{ t('landing.hero.tagline') }}</h1>
      <p class="hero-subtitle">{{ t('landing.hero.subtitle') }}</p>
      <div class="hero-ctas">
        <NuxtLink to="/auth/register" class="btn-primary">{{ t('landing.hero.ctaPrimary') }}</NuxtLink>
        <NuxtLink to="/auth/login" class="btn-secondary">{{ t('landing.hero.ctaSecondary') }}</NuxtLink>
      </div>
    </div>
  </section>
</template>

<style scoped>
.hero {
  min-height: 80vh;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: 60px 24px;
  background:
    radial-gradient(ellipse 70% 50% at 50% 0%, color-mix(in srgb, var(--accent) 8%, transparent) 0%, transparent 70%),
    var(--bg-base);
}

.hero-inner {
  max-width: 680px;
  width: 100%;
  animation: fadeUp 0.6s ease both;
}

.hero-logo {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  margin-bottom: 32px;
}

.hero-logo-img {
  height: 206px;
  width: 206px;
  object-fit: contain;
}

.hero-logo-text {
  font-size: 22px;
  font-weight: 800;
  color: var(--accent-soft);
  letter-spacing: -0.5px;
}

.hero-tagline {
  font-size: clamp(2rem, 5vw, 3.2rem);
  font-weight: 800;
  line-height: 1.15;
  color: var(--text-primary);
  letter-spacing: -1px;
  margin: 0 0 16px;
}

.hero-subtitle {
  font-size: clamp(1rem, 2.5vw, 1.2rem);
  color: var(--text-muted);
  line-height: 1.6;
  margin: 0 0 36px;
  max-width: 520px;
  margin-inline: auto;
  margin-bottom: 36px;
}

.hero-ctas {
  display: flex;
  gap: 14px;
  justify-content: center;
  flex-wrap: wrap;
}

.btn-primary {
  display: inline-flex;
  align-items: center;
  padding: 14px 32px;
  border-radius: 14px;
  font-weight: 700;
  font-size: 15px;
  text-decoration: none;
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff;
  transition: opacity 0.2s, transform 0.15s;
}

.btn-primary:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.btn-secondary {
  display: inline-flex;
  align-items: center;
  padding: 14px 32px;
  border-radius: 14px;
  font-weight: 700;
  font-size: 15px;
  text-decoration: none;
  border: 1.5px solid var(--border);
  color: var(--text-muted);
  background: transparent;
  transition: border-color 0.2s, color 0.2s, transform 0.15s;
}

.btn-secondary:hover {
  border-color: var(--accent-soft);
  color: var(--accent-soft);
  transform: translateY(-1px);
}

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}
</style>
