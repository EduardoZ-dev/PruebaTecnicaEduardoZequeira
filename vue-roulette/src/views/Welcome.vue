<template>
  <div class="login-container">
    <!-- Fondo temático -->
    <div class="casino-background">
      <div class="roulette-chip"></div>
      <div class="poker-chip"></div>
    </div>

    <!-- Formulario -->
    <div class="login-card">
      <h1>🎰 Bienvenido al Casino</h1>
      <form @submit.prevent="startGame">
        <!-- Input Usuario -->
        <div class="input-group">
          <label for="userName">Usuario</label>
          <input 
            id="userName" 
            v-model="userName" 
            type="text" 
            placeholder="Ej: JamesBond" 
            required
          >
          <span class="icon">👤</span>
        </div>

        <!-- Slider para el monto (estilo apuesta) -->
        <div class="input-group">
          <label for="amount">Monto inicial: <strong>{{ amount }} $</strong></label>
          <input
            id="amount"
            v-model.number="amount"
            type="range"
            min="100"
            max="5000"
            step="100"
            class="bet-slider"
          >
          <div class="bet-markers">
            <span>100</span>
            <span>2500</span>
            <span>5000</span>
          </div>
        </div>

        <!-- Botón con efecto casino -->
        <button type="submit" class="bet-button">
          <span class="glow">¡Jugar!</span>
          <div class="chip-animation">🪙</div>
        </button>
      </form>

      <!-- Mensaje de error -->
      <p v-if="error" class="error-message">{{ error }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const userName = ref('');
const amount = ref(1000); // Valor inicial del slider
const error = ref('');
const success = ref('');
const router = useRouter();

async function startGame() {
  error.value = '';
  success.value = '';

  if (!userName.value.trim()) {
    error.value = 'Por favor, ingresa tu nombre antes de continuar.';
    return;
  }

  try {
    await axios.post('https://localhost:7156/api/User/balance', {
      userName: userName.value,
      amount: amount.value
    });

    // Guardar el nombre del usuario para que la ruleta lo sepa
    localStorage.setItem('userName', userName.value);

    success.value = '¡Usuario creado exitosamente! Redirigiendo...';

    setTimeout(() => {
      router.push('/roulette');
    }, 1500);

  } catch (err) {
    error.value = err.response?.data?.message || 'Error al conectar con el servidor';
  }
}
</script>

<style scoped>
/* Estilo base */
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #1a2a3a, #0f1923);
  position: relative;
  overflow: hidden;
}

/* Fondo con elementos de casino */
.casino-background {
  position: absolute;
  width: 100%;
  height: 100%;
}

.roulette-chip {
  position: absolute;
  top: 20%;
  left: 10%;
  width: 60px;
  height: 60px;
  background: radial-gradient(circle, #e74c3c, #c0392b);
  border-radius: 50%;
  opacity: 0.3;
  animation: float 6s ease-in-out infinite;
}

.poker-chip {
  position: absolute;
  bottom: 15%;
  right: 10%;
  width: 80px;
  height: 80px;
  background: radial-gradient(circle, #3498db, #2980b9);
  border-radius: 50%;
  opacity: 0.3;
  animation: float 8s ease-in-out infinite 2s;
}

@keyframes float {
  0%, 100% { transform: translateY(0) rotate(0deg); }
  50% { transform: translateY(-20px) rotate(10deg); }
}

/* Tarjeta del formulario */
.login-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  padding: 2.5rem;
  border-radius: 15px;
  width: 100%;
  max-width: 450px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
  z-index: 1;
}

h1 {
  color: #f1c40f;
  text-align: center;
  margin-bottom: 2rem;
  font-size: 1.8rem;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

/* Inputs */
.input-group {
  margin-bottom: 1.5rem;
  position: relative;
}

label {
  display: block;
  color: #ecf0f1;
  margin-bottom: 0.5rem;
  font-weight: 300;
}

input[type="text"],
input[type="range"] {
  width: 100%;
  padding: 0.8rem 1rem;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  color: white;
  font-size: 1rem;
}

input[type="text"] {
  padding-left: 2.5rem;
}

.icon {
  position: absolute;
  left: 12px;
  top: 38px;
  font-size: 1.2rem;
}

/* Slider de apuesta */
.bet-slider {
  -webkit-appearance: none;
  height: 8px;
  background: linear-gradient(90deg, #e74c3c, #f1c40f);
  border-radius: 4px;
  margin-top: 0.5rem;
}

.bet-slider::-webkit-slider-thumb {
  -webkit-appearance: none;
  width: 20px;
  height: 20px;
  background: white;
  border-radius: 50%;
  cursor: pointer;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.bet-markers {
  display: flex;
  justify-content: space-between;
  color: #bdc3c7;
  font-size: 0.8rem;
  margin-top: 0.3rem;
}

/* Botón de apuesta */
.bet-button {
  width: 100%;
  padding: 1rem;
  background: linear-gradient(45deg, #e74c3c, #f1c40f);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: transform 0.3s;
  margin-top: 1rem;
}

.bet-button:hover {
  transform: translateY(-3px);
}

.glow {
  position: relative;
  z-index: 1;
}

.chip-animation {
  position: absolute;
  right: 20px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 1.5rem;
  animation: chip-spin 2s linear infinite;
}

@keyframes chip-spin {
  0% { transform: translateY(-50%) rotate(0deg); }
  100% { transform: translateY(-50%) rotate(360deg); }
}

/* Mensaje de error */
.error-message {
  color: #e74c3c;
  text-align: center;
  margin-top: 1.5rem;
  font-size: 0.9rem;
}
</style>