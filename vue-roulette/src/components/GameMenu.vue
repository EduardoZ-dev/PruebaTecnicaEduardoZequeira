<template>
  <div class="game-menu">
    <!-- Pantalla de bienvenida solo si todo está falso -->
    <div v-if="!optionsVisible && !showNewGameForm && !showLoadUserForm" class="welcome-message">
      <h1>Comenzar juego</h1>
      <button class="btn-start" @click="startGame">Empezar</button>
    </div>

    <!-- Menú principal de opciones -->
    <div v-else-if="optionsVisible && !showNewGameForm && !showLoadUserForm" class="button-options">
      <button class="btn-option" @click="newGame">Nuevo Juego</button>
      <button class="btn-option" @click="loadUser">Cargar Juego</button>
      <button class="btn-exit" @click="exitGame">Salir</button>
    </div>

    <!-- Formulario para Nuevo Juego -->
    <div v-if="showNewGameForm" class="new-game-form">
      <h2>Nuevo Juego</h2>
      <form @submit.prevent="submitNewGame">
        <div class="form-group">
          <label for="name">Nombre:</label>
          <input 
            id="name" 
            type="text" 
            v-model="newUserName" 
            placeholder="Ingresa tu nombre" 
            required 
          />
        </div>
        <div class="form-group">
          <label for="balance">Saldo Inicial:</label>
          <input 
            id="balance" 
            type="number" 
            v-model.number="newUserBalance" 
            placeholder="Ingresa el saldo inicial" 
            min="1" 
            required 
          />
        </div>
        <div class="form-buttons">
          <!-- Se emite el evento "start-new-game" con los datos ingresados -->
          <button type="submit" class="btn-submit">Iniciar Juego</button>
          <button type="button" class="btn-cancel" @click="cancelNewGame">Cancelar</button>
        </div>
      </form>
    </div>

    <!-- Formulario para Cargar Juego -->
    <div v-if="showLoadUserForm" class="new-game-form">
      <h2>Cargar Juego</h2>
      <form @submit.prevent="submitLoadUser">
        <div class="form-group">
          <label for="loadName">Nombre:</label>
          <input 
            id="loadName" 
            type="text" 
            v-model="loadUserName" 
            placeholder="Nombre guardado" 
            required 
          />
        </div>
        <div class="form-group">
          <label for="loadBalance">Saldo:</label>
          <input 
            id="loadBalance" 
            type="number" 
            v-model.number="loadUserBalance" 
            placeholder="Saldo a recargar (opcional)" 
            min="0" 
          />
        </div>
        <div class="form-buttons">
          <!-- Se emite el evento "load-game" con los datos ingresados -->
          <button type="submit" class="btn-submit">Cargar Juego</button>
          <button type="button" class="btn-cancel" @click="cancelLoadGame">Cancelar</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

// Utilizamos el router si es necesario para redireccionamientos (aunque en este flujo la redirección la gestiona el padre)
const router = useRouter()

// Definición de eventos personalizados: uno para Nuevo Juego y otro para Cargar Juego.
const emit = defineEmits(['start-new-game', 'load-game'])

// Estados internos para controlar la UI del menú:
const optionsVisible = ref(false)
const showNewGameForm = ref(false)
const showLoadUserForm = ref(false)

// Variables para el formulario de "Nuevo Juego":
const newUserName = ref('')
const newUserBalance = ref(0)

// Variables para el formulario de "Cargar Juego":
const loadUserName = ref('')
const loadUserBalance = ref(0)

// Acciones del menú:

// Al presionar "Empezar" se muestran las opciones principales.
const startGame = () => {
  optionsVisible.value = true
  // Aquí podemos decidir qué formulario mostrar por defecto; en este ejemplo dejamos la elección al usuario.
}

// Cuando se selecciona "Nuevo Juego":
const newGame = () => {
  showNewGameForm.value = true
  showLoadUserForm.value = false
}

// Cuando se selecciona "Cargar Juego":
const loadUser = () => {
  showLoadUserForm.value = true
  showNewGameForm.value = false
}

// Opción de salir: se ocultan todas las vistas para volver a la pantalla de bienvenida.
const exitGame = () => {
  optionsVisible.value = false
  showNewGameForm.value = false
  showLoadUserForm.value = false
}

