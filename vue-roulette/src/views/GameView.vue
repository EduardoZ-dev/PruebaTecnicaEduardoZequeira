<template>
  <div class="game-view">
    <!-- 1. GameMenu se muestra si no hay usuario activo -->
    <GameMenu v-if="!currentUser || !showBetForm" @start-new-game="handleStartNewGame" @load-game="handleGameLoad" />
    
    <!-- 2. Formulario de Apuestas se muestra cuando hay usuario y showBetForm es true -->
    <div v-else-if="showBetForm" class="bet-form">
      <div class="user-info">
        <h3>Información del Jugador</h3>
        <p><strong>Nombre:</strong> {{ currentUser?.name || 'No disponible' }}</p>
        <p><strong>Saldo:</strong> ${{ currentUser?.balance?.toFixed(2) || '0.00' }}</p>
        <p><strong>ID Sesión:</strong> {{ currentSessionId || 'No disponible' }}</p>
      </div>

      <div v-if="currentUser && currentUser.balance > 0" class="betting-options">
        <h3>Opciones de Apuesta</h3>
        <form @submit.prevent="submitBet">
          <div class="form-group">
            <label for="betAmount">Cantidad a Apostar:</label>
            <input
              type="number"
              id="betAmount"
              v-model="amount"
              required
              min="0.01"
              :max="currentUser.balance"
              step="0.01"
              class="form-control"
            />
          </div>

          <div class="form-group">
            <label>Tipo de Apuesta:</label>
            <div class="bet-type-options">
              <div class="form-check">
                <input
                  type="radio"
                  id="colorBet"
                  v-model="betType"
                  value="color"
                  class="form-check-input"
                />
                <label for="colorBet" class="form-check-label">Color</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="evenOddBet"
                  v-model="betType"
                  value="evenOdd"
                  class="form-check-input"
                />
                <label for="evenOddBet" class="form-check-label">Par/Impar</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="numberBet"
                  v-model="betType"
                  value="number"
                  class="form-check-input"
                />
                <label for="numberBet" class="form-check-label">Número</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="numberColorBet"
                  v-model="betType"
                  value="numberColor"
                  class="form-check-input"
                />
                <label for="numberColorBet" class="form-check-label">Número y Color</label>
              </div>
            </div>
          </div>

          <div v-if="betType === 'color'" class="form-group">
            <label>Seleccione Color:</label>
            <div class="color-options">
              <div class="form-check">
                <input
                  type="radio"
                  id="redColor"
                  v-model="selectedOption"
                  value="rojo"
                  class="form-check-input"
                />
                <label for="redColor" class="form-check-label">Rojo</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="blackColor"
                  v-model="selectedOption"
                  value="negro"
                  class="form-check-input"
                />
                <label for="blackColor" class="form-check-label">Negro</label>
              </div>
            </div>
          </div>

          <div v-if="betType === 'evenOdd'" class="form-group">
            <label>Seleccione Par/Impar y Color:</label>
            <div class="even-odd-options">
              <div class="form-check">
                <input
                  type="radio"
                  id="parRojo"
                  v-model="selectedOption"
                  value="par-rojo"
                  class="form-check-input"
                />
                <label for="parRojo" class="form-check-label">Par Rojo</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="parNegro"
                  v-model="selectedOption"
                  value="par-negro"
                  class="form-check-input"
                />
                <label for="parNegro" class="form-check-label">Par Negro</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="imparRojo"
                  v-model="selectedOption"
                  value="impar-rojo"
                  class="form-check-input"
                />
                <label for="imparRojo" class="form-check-label">Impar Rojo</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="imparNegro"
                  v-model="selectedOption"
                  value="impar-negro"
                  class="form-check-input"
                />
                <label for="imparNegro" class="form-check-label">Impar Negro</label>
              </div>
            </div>
          </div>

          <div v-if="betType === 'number'" class="form-group">
            <label for="numberSelect">Seleccione Número (0-36):</label>
            <input
              type="number"
              id="numberSelect"
              v-model="selectedOption"
              min="0"
              max="36"
              class="form-control"
            />
          </div>

          <div v-if="betType === 'numberColor'" class="form-group">
            <label for="numberColorSelect">Seleccione Número (0-36):</label>
            <input
              type="number"
              id="numberColorSelect"
              v-model="selectedNumber"
              min="0"
              max="36"
              class="form-control"
            />
            <label>Seleccione Color:</label>
            <div class="color-options">
              <div class="form-check">
                <input
                  type="radio"
                  id="redColorNumber"
                  v-model="selectedColor"
                  value="rojo"
                  class="form-check-input"
                />
                <label for="redColorNumber" class="form-check-label">Rojo</label>
              </div>
              <div class="form-check">
                <input
                  type="radio"
                  id="blackColorNumber"
                  v-model="selectedColor"
                  value="negro"
                  class="form-check-input"
                />
                <label for="blackColorNumber" class="form-check-label">Negro</label>
              </div>
            </div>
          </div>

          <button type="submit" class="btn btn-primary">Realizar Apuesta</button>
        </form>
      </div>
      <div v-else class="no-balance-warning">
        <p>No tienes saldo suficiente para realizar apuestas.</p>
      </div>
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
import { useUserStore } from '@/stores/user'

