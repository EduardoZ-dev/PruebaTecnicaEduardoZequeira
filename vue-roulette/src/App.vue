<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script setup>
import GameView from '@/views/GameView.vue';
import Welcome  from '@/views/Welcome.vue';

</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  margin-top: 60px;
}
</style>