import { Router } from 'express';
import { authMiddleware, authorize } from '../../shared/middlewares/auth.middleware';
import prisma from '../../config/database';
import bcrypt from 'bcrypt';
import { config } from '../../config';
import { AppError } from '../../shared/middlewares/error.middleware';

const router = Router();

// Todas as rotas de usuários requerem autenticação
router.use(authMiddleware);

// Listar usuários (apenas ADMIN)
router.get('/', authorize('ADMIN'), async (req, res, next) => {
  try {
    const { page = 1, limit = 10, perfil, status } = req.query;
    const skip = (Number(page) - 1) * Number(limit);

    const whereClause: any = {};
    if (perfil) whereClause.perfil = perfil;
    if (status) whereClause.status = status;

    const [usuarios, total] = await Promise.all([
      prisma.usuario.findMany({
        where: whereClause,
        skip,
        take: Number(limit),
        select: {
          id: true,
          nome: true,
          email: true,
          perfil: true,
          status: true,
          telefone: true,
          ubsId: true,
          ultimoAcesso: true,
          createdAt: true,
          ubs: {
            select: {
              id: true,
              nome: true,
              codigo: true,
            },
          },
        },
        orderBy: {
          nome: 'asc',
        },
      }),
      prisma.usuario.count({ where: whereClause }),
    ]);

    res.json({
      usuarios,
      total,
      page: Number(page),
      limit: Number(limit),
      totalPages: Math.ceil(total / Number(limit)),
    });
  } catch (error) {
    next(error);
  }
});

// Buscar usuário por ID
router.get('/:id', authorize('ADMIN', 'GESTOR'), async (req, res, next) => {
  try {
    const { id } = req.params;

    const usuario = await prisma.usuario.findUnique({
      where: { id },
      select: {
        id: true,
        nome: true,
        email: true,
        perfil: true,
        status: true,
        telefone: true,
        ubsId: true,
        ultimoAcesso: true,
        createdAt: true,
        updatedAt: true,
        ubs: {
          select: {
            id: true,
            nome: true,
            codigo: true,
          },
        },
      },
    });

    if (!usuario) {
      throw new AppError('Usuário não encontrado', 404);
    }

    res.json(usuario);
  } catch (error) {
    next(error);
  }
});

// Criar usuário (apenas ADMIN)
router.post('/', authorize('ADMIN'), async (req, res, next) => {
  try {
    const { nome, email, senha, perfil, ubsId, telefone } = req.body;

    const usuarioExistente = await prisma.usuario.findUnique({
      where: { email },
    });

    if (usuarioExistente) {
      throw new AppError('Email já cadastrado', 400);
    }

    const senhaHash = await bcrypt.hash(senha, config.bcrypt.rounds);

    const usuario = await prisma.usuario.create({
      data: {
        nome,
        email,
        senhaHash,
        perfil,
        telefone,
        ubsId,
      },
      select: {
        id: true,
        nome: true,
        email: true,
        perfil: true,
        status: true,
        telefone: true,
        ubsId: true,
        ubs: {
          select: {
            id: true,
            nome: true,
            codigo: true,
          },
        },
      },
    });

    res.status(201).json(usuario);
  } catch (error) {
    next(error);
  }
});

// Atualizar usuário
router.put('/:id', authorize('ADMIN'), async (req, res, next) => {
  try {
    const { id } = req.params;
    const { nome, email, perfil, status, ubsId, telefone, senha } = req.body;

    const usuarioExistente = await prisma.usuario.findUnique({
      where: { id },
    });

    if (!usuarioExistente) {
      throw new AppError('Usuário não encontrado', 404);
    }

    const updateData: any = {};
    if (nome) updateData.nome = nome;
    if (email) updateData.email = email;
    if (perfil) updateData.perfil = perfil;
    if (status) updateData.status = status;
    if (telefone !== undefined) updateData.telefone = telefone;
    if (ubsId !== undefined) updateData.ubsId = ubsId;
    if (senha) {
      updateData.senhaHash = await bcrypt.hash(senha, config.bcrypt.rounds);
    }

    const usuario = await prisma.usuario.update({
      where: { id },
      data: updateData,
      select: {
        id: true,
        nome: true,
        email: true,
        perfil: true,
        status: true,
        telefone: true,
        ubsId: true,
        ubs: {
          select: {
            id: true,
            nome: true,
            codigo: true,
          },
        },
      },
    });

    res.json(usuario);
  } catch (error) {
    next(error);
  }
});

// Deletar usuário
router.delete('/:id', authorize('ADMIN'), async (req, res, next) => {
  try {
    const { id } = req.params;

    await prisma.usuario.delete({
      where: { id },
    });

    res.json({ message: 'Usuário deletado com sucesso' });
  } catch (error) {
    next(error);
  }
});

export default router;
