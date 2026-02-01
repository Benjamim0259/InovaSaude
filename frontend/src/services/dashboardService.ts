import api from './api';

export interface DashboardStats {
  totalUbs: number;
  totalDespesas: number;
  despesasPendentes: number;
  relatoriosGerados: number;
  valorTotalDespesas: number;
  mediaDespesasPorUbs: number;
  topDespesasPorCategoria: Array<{
    categoria: string;
    valor: number;
    porcentagem: number;
  }>;
  despesasPorMes: Array<{
    mes: string;
    valor: number;
    quantidade: number;
  }>;
  statusDespesas: {
    aprovadas: number;
    pendentes: number;
    rejeitadas: number;
  };
}

export interface DashboardFilters {
  dataInicio?: string;
  dataFim?: string;
  ubsId?: number;
  categoria?: string;
}

export const dashboardService = {
  async getStats(filtros?: DashboardFilters): Promise<DashboardStats> {
    try {
      const params = new URLSearchParams();

      if (filtros?.dataInicio) params.append('dataInicio', filtros.dataInicio);
      if (filtros?.dataFim) params.append('dataFim', filtros.dataFim);
      if (filtros?.ubsId) params.append('ubsId', filtros.ubsId.toString());
      if (filtros?.categoria) params.append('categoria', filtros.categoria);

      const response = await api.get(`/dashboard/stats?${params.toString()}`);
      return response.data;
    } catch (error) {
      console.error('Erro ao buscar estatísticas do dashboard:', error);
      // Retornar dados mockados em caso de erro
      return this.getMockStats();
    }
  },

  async getDespesasPorCategoria(): Promise<Array<{ categoria: string; valor: number; quantidade: number }>> {
    try {
      const response = await api.get('/dashboard/despesas-por-categoria');
      return response.data;
    } catch (error) {
      console.error('Erro ao buscar despesas por categoria:', error);
      return this.getMockDespesasPorCategoria();
    }
  },

  async getDespesasPorMes(): Promise<Array<{ mes: string; valor: number; quantidade: number }>> {
    try {
      const response = await api.get('/dashboard/despesas-por-mes');
      return response.data;
    } catch (error) {
      console.error('Erro ao buscar despesas por mês:', error);
      return this.getMockDespesasPorMes();
    }
  },

  async getTopUbsPorDespesas(): Promise<Array<{ ubs: string; valor: number; quantidade: number }>> {
    try {
      const response = await api.get('/dashboard/top-ubs-despesas');
      return response.data;
    } catch (error) {
      console.error('Erro ao buscar top UBS por despesas:', error);
      return this.getMockTopUbsPorDespesas();
    }
  },

  // Dados mockados para desenvolvimento
  getMockStats(): DashboardStats {
    return {
      totalUbs: 15,
      totalDespesas: 234,
      despesasPendentes: 12,
      relatoriosGerados: 8,
      valorTotalDespesas: 456789.50,
      mediaDespesasPorUbs: 30452.63,
      topDespesasPorCategoria: [
        { categoria: 'Medicamentos', valor: 125000, porcentagem: 27.3 },
        { categoria: 'Equipamentos', valor: 98000, porcentagem: 21.4 },
        { categoria: 'Manutenção', valor: 76500, porcentagem: 16.7 },
        { categoria: 'Material de Consumo', valor: 67800, porcentagem: 14.8 },
        { categoria: 'Serviços', valor: 45600, porcentagem: 10.0 }
      ],
      despesasPorMes: [
        { mes: 'Jan', valor: 35000, quantidade: 45 },
        { mes: 'Fev', valor: 42000, quantidade: 52 },
        { mes: 'Mar', valor: 38000, quantidade: 48 },
        { mes: 'Abr', valor: 41000, quantidade: 51 },
        { mes: 'Mai', valor: 39000, quantidade: 49 },
        { mes: 'Jun', valor: 43000, quantidade: 53 }
      ],
      statusDespesas: {
        aprovadas: 198,
        pendentes: 12,
        rejeitadas: 24
      }
    };
  },

  getMockDespesasPorCategoria(): Array<{ categoria: string; valor: number; quantidade: number }> {
    return [
      { categoria: 'Medicamentos', valor: 125000, quantidade: 89 },
      { categoria: 'Equipamentos', valor: 98000, quantidade: 34 },
      { categoria: 'Manutenção', valor: 76500, quantidade: 28 },
      { categoria: 'Material de Consumo', valor: 67800, quantidade: 156 },
      { categoria: 'Serviços', valor: 45600, quantidade: 23 },
      { categoria: 'Outros', valor: 23400, quantidade: 12 }
    ];
  },

  getMockDespesasPorMes(): Array<{ mes: string; valor: number; quantidade: number }> {
    return [
      { mes: 'Jan', valor: 35000, quantidade: 45 },
      { mes: 'Fev', valor: 42000, quantidade: 52 },
      { mes: 'Mar', valor: 38000, quantidade: 48 },
      { mes: 'Abr', valor: 41000, quantidade: 51 },
      { mes: 'Mai', valor: 39000, quantidade: 49 },
      { mes: 'Jun', valor: 43000, quantidade: 53 }
    ];
  },

  getMockTopUbsPorDespesas(): Array<{ ubs: string; valor: number; quantidade: number }> {
    return [
      { ubs: 'UBS Centro', valor: 45600, quantidade: 34 },
      { ubs: 'UBS Jardim', valor: 42300, quantidade: 31 },
      { ubs: 'UBS São João', valor: 38900, quantidade: 28 },
      { ubs: 'UBS Industrial', valor: 35600, quantidade: 26 },
      { ubs: 'UBS Vila Nova', valor: 31200, quantidade: 23 }
    ];
  }
};