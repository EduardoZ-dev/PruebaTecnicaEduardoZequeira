<template>
  <div class="game-view">
    <h2>Juego de la Ruleta</h2>
    <RouletteWheel />
  </div>
</template>

<script setup>
import RouletteWheel from '@/components/RouletteWheel.vue';
</script>

<style scoped>
.game-view {
  text-align: center;
  padding: 20px;
}
</style>