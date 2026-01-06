import { Request, Response, NextFunction } from 'express';
import { DespesasService } from './despesas.service';
import { AuthRequest } from '../../shared/middlewares/auth.middleware';

const despesasService = new DespesasService();

export class DespesasController {
  async create(req: AuthRequest, res: Response, next: NextFunction) {
    try {
      if (!req.user) {
        return res.status(401).json({ error: 'Não autenticado' });
      }

      const despesa = await despesasService.create(req.body, req.user.id);
      return res.status(201).json(despesa);
    } catch (error) {
      next(error);
    }
  }

  async findById(req: Request, res: Response, next: NextFunction) {
    try {
      const { id } = req.params;
      const despesa = await despesasService.findById(id);
      return res.json(despesa);
    } catch (error) {
      next(error);
    }
  }

  async findAll(req: Request, res: Response, next: NextFunction) {
    try {
      const filters = {
        page: req.query.page ? parseInt(req.query.page as string) : undefined,
        limit: req.query.limit ? parseInt(req.query.limit as string) : undefined,
        ubsId: req.query.ubsId as string,
        categoriaId: req.query.categoriaId as string,
        fornecedorId: req.query.fornecedorId as string,
        status: req.query.status as string,
        dataInicio: req.query.dataInicio ? new Date(req.query.dataInicio as string) : undefined,
        dataFim: req.query.dataFim ? new Date(req.query.dataFim as string) : undefined,
      };

      const result = await despesasService.findAll(filters);
      return res.json(result);
    } catch (error) {
      next(error);
    }
  }

  async update(req: AuthRequest, res: Response, next: NextFunction) {
    try {
      if (!req.user) {
        return res.status(401).json({ error: 'Não autenticado' });
      }

      const { id } = req.params;
      const despesa = await despesasService.update(id, req.body, req.user.id);
      return res.json(despesa);
    } catch (error) {
      next(error);
    }
  }

  async delete(req: AuthRequest, res: Response, next: NextFunction) {
    try {
      if (!req.user) {
        return res.status(401).json({ error: 'Não autenticado' });
      }

      const { id } = req.params;
      const result = await despesasService.delete(id, req.user.id);
      return res.json(result);
    } catch (error) {
      next(error);
    }
  }

  async aprovar(req: AuthRequest, res: Response, next: NextFunction) {
    try {
      if (!req.user) {
        return res.status(401).json({ error: 'Não autenticado' });
      }

      const { id } = req.params;
      const despesa = await despesasService.aprovar(id, req.user.id);
      return res.json(despesa);
    } catch (error) {
      next(error);
    }
  }

  async rejeitar(req: AuthRequest, res: Response, next: NextFunction) {
    try {
      if (!req.user) {
        return res.status(401).json({ error: 'Não autenticado' });
      }

      const { id } = req.params;
      const { observacao } = req.body;
      const despesa = await despesasService.rejeitar(id, req.user.id, observacao);
      return res.json(despesa);
    } catch (error) {
      next(error);
    }
  }

  async marcarComoPaga(req: AuthRequest, res: Response, next: NextFunction) {
    try {
      if (!req.user) {
        return res.status(401).json({ error: 'Não autenticado' });
      }

      const { id } = req.params;
      const { dataPagamento } = req.body;
      const despesa = await despesasService.marcarComoPaga(
        id,
        req.user.id,
        dataPagamento ? new Date(dataPagamento) : undefined
      );
      return res.json(despesa);
    } catch (error) {
      next(error);
    }
  }
}
