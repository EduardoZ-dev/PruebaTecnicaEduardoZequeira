<template>
  <div class="roulette-main">
    <!-- Información del jugador y apuestas (fijo en la esquina superior izquierda) -->
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

    <!-- Ruleta -->
    <div class="roulette-outer">
      <div class="pointer"></div>
      <div class="layer-1"></div>
      <div class="layer-2 wheel" ref="layer2"></div>
      <div class="layer-3"></div>
      <div class="layer-4 wheel" ref="layer4"></div>
      <div class="layer-5"></div>
    </div>

    <!-- Resultado -->
    <div class="result" ref="resultContainer"></div>

<!-- Formulario de apuesta -->
<div class="betting-form">
  <input type="number" v-model="newBetNumber" placeholder="Número (0-36)" min="0" max="36" />

  <!-- Selector de color -->
  <div class="color-options">
    <label>
      <input type="radio" v-model="selectedColor" value="red" />
      🔴 Rojo
    </label>
    <label>
      <input type="radio" v-model="selectedColor" value="black" />
      ⚫ Negro
    </label>
    <label>
      <input type="radio" v-model="selectedColor" value="green" />
      🟢 Verde
    </label>
  </div>

  <input type="number" v-model="newBetAmount" placeholder="Monto" min="1" />
  <button @click="placeBet">Apostar</button>
</div>

    <!-- Botón para girar -->
    <button class="spin-button" @click="handleSpin">
      <span class="spark"></span>
      <span class="spark"></span>
      <span class="spark"></span>
      <span class="spark"></span>
      <span class="text">Girar la Ruleta</span>
      <span class="roulette-icon">🎡</span>
    </button>
  </div>
</template>



<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

// Variables de estado:
const bets = ref([]);           // Apuestas locales
const history = ref([]);        // Historial de apuestas
const userName = ref(localStorage.getItem('userName') || '');
const user = ref(null);         // Datos completos del usuario
const balance = ref(0);         // Saldo actual
const selectedColor = ref(""); // "", "red", "black", "green"

// Variables para el formulario de apuesta:
const newBetNumber = ref(null);
const newBetAmount = ref(null);

// Variables para la ruleta (refs para la animación y resultado):
const layer2 = ref(null);
const layer4 = ref(null);
const resultContainer = ref(null);

// Función para obtener todos los usuarios y actualizar el usuario actual:
const fetchUsersAndSetCurrentUser = async () => {
  try {
    const res = await fetch("https://localhost:7156/api/User");
    if (!res.ok || !res.headers.get("content-type")?.includes("application/json")) {
      throw new Error("Respuesta no válida o no es JSON");
    }
    const data = await res.json();
    console.log("Usuarios:", data);
    const allUsers = data.$values;
    // Usamos el nombre guardado en localStorage para filtrar
    user.value = allUsers.find(u => u.name === userName.value);
    if (user.value) {
      console.log("Usuario encontrado:", user.value);
      balance.value = user.value.balance;
    } else {
      console.warn("Usuario no encontrado.");
    }
  } catch (error) {
    console.error("Error al obtener los datos del usuario:", error);
  }
};



// Función que refresca el usuario (balance, etc.)
const refreshUserData = async () => {
  await fetchUsersAndSetCurrentUser();
};
const createRound = async () => {
  try {
    const response = await fetch("https://localhost:7156/api/Rounds/start", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        userName: userName.value
      })
    });

    if (!response.ok) {
      throw new Error("Error al crear la ronda");
    }
    const data = await response.json();
    console.log("Ronda creada con éxito:", data);
    // Si la respuesta incluye un ID de ronda u otra info útil, puedes almacenarla aquí
    // Por ejemplo: roundId.value = data.id;
  } catch (error) {
    console.error("Error al iniciar la ronda:", error);
  }
};

