<template>
  <div class="roulette-main">
    <!-- 🎲 Información del Jugador -->
    <div class="player-info">
      <div class="player-title">🎲 Jugador</div>
      <div v-if="userStore.name">
        <div><strong>Nombre:</strong> {{ userStore.name }}</div>
        <div><strong>Balance:</strong> ${{ userStore.balance }}</div>
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
    <!-- Botón para abrir el modal de apuesta -->
    <button 
      class="btn btn-success" 
      @click="showApuestaModal = true"
      style="margin-top: 1rem; font-size: 1.1rem; font-weight: bold;"
    >
      🎲 Realizar Apuesta
    </button>
  </div>

  <div class="resultado-y-apuesta">
    <!-- 🎯 Resultado del Giro -->
    <div class="resultado-ruleta" v-if="winningNumber !== null">
      <h3>🎯 Resultado del Giro</h3>

      <p style="font-size: 2.2em; margin-bottom: 1rem;">
        <strong>{{ winningNumber }}</strong>
        <span :style="{ color: winningColor === 'red' ? '#ff4444' : winningColor === 'black' ? '#333' : '#00ff88' }">
          ● {{ winningColor }}
        </span>
      </p>

      <p><strong>Tipo de apuesta:</strong> {{ currentBet.type }}</p>
      <p><strong>Monto apostado:</strong> ${{ currentBet.amount }}</p>
      <p><strong>Opción elegida:</strong> {{ currentBet.value }}</p>

      <p :style="{ color: isBetWinner ? '#00ff88' : '#ff4444', fontWeight: 'bold', marginTop: '1rem' }">
        {{ isBetWinner ? '✅ ¡GANASTE!' : '❌ ¡PERDISTE!' }}
      </p>

      <div class="volver-a-apostar">
        <button @click="resetGame" class="btn-primary">
          🔄 Volver a Apostar
        </button>
      </div>
    </div>

    <!-- 📋 Datos de la Apuesta -->
    <div class="apuesta-info">
      <h3 style="color: aliceblue;">📋 Datos de la Apuesta</h3>
      <p><strong>Usuario:</strong> {{ userStore.name }}</p>
      <p><strong>Saldo:</strong> ${{ userStore.balance }}</p>
      <p><strong>Saldo de la sesión:</strong> ${{ userStore.sessionBalance }}</p>
      <p><strong>Monto apostado:</strong> ${{ currentBet.amount || 0 }}</p>
      <p><strong>Opción elegida:</strong> {{ currentBet.value || '-' }}</p>

      <button class="save-button" @click="guardarSaldo" v-if="userStore.balance > 0">
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
          <option value="Color">Color</option>
          <option value="ParImpar">Par/Impar</option>
          <option value="NumeroColor">Número y Color</option>
        </select>
      </label>

      <!-- Selector dinámico según tipo -->
      <div v-if="bet.type === 'Color'">
        <label>
          <span>🎨 Color:</span>
          <select v-model="bet.selectedColor">
            <option disabled value="">Selecciona un color</option>
            <option value="negro">Negro</option>
            <option value="rojo">Rojo</option>
          </select>
        </label>
      </div>

      <div v-if="bet.type === 'Numero'">
        <label>
          <span>🔢 Número (0-36):</span>
          <input 
            type="number" 
            v-model.number="bet.selectedNumber" 
            min="0" 
            max="36" 
            placeholder="Ej: 17"
            @input="validateNumberRange"
          />
        </label>
      </div>

      <div v-if="bet.type === 'NumeroColor'">
        <label>
          <span>🎨 Color:</span>
          <select v-model="bet.selectedColor">
            <option value="rojo">Rojo</option>
            <option value="negro">Negro</option>
            <option value="verde">Verde</option>
          </select>
        </label>
        <label>
          <span>🔢 Número (0-36):</span>
          <input 
            type="number" 
            v-model.number="bet.selectedNumber" 
            min="0" 
            max="36" 
            placeholder="Ej: 17"
            @input="validateNumberRange"
          />
        </label>
        <div v-if="bet.selectedNumber !== null" class="color-preview">
          <span>Color real del número: {{ getColorForNumber(bet.selectedNumber) }}</span>
          <span v-if="bet.selectedColor" class="color-warning" :class="{ 'color-match': isColorMatch }">
            {{ isColorMatch ? '✅ Color correcto' : '❌ El color no coincide con el número' }}
          </span>
        </div>
      </div>

      <div v-if="bet.type === 'ParImpar'">
        <label>
          <span>🔢 Par/Impar y Color:</span>
          <select v-model="bet.selectedOption">
            <option disabled value="">Selecciona una opción</option>
            <option value="par-rojo">Par Rojo</option>
            <option value="par-negro">Par Negro</option>
            <option value="impar-rojo">Impar Rojo</option>
            <option value="impar-negro">Impar Negro</option>
          </select>
        </label>
      </div>

      <div class="modal-buttons">
        <button @click="confirmarNuevaApuesta" class="btn-submit">✅ Apostar</button>
        <button @click="showApuestaModal = false" class="btn-cancel">❌ Cancelar</button>
      </div>
    </div>
  </div>

