<template>
    <div class="roulette-svg-container">
      <svg
        :width="size"
        :height="size"
        viewBox="0 0 500 500"
        @click="spin"
      >
        <!-- Definición de la ruta circular interna para la bola -->
        <defs>
          <circle
            id="circlePath"
            :cx="center"
            :cy="center"
            :r="innerRadius"
            fill="none"
          />
        </defs>
  
        <!-- Grupo que contiene los segmentos de la ruleta -->
        <g
          ref="wheelGroup"
          :style="wheelStyle"
          :transform="`translate(${center},${center})`"
        >
          <template v-for="(seg, i) in segments" :key="i">
            <!-- Segmento coloreado -->
            <path
              :d="describeSegment(i)"
              :fill="seg.color"
              stroke="#333"
              stroke-width="1"
            />
            <!-- Número -->
            <text
              :x="textPosition(i).x"
              :y="textPosition(i).y"
              fill="#fff"
              font-size="16"
              text-anchor="middle"
              dominant-baseline="middle"
            >
              {{ seg.number }}
            </text>
          </template>
        </g>
  
        <!-- Bola animada a lo largo del círculo interno -->
        <circle r="8" fill="#fff" stroke="#000" stroke-width="1">
          <animateMotion
            ref="ballAnim"
            :dur="duration + 'ms'"
            fill="freeze"
            :path="motionPath"
            begin="indefinite"
          />
        </circle>
  
        <!-- Puntero -->
        <polygon
          :points="`${center-10},${center-outerRadius-10} ${center+10},${center-outerRadius-10} ${center},${center-outerRadius+20}`"
          fill="#333"
        />
      </svg>
      <div class="result">{{ resultText }}</div>
      <button class="btn-spin" @click="spin">Girar la Ruleta</button>
    </div>
  </template>
  
  <script setup>
  //import { ref } from 'vue';
  import { ref, computed } from 'vue';
  
  const size = 500;
  const center = size / 2;
  const outerRadius = 240;
  const innerRadius = 200;
  const duration = 5000; // ms
  
  const segments = [
    { number: '0', color: 'green' },
    { number: '32', color: 'red' },
    { number: '15', color: 'black' },
    { number: '19', color: 'red' },
    { number: '4', color: 'black' },
    { number: '21', color: 'red' },
    { number: '2', color: 'black' },
    { number: '25', color: 'red' },
    { number: '17', color: 'black' },
    { number: '34', color: 'red' },
    { number: '6', color: 'black' },
    { number: '27', color: 'red' },
    { number: '13', color: 'black' },
    { number: '36', color: 'red' },
    { number: '11', color: 'black' },
    { number: '30', color: 'red' },
    { number: '8', color: 'black' },
    { number: '23', color: 'red' },
    { number: '10', color: 'black' },
    { number: '5', color: 'red' },
    { number: '24', color: 'black' },
    { number: '16', color: 'red' },
    { number: '33', color: 'black' },
    { number: '1', color: 'red' },
    { number: '20', color: 'black' },
    { number: '14', color: 'red' },
    { number: '31', color: 'black' },
    { number: '9', color: 'red' },
    { number: '22', color: 'black' },
    { number: '18', color: 'red' },
    { number: '29', color: 'black' },
    { number: '7', color: 'red' },
    { number: '28', color: 'black' },
    { number: '12', color: 'red' },
    { number: '35', color: 'black' },
    { number: '3', color: 'red' },
    { number: '26', color: 'black' }
  ];
  const numSeg = segments.length;
  const segmentAngle = 360 / numSeg;
  
  const wheelGroup = ref(null);
  const ballAnim = ref(null);
  const resultText = ref('');
  
  // Calcula el path de cada segmento
  function describeSegment(i) {
    const start = (i * segmentAngle) * (Math.PI/180);
    const end = ((i+1) * segmentAngle) * (Math.PI/180);
    const x1 = Math.cos(start) * outerRadius;
    const y1 = Math.sin(start) * outerRadius;
    const x2 = Math.cos(end) * outerRadius;
    const y2 = Math.sin(end) * outerRadius;
    return `M0,0 L${x1},${y1} A${outerRadius},${outerRadius} 0 0,1 ${x2},${y2} Z`;
  }
  
  // Posición de los números
  function textPosition(i) {
    const theta = ((i + 0.5) * segmentAngle) * (Math.PI/180);
    const r = (outerRadius + innerRadius) / 2;
    return {
      x: center + Math.cos(theta) * r,
      y: center + Math.sin(theta) * r
    };
  }
  
  // Path para animateMotion (círculo interno)
  const motionPath = computed(() => {
    // SVG path para círculo: M cx+R,cy A R,R 0 1,1 cx-R,cy A R,R 0 1,1 cx+R,cy
    return `
      M ${center+innerRadius},${center}
      A ${innerRadius},${innerRadius} 0 1,1 ${center-innerRadius},${center}
      A ${innerRadius},${innerRadius} 0 1,1 ${center+innerRadius},${center}
    `;
  });
  
  function spin() {
    // Rotación de la rueda
    const speed = Math.floor(Math.random() * numSeg);
    const delta = speed * segmentAngle;
    wheelGroup.value.setAttribute(
      'transform',
      `translate(${center},${center}) rotate(${-delta})`
    );
    wheelGroup.value.style.transition = `transform ${duration}ms cubic-bezier(0.165,0.84,0.44,1.005)`;
  
    // Animación de la bola
    ballAnim.value.beginElement();
  
    // Resultado
    const winner = segments[(speed) % numSeg];
    setTimeout(() => {
      resultText.value = `Número: ${winner.number} (${winner.color})`;
    }, duration);
  }
  </script>
  
  <style scoped>
  .roulette-main {
    text-align: center;
    margin: 0 auto;
    max-width: 600px;
  }
  
  .roulette-container {
    position: relative;
    width: 500px;
    height: 500px;
    margin: 0 auto 20px;
  }
  
  .roulette {
    /* El <g> usa transform en JS */
  }
  
  .pointer {
    position: absolute;
    top: calc(50% - 250px - 10px);
    left: 50%;
    transform: translateX(-50%);
    width: 0;
    height: 0;
    border-left: 15px solid transparent;
    border-right: 15px solid transparent;
    border-bottom: 30px solid #333;
    z-index: 10;
  }
  
  .ball-container {
    position: absolute;
    width: 16px;
    height: 16px;
  }
  
  .ball {
    width: 100%;
    height: 100%;
    border-radius: 50%;
    background: #fff radial-gradient(circle at 4px 4px,#fff,#444);
    box-shadow: 1px 1px 4px #000;
  }
  
  /* Animación de la bola */
  .spinningBall circle {
    /* animateMotion en SVG se encarga */
  }
  
  .result {
    font-size: 18px;
    margin-bottom: 10px;
  }
  
  .btn-spin {
    padding: 10px 20px;
    font-size: 16px;
    cursor: pointer;
  }
  </style>