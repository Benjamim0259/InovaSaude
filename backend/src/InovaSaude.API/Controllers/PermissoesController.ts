import { Router, Request, Response } from 'express';
import { PermissaoService } from '../../InovaSaude.Core/Services/PermissaoService';
import { Permissao, PerfilUsuario } from '@prisma/client';
import { verificarPermissao, verificarPerfilAdmin } from '../../InovaSaude.Core/Middlewares/AuthorizationMiddleware';

const router = Router();

// Listar todas as permissões disponíveis
router.get('/disponiveis', async (_req: Request, res: Response) => {
  try {
    const permissoes = PermissaoService.getTodasPermissoes();
    res.json({
      success: true,
      data: permissoes
    });
  } catch (error) {
    console.error('Erro ao listar permissões disponíveis:', error);
    res.status(500).json({
      success: false,
      message: 'Erro interno do servidor'
    });
  }
});

// Listar permissões por perfil
router.get('/por-perfil/:perfil', async (req: Request, res: Response) => {
  try {
    const { perfil } = req.params;

    if (!Object.values(PerfilUsuario).includes(perfil as PerfilUsuario)) {
      return res.status(400).json({
        success: false,
        message: 'Perfil inválido'
      });
    }

    const permissoes = PermissaoService.getPermissoesPorPerfil(perfil as PerfilUsuario);
    res.json({
      success: true,
      data: permissoes
    });
  } catch (error) {
    console.error('Erro ao listar permissões por perfil:', error);
    res.status(500).json({
      success: false,
      message: 'Erro interno do servidor'
    });
  }
});

// Listar permissões de um usuário
router.get('/usuario/:usuarioId',
  verificarPermissao(Permissao.USUARIOS_VISUALIZAR),
  async (req: Request, res: Response) => {
    try {
      const { usuarioId } = req.params;

      // Verificar se o usuário pode visualizar permissões de outros usuários
      if (req.user?.id !== usuarioId && req.user?.perfil !== 'ADMIN') {
        return res.status(403).json({
          success: false,
          message: 'Acesso negado. Você só pode visualizar suas próprias permissões.'
        });
      }

      const permissoes = await PermissaoService.listarPermissoesUsuario(usuarioId);
      res.json({
        success: true,
        data: permissoes
      });
    } catch (error) {
      console.error('Erro ao listar permissões do usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Conceder permissão a um usuário
router.post('/conceder',
  verificarPerfilAdmin(),
  async (req: Request, res: Response) => {
    try {
      const { usuarioId, permissao } = req.body;

      if (!usuarioId || !permissao) {
        return res.status(400).json({
          success: false,
          message: 'Usuário e permissão são obrigatórios'
        });
      }

      if (!Object.values(Permissao).includes(permissao)) {
        return res.status(400).json({
          success: false,
          message: 'Permissão inválida'
        });
      }

      await PermissaoService.concederPermissao(usuarioId, permissao, req.user!.id);

      res.json({
        success: true,
        message: 'Permissão concedida com sucesso'
      });
    } catch (error) {
      console.error('Erro ao conceder permissão:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Revogar permissão de um usuário
router.post('/revogar',
  verificarPerfilAdmin(),
  async (req: Request, res: Response) => {
    try {
      const { usuarioId, permissao } = req.body;

      if (!usuarioId || !permissao) {
        return res.status(400).json({
          success: false,
          message: 'Usuário e permissão são obrigatórios'
        });
      }

      await PermissaoService.revogarPermissao(usuarioId, permissao);

      res.json({
        success: true,
        message: 'Permissão revogada com sucesso'
      });
    } catch (error) {
      console.error('Erro ao revogar permissão:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Atualizar todas as permissões de um usuário
router.put('/usuario/:usuarioId',
  verificarPerfilAdmin(),
  async (req: Request, res: Response) => {
    try {
      const { usuarioId } = req.params;
      const { permissoes } = req.body;

      if (!Array.isArray(permissoes)) {
        return res.status(400).json({
          success: false,
          message: 'Permissões devem ser um array'
        });
      }

      // Validar se todas as permissões são válidas
      const permissoesInvalidas = permissoes.filter(p => !Object.values(Permissao).includes(p));
      if (permissoesInvalidas.length > 0) {
        return res.status(400).json({
          success: false,
          message: 'Permissões inválidas encontradas',
          invalidPermissions: permissoesInvalidas
        });
      }

      await PermissaoService.atualizarPermissoesUsuario(usuarioId, permissoes, req.user!.id);

      res.json({
        success: true,
        message: 'Permissões atualizadas com sucesso'
      });
    } catch (error) {
      console.error('Erro ao atualizar permissões do usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Sincronizar permissões com base no perfil
router.post('/sincronizar/:usuarioId',
  verificarPerfilAdmin(),
  async (req: Request, res: Response) => {
    try {
      const { usuarioId } = req.params;
      const { perfil } = req.body;

      if (!perfil || !Object.values(PerfilUsuario).includes(perfil)) {
        return res.status(400).json({
          success: false,
          message: 'Perfil inválido'
        });
      }

      await PermissaoService.sincronizarPermissoesPorPerfil(
        usuarioId,
        perfil as PerfilUsuario,
        req.user!.id
      );

      res.json({
        success: true,
        message: 'Permissões sincronizadas com sucesso'
      });
    } catch (error) {
      console.error('Erro ao sincronizar permissões:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Verificar se usuário tem uma permissão específica
router.get('/verificar/:usuarioId/:permissao',
  verificarPermissao(Permissao.USUARIOS_VISUALIZAR),
  async (req: Request, res: Response) => {
    try {
      const { usuarioId, permissao } = req.params;

      // Verificar se o usuário pode verificar permissões de outros usuários
      if (req.user?.id !== usuarioId && req.user?.perfil !== 'ADMIN') {
        return res.status(403).json({
          success: false,
          message: 'Acesso negado. Você só pode verificar suas próprias permissões.'
        });
      }

      if (!Object.values(Permissao).includes(permissao as Permissao)) {
        return res.status(400).json({
          success: false,
          message: 'Permissão inválida'
        });
      }

      const temPermissao = await PermissaoService.verificarPermissao(
        usuarioId,
        permissao as Permissao
      );

      res.json({
        success: true,
        data: {
          usuarioId,
          permissao,
          hasPermission: temPermissao
        }
      });
    } catch (error) {
      console.error('Erro ao verificar permissão:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

export default router;