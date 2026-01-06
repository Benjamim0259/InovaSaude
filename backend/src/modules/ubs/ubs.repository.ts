import prisma from '../../config/database';
import { Prisma } from '@prisma/client';

export class UBSRepository {
  async create(data: Prisma.UBSCreateInput) {
    return await prisma.uBS.create({
      data,
      include: {
        coordenador: {
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
    return await prisma.uBS.findUnique({
      where: { id },
      include: {
        coordenador: {
          select: {
            id: true,
            nome: true,
            email: true,
            telefone: true,
          },
        },
        usuarios: {
          select: {
            id: true,
            nome: true,
            email: true,
            perfil: true,
          },
        },
        _count: {
          select: {
            despesas: true,
          },
        },
      },
    });
  }

  async findAll(filters?: { page?: number; limit?: number; status?: string }) {
    const { page = 1, limit = 10, status } = filters || {};
    const skip = (page - 1) * limit;

    const whereClause: any = {};
    if (status) whereClause.status = status;

    const [ubs, total] = await Promise.all([
      prisma.uBS.findMany({
        where: whereClause,
        skip,
        take: limit,
        include: {
          coordenador: {
            select: {
              id: true,
              nome: true,
              email: true,
            },
          },
          _count: {
            select: {
              despesas: true,
              usuarios: true,
            },
          },
        },
        orderBy: {
          nome: 'asc',
        },
      }),
      prisma.uBS.count({ where: whereClause }),
    ]);

    return {
      ubs,
      total,
      page,
      limit,
      totalPages: Math.ceil(total / limit),
    };
  }

  async update(id: string, data: Prisma.UBSUpdateInput) {
    return await prisma.uBS.update({
      where: { id },
      data,
      include: {
        coordenador: {
          select: {
            id: true,
            nome: true,
            email: true,
          },
        },
      },
    });
  }

  async delete(id: string) {
    return await prisma.uBS.delete({
      where: { id },
    });
  }

  async findByCodigo(codigo: string) {
    return await prisma.uBS.findUnique({
      where: { codigo },
    });
  }
}
