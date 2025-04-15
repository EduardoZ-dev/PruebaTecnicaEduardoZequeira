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
    <button 
      class="btn btn-primary" 
      @click="handleSpin" 
      :disabled="isSpinning"
      :class="{ 'opacity-50 cursor-not-allowed': isSpinning }"
    >
      {{ isSpinning ? 'Girando...' : 'Girar' }}
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

      <p :style="{ color: isBetWinner ? '#00ff88' : '#ff4444', fontWeight: 'bold' }">
        {{ isBetWinner ? '✅ GANASTE' : '❌ PERDISTEEEE' }}
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

      <button class="save-button" @click="guardarSaldo" v-if="bet.balance > 0">
        💾 Guardar Saldo Actual
      </button>

    </div>
  </div>

  <div v-if="showApuestaModal" class="modal-overlay">
    <div class="modal-content">
      <h2>🔁 Nueva Apuesta</h2>

      <label>
        <span>💰 Monto:</span>
        <input type="number" v-model.number="bet.amount" min="1" :max="bet.balance" />
      </label>

      <label>
        <span>🎯 Tipo:</span>
        <select v-model="bet.type">
          <option value="color">Color</option>
          <option value="numero">Número</option>
          <option value="paridad">Paridad</option>
        </select>
      </label>

      <!-- Selector dinámico según tipo -->
      <div v-if="bet.type === 'color'">
        <label>
          <span>🎨 Color:</span>
          <select v-model="bet.value">
            <option disabled value="">Selecciona un color</option>
            <option value="red">Rojo</option>
            <option value="black">Negro</option>
          </select>
        </label>
      </div>

      <div v-if="bet.type === 'paridad'">
        <label>
          <span>🔢 Paridad:</span>
          <select v-model="bet.value">
            <option disabled value="">Selecciona paridad</option>
            <option value="par">Par</option>
            <option value="impar">Impar</option>
          </select>
        </label>
      </div>

      <div v-if="bet.type === 'numero'">
        <label>
          <span>🔢 Número (0-36):</span>
          <input type="number" v-model.number="bet.value" min="0" max="36" placeholder="Ej: 17" />
        </label>
      </div>

      <button @click="confirmarNuevaApuesta">✅ Apostar</button>
    </div>
  </div>

</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { 
  startSession, 
  processBet, 
  saveSession, 
  spinRoulette, 
  placeBet, 
  loadUser, 
  updateUserBalance,
  saveUserBalance
} from '@/services/api';

const router = useRouter()
const route = useRoute()

// Variables reactivas
const resultContainer = ref(null);
const resultMessages = ref([]);
const isBetWinner = ref(null);
const isSpinning = ref(false);
const showApuestaModal = ref(false);
const currentSessionId = ref(null);

const winningNumber = ref(null);
const winningColor = ref('');

const currentBalance = ref(Number(route.query.balance));

const currentUser = ref({
  name: route.query.userName,
  balance: Number(route.query.balance)
});

const currentBet = ref({
  userName: route.query.userName,
  balance: Number(route.query.balance),
  amount: Number(route.query.amount),
  value: route.query.option,
  type: route.query.betType
});

const spinResult = ref(JSON.parse(route.query.spinResult || '{}'));
const prize = ref(Number(route.query.prize));
const message = ref(route.query.message);

const history = ref([]);
const user = ref(null);

const bet = ref({
  userName: route.query.userName,
  balance: Number(route.query.balance),
  amount: Number(route.query.amount),
  value: route.query.option,
  type: route.query.betType
});

const resetGame = () => {
  winningNumber.value = null;
  winningColor.value = '';
  isBetWinner.value = null;

  bet.value.amount = 0;
  bet.value.value = '';
  bet.value.type = '';

  showApuestaModal.value = true;
};

const confirmarNuevaApuesta = () => {
  // Validaciones básicas de UI
  if (!bet.value.amount || !bet.value.type || !bet.value.value) {
    alert("Completa todos los campos.");
    return;
  }

  // Validación de monto positivo
  if (bet.value.amount <= 0) {
    alert("El monto debe ser mayor que cero.");
    return;
  }

  // Validación de saldo suficiente
  if (bet.value.amount > bet.value.balance) {
    alert("No tienes suficiente saldo para esta apuesta.");
    return;
  }

  showApuestaModal.value = false;
  handleSpin();
};

