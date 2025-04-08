<template>
    <div>
      <img
        :src="imageSrc"
        @load="onLoad"
        alt="Ruleta"
        style="display: none;"
      />
      <div v-if="loaded">
        <p>Imagen cargada:</p>
        <p>Ancho: {{ width }}px</p>
        <p>Alto: {{ height }}px</p>
        <p>Outer Radius: {{ outerRadius }}px</p>
        <p>Inner Radius: {{ innerRadius }}px></p>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  
  const imageSrc = '/assets/roulette_2.png'; // ruta en public/assets
  const width = ref(0);
  const height = ref(0);
  const outerRadius = ref(0);
  const innerRadius = ref(0);
  const loaded = ref(false);
  
  function onLoad(event) {
    width.value = event.target.naturalWidth;
    height.value = event.target.naturalHeight;
    outerRadius.value = width.value / 2;
    innerRadius.value = outerRadius.value - 40; // ajusta 40 al grosor de banda
    loaded.value = true;
  }
  </script>