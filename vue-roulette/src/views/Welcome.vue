<template>
    <div class="welcome">
      <h1>Bienvenido a la Ruleta</h1>
      <form @submit.prevent="startGame" class="welcome-form">
        <div class="form-group">
          <label for="userName">Nombre de Usuario</label>
          <input
            id="userName"
            v-model="userName"
            type="text"
            required
            placeholder="Tu nombre"
          />
        </div>
        <div class="form-group">
          <label for="balance">Balance Inicial</label>
          <input
            id="balance"
            v-model.number="balance"
            type="number"
            min="0.01"
            required
            placeholder="Monto inicial"
          />
        </div>
        <button type="submit" class="btn-start">Comenzar</button>
      </form>
      <p v-if="error" class="error">{{ error }}</p>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  import axios from 'axios';
  import { useRouter } from 'vue-router';
  
  const userName = ref('');
  const balance = ref(null);
  const error = ref('');
  const router = useRouter();
  
  async function startGame() {
    error.value = '';
    try {
      await axios.post('http://localhost:5000/api/User/balance', {
        userName: userName.value,
        balance: balance.value
      });
      // Al terminar, navega a la ruleta
      router.push('/roulette');
    } catch (err) {
      console.error(err);
      error.value = 'No se pudo guardar el balance. Intenta de nuevo.';
    }
  }
  </script>
  
  <style scoped>
  .welcome {
    max-width: 400px;
    margin: 60px auto;
    text-align: center;
  }
  .welcome-form {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }
  .form-group {
    display: flex;
    flex-direction: column;
    text-align: left;
  }
  label {
    margin-bottom: 4px;
    font-weight: bold;
  }
  input {
    padding: 8px;
    font-size: 16px;
  }
  .btn-start {
    padding: 10px 20px;
    font-size: 16px;
    background-color: #527c14;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }
  .error {
    color: red;
    margin-top: 12px;
  }
  </style>