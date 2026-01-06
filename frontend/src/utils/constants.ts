/**
 * Constantes do sistema
 */

// Status de Despesas
export const DESPESA_STATUS = {
  PENDENTE: 'Pendente',
  APROVADA: 'Aprovada',
  PAGA: 'Paga',
  REJEITADA: 'Rejeitada',
  CANCELADA: 'Cancelada',
} as const;

// Cores para status de despesas
export const DESPESA_STATUS_COLORS = {
  PENDENTE: 'bg-yellow-100 text-yellow-800',
  APROVADA: 'bg-green-100 text-green-800',
  PAGA: 'bg-blue-100 text-blue-800',
  REJEITADA: 'bg-red-100 text-red-800',
  CANCELADA: 'bg-gray-100 text-gray-800',
} as const;

// Tipos de Despesas
export const DESPESA_TIPOS = {
  FIXA: 'Fixa',
  VARIAVEL: 'Variável',
  EVENTUAL: 'Eventual',
} as const;

// Perfis de Usuário
export const PERFIS_USUARIO = {
  ADMIN: 'Administrador',
  COORDENADOR: 'Coordenador',
  GESTOR: 'Gestor',
  AUDITOR: 'Auditor',
} as const;

// Status de UBS
export const UBS_STATUS = {
  ATIVA: 'Ativa',
  INATIVA: 'Inativa',
  EM_MANUTENCAO: 'Em Manutenção',
} as const;

// Tipos de Categoria
export const CATEGORIA_TIPOS = {
  PESSOAL: 'Pessoal',
  MATERIAL: 'Material',
  SERVICO: 'Serviço',
  EQUIPAMENTO: 'Equipamento',
  INFRAESTRUTURA: 'Infraestrutura',
  OUTROS: 'Outros',
} as const;

// Configurações de paginação
export const PAGINATION = {
  DEFAULT_PAGE: 1,
  DEFAULT_LIMIT: 10,
  MAX_LIMIT: 100,
} as const;

// Limites de arquivo
export const FILE_UPLOAD = {
  MAX_SIZE: 10 * 1024 * 1024, // 10MB
  ALLOWED_TYPES: ['image/jpeg', 'image/png', 'application/pdf', 'text/csv', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'],
} as const;

// Mensagens de erro
export const ERROR_MESSAGES = {
  GENERIC: 'Ocorreu um erro. Tente novamente.',
  NETWORK: 'Erro de conexão. Verifique sua internet.',
  UNAUTHORIZED: 'Você não tem permissão para acessar este recurso.',
  NOT_FOUND: 'Recurso não encontrado.',
  VALIDATION: 'Dados inválidos. Verifique os campos.',
} as const;
