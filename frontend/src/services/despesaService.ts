import api from './api';
import type { Despesa, CreateDespesaDto } from '../types';

export const despesaService = {
  async getAll(filters?: {
    ubsId?: string;
    dataInicio?: string;
    dataFim?: string;
  }): Promise<Despesa[]> {
    const params = new URLSearchParams();
    if (filters?.ubsId) params.append('ubsId', filters.ubsId);
    if (filters?.dataInicio) params.append('dataInicio', filters.dataInicio);
    if (filters?.dataFim) params.append('dataFim', filters.dataFim);
    
    const response = await api.get(`/despesas?${params.toString()}`);
    return response.data;
  },

  async getById(id: string): Promise<Despesa> {
    const response = await api.get(`/despesas/${id}`);
    return response.data;
  },

  async create(data: CreateDespesaDto): Promise<Despesa> {
    const response = await api.post('/despesas', data);
    return response.data;
  },

  async update(id: string, data: Partial<Despesa>): Promise<void> {
    await api.put(`/despesas/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/despesas/${id}`);
  },

  async uploadComprovante(id: string, file: File): Promise<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await api.post(`/despesas/${id}/comprovante`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  }
};
