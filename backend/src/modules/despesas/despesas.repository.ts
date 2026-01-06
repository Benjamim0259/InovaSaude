import prisma from '../../config/database';
import { Prisma } from '@prisma/client';

export class DespesasRepository {
  async create(data: Prisma.DespesaCreateInput) {
    return await prisma.despesa.create({
      data,
      include: {
        categoria: true,
        ubs: true,
        fornecedor: true,
        usuarioCriacao: {
          select: {
            id: true,
            nome: true,
            email: true,
          },
        },
      },
    });
  }

  async findById(id: string) {
    return await prisma.despesa.findUnique({
      where: { id },
      include: {
        categoria: true,
        ubs: true,
        fornecedor: true,
        usuarioCriacao: {
          select: {
            id: true,
            nome: true,
            email: true,
          },
        },
        usuarioAprovacao: {
          select: {
            id: true,
            nome: true,
            email: true,
          },
        },
        anexos: true,
        historicoStatus: {
          orderBy: {
            createdAt: 'desc',
          },
        },
      },
    });
  }

  async findAll(filters: {
    page?: number;
    limit?: number;
    ubsId?: string;
    categoriaId?: string;
    fornecedorId?: string;
    status?: string;
    dataInicio?: Date;
    dataFim?: Date;
  }) {
    const { page = 1, limit = 10, ...where } = filters;
    const skip = (page - 1) * limit;

    const whereClause: any = {};

    if (where.ubsId) whereClause.ubsId = where.ubsId;
    if (where.categoriaId) whereClause.categoriaId = where.categoriaId;
    if (where.fornecedorId) whereClause.fornecedorId = where.fornecedorId;
    if (where.status) whereClause.status = where.status;

    if (where.dataInicio || where.dataFim) {
      whereClause.dataVencimento = {};
      if (where.dataInicio) whereClause.dataVencimento.gte = where.dataInicio;
      if (where.dataFim) whereClause.dataVencimento.lte = where.dataFim;
    }

    const [despesas, total] = await Promise.all([
      prisma.despesa.findMany({
        where: whereClause,
        skip,
        take: limit,
        include: {
          categoria: true,
          ubs: {
            select: {
              id: true,
              nome: true,
              codigo: true,
            },
          },
          fornecedor: {
            select: {
              id: true,
              razaoSocial: true,
              cnpj: true,
            },
          },
          usuarioCriacao: {
            select: {
              id: true,
              nome: true,
              email: true,
            },
          },
        },
        orderBy: {
          createdAt: 'desc',
        },
      }),
      prisma.despesa.count({ where: whereClause }),
    ]);

    return {
      despesas,
      total,
      page,
      limit,
      totalPages: Math.ceil(total / limit),
    };
  }

  async update(id: string, data: Prisma.DespesaUpdateInput) {
    return await prisma.despesa.update({
      where: { id },
      data,
      include: {
        categoria: true,
        ubs: true,
        fornecedor: true,
      },
    });
  }

  async delete(id: string) {
    return await prisma.despesa.delete({
      where: { id },
    });
  }

  async createHistorico(data: {
    despesaId: string;
    statusAnterior?: string;
    statusNovo: string;
    usuarioId?: string;
    observacao?: string;
  }) {
    return await prisma.historicoDespesa.create({
      data: data as any,
    });
  }
}
