import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Loading from '../components/Loading/index';
import { Despesa } from '../types';

const Despesas: React.FC = () => {
  const [despesas, setDespesas] = useState<Despesa[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [limit] = useState(10);
  const [total, setTotal] = useState(0);
  const [statusFilter, setStatusFilter] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingDespesa, setEditingDespesa] = useState<Despesa | null>(null);
  const [formData, setFormData] = useState({
    descricao: '',
    valor: '',
    dataVencimento: '',
    categoriaId: '',
    tipo: 'VARIAVEL',
    ubsId: '',
    fornecedorId: '',
    observacoes: '',
  });

  useEffect(() => {
    carregarDespesas();
  }, [page, statusFilter]);

  const carregarDespesas = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const params = new URLSearchParams({
        page: page.toString(),
        limit: limit.toString(),
        ...(statusFilter && { status: statusFilter }),
      });

      const response = await axios.get(
        `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/despesas?${params}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setDespesas(response.data.data || []);
      setTotal(response.data.total || 0);
      setError(null);
    } catch (err: any) {
      setError(err.response?.data?.error || 'Erro ao carregar despesas');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('token');
      
      if (editingDespesa) {
        await axios.put(
          `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/despesas/${editingDespesa.id}`,
          formData,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      } else {
        await axios.post(
          `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/despesas`,
          formData,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      }

      setShowModal(false);
      setFormData({
        descricao: '',
        valor: '',
        dataVencimento: '',
        categoriaId: '',
        tipo: 'VARIAVEL',
        ubsId: '',
        fornecedorId: '',
        observacoes: '',
      });
      setEditingDespesa(null);
      carregarDespesas();
    } catch (err: any) {
      alert(err.response?.data?.error || 'Erro ao salvar despesa');
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja deletar esta despesa?')) return;

    try {
      const token = localStorage.getItem('token');
      await axios.delete(
        `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/despesas/${id}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );
      carregarDespesas();
    } catch (err: any) {
      alert(err.response?.data?.error || 'Erro ao deletar despesa');
    }
  };

  const handleEdit = (despesa: Despesa) => {
    setEditingDespesa(despesa);
    setFormData({
      descricao: despesa.descricao,
      valor: despesa.valor.toString(),
      dataVencimento: despesa.dataVencimento || '',
      categoriaId: despesa.categoriaId,
      tipo: despesa.tipo,
      ubsId: despesa.ubsId,
      fornecedorId: despesa.fornecedorId || '',
      observacoes: despesa.observacoes || '',
    });
    setShowModal(true);
  };

  const formatarMoeda = (valor: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  };

  if (loading) return <Loading />;

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="max-w-7xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-900">Gestão de Despesas</h1>
          <button
            onClick={() => {
              setEditingDespesa(null);
              setFormData({
                descricao: '',
                valor: '',
                dataVencimento: '',
                categoriaId: '',
                tipo: 'VARIAVEL',
                ubsId: '',
                fornecedorId: '',
                observacoes: '',
              });
              setShowModal(true);
            }}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg"
          >
            + Nova Despesa
          </button>
        </div>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        <div className="mb-4 flex gap-4">
          <select
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPage(1);
            }}
            className="px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">Todos os Status</option>
            <option value="PENDENTE">Pendente</option>
            <option value="APROVADA">Aprovada</option>
            <option value="PAGA">Paga</option>
            <option value="REJEITADA">Rejeitada</option>
            <option value="CANCELADA">Cancelada</option>
          </select>
        </div>

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Descrição
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Valor
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Tipo
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {despesas.map((despesa) => (
                <tr key={despesa.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {despesa.descricao}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {formatarMoeda(Number(despesa.valor))}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {despesa.tipo}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-semibold ${
                        despesa.status === 'PAGA'
                          ? 'bg-green-100 text-green-800'
                          : despesa.status === 'REJEITADA'
                          ? 'bg-red-100 text-red-800'
                          : despesa.status === 'PENDENTE'
                          ? 'bg-yellow-100 text-yellow-800'
                          : 'bg-blue-100 text-blue-800'
                      }`}
                    >
                      {despesa.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                    <button
                      onClick={() => handleEdit(despesa)}
                      className="text-blue-600 hover:text-blue-800"
                    >
                      Editar
                    </button>
                    <button
                      onClick={() => handleDelete(despesa.id)}
                      className="text-red-600 hover:text-red-800"
                    >
                      Deletar
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        <div className="mt-4 flex justify-between items-center">
          <div className="text-sm text-gray-600">
            Mostrando {(page - 1) * limit + 1} a {Math.min(page * limit, total)} de {total}
          </div>
          <div className="space-x-2">
            <button
              onClick={() => setPage(Math.max(1, page - 1))}
              disabled={page === 1}
              className="px-4 py-2 border rounded disabled:opacity-50"
            >
              Anterior
            </button>
            <button
              onClick={() => setPage(page + 1)}
              disabled={page * limit >= total}
              className="px-4 py-2 border rounded disabled:opacity-50"
            >
              Próxima
            </button>
          </div>
        </div>

        {/* Modal */}
        {showModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
            <div className="bg-white rounded-lg p-8 max-w-md w-full">
              <h2 className="text-2xl font-bold mb-4">
                {editingDespesa ? 'Editar Despesa' : 'Nova Despesa'}
              </h2>
              <form onSubmit={handleSubmit}>
                <div className="space-y-4">
                  <input
                    type="text"
                    placeholder="Descrição"
                    value={formData.descricao}
                    onChange={(e) =>
                      setFormData({ ...formData, descricao: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                    required
                  />
                  <input
                    type="number"
                    placeholder="Valor"
                    step="0.01"
                    value={formData.valor}
                    onChange={(e) =>
                      setFormData({ ...formData, valor: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                    required
                  />
                  <input
                    type="date"
                    value={formData.dataVencimento}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        dataVencimento: e.target.value,
                      })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                  <select
                    value={formData.tipo}
                    onChange={(e) =>
                      setFormData({ ...formData, tipo: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  >
                    <option value="FIXA">Fixa</option>
                    <option value="VARIAVEL">Variável</option>
                    <option value="EVENTUAL">Eventual</option>
                  </select>
                  <textarea
                    placeholder="Observações"
                    value={formData.observacoes}
                    onChange={(e) =>
                      setFormData({ ...formData, observacoes: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                </div>
                <div className="mt-6 space-x-3">
                  <button
                    type="submit"
                    className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
                  >
                    Salvar
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowModal(false)}
                    className="bg-gray-300 text-gray-800 px-4 py-2 rounded-lg hover:bg-gray-400"
                  >
                    Cancelar
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Despesas;

