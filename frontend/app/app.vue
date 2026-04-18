<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<style>
*, *::before, *::after { box-sizing: border-box; }

body {
  margin: 0;
  background: var(--bg-base);
  font-family: 'DM Sans', sans-serif;
  color: var(--text-primary);
  -webkit-font-smoothing: antialiased;
  transition: background-color 0.2s, color 0.2s;
}
input, select, button { font-family: inherit; }
input[type="number"]::-webkit-inner-spin-button { -webkit-appearance: none; }
</style>
