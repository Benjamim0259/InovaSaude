import { Request, Response, NextFunction } from 'express';
import { AuthService } from './auth.service';
import logger from '../../config/logger';

const authService = new AuthService();

export class AuthController {
  async login(req: Request, res: Response, next: NextFunction) {
    try {
      const { email, senha } = req.body;
      const result = await authService.login(email, senha);
      return res.json(result);
    } catch (error) {
      next(error);
    }
  }

  async register(req: Request, res: Response, next: NextFunction) {
    try {
      const result = await authService.register(req.body);
      return res.status(201).json(result);
    } catch (error) {
      next(error);
    }
  }

  async forgotPassword(req: Request, res: Response, next: NextFunction) {
    try {
      const { email } = req.body;
      const result = await authService.forgotPassword(email);
      return res.json(result);
    } catch (error) {
      next(error);
    }
  }

  async resetPassword(req: Request, res: Response, next: NextFunction) {
    try {
      const { token, novaSenha } = req.body;
      const result = await authService.resetPassword(token, novaSenha);
      return res.json(result);
    } catch (error) {
      next(error);
    }
  }

  async logout(req: Request, res: Response) {
    // Com JWT stateless, o logout é feito no cliente removendo o token
    // Aqui podemos registrar o logout para auditoria
    logger.info('Logout realizado');
    return res.json({ message: 'Logout realizado com sucesso' });
  }

  async refresh(req: Request, res: Response, next: NextFunction) {
    try {
      const authHeader = req.headers.authorization;
      if (!authHeader) {
        return res.status(401).json({ error: 'Token não fornecido' });
      }

      const token = authHeader.split(' ')[1];
      const decoded = await authService.validateToken(token);

      return res.json({ valid: true, user: decoded });
    } catch (error) {
      next(error);
    }
  }
}
