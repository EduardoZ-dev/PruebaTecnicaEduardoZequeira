<template>
  <div class="roulette-main">
    <!-- 🎲 Información del Jugador -->
    <div class="player-info">
      <div class="player-title">🎲 Jugador</div>
      <div v-if="user">
        <div><strong>Nombre:</strong> {{ user.name }}</div>
        <div><strong>Balance:</strong> ${{ user.balance }}</div>
      </div>
      <div v-else>
        <p>Cargando usuario...</p>
      </div>

      <hr>

      <!-- 🧾 Historial de Apuestas -->
      <div class="history-title">🧾 Historial de Apuestas</div>
      <ul class="history-list">
        <li v-for="(entry, index) in history.slice(0, 10)" :key="index">
          Nº {{ entry.number }} - ${{ entry.amount }} -
          <span :style="{ color: entry.result === 'Ganó' ? 'lime' : 'tomato' }">
            {{ entry.result }}
          </span>
        </li>
      </ul>
    </div>

    <!-- 🎡 Ruleta Visual -->
    <div class="roulette-outer">
      <div class="pointer"></div>
      <div class="layer-1"></div>
      <div class="layer-2 wheel" ref="layer2"></div>
      <div class="layer-3"></div>
      <div class="layer-4 wheel" ref="layer4"></div>
      <div class="layer-5"></div>
    </div>

    <!-- 🏁 Animación de resultado (flipper) -->
    <div class="result" ref="resultContainer">
      <div v-for="(message, index) in resultMessages" :key="index" v-html="message.html"></div>
    </div>

    <!-- 🕹️ Botón para Girar -->
    <button class="spin-button" @click="handleSpin">
      <span class="spark"></span>
      <span class="spark"></span>
      <span class="spark"></span>
      <span class="spark"></span>
      <span class="text">Girar la Ruleta</span>
      <span class="roulette-icon">🎡</span>
    </button>
  </div>

  <div class="resultado-y-apuesta">
    <!-- 🎯 Resultado del Giro -->
    <div class="resultado-ruleta" v-if="winningNumber !== null">
      <h3>🎯 Resultado del Giro</h3>

      <p style="font-size: 2.2em; margin-bottom: 1rem;">
        <strong>{{ winningNumber }}</strong>
        <span :style="{ color: winningColor === 'red' ? 'red' : winningColor === 'black' ? 'black' : 'green' }">
          {{ winningColor }}
        </span>
      </p>

      <p><strong>Tipo de apuesta:</strong> {{ bet.type }}</p>
      <p><strong>Valor apostado:</strong> {{ bet.value }}</p>

      <p :style="{ color: resultadoApuesta ? '#00ff88' : '#ff4444', fontWeight: 'bold' }">
        {{ resultadoApuesta ? '✅ GANASTE' : '❌ PERDISTEEEE' }}
      </p>
    </div>

    <div class="volver-a-apostar" v-if="winningNumber !== null">
      <button @click="resetGame">🔁 Volver a Apostar</button>
    </div>

    <!-- 📋 Datos de la Apuesta -->
    <div class="apuesta-info">
      <h3>📋 Datos de la Apuesta</h3>
      <p><strong>Usuario:</strong> {{ bet.userName }}</p>
      <p><strong>Saldo:</strong> ${{ bet.balance }}</p>
      <p><strong>Monto apostado:</strong> ${{ bet.amount }}</p>
      <p><strong>Opción elegida:</strong> {{ bet.value }}</p>
    </div>
  </div>

</template>

<script setup>
import { ref, onMounted, computed } from 'vue'; // 👈 Añade computed
import { useRouter, useRoute } from 'vue-router'  // 👈 Importa ambos

const router = useRouter()
const route = useRoute()  // 👈 Necesario para acceder a route.query

const resultadoApuesta = ref(null); // Puede ser null, true o false según corresponda

//Variables reactivas
const resultContainer = ref(null);
const resultMessages = ref([]);

const winningNumber = ref(null);
const winningColor = ref('');

const currentBalance = ref(Number(route.query.balance));
//const isSpinning = ref(false);

// Variables de estado:
//const bets = ref([]);
const history = ref([]);
const userName = ref(localStorage.getItem('userName') || '');
const user = ref(null);
const balance = ref(0);




function handleSpin() {

  const speed = Math.floor(Math.random() * numSeg);
  const num = getNum(-speed);
  const col = getCol(-speed);

  animateWheel(speed);

  setTimeout(() => {
    const write = addFlipper();
    write(num, col);

    // Guardar los valores visibles para el usuario
    winningNumber.value = num;
    winningColor.value = col; // <- esto es como 'rojo', 'negro', etc.
    console.log('Resultado =>', winningNumber.value, winningColor.value); // 👈

    evaluateBets(num, col);
  }, 3000);
}