// Función para confirmar "Nuevo Juego".
const submitNewGame = () => {
  console.log("Nombre del usuario:", newUserName.value)
  console.log("Saldo del usuario:", newUserBalance.value)

  if (!newUserName.value.trim() || newUserBalance.value <= 0) {
    alert("Ingresa un nombre y un saldo válido.")
    return
  }
  console.log("Enviando datos al padre (Nuevo Juego):", {
    name: newUserName.value,
    balance: newUserBalance.value
  })

  // Emitir evento al padre con los datos del nuevo juego.
  emit('start-new-game', {
    name: newUserName.value,
    balance: newUserBalance.value
  })

  // Reiniciar todos los estados
  newUserName.value = ''
  newUserBalance.value = 0
  showNewGameForm.value = false
  showLoadUserForm.value = false
  optionsVisible.value = false
}

// Función para confirmar "Cargar Juego".
const submitLoadUser = () => {
  if (!loadUserName.value.trim()) {
    alert("Ingresa un nombre válido.")
    return
  }

  console.log("Enviando datos al padre (Cargar Juego):", {
    name: loadUserName.value
  })

  // Emitir evento al padre con solo el nombre para cargar juego.
  emit("load-game", {
    name: loadUserName.value
  })

  // Reiniciar los valores internos y ocultar los formularios y opciones.
  loadUserName.value = ''
  loadUserBalance.value = 0
  showLoadUserForm.value = false
  optionsVisible.value = false
}

// Funciones para cancelar formularios y reiniciar el estado:
const cancelNewGame = () => {
  newUserName.value = ''
  newUserBalance.value = 0
  showNewGameForm.value = false
}

const cancelLoadGame = () => {
  loadUserName.value = ''
  loadUserBalance.value = 0
  showLoadUserForm.value = false
}
</script>

<style scoped lang="scss">
.game-menu {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  text-align: center;
  background: linear-gradient(135deg, #1a2a3a 0%, #0f1c26 100%);
  color: #f0f4f8;
  padding: 2rem;

  .welcome-message {
    animation: fadeIn 0.6s ease-out;
    
    h1 {
      font-size: 2.8rem;
      margin-bottom: 2rem;
      background: linear-gradient(90deg, #64f0ff 0%, #6e8efb 100%);
      -webkit-background-clip: text;
      background-clip: text;
      color: transparent;
      text-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .btn-start {
      padding: 14px 32px;
      font-size: 1.3rem;
      font-weight: 600;
      background: linear-gradient(90deg, #4facfe 0%, #00f2fe 100%);
      color: #1e3a5c;
      border-radius: 50px;
      box-shadow: 0 4px 15px rgba(0, 178, 255, 0.3);
      transition: all 0.4s;
      border: none;
      
      &:hover {
        transform: translateY(-3px);
        box-shadow: 0 6px 20px rgba(0, 178, 255, 0.4);
      }
    }
  }

  .button-options {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1.5rem;
    width: 100%;
    max-width: 600px;
    margin: 2rem 0;

    button {
      padding: 14px 0;
      font-size: 1.1rem;
      font-weight: 500;
      border-radius: 8px;
      transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
      border: none;
      cursor: pointer;
    }

    .btn-option {
      background: rgba(255, 255, 255, 0.1);
      backdrop-filter: blur(10px);
      color: #64f0ff;
      border: 1px solid rgba(100, 240, 255, 0.3);
      
      &:hover {
        background: rgba(100, 240, 255, 0.2);
        transform: translateY(-2px);
      }
    }

    .btn-exit {
      background: rgba(220, 53, 69, 0.8);
      color: white;
      grid-column: span 2;
      
      &:hover {
        background: rgba(200, 35, 51, 0.9);
      }
    }
  }

  .new-game-form {
    width: 100%;
    max-width: 500px;
    padding: 2.5rem;
    border-radius: 12px;
    background: rgba(15, 28, 38, 0.8);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(100, 240, 255, 0.1);
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
    animation: slideUp 0.5s ease-out;

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
        label {
          display: block;
          margin-bottom: 0.8rem;
          color: #a3c4d8;
          font-weight: 500;
        }

        input {
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
}

/* Animaciones */
@keyframes fadeIn {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}
</style> 