<template>
  <div class="betting-form">
    <h2>Apostar por {{ betTypeLabel }}</h2>
    <form @submit.prevent="submitBet">
      <!-- Campo para seleccionar color (común a todos los tipos) -->
      <div class="form-group" v-if="hasColorField">
        <label for="betColor">Color:</label>
        <select id="betColor" v-model="betColor" required>
          <option value="rojo">Rojo</option>
          <option value="negro">Negro</option>
        </select>
      </div>

      <!-- Campo para el número (solo para apuesta por número) -->
      <div class="form-group" v-if="betType === 'numero'">
        <label for="betNumber">Número (0-36):</label>
        <input id="betNumber" type="number" v-model.number="betNumber" min="0" max="36" required />
      </div>

      <!-- Campo para la paridad (solo para apuesta por paridad) -->
      <div class="form-group" v-if="betType === 'paridad'">
        <label for="betParity">Paridad:</label>
        <select id="betParity" v-model="betParity" required>
          <option value="par">Par</option>
          <option value="impar">Impar</option>
        </select>
      </div>

      <!-- Campo para el monto a apostar, común a todos -->
      <div class="form-group">
        <label for="betAmount">Monto a Apostar:</label>
        <input id="betAmount" type="number" v-model.number="betAmount" min="1" required />
      </div>

      <!-- Botones para enviar o cancelar -->
      <div class="form-buttons">
        <button type="submit" class="btn-submit">Apostar</button>
        <button type="button" class="btn-cancel" @click="cancelBet">Cancelar</button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

// Prop que indica el tipo de apuesta: 'numero', 'color' o 'paridad'
const props = defineProps({
  betType: {
    type: String,
    required: true,
    validator: (value) => ['numero', 'color', 'paridad'].includes(value)
  }
})

// Emite el resultado de la apuesta
const emit = defineEmits(['bet-submitted', 'bet-cancelled'])

const betNumber = ref(null)
const betColor = ref('rojo')
const betParity = ref('par')
const betAmount = ref(0)

// Computed para mostrar una etiqueta amigable en el título
const betTypeLabel = computed(() => {
  if (props.betType === 'numero') return 'Número (y Color)'
  if (props.betType === 'color') return 'Color'
  if (props.betType === 'paridad') return 'Paridad (par/impar y Color)'
  return ''
})

// Si el campo de selección de color es común para todos los tipos
const hasColorField = computed(() => {
  return ['numero', 'color', 'paridad'].includes(props.betType)
})

const submitBet = () => {
  // Validamos campos específicos
  if (props.betType === 'numero' && (betNumber.value === null || betNumber.value < 0 || betNumber.value > 36)) {
    alert('Ingresa un número válido entre 0 y 36.')
    return
  }
  // Preparamos la data de la apuesta
  const betData = {
    betType: props.betType,
    amount: betAmount.value,
    color: betColor.value
  }
  if (props.betType === 'numero') {
    betData.number = betNumber.value
  }
  if (props.betType === 'paridad') {
    betData.parity = betParity.value
  }
  emit('bet-submitted', betData)
}

const cancelBet = () => {
  emit('bet-cancelled')
}
</script>

<style scoped lang="scss">
.betting-form {
  max-width: 400px;
  margin: 0 auto;
  padding: 20px;
  border: 1px solid #ccc;
  border-radius: 4px;
  background-color: #f1f1f1;

  h2 {
    margin-bottom: 1rem;
  }

  .form-group {
    margin-bottom: 1rem;
    text-align: left;

    label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: bold;
    }

    input,
    select {
      width: 100%;
      padding: 8px;
      box-sizing: border-box;
      border: 1px solid #ccc;
      border-radius: 4px;
    }
  }

  .form-buttons {
    display: flex;
    gap: 1rem;
    justify-content: center;

    button {
      flex: 1;
      padding: 10px 20px;
      font-size: 1rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      transition: background-color 0.3s;
    }

    .btn-submit {
      background-color: #007bff;
      color: #fff;

      &:hover {
        background-color: #0056b3;
      }
    }

    .btn-cancel {
      background-color: #dc3545;
      color: #fff;

      &:hover {
        background-color: #c82333;
      }
    }
  }
}
</style>