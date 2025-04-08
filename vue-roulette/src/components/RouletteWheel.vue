<template>
  <div class="roulette-main">
    <div class="roulette-outer">
      <!-- Puntero -->
      <div class="pointer"></div>

      <!-- Capa 1: aro dorado + centro blanco grande -->
      <div class="layer-1"></div>

      <!-- Capa 2: números (gira) -->
      <div class="layer-2 wheel" ref="layer2"></div>

      <!-- Capa 3: aro dorado/amarillo + centro blanco pequeño -->
      <div class="layer-3"></div>

      <!-- Capa 4: sol pequeño + rayos (gira) -->
      <div class="layer-4 wheel" ref="layer4"></div>

      <!-- Capa 5: espacios oscuros + aro dorado central -->
      <div class="layer-5"></div>
    </div>

    <!-- Resultado -->
    <div class="result" ref="resultContainer"></div>

    <!-- Botón -->
    <button class="btn-spin" @click="handleSpin">Girar la Ruleta</button>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const layer2 = ref(null);
const layer4 = ref(null);
const resultContainer = ref(null);

const numbers = [
  0,32,15,19,4,21,2,25,17,34,6,27,
  13,36,11,30,8,23,10,5,24,16,33,1,
  20,14,31,9,22,18,29,7,28,12,35,3,26
];
const numSeg = numbers.length;
const segmentAngle = 360 / numSeg;

let accumulatedRotation = 0;

function getNum(idx) {
  const i = (idx % numSeg + numSeg) % numSeg;
  return numbers[i];
}
function getCol(idx) {
  const i = (idx % numSeg + numSeg) % numSeg;
  return i === 0 ? 'green' : i % 2 === 0 ? 'black' : 'red';
}

function addFlipper() {
  const fl = document.createElement('div');
  fl.classList.add('flipper');
  const front = document.createElement('div'), back = document.createElement('div');
  front.classList.add('front-face'); back.classList.add('back-face');
  fl.append(front, back);
  resultContainer.value.innerHTML = '';
  resultContainer.value.append(fl);
  return (num, col) => {
    fl.classList.add('flip', col);
    back.innerText = num;
  };
}

function animateWheel(speed) {
  const start = performance.now();
  const initial = accumulatedRotation;
  const delta = segmentAngle * speed;
  function frame(now) {
    const t = Math.min((now - start) / 5000, 1);
    const eased = (--t) * t * t + 1; // easeOutCubic
    const current = initial + delta * eased;
    layer2.value.style.transform = `translate(-50%,-50%) rotate(${current}deg)`;
    layer4.value.style.transform = `translate(-50%,-50%) rotate(${current}deg)`;
    if (t < 1) {
      requestAnimationFrame(frame);
    } else {
      accumulatedRotation = initial + delta;
    }
  }
  requestAnimationFrame(frame);
}

function handleSpin() {
  const speed = Math.floor(Math.random() * numSeg);
  const num = getNum(-speed), col = getCol(-speed);

  animateWheel(speed);

  setTimeout(() => {
    const write = addFlipper();
    write(num, col);
  }, 5000);
}
</script>

<style scoped>
.roulette-main {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
}
.roulette-outer {
  position: relative;
  width: 500px;
  height: 500px;
  margin: 0 auto 20px;
  border-radius: 50%;
  background: radial-gradient(circle at center, #8e5000 40%, #663500 100%);
  box-shadow: 0 0 15px rgba(0,0,0,0.5);
  overflow: hidden;
}
/* Capas estáticas */
.layer-1, .layer-3, .layer-5 {
  position: absolute;
  top:50%; left:50%;
  width:500px; height:500px;
  transform: translate(-50%,-50%);
  background-repeat: no-repeat;
  background-position: center;
  background-size: cover;
}
.layer-1 { background-image: url("/assets/roulette_1.jpg"); }
.layer-3 { background-image: url("/assets/roulette_3.png"); }
.layer-5 { background-image: url("/assets/roulette_5.png"); }

/* Capas que giran */
.wheel {
  position: absolute;
  top:50%; left:50%;
  width:500px; height:500px;
  border-radius:50%;
  background-repeat: no-repeat;
  background-position: center;
  background-size: cover;
  transform: translate(-50%,-50%) rotate(0deg);
  will-change: transform;
}
.layer-2 { background-image: url("/assets/roulette_2.png"); }
.layer-4 { background-image: url("/assets/roulette_4.png"); }

/* Puntero */
.pointer {
  position: absolute;
  top: -30px; left: 50%;
  transform: translateX(-50%);
  border-left:25px solid transparent;
  border-right:25px solid transparent;
  border-bottom:40px solid #333;
  z-index: 10;
}

/* Resultado */
.result {
  margin-top: 20px;
  min-height: 50px;
}

/* Flipper */
.flipper {
  width: 100px; height: 100px;
  perspective: 1000px;
  margin: 0 auto;
}
.flipper.flip {
  animation: flipAnimation 1s forwards;
}
.front-face, .back-face {
  position: absolute;
  width: 100%; height: 100%;
  backface-visibility: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2em;
  background: #333;
  color: #fff;
  border-radius: 10px;
}
.back-face {
  transform: rotateY(180deg);
}
@keyframes flipAnimation {
  from { transform: rotateY(0); }
  to   { transform: rotateY(180deg); }
}

/* Botón */
.btn-spin {
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
}
</style>