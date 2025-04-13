<template>
    <div class="load-user">
      <h2>Cargar Usuario</h2>
      <!-- Formulario para ingresar el nombre de usuario -->
      <form @submit.prevent="handleLoadUser">
        <div class="form-group">
          <label for="userName">Nombre de Usuario:</label>
          <input
            id="userName"
            v-model="userName"
            placeholder="Ingresa tu nombre"
            required
          />
        </div>
        <button type="submit" class="btn-load">Cargar Usuario</button>
      </form>
      
      <!-- Muestra la información del usuario si ya fue cargada -->
      <div v-if="loadedUser" class="user-details">
        <h3>Detalles del Usuario</h3>
        <p><strong>Nombre:</strong> {{ loadedUser.name }}</p>
        <p><strong>Saldo:</strong> {{ loadedUser.balance }}</p>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue'
  import axios from 'axios'
  
  
  const userName = ref('')
  const loadedUser = ref(null)
  
  const handleLoadUser = async () => {
    if (!userName.value.trim()) {
      alert('Ingresa un nombre de usuario')
      return
    }
    
    try {
      // Se realiza la petición al endpoint para cargar usuario, 
      // asumiendo que éste devuelve un objeto con { name, balance }.
      const response = await axios.get(`/api/users/load/${userName.value}`)
      loadedUser.value = response.data
    } catch (error) {
      console.error("Error al cargar usuario:", error)
      alert("No se pudo cargar el usuario. Verifica el nombre ingresado.")
    }
  }
  </script>
  
  <style scoped lang="scss">
  .load-user {
    max-width: 400px;
    margin: 0 auto;
    padding: 20px;
    border: 1px solid #ccc;
    border-radius: 4px;
    text-align: center;
  
    h2 {
      margin-bottom: 1.5rem;
    }
  
    .form-group {
      margin-bottom: 1rem;
      text-align: left;
      
      label {
        display: block;
        margin-bottom: 0.5rem;
        font-weight: bold;
      }
      
      input {
        width: 100%;
        padding: 8px;
        border: 1px solid #ccc;
        border-radius: 4px;
        box-sizing: border-box;
      }
    }
    
    .btn-load {
      padding: 10px 20px;
      font-size: 1rem;
      cursor: pointer;
      border: none;
      border-radius: 4px;
      background-color: #007bff;
      color: #fff;
      transition: background-color 0.3s;
      
      &:hover {
        background-color: #0056b3;
      }
    }
    
    .user-details {
      margin-top: 1.5rem;
      
      h3 {
        margin-bottom: 0.5rem;
      }
      
      p {
        margin: 0.25rem 0;
        font-size: 1rem;
      }
    }
  }
  </style>