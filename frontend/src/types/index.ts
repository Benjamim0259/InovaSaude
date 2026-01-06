export interface Usuario {
  id: string;
  nome: string;
  email: string;
  perfil: 'ADMIN' | 'COORDENADOR' | 'GESTOR' | 'AUDITOR';
  status: 'ATIVO' | 'INATIVO' | 'BLOQUEADO';
  telefone?: string;
  ubsId?: string;
  ubs?: UBS;
  ultimoAcesso?: string;
  createdAt: string;
  updatedAt: string;
}

export interface UBS {
  id: string;
  nome: string;
  codigo: string;
  endereco?: string;
  bairro?: string;
  cep?: string;
  telefone?: string;
  email?: string;
  coordenadorId?: string;
  coordenador?: Usuario;
  status: 'ATIVA' | 'INATIVA' | 'EM_MANUTENCAO';
  capacidadeAtendimento?: number;
  observacoes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Despesa {
  id: string;
  descricao: string;
  valor: number;
  dataVencimento?: string;
  dataPagamento?: string;
  categoriaId: string;
  categoria?: Categoria;
  tipo: 'FIXA' | 'VARIAVEL' | 'EVENTUAL';
  status: 'PENDENTE' | 'APROVADA' | 'PAGA' | 'REJEITADA' | 'CANCELADA';
  ubsId: string;
  ubs?: UBS;
  fornecedorId?: string;
  fornecedor?: Fornecedor;
  usuarioCriacaoId: string;
  usuarioCriacao?: Usuario;
  usuarioAprovacaoId?: string;
  usuarioAprovacao?: Usuario;
  dataAprovacao?: string;
  observacoes?: string;
  numeroNota?: string;
  numeroEmpenho?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Fornecedor {
  id: string;
  razaoSocial: string;
  nomeFantasia?: string;
  cnpj: string;
  inscricaoEstadual?: string;
  endereco?: string;
  bairro?: string;
  cidade?: string;
  estado?: string;
  cep?: string;
  telefone?: string;
  email?: string;
  contato?: string;
  status: 'ATIVO' | 'INATIVO' | 'BLOQUEADO';
  observacoes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Categoria {
  id: string;
  nome: string;
  descricao?: string;
  tipo: 'PESSOAL' | 'MATERIAL' | 'SERVICO' | 'EQUIPAMENTO' | 'INFRAESTRUTURA' | 'OUTROS';
  orcamentoMensal?: number;
  cor?: string;
  icone?: string;
  createdAt: string;
  updatedAt: string;
}

export interface AuthResponse {
  token: string;
  usuario: Usuario;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}
