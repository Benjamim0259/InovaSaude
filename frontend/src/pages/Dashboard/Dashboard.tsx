import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { dashboardService, type DashboardStats, type DashboardFilters } from '../../services/dashboardService';
import { BarChartComponent, PieChartComponent, LineChartComponent, MultiBarChartComponent } from '../../components/Charts';
import { format, subMonths } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import './Dashboard.css';

export const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState<DashboardFilters>({
    dataInicio: format(subMonths(new Date(), 6), 'yyyy-MM-dd'),
    dataFim: format(new Date(), 'yyyy-MM-dd')
  });

  useEffect(() => {
    loadDashboardData();
  }, [filters]);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      const data = await dashboardService.getStats(filters);
      setStats(data);
    } catch (error) {
      console.error('Erro ao carregar dados do dashboard:', error);
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  };

  const formatNumber = (value: number) => {
    return value.toLocaleString('pt-BR');
  };

  if (loading) {
    return (
      <div className="dashboard">
        <div className="loading-grid">
          {[...Array(6)].map((_, i) => (
            <div key={i} className="loading-card">
              <div className="loading-shimmer"></div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <div className="header-content">
          <h1>Bem-vindo, {user?.nome}!</h1>
          <p>Painel de controle do InovaSaúde - Sistema de Gestão de Gastos por UBS</p>
        </div>

        <div className="filters-section">
          <div className="filter-group">
            <label>Data Início:</label>
            <input
              type="date"
              value={filters.dataInicio}
              onChange={(e) => setFilters(prev => ({ ...prev, dataInicio: e.target.value }))}
              className="form-input"
            />
          </div>
          <div className="filter-group">
            <label>Data Fim:</label>
            <input
              type="date"
              value={filters.dataFim}
              onChange={(e) => setFilters(prev => ({ ...prev, dataFim: e.target.value }))}
              className="form-input"
            />
          </div>
          <button onClick={loadDashboardData} className="btn btn-primary">
            Atualizar
          </button>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="quick-actions">
        <h2>Ações Rápidas</h2>
        <div className="quick-actions-grid">
          <Link to="/ubs" className="quick-action-card">
            <div className="quick-action-icon">🏥</div>
            <div className="quick-action-content">
              <h3>Gerenciar UBS</h3>
              <p>Cadastrar e editar unidades de saúde</p>
            </div>
          </Link>

          <Link to="/despesas" className="quick-action-card">
            <div className="quick-action-icon">💰</div>
            <div className="quick-action-content">
              <h3>Registrar Despesas</h3>
              <p>Adicionar novos gastos e importações</p>
            </div>
          </Link>

          <Link to="/relatorios" className="quick-action-card">
            <div className="quick-action-icon">📊</div>
            <div className="quick-action-content">
              <h3>Visualizar Relatórios</h3>
              <p>Análises e relatórios de despesas</p>
            </div>
          </Link>

          <Link to="/usuarios" className="quick-action-card">
            <div className="quick-action-icon">👥</div>
            <div className="quick-action-content">
              <h3>Gerenciar Usuários</h3>
              <p>Controle de acessos e permissões</p>
            </div>
          </Link>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="dashboard-grid">
        <div className="dashboard-card">
          <h3>Total de UBS</h3>
          <div className="card-value">{formatNumber(stats?.totalUbs || 0)}</div>
          <p>Unidades cadastradas no sistema</p>
        </div>

        <div className="dashboard-card">
          <h3>Total de Despesas</h3>
          <div className="card-value">{formatCurrency(stats?.valorTotalDespesas || 0)}</div>
          <p>Valor total no período</p>
        </div>

        <div className="dashboard-card">
          <h3>Despesas Pendentes</h3>
          <div className="card-value">{formatNumber(stats?.despesasPendentes || 0)}</div>
          <p>Aguardando aprovação</p>
        </div>

        <div className="dashboard-card">
          <h3>Média por UBS</h3>
          <div className="card-value">{formatCurrency(stats?.mediaDespesasPorUbs || 0)}</div>
          <p>Gasto médio por unidade</p>
        </div>
      </div>

      {/* Charts Section */}
      <div className="charts-section">
        <div className="chart-row">
          <div className="chart-card">
            <h3>Despesas por Mês</h3>
            <LineChartComponent
              data={stats?.despesasPorMes.map(item => ({
                name: item.mes,
                value: item.valor
              })) || []}
              height={300}
            />
          </div>

          <div className="chart-card">
            <h3>Status das Despesas</h3>
            <PieChartComponent
              data={[
                { name: 'Aprovadas', value: stats?.statusDespesas.aprovadas || 0 },
                { name: 'Pendentes', value: stats?.statusDespesas.pendentes || 0 },
                { name: 'Rejeitadas', value: stats?.statusDespesas.rejeitadas || 0 }
              ]}
              height={300}
            />
          </div>
        </div>

        <div className="chart-row">
          <div className="chart-card full-width">
            <h3>Top 5 Categorias de Despesa</h3>
            <BarChartComponent
              data={stats?.topDespesasPorCategoria.map(item => ({
                name: item.categoria,
                value: item.valor
              })) || []}
              height={350}
            />
          </div>
        </div>

        <div className="chart-row">
          <div className="chart-card full-width">
            <h3>Quantidade vs Valor por Mês</h3>
            <MultiBarChartComponent
              data={stats?.despesasPorMes.map(item => ({
                name: item.mes,
                value: item.valor,
                quantidade: item.quantidade
              })) || []}
              height={350}
            />
          </div>
        </div>
      </div>

      {/* System Information */}
      <div className="dashboard-info">
        <h2>Sistema de Gestão de Despesas - UBS</h2>
        <p>
          O InovaSaúde é uma plataforma completa para gerenciamento eficiente de despesas
          das Unidades Básicas de Saúde. Nosso sistema oferece ferramentas avançadas para
          controle financeiro, geração de relatórios e análise de dados.
        </p>

        <div className="info-grid">
          <div className="info-item">
            <h4>📊 Dashboard Interativo</h4>
            <p>Visualize dados em tempo real com gráficos dinâmicos e métricas atualizadas.</p>
          </div>

          <div className="info-item">
            <h4>🔐 Controle de Acesso</h4>
            <p>Sistema de permissões granular para diferentes níveis de usuário.</p>
          </div>

          <div className="info-item">
            <h4>📈 Relatórios Avançados</h4>
            <p>Análises detalhadas com filtros customizáveis e exportação múltipla.</p>
          </div>
        </div>

        <div style={{
          marginTop: '2rem',
          padding: '1.5rem',
          background: 'var(--gray-50)',
          borderRadius: '0.5rem',
          border: '1px solid var(--gray-200)'
        }}>
          <h4 style={{ margin: '0 0 0.5rem 0', color: 'var(--gray-900)' }}>
            💡 Última Atualização
          </h4>
          <p style={{ margin: 0, color: 'var(--gray-600)' }}>
            Dados atualizados em {format(new Date(), 'dd/MM/yyyy HH:mm', { locale: ptBR })}.
            Use os filtros acima para visualizar dados de períodos específicos.
          </p>
        </div>
      </div>
    </div>
  );
};