</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
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
import { useUserStore } from '@/stores/user';

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

const userStore = useUserStore();

const currentBet = ref({
  userName: userStore.name,
  balance: userStore.balance,
  amount: 0,
  value: '',
  type: ''
});

const spinResult = ref(JSON.parse(route.query.spinResult || '{}'));
const prize = ref(Number(route.query.prize));
const message = ref(route.query.message);

const history = ref([]);
const user = ref(null);

const bet = ref({
  type: 'Color',
  selectedColor: '',
  selectedNumber: null,
  value: 0
});

const confirmarNuevaApuesta = () => {
  // Validación del monto
  if (!bet.value.amount || bet.value.amount <= 0 || bet.value.amount > userStore.balance) {
    alert('Por favor ingresa un monto válido');
    return;
  }

  // Validación según el tipo de apuesta
  let isValid = true;
  let errorMessage = '';

  console.log('DEBUG - Validando apuesta:', bet.value);

  switch (bet.value.type) {
    case 'Color':
      if (!bet.value.selectedColor) {
        isValid = false;
        errorMessage = 'Por favor selecciona un color';
      }
      break;
    case 'ParImpar':
      if (!bet.value.selectedOption) {
        isValid = false;
        errorMessage = 'Por favor selecciona una opción de Par/Impar y color';
      }
      break;
    case 'Numero':
      if (bet.value.selectedNumber === null || bet.value.selectedNumber < 0 || bet.value.selectedNumber > 36) {
        isValid = false;
        errorMessage = 'Por favor ingresa un número entre 0 y 36';
      }
      break;
    case 'NumeroColor':
      if (bet.value.selectedNumber === null || bet.value.selectedNumber < 0 || bet.value.selectedNumber > 36) {
        isValid = false;
        errorMessage = 'Por favor ingresa un número entre 0 y 36';
      } else if (!bet.value.selectedColor) {
        isValid = false;
        errorMessage = 'Por favor selecciona un color';
      }
      break;
  }

  if (!isValid) {
    alert(errorMessage);
    return;
  }

  // Preparar los datos de la apuesta
  currentBet.value = {
    userName: userStore.name,
    balance: userStore.balance,
    amount: bet.value.amount,
    type: bet.value.type,
    selectedNumber: bet.value.selectedNumber,
    selectedColor: bet.value.type === 'ParImpar' ? bet.value.selectedOption.split('-')[1] : bet.value.selectedColor,
    selectedParity: bet.value.type === 'ParImpar' ? bet.value.selectedOption.split('-')[0] : null
  };

  console.log('DEBUG - Apuesta confirmada:', currentBet.value);
  showApuestaModal.value = false;
};

const resetGame = () => {
  // Resetear los valores del resultado
  winningNumber.value = null;
  winningColor.value = '';
  isBetWinner.value = null;
  
  // Mostrar el modal de nueva apuesta
  showApuestaModal.value = true;
  
  // Resetear el formulario de apuesta
  bet.value = {
    type: 'Color',
    selectedColor: '',
    selectedNumber: null,
    value: 0
  };
};

const handleSpin = async () => {
  if (isSpinning.value) return;
  try {
    isSpinning.value = true;
    if (!userStore.name) throw new Error('No hay un usuario activo');
    if (!currentBet.value.amount || !currentBet.value.type) throw new Error('Datos de apuesta incompletos');
    if (!userStore.sessionId) throw new Error('No hay una sesión activa');

    // Mapear el tipo de apuesta al enum del backend
    const betTypeMap = {
      'Color': 0,
      'ParImpar': 1,
      'Numero': 2,
      'NumeroColor': 3
    };

    // Construir el objeto de apuesta
    const betRequest = {
      userName: userStore.name,
      type: betTypeMap[currentBet.value.type],
      amount: parseFloat(currentBet.value.amount),
      selectedColor: currentBet.value.selectedColor,
      selectedParity: currentBet.value.selectedParity,
      selectedNumber: currentBet.value.selectedNumber
    };

    console.log('DEBUG - Enviando apuesta:', {
      sessionId: userStore.sessionId,
      currentBalance: userStore.sessionBalance,
      betRequest: JSON.stringify(betRequest, null, 2)
    });

    const betResult = await processBet(userStore.sessionId, betRequest);
    
    console.log('DEBUG - Respuesta recibida:', JSON.stringify(betResult, null, 2));

    if (!betResult || !betResult.betResult || !betResult.betResult.spinResult) {
      throw new Error('Respuesta inválida del servidor');
    }

    const { spinResult, prize, newBalance } = betResult.betResult;
    userStore.updateSessionBalance(newBalance);

    currentBet.value.balance = newBalance;
    bet.value.balance = newBalance;

    if (!spinResult || typeof spinResult.number === 'undefined') {
      console.error('DEBUG - Resultado inválido:', spinResult);
      throw new Error('Resultado de giro inválido');
    }

    winningNumber.value = spinResult.number;
    winningColor.value = spinResult.color;
    isBetWinner.value = prize > 0;


    //userStore.updateBalance(newBalance);
    
    history.value.unshift({
      number: spinResult.number,
      amount: currentBet.value.amount,
      result: prize > 0 ? `Ganó ${prize}` : `Perdió ${currentBet.value.amount}`
    });
    
    if (history.value.length > 10) {
      history.value = history.value.slice(0, 10);
    }
    
    animateWheel(winningNumber.value);
  } catch (error) {
    console.error('Error detallado en handleSpin:', error);
    alert(error.message || 'Error al procesar la apuesta');
  } finally {
    isSpinning.value = false;
  }
};