// --- Funciones de la ruleta ---
const numbers = [
  0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
  13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1,
  20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26
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
  const front = document.createElement('div');
  const back = document.createElement('div');
  front.classList.add('front-face');
  back.classList.add('back-face');
  fl.append(front, back);
  // Limpia el contenedor de resultado
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

function evaluateBets(winningNumber) {
  if (bets.value.length === 0) {
    resultContainer.value.innerHTML += `<div style="margin-top: 10px; color: gray;">No hiciste apuestas</div>`;
    return;
  }

  let totalWin = 0;
  let resultHTML = `<div style="margin-top: 10px;">Resultados:</div>`;
  const winningColor = getCol(numbers.indexOf(winningNumber));

  bets.value.forEach(bet => {
    let winAmount = 0;
    let result = "Perdió";

    const betNumberMatch = bet.number !== null && parseInt(bet.number) === winningNumber;
    const betColorMatch = bet.color !== null && bet.color === winningColor;
    const isEvenOddMatch = bet.color !== null &&
      ((bet.color === "even" && winningNumber !== 0 && winningNumber % 2 === 0) ||
       (bet.color === "odd" && winningNumber % 2 === 1));

    // Gana si acierta número y color específicos
    if (betNumberMatch && betColorMatch) {
      winAmount = bet.amount * 3;
      result = "Ganó x3 (Número + Color)";
    }
    // Gana si acierta solo el número
    else if (betNumberMatch) {
      winAmount = bet.amount * 2;  // Podemos ajustar a *36 si quieres estilo ruleta real
      result = "Ganó x2 (Número)";
    }
    // Gana si acierta par/impar
    else if (isEvenOddMatch) {
      winAmount = bet.amount;
      result = "Ganó x1 (Par/Impar)";
    }
    // Gana si acierta el color
    else if (betColorMatch) {
      winAmount = bet.amount * 0.5;
      result = "Ganó x0.5 (Color)";
    }

    // Actualiza el balance
    balance.value += winAmount;
    totalWin += winAmount;

    // Historial
    history.value.unshift({
      number: bet.number,
      color: bet.color,
      amount: bet.amount,
      result,
      payout: winAmount
    });

    // Mostrar resultado en HTML
    if (winAmount > 0) {
      resultHTML += `<div style="color: green;">${result} 💰 Apuesta: $${bet.amount} → Ganancia: $${winAmount}</div>`;
    } else {
      resultHTML += `<div style="color: red;">Perdiste ❌ Apostaste a ${bet.number ?? ''} ${bet.color ?? ''}</div>`;
    }
  });

  resultHTML += `<div style="margin-top: 10px; font-weight: bold;">Total ganado: $${totalWin}</div>`;
  resultContainer.value.innerHTML += resultHTML;
  bets.value = [];
}


function handleSpin() {
  const speed = Math.floor(Math.random() * numSeg);
  const num = getNum(-speed);
  const col = getCol(-speed);

  animateWheel(speed);

  setTimeout(() => {
    const write = addFlipper();
    write(num, col);
    evaluateBets(num);
    refreshUserData(); // Refresca el balance desde el backend
  }, 5000);
}

// --- Inicialización en onMounted ---
onMounted(async () => {
  if (!userName.value) {
    router.push('/');
    return;
  }
  // Primera carga de usuario y ronda:
  await refreshUserData();
  await createRound();
  // Refrescar balance automáticamente cada 10 segundos
  setInterval(refreshUserData, 1000);
});

const placeBet = async () => {
  if (!userName.value) {
    alert("No se ha identificado al usuario");
    return;
  }

  const numberSelected = newBetNumber.value !== null && newBetNumber.value !== "";
  const colorSelected = selectedColor.value !== "";

  if (!numberSelected && !colorSelected) {
    alert("Debes seleccionar un número o un color");
    return;
  }

  if (numberSelected) {
    const number = parseInt(newBetNumber.value);
    if (isNaN(number) || number < 0 || number > 36) {
      alert("Por favor ingresa un número válido entre 0 y 36");
      return;
    }
  }

  // Aseguramos que sea un número decimal válido
  const amount = parseFloat(newBetAmount.value);
  if (isNaN(amount) || amount <= 0) {
    alert("Por favor ingresa un monto válido mayor a 0");
    return;
  }

  if (amount > balance.value) {
    alert("No tienes suficiente saldo para esta apuesta");
    return;
  }

  const betData = {
    userName: userName.value,
    number: numberSelected ? parseInt(newBetNumber.value) : null,
    color: colorSelected ? selectedColor.value : null,
    amount: amount
  };

  try {
    const balanceResponse = await fetch("https://localhost:7156/api/User/balance", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        userName: userName.value,
        amount: -Math.abs(amount)
      })
    });

    if (!balanceResponse.ok) {
      throw new Error("Error al descontar saldo del usuario");
    }

    const response = await fetch("https://localhost:7156/api/Roulette/bet", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(betData)
    });

    if (!response.ok) {
      throw new Error("No se pudo registrar la apuesta en el servidor");
    }

    bets.value.push(betData);
    await refreshUserData();

    newBetNumber.value = null;
    newBetAmount.value = null;
    selectedColor.value = "";

    console.log("Apuesta registrada:", betData);
  } catch (error) {
    console.error("Error al procesar la apuesta:", error);
    alert("Error al procesar la apuesta. Intenta de nuevo.");
  }
};

</script>


<style scoped>
.roulette-main {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  position: relative;
}

/* PANEL DE INFORMACIÓN: se coloca fijo en la esquina superior izquierda de la ventana */
.player-info {
  position: fixed;
  top: 100px; /* Ajústalo hasta que quede debajo del título */
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
  margin: 80px auto 20px;  /* Se aumenta el margen superior para dejar espacio al panel fijo */
  border-radius: 50%;
  background: radial-gradient(circle at center, #8e5000 40%, #663500 100%);
  box-shadow: 0 0 15px rgba(0,0,0,0.5);
  overflow: hidden;
}

/* Capas estáticas */
.layer-1, .layer-3, .layer-5 {
  position: absolute;
  top: 50%; left: 50%;
  width: 500px; height: 500px;
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
  top: 50%; left: 50%;
  width: 500px; height: 500px;
  border-radius: 50%;
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
