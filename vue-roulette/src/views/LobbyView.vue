<template>
    <div class="lobby-container">
      <!-- Header con info del jugador -->
      <div class="player-info">
        <h1>🎮 Bienvenido, <span class="gold-text">{{ userName }}</span></h1>
        <div class="balance-box">
          <div class="chip-icon">🪙</div>
          <div class="balance">Balance: ${{ formatNumber(userBalance) }}</div>
        </div>
      </div>
  
      <!-- Formulario de apuestas -->
      <div class="bet-card">
        <h2>💸 Realizar Apuesta</h2>
        <form @submit.prevent="placeBet" class="bet-form">
          <!-- Tipo de apuesta -->
          <div class="form-group">
            <label>Tipo de Apuesta:</label>
            <select v-model="betType" class="bet-select">
              <option value="0">Número específico</option>
              <option value="1">Color</option>
              <option value="2">Par/Impar</option>
            </select>
          </div>
  
          <!-- Valor de la apuesta (dinámico según tipo) -->
          <div class="form-group">
            <label>{{ betTypeLabel }}:</label>
            <input
              v-if="betType === '0'"
              v-model.number="betValue"
              type="number"
              min="0"
              max="36"
              class="bet-input"
            >
            <select v-else-if="betType === '1'" v-model="betValue" class="bet-select">
              <option value="rojo">Rojo</option>
              <option value="negro">Negro</option>
            </select>
            <select v-else v-model="betValue" class="bet-select">
              <option value="par">Par</option>
              <option value="impar">Impar</option>
            </select>
          </div>
  
          <!-- Monto a apostar -->
          <div class="form-group">
            <label>Monto:</label>
            <div class="amount-selector">
              <button 
                v-for="amt in [100, 500, 1000]" 
                :key="amt" 
                @click.prevent="setAmount(amt)"
                :class="{ 'active': betAmount === amt }"
                class="chip-button"
              >
                ${{ amt }}
              </button>
              <input
                v-model.number="betAmount"
                type="number"
                min="100"
                :max="userBalance"
                class="custom-amount"
                placeholder="Otro monto"
              >
            </div>
          </div>
  
          <button type="submit" class="bet-button">
            🎰 Apostar ${{ formatNumber(betAmount) }}
          </button>
        </form>
      </div>
  
      <!-- Notificaciones y errores -->
      <div v-if="error" class="error-message">{{ error }}</div>
      <div v-if="message" class="success-message">{{ message }}</div>
    </div>
  </template>
  
  <script setup>
  import { ref, computed, onMounted } from 'vue'
  import axios from 'axios'
  import { useRouter } from 'vue-router'
  
  const router = useRouter()
  
  // Datos de sesión
  const userName = ref(localStorage.getItem('userName') || '')
  const userBalance = ref(0)
  const roundId = ref(null)
  
  // Datos de apuesta
  const betType = ref('0')
  const betValue = ref('')
  const betAmount = ref(100)
  const error = ref('')
  const message = ref('')
  
  // Labels dinámicos
  const betTypeLabel = computed(() => {
    return {
      '0': 'Número (0-36)',
      '1': 'Color',
      '2': 'Paridad'
    }[betType.value]
  })
  
  // Formatear números
  const formatNumber = (num) => {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",")
  }
  
  // Obtener balance inicial
  const fetchBalance = async () => {
    try {
      const response = await axios.get(`/api/User/balance/${userName.value}`)
      userBalance.value = response.data.balance
    } catch (err) {
      error.value = 'Error obteniendo balance'
    }
  }
  
  // Crear nueva ronda
  const createRound = async () => {
    try {
      const response = await axios.post('/api/Round/create', {
        userName: userName.value
      })
      roundId.value = response.data.roundId
    } catch (err) {
      error.value = 'Error creando ronda'
    }
  }
  
  // Enviar apuesta
  const placeBet = async () => {
    try {
      if (!roundId.value) await createRound()
      
      const betData = {
        userName: userName.value,
        roundId: roundId.value,
        amount: betAmount.value,
        type: parseInt(betType.value),
        betValue: betValue.value.toString()
      }
  
      await axios.post('/api/Bet/create', betData)
      
      // Actualizar balance localmente
      userBalance.value -= betAmount.value
      message.value = `Apuesta de $${betAmount.value} realizada!`
      
      // Redirigir a ruleta después de 2 segundos
      setTimeout(() => router.push('/roulette'), 2000)
      
    } catch (err) {
      error.value = err.response?.data?.message || 'Error realizando apuesta'
    }
  }
  
  // Configurar monto rápido
  const setAmount = (amount) => {
    betAmount.value = amount
  }
  
  // Inicializar componente
  onMounted(async () => {
    if (!userName.value) router.push('/')
    await fetchBalance()
    await createRound()
  })
  </script>
  
  <style scoped>
  .lobby-container {
    max-width: 800px;
    margin: 2rem auto;
    padding: 2rem;
    background: linear-gradient(135deg, #1a1a2e, #16213e);
    border-radius: 15px;
  }
  
  .player-info {
    text-align: center;
    margin-bottom: 2rem;
  }
  
  .gold-text {
    color: #f1c40f;
    text-shadow: 0 0 10px rgba(241, 196, 15, 0.5);
  }
  
  .balance-box {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 1rem;
    padding: 1rem;
    background: rgba(0, 0, 0, 0.3);
    border-radius: 10px;
    margin-top: 1rem;
  }
  
  .chip-icon {
    font-size: 2rem;
  }
  
  .balance {
    font-size: 1.5rem;
    color: #fff;
  }
  
  .bet-card {
    background: rgba(255, 255, 255, 0.1);
    padding: 2rem;
    border-radius: 15px;
    backdrop-filter: blur(5px);
  }
  
  .bet-form {
    display: grid;
    gap: 1.5rem;
  }
  
  .bet-select, .bet-input {
    width: 100%;
    padding: 0.8rem;
    background: rgba(0, 0, 0, 0.3);
    border: 1px solid #f1c40f;
    color: white;
    border-radius: 8px;
  }
  
  .amount-selector {
    display: grid;
    grid-template-columns: repeat(3, 1fr) 2fr;
    gap: 0.5rem;
  }
  
  .chip-button {
    padding: 0.8rem;
    background: #f1c40f;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.3s;
  }
  
  .chip-button.active {
    background: #e67e22;
    transform: scale(1.05);
  }
  
  .custom-amount {
    grid-column: span 1;
    padding: 0.8rem;
    background: rgba(0, 0, 0, 0.3);
    border: 1px solid #f1c40f;
    color: white;
    border-radius: 8px;
  }
  
  .bet-button {
    width: 100%;
    padding: 1rem;
    background: linear-gradient(45deg, #f1c40f, #e67e22);
    color: white;
    border: none;
    border-radius: 8px;
    font-size: 1.2rem;
    cursor: pointer;
    transition: transform 0.3s;
  }
  
  .bet-button:hover {
    transform: translateY(-3px);
  }
  
  .error-message {
    color: #e74c3c;
    text-align: center;
    margin-top: 1rem;
  }
  
  .success-message {
    color: #2ecc71;
    text-align: center;
    margin-top: 1rem;
  }
  </style>