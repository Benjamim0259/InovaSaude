import { z } from 'zod';

export const createUBSSchema = z.object({
  body: z.object({
    nome: z.string().min(3, 'Nome deve ter no mínimo 3 caracteres'),
    codigo: z.string().min(2, 'Código deve ter no mínimo 2 caracteres'),
    endereco: z.string().optional(),
    bairro: z.string().optional(),
    cep: z.string().optional(),
    telefone: z.string().optional(),
    email: z.string().email().optional(),
    coordenadorId: z.string().uuid().optional(),
    capacidadeAtendimento: z.number().positive().optional(),
    observacoes: z.string().optional(),
  }),
});

export const updateUBSSchema = z.object({
  body: z.object({
    nome: z.string().min(3).optional(),
    codigo: z.string().min(2).optional(),
    endereco: z.string().optional(),
    bairro: z.string().optional(),
    cep: z.string().optional(),
    telefone: z.string().optional(),
    email: z.string().email().optional(),
    coordenadorId: z.string().uuid().optional(),
    status: z.enum(['ATIVA', 'INATIVA', 'EM_MANUTENCAO']).optional(),
    capacidadeAtendimento: z.number().positive().optional(),
    observacoes: z.string().optional(),
  }),
});