onMounted(async () => {
  try {
    if (!route.query.sessionId) {
      router.push('/');
      return;
    }
    let sessionData;
    try {
      const response = await fetch(`https://localhost:7156/api/Session/history/${route.query.sessionId}`);
      if (!response.ok) {
        router.push('/');
        return;
      }
      sessionData = await response.json();
    } catch (error) {
      router.push('/');
      return;
    }
    userStore.setUser({
      name: sessionData.userName,
      balance: sessionData.currentBalance,
      sessionId: route.query.sessionId
    });
    bet.value = {
      userName: sessionData.userName,
      balance: sessionData.currentBalance,
      amount: 0,
      value: '',
      type: ''
    };
    currentBet.value = {
      userName: sessionData.userName,
      balance: sessionData.currentBalance,
      amount: 0,
      value: '',
      type: ''
    };
    if (sessionData.betHistory && sessionData.betHistory.length > 0) {
      history.value = sessionData.betHistory.map(bet => ({
        number: bet.spinResult.number,
        amount: bet.betAmount,
        result: bet.prize > 0 ? `Ganó ${bet.prize}` : `Perdió ${bet.betAmount}`
      }));
    }
  } catch (error) {
    router.push('/');
  }
});

const guardarSaldo = async () => {
  try {
    if (!userStore.name) {
      alert("Error: No hay usuario registrado");
      return;
    }
    if (!userStore.sessionId) {
      alert("Error: No hay una sesión activa");
      return;
    }

    const dataToSend = {
      sessionId: userStore.sessionId,
      currentBalance: userStore.sessionBalance  // 👈 este es el que faltaba
    };

    const response = await fetch('https://localhost:7156/api/Session/save', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(dataToSend)
    });

    if (!response.ok) {
      throw new Error('Error al guardar el saldo');
    }

    const result = await response.json();

    userStore.updateBalance(result.balance);
    userStore.updateSessionBalance(result.balance);

    bet.value.balance = result.balance;
    currentBet.value.balance = result.balance;

    alert(`✅ Saldo guardado correctamente: $${result.balance}`);
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
    let t = Math.min((now - start) / 5000, 1);
    t = (--t) * t * t + 1;
    const current = initial + delta * t;
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

const layer2 = ref(null);
const layer4 = ref(null);

const getColorForNumber = (number) => {
  if (number === 0) return 'verde';
  const redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];
  return redNumbers.includes(number) ? 'rojo' : 'negro';
};

const resetBetDetails = () => {
  bet.value.selectedColor = '';
  bet.value.selectedNumber = null;
};

const isValidBet = computed(() => {
  if (bet.value.amount <= 0 || bet.value.amount > userStore.balance) return false;

  switch (bet.value.type) {
    case 'Color':
      return !!bet.value.selectedColor;
    case 'Numero':
      return bet.value.selectedNumber !== null && 
             bet.value.selectedNumber >= 0 && 
             bet.value.selectedNumber <= 36;
    case 'NumeroColor':
      return bet.value.selectedNumber !== null && 
             bet.value.selectedNumber >= 0 && 
             bet.value.selectedNumber <= 36 && 
             !!bet.value.selectedColor;
    default:
      return false;
  }
});

const validateNumberRange = (event) => {
  const value = parseInt(event.target.value);
  if (value < 0) {
    bet.value.selectedNumber = 0;
  } else if (value > 36) {
    bet.value.selectedNumber = 36;
  }
};

const isColorMatch = computed(() => {
  if (bet.value.type !== 'NumeroColor' || bet.value.selectedNumber === null || !bet.value.selectedColor) {
    return true;
  }
  const actualColor = getColorForNumber(bet.value.selectedNumber);
  return actualColor === bet.value.selectedColor;
});

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

.modal-buttons {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}

.modal-buttons button {
  flex: 1;
  padding: 0.8rem;
  font-size: 1rem;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.modal-buttons .btn-submit {
  background-color: #4CAF50;
  color: white;
}

.modal-buttons .btn-cancel {
  background-color: #f44336;
  color: white;
}

.modal-buttons button:hover {
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(0,0,0,0.2);
}

.modal-buttons button:active {
  transform: translateY(0);
}

.color-preview {
  margin-top: 10px;
  padding: 5px;
  background-color: #f0f0f0;
  border-radius: 4px;
}

.color-preview span {
  display: block;
  font-size: 0.9em;
  margin: 5px 0;
}

.color-warning {
  color: #f44336;
  font-weight: bold;
}

.color-match {
  color: #4CAF50;
}
</style>