onMounted(async () => {
  // Verificar que tenemos todos los parámetros necesarios
  if (!route.query.userName || !route.query.betType) {
    router.push('/');
    return;
  }
});



const bet = computed(() => ({
  userName: route.query.userName,
  balance: Number(route.query.balance),
  amount: Number(route.query.amount),
  value: route.query.option,
  type: route.query.betType // Ahora sí se actualizará al cambiar la ruta
}));

function evaluateBets(winningNumber, winningColorResult) {

  console.log("🎯 Número que cayó:", winningNumber);
  console.log("🎨 Color real:", winningColorResult);
  console.log("🎰 Apuesta:", bet.value);

  if (!bet.value.type) {
    //addResultMessage('No se recibió tipo de apuesta', 'red');
    return;
  }

  const betType = bet.value.type;
  const betVal = bet.value.value;
  const betAmount = Number(bet.value.amount);

  const betTypeConfig = BET_CONFIG[betType];

  if (!betTypeConfig) {
    //addResultMessage(`Tipo de apuesta no válido: ${betType}`, 'red');
    return;
  }

  // Variables para determinar el resultado
  console.log("Comparando =>", betVal.toLowerCase(), "===", winningColorResult.toLowerCase());
  let isWinner = false;
  console.log("¿Ganó?", isWinner);
  let multiplier = betTypeConfig.multiplier;
  let winAmount = 0;
  let resultMessage = '';

  // Para apuesta de "color", usas la lógica definida; de lo contrario, la función check del BET_CONFIG
  if (betType === 'color') {
    isWinner = betVal.toLowerCase() === winningColorResult.toLowerCase();
  } else {
    isWinner = betTypeConfig.check(betVal, winningNumber);
  }

  // Actualiza la variable reactiva para que el template pueda mostrar el resultado
  resultadoApuesta.value = isWinner;

  winAmount = isWinner ? betAmount * multiplier : 0;
  resultMessage = isWinner
    ? betTypeConfig.winMessage(multiplier)
    : `Perdiste ❌ Apostaste a ${betVal} (${betType})`;

  //updateBalance(winAmount, betAmount);
  //addResultMessage(resultMessage, isWinner ? 'green' : 'red', betAmount, winAmount);
}

const BET_CONFIG = {
  numero: {
    multiplier: 2,
    check: (betValue, winningNumber) => parseInt(betValue) === winningNumber,
    winMessage: (multiplier) => `Ganó x${multiplier} (Número)`
  },
  color: {
    multiplier: 1,
    check: (betValue, winningNumber) => {
      const winningColor = winningNumber === 0 ? 'verde' : winningNumber % 2 === 0 ? 'black' : 'red';
      return betValue.toLowerCase() === winningColor;
    },
    winMessage: (multiplier) => `Ganó x${multiplier} (Color)`
  },
  paridad: {
    multiplier: 1,
    check: (betValue, winningNumber) => {
      if (winningNumber === 0) return false;
      const winningParity = winningNumber % 2 === 0 ? 'par' : 'impar';
      return betValue.toLowerCase() === winningParity;
    },
    winMessage: (multiplier) => `Ganó x${multiplier} (Paridad)`
  }
};

// Datos de la ruleta
const numbers = [
  0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
  13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1,
  20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26
];
const numSeg = numbers.length;
const segmentAngle = 360 / numSeg;
let accumulatedRotation = 0;

const colorMap = {
  0: 'green',
  1: 'red',   2: 'black', 3: 'red',   4: 'black', 5: 'red', 6: 'black',
  7: 'red',   8: 'black', 9: 'red',  10: 'black', 11: 'black', 12: 'red',
  13: 'black', 14: 'red', 15: 'black', 16: 'red', 17: 'black', 18: 'red',
  19: 'red', 20: 'black', 21: 'red', 22: 'black', 23: 'red', 24: 'black',
  25: 'red', 26: 'black', 27: 'red', 28: 'black', 29: 'black', 30: 'red',
  31: 'black', 32: 'red', 33: 'black', 34: 'red', 35: 'black', 36: 'red'
};

function getNum(idx) {
  const i = (idx % numSeg + numSeg) % numSeg;
  return numbers[i];
}

function getCol(idx) {
  const i = (idx % numSeg + numSeg) % numSeg;
  const num = numbers[i];
  // Añadimos una verificación y un log
  if (colorMap[num] === undefined) {
    console.error(`No se encontró color para el número ${num}`);
    return 'green'; // Valor por defecto
  }
  return colorMap[num];
}