// Router
const router = useRouter()

// Estado del juego y del usuario
const userStore = useUserStore()
const showBetForm = ref(false)

// Variables para el formulario de apuesta
const amount = ref(0)              // Monto a apostar
const betType = ref(null)             // Tipo de apuesta seleccionado ('color', 'paridad' o 'numero')
const selectedOption = ref('')        // Opción seleccionada para la apuesta (ya sea color, paridad o número)
const selectedNumber = ref(null)      // Número seleccionado para la apuesta de número y color
const selectedColor = ref(null)       // Color seleccionado para la apuesta de número y color

// Monitorear cambios en currentUser
watch(userStore.user, (newVal) => {
  console.log("Watch - currentUser cambió a:", newVal)
})

// -----------------------------------------------------------------
// Funciones para recibir los datos del usuario desde GameMenu.vue
// -----------------------------------------------------------------

const handleStartNewGame = async (userData) => {
  try {
    console.log('Iniciando nuevo juego con datos:', userData);
    const requestData = {
      userName: userData.name,
      initialBalance: Number(userData.balance) || 0
    };

    const response = await fetch('https://localhost:7156/api/Session/start', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestData)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    console.log('Respuesta del servidor:', data);
    
    // Guardar usuario en Pinia
    userStore.setUser({
      name: data.userName,
      balance: data.balance,
      sessionId: data.sessionId
    });

    // Navegar directamente a la ruleta
    router.push({
      name: 'RouletteWheel',
      query: {
        sessionId: data.sessionId
      }
    });

  } catch (error) {
    console.error('Error al iniciar nuevo juego:', error);
    alert('Error al iniciar el juego. Por favor, intente nuevamente.');
  }
};

// Recibe el usuario cuando se inicia un juego (Nuevo Juego o Cargar Juego)
/*const handleGameStart = (userData) => {
  console.log("Recibido en handleGameStart:", userData)
  currentUser.value = userData  // Actualizar usuario
  showBetForm.value = true        // Mostrar formulario de apuestas
  console.log("currentUser asignado:", currentUser.value)
  console.log("showBetForm:", showBetForm.value)
}*/

// Si deseas manejar por separado la carga de un juego ya existente, puedes definir otro método:
const handleGameLoad = async (userData) => {
  try {
    // 1. Consultar el backend para obtener el usuario y su balance
    const user = await loadUser(userData.name)
    if (!user || !user.userName) {
      alert('El usuario no existe. Por favor, verifica el nombre ingresado.');
      return;
    }

    // 2. Crear una nueva sesión con el balance actual del usuario
    const session = await startSession(user.userName, userData.balance || 0)
    if (!session || !session.sessionId) {
      alert('No se pudo iniciar la sesión para el usuario.');
      return;
    }

    // 3. Guardar usuario y sessionId en Pinia
    userStore.setUser({
      name: user.userName,
      balance: session.balance,
      sessionId: session.sessionId
    })

    // 4. Navegar a la ruleta
    router.push({
      name: 'RouletteWheel',
      query: {
        sessionId: session.sessionId
      }
    })
  } catch (error) {
    // Si el error es porque no existe el usuario o la respuesta no es JSON, mostrar mensaje amigable
    if (error.message && (error.message.includes('not found') || error.message.includes('No se encontró') || error.message.includes('Unexpected token'))) {
      alert('El usuario no existe. Por favor, verifica el nombre ingresado.');
    } else {
      alert('Error al cargar el usuario: ' + (error.message || error));
    }
  }
}

// -----------------------------------------------------------------
// Función para procesar la apuesta
// -----------------------------------------------------------------

