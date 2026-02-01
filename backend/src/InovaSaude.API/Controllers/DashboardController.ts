import { Router, Request, Response } from 'express';
import { PrismaClient } from '@prisma/client';
import { startOfMonth, endOfMonth, format, subMonths } from 'date-fns';

const prisma = new PrismaClient();
const router = Router();

// Obter estatísticas do dashboard
router.get('/stats', async (req: Request, res: Response) => {
  try {
    const {
      dataInicio,
      dataFim,
      ubsId,
      categoria
    } = req.query;

    // Filtros de data
    const startDate = dataInicio ? new Date(dataInicio as string) : subMonths(new Date(), 6);
    const endDate = dataFim ? new Date(dataFim as string) : new Date();

    // Construir filtros
    const whereDespesas: any = {
      createdAt: {
        gte: startDate,
        lte: endDate,
      },
    };

    const whereUbs: any = {};

    if (ubsId) {
      whereDespesas.ubsId = ubsId;
      whereUbs.id = ubsId;
    }

    if (categoria) {
      whereDespesas.categoria = {
        nome: { contains: categoria as string, mode: 'insensitive' }
      };
    }

    // Estatísticas básicas
    const [
      totalUbs,
      totalDespesas,
      despesasPendentes,
      valorTotalDespesas,
      mediaDespesasPorUbs,
      statusDespesas
    ] = await Promise.all([
      // Total de UBS
      prisma.uBS.count({ where: whereUbs }),

      // Total de despesas
      prisma.despesa.count({ where: whereDespesas }),

      // Despesas pendentes
      prisma.despesa.count({
        where: {
          ...whereDespesas,
          status: 'PENDENTE'
        }
      }),

      // Valor total das despesas
      prisma.despesa.aggregate({
        where: whereDespesas,
        _sum: { valor: true }
      }),

      // Média de despesas por UBS
      prisma.despesa.groupBy({
        by: ['ubsId'],
        where: {
          ...whereDespesas,
          ubsId: { not: null }
        },
        _sum: { valor: true }
      }).then(groups => {
        const total = groups.reduce((sum, group) => sum + (Number(group._sum?.valor) || 0), 0);
        return groups.length > 0 ? total / groups.length : 0;
      }),

      // Status das despesas
      prisma.despesa.groupBy({
        by: ['status'],
        where: whereDespesas,
        _count: true
      })
    ]);

    // Processar status das despesas
    const statusMap = {
      aprovadas: statusDespesas.find(s => s.status === 'APROVADA')?._count || 0,
      pendentes: statusDespesas.find(s => s.status === 'PENDENTE')?._count || 0,
      rejeitadas: statusDespesas.find(s => s.status === 'REJEITADA')?._count || 0,
    };

    // Top 5 categorias
    const topCategorias = await prisma.despesa.groupBy({
      by: ['categoriaId'],
      where: whereDespesas,
      _sum: { valor: true },
      _count: true,
      orderBy: { _sum: { valor: 'desc' } },
      take: 5,
    });

    const categoriaIds = topCategorias.map(c => c.categoriaId).filter(Boolean);
    const categorias = await prisma.categoria.findMany({
      where: { id: { in: categoriaIds } },
      select: { id: true, nome: true }
    });

    const categoriaMap = new Map(categorias.map(c => [c.id, c.nome]));

    const topDespesasPorCategoria = topCategorias.map(cat => ({
      categoria: categoriaMap.get(cat.categoriaId!) || 'Categoria não encontrada',
      valor: Number(cat._sum.valor) || 0,
      porcentagem: valorTotalDespesas._sum.valor ?
        ((Number(cat._sum.valor) || 0) / Number(valorTotalDespesas._sum.valor)) * 100 : 0
    }));

    // Despesas por mês (últimos 6 meses)
    const despesasPorMes = [];
    for (let i = 5; i >= 0; i--) {
      const mes = subMonths(new Date(), i);
      const inicioMes = startOfMonth(mes);
      const fimMes = endOfMonth(mes);

      const [valor, quantidade] = await Promise.all([
        prisma.despesa.aggregate({
          where: {
            ...whereDespesas,
            createdAt: {
              gte: inicioMes,
              lte: fimMes,
            },
          },
          _sum: { valor: true }
        }),
        prisma.despesa.count({
          where: {
            ...whereDespesas,
            createdAt: {
              gte: inicioMes,
              lte: fimMes,
            },
          }
        })
      ]);

      despesasPorMes.push({
        mes: format(mes, 'MMM', { locale: require('date-fns/locale/pt-BR') }),
        valor: Number(valor._sum.valor) || 0,
        quantidade
      });
    }

    const stats = {
      totalUbs,
      totalDespesas,
      despesasPendentes,
      relatoriosGerados: 0, // TODO: implementar contagem de relatórios
      valorTotalDespesas: Number(valorTotalDespesas._sum.valor) || 0,
      mediaDespesasPorUbs,
      topDespesasPorCategoria,
      despesasPorMes,
      statusDespesas: statusMap
    };

    res.json({
      success: true,
      data: stats
    });
  } catch (error) {
    console.error('Erro ao obter estatísticas do dashboard:', error);
    res.status(500).json({
      success: false,
      message: 'Erro interno do servidor'
    });
  }
});

