<template>
  <div class="game-view">
    <!-- 1. GameMenu se muestra si no hay usuario activo -->
    <GameMenu v-if="!currentUser" @start-new-game="handleStartNewGame" @load-game="handleGameLoad" />
    
    <!-- 2. Formulario de Apuestas se muestra cuando hay usuario y showBetForm es true -->
    <div v-else-if="showBetForm" class="bet-form">
      <h2>Realiza tu Apuesta</h2>
      <!-- Muestra datos del usuario -->
      <div class="user-info">
        <p><strong>Jugador:</strong> {{ currentUser.name }}</p>
        <p><strong>Saldo:</strong> ${{ currentUser.balance }}</p>
      </div>

      <!-- Formulario de Apuestas -->
      <!-- Nota: Solamente se permite una opción: Color, Paridad o Número -->
      <form @submit.prevent="submitBet">
        <!-- Monto a apostar -->
        <div class="form-group">
          <label for="betAmount">Monto a Apostar:</label>
          <input type="number" id="betAmount" v-model.number="betAmount" :max="currentUser.balance" min="1" required />
        </div>

        <!-- Selector de Tipo de Apuesta -->
        <div class="form-group">
          <label>Tipo de apuesta:</label>
          <div class="bet-type-selector">
            <label>
              <input type="radio" v-model="betType" value="color"
                @change="selectedOption = '', selectedNumber = null" />
              Color
            </label>
            <label>
              <input type="radio" v-model="betType" value="paridad"
                @change="selectedOption = '', selectedNumber = null" />
              Paridad
            </label>
            <label>
              <input type="radio" v-model="betType" value="numero"
                @change="selectedOption = '', selectedNumber = null" />
              Número
            </label>
          </div>
        </div>

        <!-- Opciones para Color -->
        <div class="form-group" v-if="betType === 'color'">
          <label for="colorOption">Selecciona un color:</label>
          <select id="colorOption" v-model="selectedOption" required>
            <option disabled value="">Selecciona una opción</option>
            <option value="red">Rojo</option>
            <option value="black">Negro</option>
          </select>
        </div>

        <!-- Opciones para Paridad -->
        <div class="form-group" v-if="betType === 'paridad'">
          <label for="paridadOption">Selecciona una paridad:</label>
          <select id="paridadOption" v-model="selectedOption" required>
            <option disabled value="">Selecciona una opción</option>
            <option value="par">Par</option>
            <option value="impar">Impar</option>
          </select>
        </div>

        <!-- Opción para Número -->
        <div class="form-group" v-if="betType === 'numero'">
          <label for="betNumber">Número (0-36):</label>
          <input type="number" id="betNumber" v-model.number="selectedOption" min="0" max="36" required />
        </div>
        <!-- Botones del formulario -->
        <div class="form-buttons">
          <button type="submit" class="btn-submit">Apostar</button>
          <button type="button" class="btn-cancel" @click="resetGame">Salir</button>
        </div>
      </form>
    </div>

    <!-- Mensaje de carga o error -->
    <div v-else class="loading-message">
      <p>Cargando...</p>
    </div>
  </div>

</template>

<script setup>
import { ref, watch } from 'vue'
import GameMenu from '../components/GameMenu.vue'
import { useRouter } from 'vue-router'
import { 
  startSession, 
  placeBet, 
  loadUser, 
  updateUserBalance,
  saveUserBalance,
  spinRoulette,
  processBet
} from '@/services/api';

// Router
const router = useRouter()

// Estado del juego y del usuario
const currentUser = ref(null)       // Objeto con datos del usuario (nombre, balance, etc.)
const showBetForm = ref(false)        // Controla la visualización del formulario de apuestas
const currentSessionId = ref(null)    // ID de la sesión actual

// Variables para el formulario de apuesta
const betAmount = ref(0)              // Monto a apostar
const betType = ref(null)             // Tipo de apuesta seleccionado ('color', 'paridad' o 'numero')
const selectedOption = ref('')        // Opción seleccionada para la apuesta (ya sea color, paridad o número)

// Monitorear cambios en currentUser
watch(currentUser, (newVal) => {
  console.log("Watch - currentUser cambió a:", newVal)
})

// -----------------------------------------------------------------
// Funciones para recibir los datos del usuario desde GameMenu.vue
// -----------------------------------------------------------------

const handleStartNewGame = (userData) => {
  console.log("Nuevo juego recibido:", userData)
  
  // Validar que los datos sean correctos
  if (!userData || !userData.name || !userData.balance) {
    console.error("Datos de usuario inválidos:", userData)
    alert("Error: Datos de usuario inválidos")
    return
  }

  // Actualizar el estado
  currentUser.value = {
    name: userData.name,
    balance: Number(userData.balance)
  }
  
  showBetForm.value = true
  
  // Verificar que el estado se actualizó correctamente
  console.log("Estado actualizado:", {
    currentUser: currentUser.value,
    showBetForm: showBetForm.value
  })
}

