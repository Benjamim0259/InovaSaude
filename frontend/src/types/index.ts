export interface Usuario {
  id: string;
  nome: string;
  email: string;
  cpf?: string;
  roles: string[];
  municipioId?: string;
  municipioNome?: string;
  ubsId?: string;
  ubsNome?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: Usuario;
}

export interface UBS {
  id: string;
  nome: string;
  endereco?: string;
  telefone?: string;
  cnes?: string;
  municipioId: string;
  municipioNome: string;
  createdAt: string;
}

export interface CreateUBSDto {
  nome: string;
  endereco?: string;
  telefone?: string;
  cnes?: string;
  municipioId: string;
}

export type DespesaStatus = 'Pendente' | 'Aprovada' | 'Rejeitada';

export interface Despesa {
  id: string;
  valor: number;
  data: string;
  descricao: string;
  status: DespesaStatus;
  comprovanteUrl?: string;
  observacoes?: string;
  ubsId: string;
  ubsNome: string;
  categoriaId: string;
  categoriaNome: string;
  usuarioId: string;
  usuarioNome: string;
  createdAt: string;
}

export interface CreateDespesaDto {
  valor: number;
  data: string;
  descricao: string;
  observacoes?: string;
  ubsId: string;
  categoriaId: string;
}
