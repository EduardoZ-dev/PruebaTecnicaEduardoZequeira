<template>
  <div class="game-view">
    <!-- GameMenu se muestra si no hay usuario activo -->
    <GameMenu v-if="!currentUser" @start-game="handleGameStart" @load-game="handleLoadGame" />

    <!-- Formulario de apuestas -->
    <div v-else-if="showBetForm" class="bet-form">
      <h2>Realiza tu Apuesta</h2>
      <p><strong>Jugador:</strong> {{ currentUser.name }}</p>
      <p><strong>Saldo:</strong> ${{ currentUser.balance }}</p>

Vamos a mejorar este formulario de apuestas ya existente, necesito separar color de paridad 


y que se mantenga numero, por cierto solo se puede 1 de las 3.


<form @submit.prevent="submitBet">
  <div class="form-group">
    <label for="betAmount">Monto a Apostar:</label>
    <input 
      type="number" 
      id="betAmount" 
      v-model.number="betAmount" 
      :max="currentUser.balance" 
      min="1" 
      required 
    />
  </div>

  <div class="form-group">
    <label>Tipo de apuesta:</label>
    <div class="bet-type-selector">
      <label>
        <input 
          type="radio" 
          v-model="betType" 
          value="color" 
          @change="selectedOption = '', selectedNumber = null"
        />
        Color
      </label>
      <label>
        <input 
          type="radio" 
          v-model="betType" 
          value="paridad" 
          @change="selectedOption = '', selectedNumber = null"
        />
        Paridad
      </label>
      <label>
        <input 
          type="radio" 
          v-model="betType" 
          value="numero" 
          @change="selectedOption = '', selectedNumber = null"
        />
        Número
      </label>
    </div>
  </div>

  <div class="form-group" v-if="betType === 'color'">
    <label for="colorOption">Selecciona un color:</label>
    <select id="colorOption" v-model="selectedOption" required>
      <option disabled value="">Selecciona una opción</option>
      <option value="red">Rojo</option>
      <option value="black">Negro</option>
    </select>
  </div>

  <div class="form-group" v-if="betType === 'paridad'">
    <label for="paridadOption">Selecciona una paridad:</label>
    <select id="paridadOption" v-model="selectedOption" required>
      <option disabled value="">Selecciona una opción</option>
      <option value="par">Par</option>
      <option value="impar">Impar</option>
    </select>
  </div>

  <div class="form-group" v-if="betType === 'numero'">
    <label for="betNumber">Número (0-36):</label>
    <input
      type="number"
      id="betNumber"
      v-model.number="selectedOption"
      min="0"
      max="36"
      required
    />
  </div>

  <div class="form-buttons">
    <button type="submit" class="btn-submit">Apostar</button>
    <button type="button" class="btn-cancel" @click="resetGame">Salir</button>
  </div>
</form>

    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import GameMenu from './GameMenu.vue'
import { useRouter } from 'vue-router'

const selectedNumber = ref(null) 

const router = useRouter()
const gameStarted = ref(false)
const user = ref(null) //Inf Usuario activo

// Estado del jugador y vista
const currentUser = ref(null)
const showBetForm = ref(false)
const betType = ref(null)
// Datos de la apuesta
const betAmount = ref(0)

const selectedOption = ref('')

// Recibe el usuario desde GameMenu.vue
const handleGameStart = (userData) => {
  console.log("Datos recibidos:", userData);
  currentUser.value = userData;  // Actualizar usuario
  showBetForm.value = true;     // Mostrar pantalla de apuestas
};

// Procesa la apuesta
const submitBet = () => {
  if (betAmount.value > currentUser.value.balance) {
    alert("No tienes suficiente saldo para esta apuesta.")
    return
  }

  if (!selectedOption.value) {
    alert("Debes seleccionar una opción de apuesta.")
    return
  }

  console.log(`Apuesta: ${currentUser.value.name} apostó $${betAmount.value} a ${selectedOption.value}`)

  // Redireccionar a RouletteWheel con los datos de la apuesta
  router.push({
    name: 'RouletteWheel',
    query: {
      userName: currentUser.value.name,
      balance: currentUser.value.balance,
      amount: betAmount.value,
      option: selectedOption.value,  
      betType: betType.value
      
    }
  })

  // (Opcional) Si prefieres descontar el saldo en la vista de la ruleta, comenta esta línea
  currentUser.value.balance -= betAmount.value

  // Limpieza (opcional)
  betAmount.value = 0
  selectedOption.value = ''
}


// Reinicia el juego
const resetGame = () => {
  currentUser.value = null
  showBetForm.value = false
}
</script>

<style scoped lang="scss">

.bet-type-selector {
  display: flex;
  gap: 1rem;
  margin: 0.5rem 0;
}

.bet-type-selector label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  background: rgba(255, 255, 255, 0.1);
  cursor: pointer;
  transition: background 0.3s;
}

.bet-type-selector label:hover {
  background: rgba(255, 255, 255, 0.2);
}

.bet-type-selector input[type="radio"] {
  margin: 0;
}


.game-view {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  padding: 2rem;
  background: linear-gradient(135deg, #1a2a3a 0%, #0f1c26 100%);
  color: #f0f4f8;
  text-align: center;
  animation: fadeIn 0.6s ease-out;
}

.bet-form {
  width: 100%;
  max-width: 500px;
  padding: 4.5rem;
  border-radius: 20px;
  background: rgba(15, 28, 38, 0.8);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(100, 240, 255, 0.1);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  margin: 2rem auto;

  h2 {
    color: #6e8efb;
    margin-bottom: 2rem;
    font-size: 1.8rem;
  }

  form {
    display: flex;
    flex-direction: column;
    gap: 1.8rem;

    .form-group {
      text-align: left;

      label {
        display: block;
        margin-bottom: 0.8rem;
        color: #a3c4d8;
        font-weight: 500;
      }

      input,
      select {
        width: 100%;
        padding: 12px 16px;
        background: rgba(26, 42, 58, 0.7);
        border: 1px solid rgba(100, 240, 255, 0.2);
        border-radius: 8px;
        color: #f0f4f8;
        font-size: 1rem;
        transition: all 0.3s;

        &:focus {
          border-color: #4facfe;
          box-shadow: 0 0 0 3px rgba(79, 172, 254, 0.2);
        }
      }
    }

    .form-buttons {
      display: flex;
      gap: 1.5rem;
      margin-top: 1rem;

      button {
        flex: 1;
        padding: 14px;
        font-size: 1.1rem;
        font-weight: 500;
        border-radius: 8px;
        transition: all 0.3s;
        border: none;
        cursor: pointer;
      }

      .btn-submit {
        background: linear-gradient(90deg, #6e8efb 0%, #4facfe 100%);
        color: white;
        
        &:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(110, 142, 251, 0.4);
        }
      }

      .btn-cancel {
        background: rgba(220, 53, 69, 0.2);
        color: #ff6b7f;
        border: 1px solid rgba(220, 53, 69, 0.3);
        
        &:hover {
          background: rgba(220, 53, 69, 0.3);
        }
      }
    }
  }
}

@keyframes fadeIn {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}
</style>