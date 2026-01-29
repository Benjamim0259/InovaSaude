import { Request, Response, NextFunction } from 'express';
import { Permissao } from '@prisma/client';
import { PermissaoService } from '../Services/PermissaoService';

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

export class AuthorizationMiddleware {
  static verificarPermissao(permissao: Permissao) {
    return async (req: Request, res: Response, next: NextFunction) => {
      try {
        if (!req.user?.id) {
          return res.status(401).json({
            message: 'Usuário não autenticado',
            code: 'USER_NOT_AUTHENTICATED'
          });
        }

        const temPermissao = await PermissaoService.verificarPermissao(req.user.id, permissao);

        if (!temPermissao) {
          return res.status(403).json({
            message: 'Acesso negado. Permissão insuficiente.',
            code: 'INSUFFICIENT_PERMISSIONS',
            requiredPermission: permissao
          });
        }

        next();
      } catch (error) {
        console.error('Erro ao verificar permissão:', error);
        res.status(500).json({
          message: 'Erro interno do servidor',
          code: 'INTERNAL_SERVER_ERROR'
        });
      }
    };
  }

  static verificarMultiplasPermissoes(permissoes: Permissao[], modo: 'AND' | 'OR' = 'AND') {
    return async (req: Request, res: Response, next: NextFunction) => {
      try {
        if (!req.user?.id) {
          return res.status(401).json({
            message: 'Usuário não autenticado',
            code: 'USER_NOT_AUTHENTICATED'
          });
        }

        let temPermissao: boolean;

        if (modo === 'AND') {
          temPermissao = await PermissaoService.verificarMultiplasPermissoes(req.user.id, permissoes);
        } else {
          temPermissao = await PermissaoService.verificarPeloMenosUmaPermissao(req.user.id, permissoes);
        }

        if (!temPermissao) {
          return res.status(403).json({
            message: 'Acesso negado. Permissões insuficientes.',
            code: 'INSUFFICIENT_PERMISSIONS',
            requiredPermissions: permissoes,
            mode: modo
          });
        }

        next();
      } catch (error) {
        console.error('Erro ao verificar múltiplas permissões:', error);
        res.status(500).json({
          message: 'Erro interno do servidor',
          code: 'INTERNAL_SERVER_ERROR'
        });
      }
    };
  }

  static verificarPerfilAdmin() {
    return (req: Request, res: Response, next: NextFunction) => {
      if (!req.user) {
        return res.status(401).json({
          message: 'Usuário não autenticado',
          code: 'USER_NOT_AUTHENTICATED'
        });
      }

      if (req.user.perfil !== 'ADMIN') {
        return res.status(403).json({
          message: 'Acesso negado. Apenas administradores podem acessar este recurso.',
          code: 'ADMIN_ACCESS_REQUIRED'
        });
      }

      next();
    };
  }

  static verificarPropriedadeUBS() {
    return async (req: Request, res: Response, next: NextFunction) => {
      try {
        if (!req.user?.id) {
          return res.status(401).json({
            message: 'Usuário não autenticado',
            code: 'USER_NOT_AUTHENTICATED'
          });
        }

        const ubsId = req.params.ubsId || req.body.ubsId || req.query.ubsId;

        if (!ubsId) {
          return next();
        }

        // Se não for admin, verificar se o usuário está associado à UBS
        if (req.user.perfil !== 'ADMIN') {
          // Aqui você pode implementar lógica para verificar se o usuário
          // é coordenador da UBS ou está associado a ela
          // Por enquanto, permite acesso
        }

        next();
      } catch (error) {
        console.error('Erro ao verificar propriedade da UBS:', error);
        res.status(500).json({
          message: 'Erro interno do servidor',
          code: 'INTERNAL_SERVER_ERROR'
        });
      }
    };
  }

  static async carregarPermissoesUsuario(req: Request, _res: Response, next: NextFunction) {
    try {
      if (req.user?.id) {
        const permissoes = await PermissaoService.listarPermissoesUsuario(req.user.id);
        req.user.permissoes = permissoes;
      }
      next();
    } catch (error) {
      console.error('Erro ao carregar permissões do usuário:', error);
      // Não bloqueia a requisição, apenas loga o erro
      next();
    }
  }
}

// Funções helper para facilitar o uso
export const verificarPermissao = AuthorizationMiddleware.verificarPermissao;
export const verificarMultiplasPermissoes = AuthorizationMiddleware.verificarMultiplasPermissoes;
export const verificarPerfilAdmin = AuthorizationMiddleware.verificarPerfilAdmin;
export const verificarPropriedadeUBS = AuthorizationMiddleware.verificarPropriedadeUBS;
export const carregarPermissoesUsuario = AuthorizationMiddleware.carregarPermissoesUsuario;