const handleSpin = async () => {
  if (isSpinning.value) return;

  try {
    isSpinning.value = true;
    
    // Validar que tenemos los datos necesarios
    if (!currentUser.value?.name) {
      throw new Error('No hay un usuario activo');
    }
    if (!currentBet.value?.amount || !currentBet.value?.type || !currentBet.value?.value) {
      throw new Error('Datos de apuesta incompletos');
    }
    if (!currentSessionId.value) {
      throw new Error('No hay una sesión activa');
    }

    console.log('Procesando apuesta con sessionId:', currentSessionId.value);
    console.log('Datos de la apuesta:', {
      userName: currentUser.value.name,
      amount: currentBet.value.amount,
      type: currentBet.value.type,
      value: currentBet.value.value
    });
    
    // Procesar la apuesta
    const betResult = await processBet(currentSessionId.value, {
      userName: currentUser.value.name,
      amount: currentBet.value.amount,
      type: currentBet.value.type,
      value: currentBet.value.value
    });
    
    console.log('Resultado de la apuesta:', betResult);
    
    // Iniciar la animación de la ruleta
    const animationDuration = 5000; // 5 segundos de animación
    animateWheel(betResult.spinResult.number);

    // Esperamos a que termine la animación antes de mostrar el resultado
    await new Promise(resolve => setTimeout(resolve, animationDuration));
    
    // Mostrar el resultado que viene del backend
    winningNumber.value = betResult.spinResult.number;
    winningColor.value = betResult.spinResult.color;
    isBetWinner.value = betResult.prize > 0;
    
    // Actualizar el saldo del usuario con el valor que viene del backend
    currentUser.value.balance = betResult.newBalance;
    currentBet.value.balance = betResult.newBalance;
    
    // Actualizar historial con los datos del backend
    history.value.unshift({
      number: betResult.spinResult.number,
      amount: currentBet.value.amount,
      result: betResult.prize > 0 ? `Ganó ${betResult.prize}` : `Perdió ${currentBet.value.amount}`
    });

    // Mantener solo los últimos 10 registros
    if (history.value.length > 10) {
      history.value = history.value.slice(0, 10);
    }
    
  } catch (error) {
    console.error('Error detallado en handleSpin:', error);
    alert(error.message || 'Error al procesar la apuesta');
  } finally {
    isSpinning.value = false;
  }
};

onMounted(async () => {
  try {
    // Verificar que tenemos los parámetros necesarios
    if (!route.query.userName) {
      throw new Error('Falta el nombre de usuario');
    }

    if (!route.query.balance) {
      throw new Error('Falta el saldo inicial');
    }

    if (!route.query.amount) {
      throw new Error('Falta el monto de la apuesta');
    }

    if (!route.query.option) {
      throw new Error('Falta la opción de apuesta');
    }

    if (!route.query.betType) {
      throw new Error('Falta el tipo de apuesta');
    }

    if (!route.query.sessionId) {
      throw new Error('Falta el ID de la sesión');
    }

    console.log('Parámetros recibidos:', route.query);

    // Inicializar el estado con los datos de la apuesta
    currentUser.value = {
      name: route.query.userName,
      balance: Number(route.query.balance)
    };

    currentBet.value = {
      userName: route.query.userName,
      balance: Number(route.query.balance),
      amount: Number(route.query.amount),
      value: route.query.option,
      type: route.query.betType
    };

    // Usar el sessionId que viene en los query parameters
    currentSessionId.value = route.query.sessionId;

    console.log('Usuario inicializado:', currentUser.value);
    console.log('Apuesta inicializada:', currentBet.value);
    console.log('Sesión:', currentSessionId.value);

    // Procesar la apuesta inicial
    await handleSpin();

  } catch (error) {
    console.error('Error al inicializar el juego:', error);
    let errorMessage = 'Error al inicializar el juego. ';
    
    if (error.message.includes('Failed to fetch')) {
      errorMessage += 'Verifica que el servidor esté corriendo.';
    } else if (error.message.includes('JSON')) {
      errorMessage += 'El servidor devolvió una respuesta inválida.';
    } else {
      errorMessage += error.message;
    }
    
    alert(errorMessage);
    router.push('/');
  }
});

// Nueva función para guardar saldo
const guardarSaldo = async () => {
  try {
    if (!currentUser.value.name) {
      alert("Error: No hay usuario registrado");
      return;
    }

    await saveUserBalance(currentUser.value.name, currentUser.value.balance);
    alert(`✅ Saldo guardado: $${currentUser.value.balance}`);
  } catch (error) {
    alert("❌ Error guardando el saldo: " + error.message);
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
  1: 'red', 2: 'black', 3: 'red', 4: 'black', 5: 'red', 6: 'black',
  7: 'red', 8: 'black', 9: 'red', 10: 'black', 11: 'black', 12: 'red',
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

function animateWheel(winningNumber) {
  const start = performance.now();
  const initial = accumulatedRotation;
  
  // Calculamos el ángulo necesario para llegar al número ganador
  const targetIndex = numbers.indexOf(winningNumber);
  const targetAngle = (targetIndex * segmentAngle) + (Math.floor(Math.random() * 2 + 5) * 360); // 5-6 vueltas completas más el ángulo del número
  
  const delta = targetAngle;
  
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
const layer2 = ref(null);
const layer4 = ref(null);

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

.modal-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-content {
  background-color: #fff;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  width: 90%;
  max-width: 400px;
}

.modal-content h2 {
  font-size: 1.5rem;
  font-weight: bold;
  text-align: center;
  margin-bottom: 1.5rem;
}

.modal-content label {
  display: block;
  margin-bottom: 1rem;
}

.modal-content label span {
  display: block;
  margin-bottom: 0.3rem;
  font-weight: 500;
  color: #333;
}

.modal-content input,
.modal-content select {
  width: 100%;
  padding: 0.6rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.2s;
}

.modal-content input:focus,
.modal-content select:focus {
  border-color: #007bff;
  outline: none;
}

.modal-content button {
  width: 100%;
  background-color: #007bff;
  color: white;
  padding: 0.8rem;
  font-size: 1rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  margin-top: 1rem;
  transition: background-color 0.2s;
}

.modal-content button:hover {
  background-color: #0056b3;
}

.save-button {
  background: #4CAF50;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  margin-top: 15px;
  transition: background 0.3s;
  font-size: 14px;
}

.save-button:hover {
  background: #45a049;
  transform: scale(1.02);
}

.save-button:active {
  transform: scale(0.98);
}
</style>
