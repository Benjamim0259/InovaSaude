/**
 * Tipos de resposta da API
 */

export interface ApiResponse<T = any> {
  data?: T;
  message?: string;
  error?: string;
  statusCode?: number;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

export interface ErrorResponse {
  error: string;
  statusCode: number;
  details?: any;
}

/**
 * Tipos de usuários
 */
export type PerfilUsuario = 'ADMIN' | 'COORDENADOR' | 'GESTOR' | 'AUDITOR';
export type StatusUsuario = 'ATIVO' | 'INATIVO' | 'BLOQUEADO';

/**
 * Tipos de despesas
 */
export type StatusDespesa = 'PENDENTE' | 'APROVADA' | 'PAGA' | 'REJEITADA' | 'CANCELADA';
export type TipoDespesa = 'FIXA' | 'VARIAVEL' | 'EVENTUAL';

/**
 * Tipos de UBS
 */
export type StatusUBS = 'ATIVA' | 'INATIVA' | 'EM_MANUTENCAO';

/**
 * Tipos de fornecedores
 */
export type StatusFornecedor = 'ATIVO' | 'INATIVO' | 'BLOQUEADO';

/**
 * Tipos de categorias
 */
export type TipoCategoria =
  | 'PESSOAL'
  | 'MATERIAL'
  | 'SERVICO'
  | 'EQUIPAMENTO'
  | 'INFRAESTRUTURA'
  | 'OUTROS';
