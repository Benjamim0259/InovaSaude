import { Request, Response, NextFunction } from 'express';
import { AuthService } from '../../modules/auth/auth.service';
import { PermissaoService } from '../Services/PermissaoService';
import { Permissao } from '@prisma/client';

const authService = new AuthService();

declare global {
  namespace Express {
    interface Request {
      user?: {
        id: string;
        nome: string;
        email: string;
        perfil: string;
        ubsId?: string;
        permissoes?: Permissao[];
      };
    }
  }
}

export class AuthMiddleware {
  static authenticate = async (req: Request, res: Response, next: NextFunction) => {
    try {
      const authHeader = req.headers.authorization;

      if (!authHeader || !authHeader.startsWith('Bearer ')) {
        return res.status(401).json({
          success: false,
          message: 'Token de autenticação não fornecido'
        });
      }

      const token = authHeader.split(' ')[1];

      // Validar token
      const decoded: any = await authService.validateToken(token);

      // Carregar dados completos do usuário
      const userData = await authService.getUserById(decoded.id);

      if (!userData) {
        return res.status(401).json({
          success: false,
          message: 'Usuário não encontrado'
        });
      }

      // Verificar se usuário está ativo
      if (userData.status !== 'ATIVO') {
        return res.status(401).json({
          success: false,
          message: 'Usuário inativo ou bloqueado'
        });
      }

      // Carregar permissões do usuário
      const permissoes = await PermissaoService.listarPermissoesUsuario(userData.id);

      // Definir dados do usuário na requisição
      req.user = {
        id: userData.id,
        nome: userData.nome,
        email: userData.email,
        perfil: userData.perfil,
        ubsId: userData.ubsId || undefined,
        permissoes: permissoes
      };

      next();
    } catch (error) {
      console.error('Erro na autenticação:', error);
      return res.status(401).json({
        success: false,
        message: 'Token inválido ou expirado'
      });
    }
  };

  static optionalAuth = async (req: Request, _res: Response, next: NextFunction) => {
    try {
      const authHeader = req.headers.authorization;

      if (!authHeader || !authHeader.startsWith('Bearer ')) {
        return next();
      }

      const token = authHeader.split(' ')[1];
      const decoded: any = await authService.validateToken(token);

      if (decoded) {
        const userData = await authService.getUserById(decoded.id);

        if (userData && userData.status === 'ATIVO') {
          const permissoes = await PermissaoService.listarPermissoesUsuario(userData.id);

          req.user = {
            id: userData.id,
            nome: userData.nome,
            email: userData.email,
            perfil: userData.perfil,
            ubsId: userData.ubsId || undefined,
            permissoes: permissoes
          };
        }
      }
    } catch (error) {
      // Ignorar erros de autenticação opcional
    }

    next();
  };
}