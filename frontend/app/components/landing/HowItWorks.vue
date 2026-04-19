<script setup lang="ts">
const { t } = useI18n()

const steps = ['crear', 'invitar', 'controlar']
</script>

<template>
  <section id="como-funciona" class="how">
    <div class="how-inner">
      <h2 class="section-title">{{ t('landing.howItWorks.title') }}</h2>
      <div class="steps">
        <div
          v-for="(step, i) in steps"
          :key="step"
          class="step"
          :style="{ animationDelay: `${i * 0.15}s` }"
        >
          <div class="step-number">{{ i + 1 }}</div>
          <div class="step-connector" v-if="i < steps.length - 1" />
          <h3 class="step-title">{{ t(`landing.howItWorks.${step}.title`) }}</h3>
          <p class="step-desc">{{ t(`landing.howItWorks.${step}.desc`) }}</p>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.how {
  padding: 72px 24px;
  background: var(--bg-card);
  border-top: 1px solid var(--border-subtle);
  border-bottom: 1px solid var(--border-subtle);
}

.how-inner {
  max-width: 860px;
  margin-inline: auto;
}

.section-title {
  text-align: center;
  font-size: clamp(1.4rem, 3vw, 2rem);
  font-weight: 800;
  color: var(--text-primary);
  letter-spacing: -0.5px;
  margin: 0 0 56px;
}

.steps {
  display: flex;
  flex-direction: column;
  gap: 32px;
  position: relative;
}

.step {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  animation: fadeUp 0.5s ease both;
}

.step-number {
  width: 52px;
  height: 52px;
  border-radius: 50%;
  background: linear-gradient(135deg, #7c3aed, #4f46e5);
  color: #fff;
  font-size: 20px;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 16px;
  flex-shrink: 0;
  position: relative;
  z-index: 1;
}

.step-title {
  font-size: 17px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 8px;
}

.step-desc {
  font-size: 14px;
  color: var(--text-muted);
  line-height: 1.6;
  margin: 0;
  max-width: 240px;
}

@media (min-width: 640px) {
  .steps {
    flex-direction: row;
    align-items: flex-start;
    gap: 0;
  }

  .step {
    flex: 1;
    padding: 0 16px;
    position: relative;
  }

  .step + .step::before {
    content: '';
    position: absolute;
    left: calc(-50% + 26px);
    top: 26px;
    width: calc(100% - 52px);
    border-top: 2px dashed var(--border-accent);
    z-index: 0;
  }
}

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}
</style>
