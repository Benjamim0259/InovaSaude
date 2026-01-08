import { Request, Response, NextFunction } from 'express';
import jwt from 'jsonwebtoken';
import { config } from '../../config';
import logger from '../../config/logger';

export interface AuthRequest extends Request {
  user?: {
    id: string;
    email: string;
    perfil: string;
    ubsId?: string;
  };
}

export const authMiddleware = (req: AuthRequest, res: Response, next: NextFunction) => {
  try {
    const authHeader = req.headers.authorization;

    if (!authHeader) {
      return res.status(401).json({ error: 'Token não fornecido' });
    }

    const parts = authHeader.split(' ');

    if (parts.length !== 2) {
      return res.status(401).json({ error: 'Formato de token inválido' });
    }

    const [scheme, token] = parts;

    if (!/^Bearer$/i.test(scheme)) {
      return res.status(401).json({ error: 'Token mal formatado' });
    }

    jwt.verify(token, config.jwt.secret, (err, decoded: any) => {
      if (err) {
        logger.warn(`Token inválido: ${err.message}`);
        return res.status(401).json({ error: 'Token inválido' });
      }

      req.user = {
        id: decoded.id,
        email: decoded.email,
        perfil: decoded.perfil,
        ubsId: decoded.ubsId,
      };

      return next();
    });
  } catch (error) {
    logger.error('Erro no middleware de autenticação:', error);
    return res.status(500).json({ error: 'Erro interno do servidor' });
  }
};

export const authorize = (...perfisPermitidos: string[]) => {
  return (req: AuthRequest, res: Response, next: NextFunction) => {
    if (!req.user) {
      return res.status(401).json({ error: 'Não autenticado' });
    }

    if (!perfisPermitidos.includes(req.user.perfil)) {
      return res.status(403).json({ error: 'Acesso negado' });
    }

    next();
  };
};
