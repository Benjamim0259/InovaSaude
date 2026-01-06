import api from './api';
import type { UBS, CreateUBSDto } from '../types';

export const ubsService = {
  async getAll(): Promise<UBS[]> {
    const response = await api.get('/ubs');
    return response.data;
  },

  async getById(id: string): Promise<UBS> {
    const response = await api.get(`/ubs/${id}`);
    return response.data;
  },

  async create(data: CreateUBSDto): Promise<UBS> {
    const response = await api.post('/ubs', data);
    return response.data;
  },

  async update(id: string, data: Partial<CreateUBSDto>): Promise<void> {
    await api.put(`/ubs/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/ubs/${id}`);
  }
};
