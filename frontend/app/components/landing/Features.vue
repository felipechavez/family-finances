<script setup lang="ts">
const { t } = useI18n()

const features = [
  { icon: '💸', key: 'gastos' },
  { icon: '🎯', key: 'presupuestos' },
  { icon: '👨‍👩‍👧', key: 'familia' },
  { icon: '📊', key: 'reportes' },
]
</script>

<template>
  <section id="funcionalidades" class="features">
    <div class="features-inner">
      <h2 class="section-title">{{ t('landing.features.title') }}</h2>
      <div class="features-grid">
        <div
          v-for="(f, i) in features"
          :key="f.key"
          class="feature-card"
          :style="{ animationDelay: `${i * 0.1}s` }"
        >
          <span class="feature-icon">{{ f.icon }}</span>
          <h3 class="feature-name">{{ t(`landing.features.${f.key}.title`) }}</h3>
          <p class="feature-desc">{{ t(`landing.features.${f.key}.desc`) }}</p>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.features {
  padding: 72px 24px;
  background: var(--bg-base);
}

.features-inner {
  max-width: 1100px;
  margin-inline: auto;
}

.section-title {
  text-align: center;
  font-size: clamp(1.4rem, 3vw, 2rem);
  font-weight: 800;
  color: var(--text-primary);
  letter-spacing: -0.5px;
  margin: 0 0 48px;
}

.features-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.feature-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 20px;
  padding: 28px 24px;
  animation: fadeUp 0.5s ease both;
}

.feature-icon {
  font-size: 40px;
  display: block;
  margin-bottom: 14px;
  line-height: 1;
}

.feature-name {
  font-size: 16px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 8px;
}

.feature-desc {
  font-size: 14px;
  color: var(--text-muted);
  line-height: 1.6;
  margin: 0;
}

@media (min-width: 1024px) {
  .features-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}
</style>
