import api from './api';
import type { LoginRequest, LoginResponse, Usuario } from '../types';

export const authService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    // Modo debug: permite login sem backend
    if (import.meta.env.VITE_DEBUG_MODE === 'true') {
      const fakeUser: Usuario = {
        id: 'dev-user-1',
        nome: 'Admin Dev',
        email: credentials.email,
        roles: ['ADMIN'],
      };
      const devResponse: LoginResponse = {
        token: 'dev-token',
        refreshToken: 'dev-refresh-token',
        expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
        user: fakeUser,
      };
      localStorage.setItem('token', devResponse.token);
      localStorage.setItem('user', JSON.stringify(devResponse.user));
      return devResponse;
    }

    const response = await api.post('/auth/login', credentials);
    return response.data;
  },

  async getCurrentUser(): Promise<Usuario> {
    // Modo debug: retorna usuário do localStorage
    if (import.meta.env.VITE_DEBUG_MODE === 'true') {
      const stored = localStorage.getItem('user');
      if (stored) return JSON.parse(stored) as Usuario;
      return {
        id: 'dev-user-1',
        nome: 'Admin Dev',
        email: 'admin@inovasaude.com.br',
        roles: ['ADMIN'],
      };
    }
    const response = await api.get('/auth/me');
    return response.data;
  },

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }
};
