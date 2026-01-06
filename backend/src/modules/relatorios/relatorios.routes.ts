import { Router } from 'express';
import { authMiddleware } from '../../shared/middlewares/auth.middleware';
import prisma from '../../config/database';
import { Prisma } from '@prisma/client';

const router = Router();

// Todas as rotas de relatórios requerem autenticação
router.use(authMiddleware);

// Dashboard - Visão geral
router.get('/dashboard', async (req, res, next) => {
  try {
    const { ubsId, dataInicio, dataFim } = req.query;

    const whereClause: any = {};
    if (ubsId) whereClause.ubsId = ubsId;
    if (dataInicio || dataFim) {
      whereClause.dataVencimento = {};
      if (dataInicio) whereClause.dataVencimento.gte = new Date(dataInicio as string);
      if (dataFim) whereClause.dataVencimento.lte = new Date(dataFim as string);
    }

    // Total de despesas por status
    const despesasPorStatus = await prisma.despesa.groupBy({
      by: ['status'],
      where: whereClause,
      _sum: {
        valor: true,
      },
      _count: {
        id: true,
      },
    });

    // Total geral
    const totalGeral = await prisma.despesa.aggregate({
      where: whereClause,
      _sum: {
        valor: true,
      },
      _count: {
        id: true,
      },
    });

    // Despesas por categoria
    const despesasPorCategoria = await prisma.despesa.groupBy({
      by: ['categoriaId'],
      where: whereClause,
      _sum: {
        valor: true,
      },
      _count: {
        id: true,
      },
    });

    const categorias = await prisma.categoria.findMany({
      where: {
        id: {
          in: despesasPorCategoria.map((d) => d.categoriaId),
        },
      },
    });

    const despesasPorCategoriaComNome = despesasPorCategoria.map((d) => ({
      ...d,
      categoria: categorias.find((c) => c.id === d.categoriaId),
    }));

    res.json({
      totalGeral,
      despesasPorStatus,
      despesasPorCategoria: despesasPorCategoriaComNome,
    });
  } catch (error) {
    next(error);
  }
});

// Gastos por UBS
router.get('/gastos-por-ubs', async (req, res, next) => {
  try {
    const { dataInicio, dataFim } = req.query;

    const whereClause: any = {};
    if (dataInicio || dataFim) {
      whereClause.dataVencimento = {};
      if (dataInicio) whereClause.dataVencimento.gte = new Date(dataInicio as string);
      if (dataFim) whereClause.dataVencimento.lte = new Date(dataFim as string);
    }

    const gastosPorUBS = await prisma.despesa.groupBy({
      by: ['ubsId'],
      where: whereClause,
      _sum: {
        valor: true,
      },
      _count: {
        id: true,
      },
    });

    const ubs = await prisma.uBS.findMany({
      where: {
        id: {
          in: gastosPorUBS.map((g) => g.ubsId),
        },
      },
    });

    const resultado = gastosPorUBS.map((g) => ({
      ...g,
      ubs: ubs.find((u) => u.id === g.ubsId),
    }));

    res.json(resultado);
  } catch (error) {
    next(error);
  }
});

// Gastos por categoria
router.get('/gastos-por-categoria', async (req, res, next) => {
  try {
    const { ubsId, dataInicio, dataFim } = req.query;

    const whereClause: any = {};
    if (ubsId) whereClause.ubsId = ubsId;
    if (dataInicio || dataFim) {
      whereClause.dataVencimento = {};
      if (dataInicio) whereClause.dataVencimento.gte = new Date(dataInicio as string);
      if (dataFim) whereClause.dataVencimento.lte = new Date(dataFim as string);
    }

    const gastosPorCategoria = await prisma.despesa.groupBy({
      by: ['categoriaId'],
      where: whereClause,
      _sum: {
        valor: true,
      },
      _count: {
        id: true,
      },
    });

    const categorias = await prisma.categoria.findMany({
      where: {
        id: {
          in: gastosPorCategoria.map((g) => g.categoriaId),
        },
      },
    });

    const resultado = gastosPorCategoria.map((g) => ({
      ...g,
      categoria: categorias.find((c) => c.id === g.categoriaId),
    }));

    res.json(resultado);
  } catch (error) {
    next(error);
  }
});

// Comparativo mensal
router.get('/comparativo-mensal', async (req, res, next) => {
  try {
    const { ubsId, ano } = req.query;
    const anoNum = ano ? parseInt(ano as string) : new Date().getFullYear();

    const whereClause: any = {
      dataVencimento: {
        gte: new Date(`${anoNum}-01-01`),
        lte: new Date(`${anoNum}-12-31`),
      },
    };

    if (ubsId) whereClause.ubsId = ubsId;

    const despesas = await prisma.despesa.findMany({
      where: whereClause,
      select: {
        valor: true,
        dataVencimento: true,
      },
    });

    // Agrupa por mês
    const gastosPorMes = Array.from({ length: 12 }, (_, i) => ({
      mes: i + 1,
      total: 0,
      quantidade: 0,
    }));

    despesas.forEach((despesa) => {
      if (despesa.dataVencimento) {
        const mes = despesa.dataVencimento.getMonth();
        gastosPorMes[mes].total += Number(despesa.valor);
        gastosPorMes[mes].quantidade += 1;
      }
    });

    res.json(gastosPorMes);
  } catch (error) {
    next(error);
  }
});

export default router;
