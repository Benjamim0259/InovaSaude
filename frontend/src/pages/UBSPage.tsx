import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Loading from '../components/Loading/index';
import { UBS } from '../types';

const UBSPage: React.FC = () => {
  const [ubsList, setUbsList] = useState<UBS[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [limit] = useState(10);
  const [total, setTotal] = useState(0);
  const [showModal, setShowModal] = useState(false);
  const [editingUbs, setEditingUbs] = useState<UBS | null>(null);
  const [formData, setFormData] = useState({
    nome: '',
    codigo: '',
    endereco: '',
    bairro: '',
    cep: '',
    telefone: '',
    email: '',
    status: 'ATIVA',
    capacidadeAtendimento: '',
    observacoes: '',
  });

  useEffect(() => {
    carregarUbs();
  }, [page]);

  const carregarUbs = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const response = await axios.get(
        `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/ubs?page=${page}&limit=${limit}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );

      setUbsList(response.data.data || []);
      setTotal(response.data.total || 0);
      setError(null);
    } catch (err: any) {
      setError(err.response?.data?.error || 'Erro ao carregar UBS');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('token');

      if (editingUbs) {
        await axios.put(
          `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/ubs/${editingUbs.id}`,
          formData,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      } else {
        await axios.post(
          `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/ubs`,
          formData,
          { headers: { Authorization: `Bearer ${token}` } }
        );
      }

      setShowModal(false);
      setFormData({
        nome: '',
        codigo: '',
        endereco: '',
        bairro: '',
        cep: '',
        telefone: '',
        email: '',
        status: 'ATIVA',
        capacidadeAtendimento: '',
        observacoes: '',
      });
      setEditingUbs(null);
      carregarUbs();
    } catch (err: any) {
      alert(err.response?.data?.error || 'Erro ao salvar UBS');
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja deletar esta UBS?')) return;

    try {
      const token = localStorage.getItem('token');
      await axios.delete(
        `${process.env.REACT_APP_API_URL || 'http://localhost:4000'}/api/ubs/${id}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );
      carregarUbs();
    } catch (err: any) {
      alert(err.response?.data?.error || 'Erro ao deletar UBS');
    }
  };

  const handleEdit = (ubs: UBS) => {
    setEditingUbs(ubs);
    setFormData({
      nome: ubs.nome,
      codigo: ubs.cnes || '',
      endereco: ubs.endereco || '',
      bairro: '',
      cep: '',
      telefone: ubs.telefone || '',
      email: '',
      status: 'ATIVA',
      capacidadeAtendimento: '',
      observacoes: '',
    });
    setShowModal(true);
  };

  if (loading) return <Loading />;

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="max-w-7xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-900">Gestão de UBS</h1>
          <button
            onClick={() => {
              setEditingUbs(null);
              setFormData({
                nome: '',
                codigo: '',
                endereco: '',
                bairro: '',
                cep: '',
                telefone: '',
                email: '',
                status: 'ATIVA',
                capacidadeAtendimento: '',
                observacoes: '',
              });
              setShowModal(true);
            }}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg"
          >
            + Nova UBS
          </button>
        </div>

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
          </div>
        )}

        <div className="bg-white shadow rounded-lg overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Nome
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Código
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  Telefone
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
              {ubsList.map((ubs) => (
                <tr key={ubs.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {ubs.nome}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {ubs.cnes}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {ubs.telefone}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    <span className="px-2 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">
                      Ativa
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                    <button
                      onClick={() => handleEdit(ubs)}
                      className="text-blue-600 hover:text-blue-800"
                    >
                      Editar
                    </button>
                    <button
                      onClick={() => handleDelete(ubs.id)}
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
            <div className="bg-white rounded-lg p-8 max-w-md w-full max-h-screen overflow-y-auto">
              <h2 className="text-2xl font-bold mb-4">
                {editingUbs ? 'Editar UBS' : 'Nova UBS'}
              </h2>
              <form onSubmit={handleSubmit}>
                <div className="space-y-4">
                  <input
                    type="text"
                    placeholder="Nome da UBS"
                    value={formData.nome}
                    onChange={(e) =>
                      setFormData({ ...formData, nome: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                    required
                  />
                  <input
                    type="text"
                    placeholder="Código (CNES)"
                    value={formData.codigo}
                    onChange={(e) =>
                      setFormData({ ...formData, codigo: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                    required
                  />
                  <input
                    type="text"
                    placeholder="Endereço"
                    value={formData.endereco}
                    onChange={(e) =>
                      setFormData({ ...formData, endereco: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                  <input
                    type="text"
                    placeholder="Bairro"
                    value={formData.bairro}
                    onChange={(e) =>
                      setFormData({ ...formData, bairro: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                  <input
                    type="text"
                    placeholder="CEP"
                    value={formData.cep}
                    onChange={(e) =>
                      setFormData({ ...formData, cep: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                  <input
                    type="tel"
                    placeholder="Telefone"
                    value={formData.telefone}
                    onChange={(e) =>
                      setFormData({ ...formData, telefone: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                  <input
                    type="email"
                    placeholder="Email"
                    value={formData.email}
                    onChange={(e) =>
                      setFormData({ ...formData, email: e.target.value })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
                  <input
                    type="number"
                    placeholder="Capacidade de Atendimento"
                    value={formData.capacidadeAtendimento}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        capacidadeAtendimento: e.target.value,
                      })
                    }
                    className="w-full px-3 py-2 border rounded-lg"
                  />
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

export default UBSPage;
