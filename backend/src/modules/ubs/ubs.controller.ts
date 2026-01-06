import { Request, Response, NextFunction } from 'express';
import { UBSService } from './ubs.service';

const ubsService = new UBSService();

export class UBSController {
  async create(req: Request, res: Response, next: NextFunction) {
    try {
      const ubs = await ubsService.create(req.body);
      return res.status(201).json(ubs);
    } catch (error) {
      next(error);
    }
  }

  async findById(req: Request, res: Response, next: NextFunction) {
    try {
      const { id } = req.params;
      const ubs = await ubsService.findById(id);
      return res.json(ubs);
    } catch (error) {
      next(error);
    }
  }

  async findAll(req: Request, res: Response, next: NextFunction) {
    try {
      const filters = {
        page: req.query.page ? parseInt(req.query.page as string) : undefined,
        limit: req.query.limit ? parseInt(req.query.limit as string) : undefined,
        status: req.query.status as string,
      };

      const result = await ubsService.findAll(filters);
      return res.json(result);
    } catch (error) {
      next(error);
    }
  }

  async update(req: Request, res: Response, next: NextFunction) {
    try {
      const { id } = req.params;
      const ubs = await ubsService.update(id, req.body);
      return res.json(ubs);
    } catch (error) {
      next(error);
    }
  }

  async delete(req: Request, res: Response, next: NextFunction) {
    try {
      const { id } = req.params;
      const result = await ubsService.delete(id);
      return res.json(result);
    } catch (error) {
      next(error);
    }
  }
}
