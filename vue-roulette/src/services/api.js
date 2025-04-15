const API_BASE_URL = 'https://localhost:7156/api';

// Función auxiliar para manejar las respuestas HTTP
const handleResponse = async (response) => {
  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Error en la petición');
  }
  return response.json();
};

// Función para iniciar una nueva sesión
export const startSession = async (userName, initialBalance) => {
  try {
    const response = await fetch(`${API_BASE_URL}/Session/start`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ userName, initialBalance })
    });
    return handleResponse(response);
  } catch (error) {
    console.error('Error al iniciar sesión:', error);
    throw error;
  }
};

// Función para procesar una apuesta
export const processBet = async (sessionId, betData) => {
  try {
    const response = await fetch('https://localhost:7156/api/session/bet', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        sessionId: sessionId,
        bet: {
          userName: betData.userName,
          amount: betData.amount,
          type: betData.type,
          value: betData.value
        }
      })
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Error en la apuesta: ${errorText}`);
    }

    return await response.json();
  } catch (error) {
    console.error('Error en processBet:', error);
    throw error;
  }
};

// Función para guardar la sesión
export const saveSession = async (sessionId) => {
  try {
    const response = await fetch(`${API_BASE_URL}/Session/save`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ sessionId })
    });
    return handleResponse(response);
  } catch (error) {
    console.error('Error al guardar sesión:', error);
    throw error;
  }
};

// Función para cargar un usuario
export const loadUser = async (userName) => {
  try {
    const response = await fetch(`${API_BASE_URL}/User/${userName}`);
    return handleResponse(response);
  } catch (error) {
    console.error('Error al cargar usuario:', error);
    throw error;
  }
};

// Función para actualizar el balance de un usuario
export const updateUserBalance = async (userName, newBalance) => {
  try {
    const response = await fetch(`${API_BASE_URL}/User/updateBalance`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ userName, newBalance })
    });
    return handleResponse(response);
  } catch (error) {
    console.error('Error al actualizar balance:', error);
    throw error;
  }
};

// Función para guardar el saldo de un usuario
export const saveUserBalance = async (userName, balance) => {
  try {
    const response = await fetch(`${API_BASE_URL}/users/save`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ userName, balance })
    });
    return handleResponse(response);
  } catch (error) {
    console.error('Error al guardar saldo:', error);
    throw error;
  }
};

// Función para girar la ruleta
export const spinRoulette = async () => {
  try {
    const response = await fetch(`${API_BASE_URL}/roulette/spin`);
    return handleResponse(response);
  } catch (error) {
    console.error('Error al girar la ruleta:', error);
    throw error;
  }
};

// Función para realizar una apuesta
export const placeBet = async (betRequest) => {
  try {
    console.log('Enviando apuesta al backend:', betRequest);
    
    const response = await fetch('https://localhost:7156/api/roulette/bet', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(betRequest)
    });

    if (!response.ok) {
      const errorData = await response.json();
      console.error('Error del servidor:', {
        status: response.status,
        statusText: response.statusText,
        errorData: errorData
      });
      
      // Si hay errores de validación, mostrarlos
      if (errorData.errors) {
        const validationErrors = Object.entries(errorData.errors)
          .map(([key, value]) => `${key}: ${value}`)
          .join('\n');
        throw new Error(`Errores de validación:\n${validationErrors}`);
      }
      
      throw new Error(errorData.message || `Error al procesar la apuesta: ${response.status} ${response.statusText}`);
    }

    const result = await response.json();
    console.log('Respuesta del servidor:', result);
    return result;
  } catch (error) {
    console.error('Error en placeBet:', error);
    throw error;
  }
};

// Función para obtener el historial de una sesión
export const getSessionHistory = async (sessionId) => {
  try {
    const response = await fetch(`${API_BASE_URL}/session/history/${sessionId}`);
    return handleResponse(response);
  } catch (error) {
    console.error('Error al obtener historial:', error);
    throw error;
  }
}; 