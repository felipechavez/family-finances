<script setup lang="ts">
</script>

<template>
  <LandingHero />
  <LandingFeatures />
  <LandingHowItWorks />
  <LandingCtaBanner />
  <footer class="landing-footer">
    <p>© {{ new Date().getFullYear() }} DomusPay Familiar</p>
  </footer>
</template>

<style scoped>
.landing-footer {
  padding: 24px;
  text-align: center;
  font-size: 13px;
  color: var(--text-muted);
  border-top: 1px solid var(--border-subtle);
}
</style>