// Despesas por categoria
router.get('/despesas-por-categoria', async (req: Request, res: Response) => {
  try {
    const { dataInicio, dataFim, ubsId } = req.query;

    const where: any = {};
    if (dataInicio) where.createdAt = { gte: new Date(dataInicio as string) };
    if (dataFim) where.createdAt = { ...where.createdAt, lte: new Date(dataFim as string) };
    if (ubsId) where.ubsId = ubsId;

    const despesasPorCategoria = await prisma.despesa.groupBy({
      by: ['categoriaId'],
      where,
      _sum: { valor: true },
      _count: true,
      orderBy: { _sum: { valor: 'desc' } },
    });

    const categoriaIds = despesasPorCategoria.map(d => d.categoriaId).filter(Boolean);
    const categorias = await prisma.categoria.findMany({
      where: { id: { in: categoriaIds } },
      select: { id: true, nome: true }
    });

    const categoriaMap = new Map(categorias.map(c => [c.id, c.nome]));

    const result = despesasPorCategoria.map(item => ({
      categoria: categoriaMap.get(item.categoriaId!) || 'Categoria não encontrada',
      valor: Number(item._sum.valor) || 0,
      quantidade: item._count
    }));

    res.json({
      success: true,
      data: result
    });
  } catch (error) {
    console.error('Erro ao obter despesas por categoria:', error);
    res.status(500).json({
      success: false,
      message: 'Erro interno do servidor'
    });
  }
});

// Despesas por mês
router.get('/despesas-por-mes', async (req: Request, res: Response) => {
  try {
    const { dataInicio, dataFim, ubsId } = req.query;

    const startDate = dataInicio ? new Date(dataInicio as string) : subMonths(new Date(), 6);
    const endDate = dataFim ? new Date(dataFim as string) : new Date();

    const where: any = {
      createdAt: {
        gte: startDate,
        lte: endDate,
      },
    };

    if (ubsId) where.ubsId = ubsId;

    const despesasPorMes = [];
    let currentDate = new Date(startDate);

    while (currentDate <= endDate) {
      const inicioMes = startOfMonth(currentDate);
      const fimMes = endOfMonth(currentDate);

      const [valor, quantidade] = await Promise.all([
        prisma.despesa.aggregate({
          where: {
            ...where,
            createdAt: {
              gte: inicioMes,
              lte: fimMes,
            },
          },
          _sum: { valor: true }
        }),
        prisma.despesa.count({
          where: {
            ...where,
            createdAt: {
              gte: inicioMes,
              lte: fimMes,
            },
          }
        })
      ]);

      despesasPorMes.push({
        mes: format(currentDate, 'MMM/yyyy', { locale: require('date-fns/locale/pt-BR') }),
        valor: Number(valor._sum.valor) || 0,
        quantidade
      });

      currentDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 1);
    }

    res.json({
      success: true,
      data: despesasPorMes
    });
  } catch (error) {
    console.error('Erro ao obter despesas por mês:', error);
    res.status(500).json({
      success: false,
      message: 'Erro interno do servidor'
    });
  }
});

// Top UBS por despesas
router.get('/top-ubs-despesas', async (req: Request, res: Response) => {
  try {
    const { dataInicio, dataFim, limit = 5 } = req.query;

    const where: any = {};
    if (dataInicio) where.createdAt = { gte: new Date(dataInicio as string) };
    if (dataFim) where.createdAt = { ...where.createdAt, lte: new Date(dataFim as string) };

    const topUbs = await prisma.despesa.groupBy({
      by: ['ubsId'],
      where: {
        ...where,
        ubsId: { not: null }
      },
      _sum: { valor: true },
      _count: true,
      orderBy: { _sum: { valor: 'desc' } },
      take: parseInt(limit as string),
    });

    const ubsIds = topUbs.map(u => u.ubsId).filter(Boolean);
    const ubs = await prisma.uBS.findMany({
      where: { id: { in: ubsIds } },
      select: { id: true, nome: true }
    });

    const ubsMap = new Map(ubs.map(u => [u.id, u.nome]));

    const result = topUbs.map(item => ({
      ubs: ubsMap.get(item.ubsId!) || 'UBS não encontrada',
      valor: Number(item._sum.valor) || 0,
      quantidade: item._count
    }));

    res.json({
      success: true,
      data: result
    });
  } catch (error) {
    console.error('Erro ao obter top UBS por despesas:', error);
    res.status(500).json({
      success: false,
      message: 'Erro interno do servidor'
    });
  }
});

export default router;