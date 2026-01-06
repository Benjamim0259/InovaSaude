import api from './api';
import { AuthResponse, Usuario } from '@types/index';

export const authService = {
  async login(email: string, senha: string): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>('/auth/login', { email, senha });
    localStorage.setItem('token', data.token);
    localStorage.setItem('user', JSON.stringify(data.usuario));
    return data;
  },

  async register(userData: {
    nome: string;
    email: string;
    senha: string;
    perfil: string;
    ubsId?: string;
  }): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>('/auth/register', userData);
    return data;
  },

  async logout(): Promise<void> {
    await api.post('/auth/logout');
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  async forgotPassword(email: string): Promise<{ message: string }> {
    const { data } = await api.post('/auth/forgot-password', { email });
    return data;
  },

  getCurrentUser(): Usuario | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  },
};