const submitBet = async () => {
  try {
    if (!userStore.user.name) {
      alert("Error: No hay usuario activo");
      return;
    }

    if (!amount.value || amount.value <= 0) {
      alert("Error: El monto de la apuesta debe ser mayor que cero");
      return;
    }

    if (userStore.user.balance < amount.value) {
      alert("Error: Saldo insuficiente");
      return;
    }

    if (!selectedOption.value) {
      alert("Error: Debes seleccionar una opción para apostar");
      return;
    }

    // Verificar que tenemos una sesión activa
    if (!userStore.user.sessionId) {
      throw new Error('No hay una sesión activa');
    }

    // Mapear el tipo de apuesta al enum del backend
    const betTypeMap = {
      'color': 0,    // Color
      'evenOdd': 1,  // ParImpar
      'number': 2,    // Numero
      'numberColor': 3  // Numero y Color
    };

    const betRequest = {
      userName: userStore.user.name,
      type: betTypeMap[betType.value],
      amount: parseFloat(amount.value),
      selectedColor: null,
      selectedParity: null,
      selectedNumber: null
    };

    console.log('DEBUG - Bet Type Mapping:');
    console.log('Original bet type:', betType.value);
    console.log('Mapped bet type:', betTypeMap[betType.value]);
    console.log('Full bet request:', betRequest);

    switch (betType.value) {
      case 'color':
        betRequest.selectedColor = selectedOption.value;
        console.log('Apuesta por color:', { color: selectedOption.value });
        break;
      case 'evenOdd':
        const [parity, color] = selectedOption.value.split('-');
        betRequest.selectedParity = parity;
        betRequest.selectedColor = color;
        console.log('Apuesta por par/impar:', { parity, color });
        break;
      case 'number':
        if (!selectedOption.value || isNaN(selectedOption.value)) {
          throw new Error('Debes seleccionar un número válido');
        }
        const number = parseInt(selectedOption.value);
        if (number < 0 || number > 36) {
          throw new Error('El número debe estar entre 0 y 36');
        }
        betRequest.selectedNumber = number;
        console.log('Apuesta por número:', { number });
        break;
      case 'numberColor':
        if (!selectedNumber.value || isNaN(selectedNumber.value)) {
          throw new Error('Debes seleccionar un número válido');
        }
        const numberColor = parseInt(selectedNumber.value);
        if (numberColor < 0 || numberColor > 36) {
          throw new Error('El número debe estar entre 0 y 36');
        }
        if (!selectedColor.value) {
          throw new Error('Debes seleccionar un color');
        }
        betRequest.selectedNumber = numberColor;
        betRequest.selectedColor = selectedColor.value;
        console.log('DEBUG - Apuesta por número y color:');
        console.log('Número seleccionado (raw):', selectedNumber.value);
        console.log('Número seleccionado (parsed):', numberColor);
        console.log('Color seleccionado:', selectedColor.value);
        console.log('Objeto betRequest:', JSON.stringify(betRequest, null, 2));
        break;
    }

    // Validar que la apuesta tenga al menos un valor seleccionado
    if (!betRequest.selectedColor && !betRequest.selectedParity && betRequest.selectedNumber === null) {
      throw new Error('Debes seleccionar al menos una opción para apostar');
    }

    // Validar que para apuestas de par/impar se haya seleccionado una opción
    if (betType.value === 'evenOdd' && !selectedOption.value) {
      throw new Error('Debes seleccionar una combinación de par/impar y color');
    }

    console.log('Datos completos de la apuesta:', {
      tipo: betType.value,
      monto: amount.value,
      numero: betRequest.selectedNumber,
      color: betRequest.selectedColor,
      paridad: betRequest.selectedParity,
      request: betRequest
    });

    // Procesar la apuesta
    const result = await processBet(userStore.user.sessionId, betRequest);
    
    // Redirigir a RouletteWheel con los resultados
    router.push({
      name: 'RouletteWheel',
      query: {
        sessionId: userStore.user.sessionId
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
  userStore.setUser({
    name: null,
    balance: 0,
    sessionId: null
  })
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
  background-color: #f8f9fa;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.user-info h3 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.user-info p {
  margin: 0.5rem 0;
  color: #495057;
}

.betting-options {
  background-color: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.no-balance-warning {
  background-color: #fff3cd;
  color: #856404;
  padding: 1rem;
  border-radius: 8px;
  margin-top: 1rem;
  text-align: center;
}

.bet-form {
  max-width: 600px;
  margin: 0 auto;
  padding: 2rem;
}

.loading-message {
  text-align: center;
  padding: 2rem;
  color: #fff;
  font-size: 1.2rem;
}
</style>