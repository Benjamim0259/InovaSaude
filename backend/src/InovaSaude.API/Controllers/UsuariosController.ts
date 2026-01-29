import { Router, Request, Response } from 'express';
import bcrypt from 'bcrypt';
import { PrismaClient, PerfilUsuario, StatusUsuario, Permissao } from '@prisma/client';
import { PermissaoService } from '../../InovaSaude.Core/Services/PermissaoService';
import { verificarPermissao } from '../../InovaSaude.Core/Middlewares/AuthorizationMiddleware';

const prisma = new PrismaClient();
const router = Router();

// Listar usuários
router.get('/',
  verificarPermissao(Permissao.USUARIOS_VISUALIZAR),
  async (req: Request, res: Response) => {
    try {
      const {
        page = 1,
        limit = 10,
        perfil,
        status,
        search
      } = req.query;

      const pageNumber = parseInt(page as string);
      const limitNumber = parseInt(limit as string);
      const skip = (pageNumber - 1) * limitNumber;

      const where: any = {};

      if (perfil && Object.values(PerfilUsuario).includes(perfil as PerfilUsuario)) {
        where.perfil = perfil;
      }

      if (status && Object.values(StatusUsuario).includes(status as StatusUsuario)) {
        where.status = status;
      }

      if (search) {
        where.OR = [
          { nome: { contains: search as string, mode: 'insensitive' } },
          { email: { contains: search as string, mode: 'insensitive' } }
        ];
      }

      // Se não for admin, só mostrar usuários da mesma UBS
      if (req.user?.perfil !== 'ADMIN') {
        where.ubsId = req.user?.ubsId;
      }

      const [usuarios, total] = await Promise.all([
        prisma.usuario.findMany({
          where,
          select: {
            id: true,
            nome: true,
            email: true,
            perfil: true,
            status: true,
            telefone: true,
            ultimoAcesso: true,
            createdAt: true,
            ubs: {
              select: {
                id: true,
                nome: true,
                codigo: true
              }
            },
            _count: {
              select: {
                despesasCriadas: true,
                despesasAprovadas: true
              }
            }
          },
          skip,
          take: limitNumber,
          orderBy: { createdAt: 'desc' }
        }),
        prisma.usuario.count({ where })
      ]);

      res.json({
        success: true,
        data: {
          usuarios,
          pagination: {
            page: pageNumber,
            limit: limitNumber,
            total,
            pages: Math.ceil(total / limitNumber)
          }
        }
      });
    } catch (error) {
      console.error('Erro ao listar usuários:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Obter usuário por ID
router.get('/:id',
  verificarPermissao(Permissao.USUARIOS_VISUALIZAR),
  async (req: Request, res: Response) => {
    try {
      const { id } = req.params;

      // Verificar se o usuário pode visualizar outros usuários
      if (req.user?.id !== id && req.user?.perfil !== 'ADMIN') {
        return res.status(403).json({
          success: false,
          message: 'Acesso negado. Você só pode visualizar seus próprios dados.'
        });
      }

      const usuario = await prisma.usuario.findUnique({
        where: { id },
        select: {
          id: true,
          nome: true,
          email: true,
          perfil: true,
          status: true,
          telefone: true,
          ultimoAcesso: true,
          createdAt: true,
          updatedAt: true,
          ubs: {
            select: {
              id: true,
              nome: true,
              codigo: true
            }
          },
          permissoes: {
            select: {
              permissao: true,
              concedidaEm: true,
              concedidaPor: true
            }
          }
        }
      });

      if (!usuario) {
        return res.status(404).json({
          success: false,
          message: 'Usuário não encontrado'
        });
      }

      res.json({
        success: true,
        data: usuario
      });
    } catch (error) {
      console.error('Erro ao obter usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Criar usuário
router.post('/',
  verificarPermissao(Permissao.USUARIOS_CRIAR),
  async (req: Request, res: Response) => {
    try {
      const {
        nome,
        email,
        senha,
        perfil,
        telefone,
        ubsId
      } = req.body;

      // Validações
      if (!nome || !email || !senha || !perfil) {
        return res.status(400).json({
          success: false,
          message: 'Nome, email, senha e perfil são obrigatórios'
        });
      }

      if (!Object.values(PerfilUsuario).includes(perfil)) {
        return res.status(400).json({
          success: false,
          message: 'Perfil inválido'
        });
      }

      // Verificar se email já existe
      const usuarioExistente = await prisma.usuario.findUnique({
        where: { email }
      });

      if (usuarioExistente) {
        return res.status(400).json({
          success: false,
          message: 'Email já cadastrado'
        });
      }

      // Hash da senha
      const senhaHash = await bcrypt.hash(senha, 10);

      // Criar usuário
      const usuario = await prisma.usuario.create({
        data: {
          nome,
          email,
          senhaHash,
          perfil,
          telefone,
          ubsId
        },
        select: {
          id: true,
          nome: true,
          email: true,
          perfil: true,
          status: true,
          telefone: true,
          createdAt: true,
          ubs: {
            select: {
              id: true,
              nome: true
            }
          }
        }
      });

      // Inicializar permissões baseadas no perfil
      await PermissaoService.inicializarPermissoesUsuario(
        usuario.id,
        perfil,
        req.user?.id
      );

      res.status(201).json({
        success: true,
        message: 'Usuário criado com sucesso',
        data: usuario
      });
    } catch (error) {
      console.error('Erro ao criar usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Atualizar usuário
router.put('/:id',
  verificarPermissao(Permissao.USUARIOS_EDITAR),
  async (req: Request, res: Response) => {
    try {
      const { id } = req.params;
      const {
        nome,
        email,
        perfil,
        telefone,
        ubsId,
        status
      } = req.body;

      // Verificar se o usuário pode editar outros usuários
      if (req.user?.id !== id && req.user?.perfil !== 'ADMIN') {
        return res.status(403).json({
          success: false,
          message: 'Acesso negado. Você só pode editar seus próprios dados.'
        });
      }

      // Verificar se email já existe (se foi alterado)
      if (email) {
        const usuarioExistente = await prisma.usuario.findFirst({
          where: {
            email,
            id: { not: id }
          }
        });

        if (usuarioExistente) {
          return res.status(400).json({
            success: false,
            message: 'Email já cadastrado'
          });
        }
      }

      const updateData: any = {};
      if (nome) updateData.nome = nome;
      if (email) updateData.email = email;
      if (telefone !== undefined) updateData.telefone = telefone;
      if (ubsId !== undefined) updateData.ubsId = ubsId;

      // Apenas admin pode alterar perfil e status
      if (req.user?.perfil === 'ADMIN') {
        if (perfil && Object.values(PerfilUsuario).includes(perfil)) {
          updateData.perfil = perfil;
        }
        if (status && Object.values(StatusUsuario).includes(status)) {
          updateData.status = status;
        }
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
          updatedAt: true,
          ubs: {
            select: {
              id: true,
              nome: true
            }
          }
        }
      });

      // Se o perfil foi alterado, sincronizar permissões
      if (perfil && req.user?.perfil === 'ADMIN') {
        await PermissaoService.sincronizarPermissoesPorPerfil(
          id,
          perfil,
          req.user.id
        );
      }

      res.json({
        success: true,
        message: 'Usuário atualizado com sucesso',
        data: usuario
      });
    } catch (error) {
      console.error('Erro ao atualizar usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Alterar senha
router.put('/:id/senha',
  async (req: Request, res: Response) => {
    try {
      const { id } = req.params;
      const { senhaAtual, novaSenha } = req.body;

      // Verificar se o usuário pode alterar a senha
      if (req.user?.id !== id && req.user?.perfil !== 'ADMIN') {
        return res.status(403).json({
          success: false,
          message: 'Acesso negado. Você só pode alterar sua própria senha.'
        });
      }

      // Se não for admin, verificar senha atual
      if (req.user?.perfil !== 'ADMIN') {
        const usuario = await prisma.usuario.findUnique({
          where: { id },
          select: { senhaHash: true }
        });

        if (!usuario) {
          return res.status(404).json({
            success: false,
            message: 'Usuário não encontrado'
          });
        }

        const senhaCorreta = await bcrypt.compare(senhaAtual, usuario.senhaHash);
        if (!senhaCorreta) {
          return res.status(400).json({
            success: false,
            message: 'Senha atual incorreta'
          });
        }
      }

      // Hash da nova senha
      const novaSenhaHash = await bcrypt.hash(novaSenha, 10);

      await prisma.usuario.update({
        where: { id },
        data: { senhaHash: novaSenhaHash }
      });

      res.json({
        success: true,
        message: 'Senha alterada com sucesso'
      });
    } catch (error) {
      console.error('Erro ao alterar senha:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Bloquear/Desbloquear usuário
router.put('/:id/status',
  verificarPermissao(Permissao.USUARIOS_BLOQUEAR),
  async (req: Request, res: Response) => {
    try {
      const { id } = req.params;
      const { status } = req.body;

      if (!status || !Object.values(StatusUsuario).includes(status)) {
        return res.status(400).json({
          success: false,
          message: 'Status inválido'
        });
      }

      const usuario = await prisma.usuario.update({
        where: { id },
        data: { status },
        select: {
          id: true,
          nome: true,
          email: true,
          status: true
        }
      });

      res.json({
        success: true,
        message: `Usuário ${status === 'ATIVO' ? 'desbloqueado' : 'bloqueado'} com sucesso`,
        data: usuario
      });
    } catch (error) {
      console.error('Erro ao alterar status do usuário:', error);
      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

// Excluir usuário
router.delete('/:id',
  verificarPermissao(Permissao.USUARIOS_EXCLUIR),
  async (req: Request, res: Response) => {
    try {
      const { id } = req.params;

      // Verificar se o usuário pode excluir outros usuários
      if (req.user?.id === id) {
        return res.status(400).json({
          success: false,
          message: 'Você não pode excluir seu próprio usuário'
        });
      }

      await prisma.usuario.delete({
        where: { id }
      });

      res.json({
        success: true,
        message: 'Usuário excluído com sucesso'
      });
    } catch (error: any) {
      console.error('Erro ao excluir usuário:', error);

      if (error.code === 'P2003') {
        return res.status(400).json({
          success: false,
          message: 'Não é possível excluir o usuário pois existem registros relacionados'
        });
      }

      res.status(500).json({
        success: false,
        message: 'Erro interno do servidor'
      });
    }
  }
);

export default router;