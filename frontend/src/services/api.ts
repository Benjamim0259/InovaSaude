import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000, // 30 segundos para free tier do Render
});

// Retry logic para requisições que falharem
const retryRequest = async (error: any) => {
  const config = error.config;

  if (!config || !error.response) {
    return Promise.reject(error);
  }

  config.retryCount = config.retryCount || 0;
  const maxRetries = 3;
  const retryDelay = 1000; // 1 segundo inicial

  // Não faz retry em erros de autenticação ou client errors
  if (error.response.status === 401 || config.retryCount >= maxRetries) {
    return Promise.reject(error);
  }

  // Incrementa o contador de tentativas
  config.retryCount++;

  // Calcula delay exponencial
  const delay = retryDelay * Math.pow(2, config.retryCount - 1);

  // Aguarda antes de fazer retry
  await new Promise(resolve => setTimeout(resolve, delay));

  return api(config);
};

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Trata erros de autenticação
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
      return Promise.reject(error);
    }

    // Faz retry em erros de servidor (5xx) e timeout
    if (
      (error.response?.status >= 500) ||
      error.code === 'ECONNABORTED' ||
      error.code === 'ENOTFOUND' ||
      !error.response
    ) {
      return retryRequest(error);
    }

    return Promise.reject(error);
  }
);

export default api;
