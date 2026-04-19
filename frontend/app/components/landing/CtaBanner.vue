<script setup lang="ts">
const { t } = useI18n()
</script>

<template>
  <section id="empezar" class="cta-banner">
    <div class="cta-inner">
      <h2 class="cta-title">{{ t('landing.cta.title') }}</h2>
      <p class="cta-sub">{{ t('landing.cta.subtitle') }}</p>
      <NuxtLink to="/auth/register" class="btn-primary">{{ t('landing.cta.button') }}</NuxtLink>
    </div>
  </section>
</template>

<style scoped>
.cta-banner {
  padding: 80px 24px;
  background:
    radial-gradient(ellipse 80% 60% at 50% 100%, color-mix(in srgb, var(--accent) 12%, transparent) 0%, transparent 70%),
    var(--bg-base);
  text-align: center;
}

.cta-inner {
  max-width: 560px;
  margin-inline: auto;
  animation: fadeUp 0.5s ease both;
}

.cta-title {
  font-size: clamp(1.6rem, 4vw, 2.4rem);
  font-weight: 800;
  color: var(--text-primary);
  letter-spacing: -0.5px;
  margin: 0 0 14px;
}

.cta-sub {
  font-size: 16px;
  color: var(--text-muted);
  line-height: 1.6;
  margin: 0 0 32px;
}

.btn-primary {
  display: inline-flex;
  align-items: center;
  padding: 16px 40px;
  border-radius: 14px;
  font-weight: 700;
  font-size: 16px;
  text-decoration: none;
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff;
  transition: opacity 0.2s, transform 0.15s;
}

.btn-primary:hover {
  opacity: 0.9;
  transform: translateY(-2px);
}

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}
</style>
