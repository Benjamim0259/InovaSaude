import { z } from 'zod';

export const createDespesaSchema = z.object({
  body: z.object({
    descricao: z.string().min(3, 'Descrição deve ter no mínimo 3 caracteres'),
    valor: z.number().positive('Valor deve ser positivo'),
    dataVencimento: z.string().datetime().optional(),
    dataPagamento: z.string().datetime().optional(),
    categoriaId: z.string().uuid('ID da categoria inválido'),
    tipo: z.enum(['FIXA', 'VARIAVEL', 'EVENTUAL']),
    ubsId: z.string().uuid('ID da UBS inválido'),
    fornecedorId: z.string().uuid().optional(),
    observacoes: z.string().optional(),
    numeroNota: z.string().optional(),
    numeroEmpenho: z.string().optional(),
  }),
});

export const updateDespesaSchema = z.object({
  body: z.object({
    descricao: z.string().min(3).optional(),
    valor: z.number().positive().optional(),
    dataVencimento: z.string().datetime().optional(),
    dataPagamento: z.string().datetime().optional(),
    categoriaId: z.string().uuid().optional(),
    tipo: z.enum(['FIXA', 'VARIAVEL', 'EVENTUAL']).optional(),
    fornecedorId: z.string().uuid().optional(),
    observacoes: z.string().optional(),
    numeroNota: z.string().optional(),
    numeroEmpenho: z.string().optional(),
    status: z.enum(['PENDENTE', 'APROVADA', 'PAGA', 'REJEITADA', 'CANCELADA']).optional(),
  }),
});

export const listDespesasSchema = z.object({
  query: z.object({
    page: z.string().optional(),
    limit: z.string().optional(),
    ubsId: z.string().uuid().optional(),
    categoriaId: z.string().uuid().optional(),
    fornecedorId: z.string().uuid().optional(),
    status: z.enum(['PENDENTE', 'APROVADA', 'PAGA', 'REJEITADA', 'CANCELADA']).optional(),
    dataInicio: z.string().datetime().optional(),
    dataFim: z.string().datetime().optional(),
  }),
});
