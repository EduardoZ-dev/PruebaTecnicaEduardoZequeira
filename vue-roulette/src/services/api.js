import axios from 'axios';

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
export const processBet = async (sessionId, betRequest) => {
  try {
    console.log('Enviando apuesta:', {
      sessionId,
      betRequest: JSON.stringify(betRequest, null, 2)
    });

    const sessionBetRequest = {
      sessionId: sessionId,
      bet: betRequest
    };

    const response = await fetch(`${API_BASE_URL}/Session/bet`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(sessionBetRequest)
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Error en processBet:', {
        status: response.status,
        statusText: response.statusText,
        errorText
      });

      let errorMessage = 'Error al procesar la apuesta';
      try {
        const errorData = JSON.parse(errorText);
        if (errorData) {
          if (typeof errorData === 'string') {
            errorMessage = errorData;
          } else if (errorData.message) {
            errorMessage = errorData.message;
          } else if (errorData.title) {
            errorMessage = errorData.title;
          }
        }
      } catch (e) {
        errorMessage = errorText || `Error ${response.status}: ${response.statusText}`;
      }

      throw new Error(errorMessage);
    }

    const responseText = await response.text();
    if (!responseText) {
      throw new Error('Respuesta vacía del servidor');
    }

    const result = JSON.parse(responseText);
    console.log('Respuesta del servidor:', result);
    return result;
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
export const loadUser = async (userName, initialBalance = null) => {
  try {
    let url = `${API_BASE_URL}/User/load/${userName}`;
    if (initialBalance !== null && initialBalance > 0) {
      url += `?initialBalance=${initialBalance}`;
    }
    
    const response = await fetch(url);
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
export const placeBet = async (sessionId, betRequest) => {
  try {
    console.log('Enviando apuesta al backend:', betRequest); 
    const response = await fetch(`https://localhost:7156/api/roulette/bet/${sessionId}`, {
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