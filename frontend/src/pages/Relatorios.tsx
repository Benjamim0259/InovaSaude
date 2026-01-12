import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Loading from '../components/Loading/index';

interface Dashboard {
  totalDespesas: number;
  despesasPorStatus: { status: string; total: number; count: number }[];
  despesasPorCategoria: { categoria: string; total: number; count: number }[];
  despesasPorUbs: { ubs: string; total: number; count: number }[];
}

const Relatorios: React.FC = () => {
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dataInicio, setDataInicio] = useState('');
  const [dataFim, setDataFim] = useState('');

  useEffect(() => {
    carregarDashboard();
  }, []);

  const carregarDashboard = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const params = new URLSearchParams();
      if (dataInicio) params.append('dataInicio', dataInicio);
      if (dataFim) params.append('dataFim', dataFim);

      const response = await axios.get(
        `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/relatorios/dashboard?${params}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setDashboard(response.data);
      setError(null);
    } catch (err: any) {
      setError(err.response?.data?.error || 'Erro ao carregar relatórios');
    } finally {
      setLoading(false);
    }
  };

  const formatarMoeda = (valor: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  };

  const handleFiltrar = (e: React.FormEvent) => {
    e.preventDefault();
    carregarDashboard();
  };

  if (loading) return <Loading />;

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-900 mb-6">Relatórios e Dashboards</h1>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        {/* Filtros */}
        <div className="bg-white shadow rounded-lg p-6 mb-6">
          <h2 className="text-xl font-bold mb-4">Filtros</h2>
          <form onSubmit={handleFiltrar} className="flex gap-4">
            <input
              type="date"
              value={dataInicio}
              onChange={(e) => setDataInicio(e.target.value)}
              className="px-4 py-2 border rounded-lg"
              placeholder="Data Início"
            />
            <input
              type="date"
              value={dataFim}
              onChange={(e) => setDataFim(e.target.value)}
              className="px-4 py-2 border rounded-lg"
              placeholder="Data Fim"
            />
            <button
              type="submit"
              className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-lg"
            >
              Filtrar
            </button>
          </form>
        </div>

        {dashboard && (
          <>
            {/* Cards Resumo */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
              <div className="bg-white shadow rounded-lg p-6">
                <h3 className="text-gray-500 text-sm font-medium">Total de Despesas</h3>
                <p className="text-3xl font-bold text-gray-900 mt-2">
                  {formatarMoeda(dashboard.totalDespesas)}
                </p>
              </div>

              {dashboard.despesasPorStatus.map((status) => (
                <div key={status.status} className="bg-white shadow rounded-lg p-6">
                  <h3 className="text-gray-500 text-sm font-medium">{status.status}</h3>
                  <p className="text-3xl font-bold text-gray-900 mt-2">
                    {formatarMoeda(Number(status.total))}
                  </p>
                  <p className="text-sm text-gray-400 mt-1">{status.count} despesa(s)</p>
                </div>
              ))}
            </div>

            {/* Tabelas */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Por Categoria */}
              <div className="bg-white shadow rounded-lg p-6">
                <h2 className="text-xl font-bold mb-4">Despesas por Categoria</h2>
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                        Categoria
                      </th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                        Total
                      </th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                        Qtd
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {dashboard.despesasPorCategoria.map((cat, idx) => (
                      <tr key={idx}>
                        <td className="px-4 py-2 text-sm text-gray-900">{cat.categoria}</td>
                        <td className="px-4 py-2 text-sm text-gray-500">
                          {formatarMoeda(Number(cat.total))}
                        </td>
                        <td className="px-4 py-2 text-sm text-gray-500">{cat.count}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {/* Por UBS */}
              <div className="bg-white shadow rounded-lg p-6">
                <h2 className="text-xl font-bold mb-4">Despesas por UBS</h2>
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                        UBS
                      </th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                        Total
                      </th>
                      <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">
                        Qtd
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {dashboard.despesasPorUbs.map((ubs, idx) => (
                      <tr key={idx}>
                        <td className="px-4 py-2 text-sm text-gray-900">{ubs.ubs}</td>
                        <td className="px-4 py-2 text-sm text-gray-500">
                          {formatarMoeda(Number(ubs.total))}
                        </td>
                        <td className="px-4 py-2 text-sm text-gray-500">{ubs.count}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Botões de Export */}
            <div className="mt-6 flex gap-4">
              <button
                onClick={() => alert('Export para Excel em desenvolvimento')}
                className="bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded-lg"
              >
                📊 Exportar para Excel
              </button>
              <button
                onClick={() => alert('Export para PDF em desenvolvimento')}
                className="bg-red-600 hover:bg-red-700 text-white px-6 py-2 rounded-lg"
              >
                📄 Exportar para PDF
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default Relatorios;