function addFlipper() {
  const fl = document.createElement('div');
  fl.classList.add('flipper');
  const front = document.createElement('div');
  const back = document.createElement('div');
  front.classList.add('front-face');
  back.classList.add('back-face');
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
    const eased = (--t) * t * t + 1;
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


// Funciones auxiliares
/*function updateBalance(winAmount, betAmount) {
  currentBalance.value = winAmount > 0
    ? currentBalance.value + winAmount
    : currentBalance.value - betAmount;
}*/

/*function addResultMessage(message, color, amount = 0, winAmount = 0) {
  const messageHtml = winAmount > 0
    ? `<div style="color: ${color};">${message} 💰 Apuesta: $${amount} → Ganancia: $${winAmount}</div>`
    : `<div style="color: ${color};">${message}</div>`;

  resultMessages.value.push({
    html: `
      <div style="margin-top: 10px;">
        ${messageHtml}
        <div style="margin-top: 5px; font-weight: bold;">
          Nuevo balance: $${currentBalance.value}
        </div>
      </div>
    `
  });
}*/




// Obtener usuario actual
/*const fetchUsersAndSetCurrentUser = async () => {
  try {
    const res = await fetch("https://localhost:7156/api/User");
    if (!res.ok || !res.headers.get("content-type")?.includes("application/json")) {
      throw new Error("Respuesta no válida o no es JSON");
    }
    const data = await res.json();
    const allUsers = data.$values;
    user.value = allUsers.find(u => u.name === userName.value);
    if (user.value) {
      balance.value = user.value.balance;
    }
  } catch (error) {
    console.error("Error al obtener los datos del usuario:", error);
  }
};*/

/*const refreshUserData = async () => {
  await fetchUsersAndSetCurrentUser();
};*/

/*async function updateBalanceInBackend(newBalance) {
  try {
    await fetch(`https://localhost:7156/api/User/updateBalance`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        UserName: userName.value,
        NewBalance: newBalance
      })
    });
  } catch (e) {
    console.error("Error al actualizar balance:", e);
  }
}*/

// Refs de la ruleta
//const layer2 = ref(null);
//const layer4 = ref(null);

</script>


<style scoped>
.volver-a-apostar {
  text-align: center;
  margin-top: 2rem;
}

.volver-a-apostar button {
  background-color: #ffcc00;
  color: #222;
  font-weight: bold;
  padding: 0.8rem 1.5rem;
  font-size: 1.1rem;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.volver-a-apostar button:hover {
  background-color: #e6b800;
}

.resultado-y-apuesta {
  display: flex;
  justify-content: space-between;
  gap: 2rem;
  margin-top: 2rem;
  flex-wrap: wrap;
  /* Por si estás en móvil */
}

.resultado-ruleta,
.apuesta-info {
  flex: 1;
  min-width: 280px;
  padding: 1.5rem;
  border-radius: 12px;
  background-color: rgba(30, 30, 30, 0.85);
  /* fondo oscuro translúcido */
  color: #f1f1f1;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  border: 1px solid #444;
  font-family: 'Segoe UI', sans-serif;
}

.resultado-ruleta h3,
.apuesta-info h3 {
  color: #222;
  font-size: 1.3em;
  margin-bottom: 1rem;
  border-bottom: 2px solid #eee;
  padding-bottom: 0.5rem;
}

.apuesta-info p,
.resultado-ruleta p {
  font-size: 1.1em;
  margin: 0.4rem 0;
}


.roulette-main {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  position: relative;
}

/* PANEL DE INFORMACIÓN: se coloca fijo en la esquina superior izquierda de la ventana */
.player-info {
  position: fixed;
  top: 100px;
  left: -250px;
  width: 260px;
  background: rgba(0, 0, 0, 0.6);
  color: #fff;
  padding: 14px;
  border-radius: 10px;
  backdrop-filter: blur(4px);
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  font-size: 14px;
  z-index: 9999;
}

.player-title {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 6px;
}

.history-title {
  font-weight: bold;
  margin-top: 10px;
}

.history-list {
  max-height: 200px;
  overflow-y: auto;
  padding-left: 20px;
  margin-top: 5px;
}

/* RULETTE */
.roulette-outer {
  position: relative;
  width: 500px;
  height: 500px;
  margin: 80px auto 20px;
  /* Se aumenta el margen superior para dejar espacio al panel fijo */
  border-radius: 50%;
  background: radial-gradient(circle at center, #8e5000 40%, #663500 100%);
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.5);
  overflow: hidden;
}

/* Capas estáticas */
.layer-1,
.layer-3,
.layer-5 {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 500px;
  height: 500px;
  transform: translate(-50%, -50%);
  background-repeat: no-repeat;
  background-position: center;
  background-size: cover;
}

.layer-1 {
  background-image: url("/assets/roulette_1.jpg");
}

.layer-3 {
  background-image: url("/assets/roulette_3.png");
}

.layer-5 {
  background-image: url("/assets/roulette_5.png");
}

/* Capas que giran */
.wheel {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 500px;
  height: 500px;
  border-radius: 50%;
  background-repeat: no-repeat;
  background-position: center;
  background-size: cover;
  transform: translate(-50%, -50%) rotate(0deg);
  will-change: transform;
}

.layer-2 {
  background-image: url("/assets/roulette_2.png");
}

.layer-4 {
  background-image: url("/assets/roulette_4.png");
}

/* Puntero */
.pointer {
  position: absolute;
  top: -30px;
  left: 50%;
  transform: translateX(-50%);
  border-left: 25px solid transparent;
  border-right: 25px solid transparent;
  border-bottom: 40px solid #333;
  z-index: 10;
}

/* Resultado */
.result {
  margin-top: 20px;
  min-height: 50px;
}

/* Flipper */
.flipper {
  width: 100px;
  height: 100px;
  perspective: 1000px;
  margin: 0 auto;
}

.flipper.flip {
  animation: flipAnimation 1s forwards;
}

.front-face,
.back-face {
  position: absolute;
  width: 100%;
  height: 100%;
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
  from {
    transform: rotateY(0);
  }

  to {
    transform: rotateY(180deg);
  }
}

/* Botón */
.spin-button {
  --gold-light: #f1c40f;
  --gold-dark: #f39c12;
  --shadow: 0 0 15px var(--gold-light);

  position: relative;
  padding: 1.2rem 2.5rem;
  font-size: 1.4rem;
  font-weight: bold;
  color: white;
  background: linear-gradient(45deg, var(--gold-dark), var(--gold-light), var(--gold-dark));
  border: none;
  border-radius: 50px;
  cursor: pointer;
  overflow: hidden;
  transition: all 0.3s;
  box-shadow: var(--shadow), 0 4px 20px rgba(0, 0, 0, 0.3);
  z-index: 1;
}

.spin-button:hover {
  transform: translateY(-3px);
  box-shadow: 0 0 25px var(--gold-light), 0 6px 25px rgba(0, 0, 0, 0.4);
}

.spin-button:active {
  transform: translateY(1px);
}

.text {
  position: relative;
  z-index: 2;
  letter-spacing: 1px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.roulette-icon {
  position: relative;
  z-index: 2;
  margin-left: 12px;
  display: inline-block;
  transition: transform 0.5s;
}

.spin-button:hover .roulette-icon {
  transform: rotate(180deg);
}

/* Efecto de chispas */
.spark {
  position: absolute;
  width: 5px;
  height: 5px;
  background: white;
  border-radius: 50%;
  opacity: 0;
  box-shadow: 0 0 5px 1px white;
  animation: spark 1.5s infinite;
}

.spark:nth-child(1) {
  top: 20%;
  left: 15%;
  animation-delay: 0.2s;
}

.spark:nth-child(2) {
  top: 10%;
  right: 20%;
  animation-delay: 0.4s;
}

.spark:nth-child(3) {
  bottom: 15%;
  left: 25%;
  animation-delay: 0.6s;
}

.spark:nth-child(4) {
  bottom: 5%;
  right: 15%;
  animation-delay: 0.8s;
}

@keyframes spark {
  0% {
    transform: scale(0);
    opacity: 0;
  }

  50% {
    transform: scale(1.2);
    opacity: 1;
  }

  100% {
    transform: scale(0);
    opacity: 0;
  }
}

/* Efecto pulsante al girar */
.spin-button.active {
  animation: pulse 0.5s infinite alternate;
}

@keyframes pulse {
  from {
    box-shadow: 0 0 15px var(--gold-light), 0 4px 20px rgba(0, 0, 0, 0.3);
  }

  to {
    box-shadow: 0 0 30px var(--gold-light), 0 6px 30px rgba(0, 0, 0, 0.4);
  }
}

/* Formularios de apuesta (ajuste opcional) */
.betting-form {
  margin-bottom: 20px;
}

.betting-form input {
  padding: 8px;
  margin: 5px;
  border-radius: 4px;
  border: 1px solid #ccc;
}

.betting-form button {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  background: #3498db;
  color: #fff;
  cursor: pointer;
  transition: background 0.3s;
}

.betting-form button:hover {
  background: #2980b9;
}
</style>