// Recibe el usuario cuando se inicia un juego (Nuevo Juego o Cargar Juego)
const handleGameStart = (userData) => {
  console.log("Recibido en handleGameStart:", userData)
  currentUser.value = userData  // Actualizar usuario
  showBetForm.value = true        // Mostrar formulario de apuestas
  console.log("currentUser asignado:", currentUser.value)
  console.log("showBetForm:", showBetForm.value)
}

// Si deseas manejar por separado la carga de un juego ya existente, puedes definir otro método:
const handleGameLoad = (userData) => {
  console.log("Recibido en handleGameLoad:", userData)
  currentUser.value = userData
  showBetForm.value = true
  console.log("currentUser asignado:", currentUser.value)
  console.log("showBetForm:", showBetForm.value)
}

// -----------------------------------------------------------------
// Función para procesar la apuesta
// -----------------------------------------------------------------

const submitBet = async () => {
  try {
    if (!currentUser.value.name) {
      alert("Error: No hay usuario activo");
      return;
    }

    if (!betAmount.value || betAmount.value <= 0) {
      alert("Error: El monto de la apuesta debe ser mayor que cero");
      return;
    }

    if (currentUser.value.balance < betAmount.value) {
      alert("Error: Saldo insuficiente");
      return;
    }

    if (!selectedOption.value) {
      alert("Error: Debes seleccionar una opción para apostar");
      return;
    }

    // Iniciar sesión si no existe
    if (!currentSessionId.value) {
      console.log('Iniciando nueva sesión para:', currentUser.value.name);
      const session = await startSession(currentUser.value.name, currentUser.value.balance);
      currentSessionId.value = session.sessionId;
      console.log('Sesión iniciada:', session);
    }

    // Mapear el tipo de apuesta al enum del backend
    const betTypeMap = {
      'color': 0,    // Color
      'paridad': 1,  // ParImpar
      'numero': 2    // Numero
    };

    // Preparar la apuesta según el tipo seleccionado
    const bet = {
      userName: currentUser.value.name,
      betType: betTypeMap[betType.value],
      betAmount: betAmount.value,
      selectedColor: betType.value === 'color' ? selectedOption.value : null,
      selectedParity: betType.value === 'paridad' ? selectedOption.value : null,
      selectedNumber: betType.value === 'numero' ? parseInt(selectedOption.value) : null
    };

    // Validar que la apuesta tenga al menos un valor seleccionado
    if (!bet.selectedColor && !bet.selectedParity && bet.selectedNumber === null) {
      throw new Error('Debes seleccionar al menos una opción para apostar');
    }

    console.log('Enviando apuesta:', { sessionId: currentSessionId.value, bet });

    // Procesar la apuesta
    const result = await processBet(currentSessionId.value, bet);
    
    // Actualizar el saldo del usuario
    currentUser.value.balance = result.newBalance;
    
    // Redirigir a RouletteWheel con los resultados
    router.push({
      name: 'RouletteWheel',
      query: {
        userName: currentUser.value.name,
        balance: result.newBalance,
        amount: betAmount.value,
        option: selectedOption.value,
        betType: betType.value,
        spinResult: JSON.stringify(result.spinResult),
        prize: result.prize,
        message: result.message,
        sessionId: currentSessionId.value
      }
    });

  } catch (error) {
    console.error('Error detallado en submitBet:', error);
    let errorMessage = 'Error al procesar la apuesta: ';
    
    if (error.message.includes('Failed to fetch')) {
      errorMessage += 'No se pudo conectar con el servidor. Verifica que esté corriendo.';
    } else if (error.message.includes('JSON')) {
      errorMessage += 'El servidor devolvió una respuesta inválida.';
    } else if (error.message.includes('Apuesta inválida')) {
      errorMessage += 'La apuesta no es válida. Verifica los datos ingresados.';
    } else {
      errorMessage += error.message;
    }
    
    alert(errorMessage);
  }
};

// -----------------------------------------------------------------
// Función para reiniciar el juego y volver al menú inicial
// -----------------------------------------------------------------

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
  from {
    opacity: 0;
    transform: scale(0.95);
  }

  to {
    opacity: 1;
    transform: scale(1);
  }
}

.user-info {
  background: rgba(255, 255, 255, 0.1);
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}

.loading-message {
  text-align: center;
  padding: 2rem;
  color: #fff;
  font-size: 1.2rem;
}
</style>