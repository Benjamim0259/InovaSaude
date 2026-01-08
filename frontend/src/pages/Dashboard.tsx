import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useLoading } from '../contexts/LoadingContext';
import { useNavigate } from 'react-router-dom';
import { SkeletonCard, SkeletonTable } from '../components/Loading/Loading';

interface DashboardStats {
  totalDespesas: number;
  despesasPendentes: number;
  despesasAprovadas: number;
  ubsAtivas: number;
}

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const { setIsLoading } = useLoading();
  const navigate = useNavigate();
  const [stats, setStats] = useState<DashboardStats>({
    totalDespesas: 0,
    despesasPendentes: 0,
    despesasAprovadas: 0,
    ubsAtivas: 0,
  });
  const [isLoadingStats, setIsLoadingStats] = useState(true);

  // Carrega dados do dashboard
  useEffect(() => {
    const loadStats = async () => {
      try {
        setIsLoadingStats(true);
        setIsLoading(true);

        // Simula delay da API (remover em produção)
        await new Promise(resolve => setTimeout(resolve, 1500));

        // TODO: Substituir com chamadas reais da API
        setStats({
          totalDespesas: 25450.00,
          despesasPendentes: 12,
          despesasAprovadas: 45,
          ubsAtivas: 8,
        });
      } catch (error) {
        console.error('Erro ao carregar estatísticas:', error);
      } finally {
        setIsLoadingStats(false);
        setIsLoading(false);
      }
    };

    loadStats();
  }, [setIsLoading]);

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const StatCard: React.FC<{
    icon: string;
    title: string;
    value: string | number;
    isLoading?: boolean;
  }> = ({ icon, title, value, isLoading: loading = false }) => {
    if (loading) return <SkeletonCard />;

    return (
      <div className="bg-white overflow-hidden shadow rounded-lg hover:shadow-lg transition-shadow">
        <div className="p-5">
          <div className="flex items-center">
            <div className="flex-shrink-0">
              <div className="text-3xl">{icon}</div>
            </div>
            <div className="ml-5 w-0 flex-1">
              <dl>
                <dt className="text-sm font-medium text-gray-500 truncate">
                  {title}
                </dt>
                <dd className="text-lg font-semibold text-gray-900">
                  {value}
                </dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-blue-600">InovaSaúde</h1>
            <p className="text-sm text-gray-500 mt-1">Sistema de Análise de Gastos por UBS</p>
          </div>
          <div className="flex items-center gap-4">
            <div className="text-right">
              <p className="text-sm font-medium text-gray-900">{user?.nome}</p>
              <p className="text-xs text-gray-500">{user?.roles?.join(', ') || 'Usuário'}</p>
            </div>
            <button
              onClick={handleLogout}
              className="px-3 py-2 rounded-md text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 transition-colors"
            >
              Sair
            </button>
          </div>
        </div>
      </header>

      {/* Navigation */}
      <nav className="bg-white border-b border-gray-200 sticky top-0 z-40">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex space-x-8">
            <button
              onClick={() => navigate('/dashboard')}
              className="border-b-2 border-blue-600 py-4 px-1 text-sm font-medium text-blue-600 hover:text-blue-700"
            >
              Dashboard
            </button>
            <button
              onClick={() => navigate('/despesas')}
              className="border-b-2 border-transparent py-4 px-1 text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300 transition-colors"
            >
              Despesas
            </button>
            <button
              onClick={() => navigate('/ubs')}
              className="border-b-2 border-transparent py-4 px-1 text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300 transition-colors"
            >
              UBS
            </button>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Statistics Cards */}
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4 mb-8">
          <StatCard
            icon="💰"
            title="Total de Despesas"
            value={`R$ ${stats.totalDespesas.toLocaleString('pt-BR', { 
              minimumFractionDigits: 2, 
              maximumFractionDigits: 2 
            })}`}
            isLoading={isLoadingStats}
          />
          <StatCard
            icon="⏳"
            title="Despesas Pendentes"
            value={stats.despesasPendentes}
            isLoading={isLoadingStats}
          />
          <StatCard
            icon="✅"
            title="Despesas Aprovadas"
            value={stats.despesasAprovadas}
            isLoading={isLoadingStats}
          />
          <StatCard
            icon="🏥"
            title="UBS Ativas"
            value={stats.ubsAtivas}
            isLoading={isLoadingStats}
          />
        </div>

        {/* Charts Section */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
          {/* Gastos por Categoria */}
          <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">
              Gastos por Categoria
            </h2>
            {isLoadingStats ? (
              <div className="h-64 bg-gray-100 rounded-lg animate-pulse flex items-center justify-center">
                <p className="text-gray-400">Carregando gráfico...</p>
              </div>
            ) : (
              <div className="h-64 flex items-center justify-center bg-gray-50 rounded-lg border border-dashed border-gray-300">
                <p className="text-gray-400">Gráfico será implementado aqui</p>
              </div>
            )}
          </div>

          {/* Tendência Mensal */}
          <div className="bg-white shadow rounded-lg p-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">
              Tendência de Gastos
            </h2>
            {isLoadingStats ? (
              <div className="h-64 bg-gray-100 rounded-lg animate-pulse flex items-center justify-center">
                <p className="text-gray-400">Carregando gráfico...</p>
              </div>
            ) : (
              <div className="h-64 flex items-center justify-center bg-gray-50 rounded-lg border border-dashed border-gray-300">
                <p className="text-gray-400">Gráfico será implementado aqui</p>
              </div>
            )}
          </div>
        </div>

        {/* Recent Activity */}
        <div className="bg-white shadow rounded-lg p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            Atividade Recente
          </h2>
          {isLoadingStats ? (
            <SkeletonTable />
          ) : (
            <div className="text-center py-8 text-gray-400">
              <p>Nenhuma atividade recente</p>
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default Dashboard;
