import { PrismaClient, Permissao, PerfilUsuario } from '@prisma/client';

const prisma = new PrismaClient();

// Mapeamento de permissões por perfil
const PERMISSOES_POR_PERFIL: Record<PerfilUsuario, Permissao[]> = {
  [PerfilUsuario.ADMIN]: [
    // Todas as permissões
    Permissao.USUARIOS_VISUALIZAR,
    Permissao.USUARIOS_CRIAR,
    Permissao.USUARIOS_EDITAR,
    Permissao.USUARIOS_EXCLUIR,
    Permissao.USUARIOS_BLOQUEAR,
    Permissao.UBS_VISUALIZAR,
    Permissao.UBS_CRIAR,
    Permissao.UBS_EDITAR,
    Permissao.UBS_EXCLUIR,
    Permissao.UBS_GERENCIAR_COORDENADORES,
    Permissao.DESPESAS_VISUALIZAR,
    Permissao.DESPESAS_CRIAR,
    Permissao.DESPESAS_EDITAR,
    Permissao.DESPESAS_EXCLUIR,
    Permissao.DESPESAS_APROVAR,
    Permissao.DESPESAS_REJEITAR,
    Permissao.DESPESAS_PAGAR,
    Permissao.RELATORIOS_VISUALIZAR,
    Permissao.RELATORIOS_EXPORTAR,
    Permissao.RELATORIOS_AVANCADOS,
    Permissao.SISTEMA_CONFIGURAR,
    Permissao.SISTEMA_AUDITAR,
    Permissao.SISTEMA_BACKUP,
  ],
  [PerfilUsuario.COORDENADOR]: [
    Permissao.USUARIOS_VISUALIZAR,
    Permissao.UBS_VISUALIZAR,
    Permissao.UBS_EDITAR,
    Permissao.UBS_GERENCIAR_COORDENADORES,
    Permissao.DESPESAS_VISUALIZAR,
    Permissao.DESPESAS_CRIAR,
    Permissao.DESPESAS_EDITAR,
    Permissao.DESPESAS_APROVAR,
    Permissao.DESPESAS_REJEITAR,
    Permissao.RELATORIOS_VISUALIZAR,
    Permissao.RELATORIOS_EXPORTAR,
  ],
  [PerfilUsuario.GESTOR]: [
    Permissao.UBS_VISUALIZAR,
    Permissao.DESPESAS_VISUALIZAR,
    Permissao.DESPESAS_CRIAR,
    Permissao.DESPESAS_EDITAR,
    Permissao.DESPESAS_APROVAR,
    Permissao.DESPESAS_PAGAR,
    Permissao.RELATORIOS_VISUALIZAR,
    Permissao.RELATORIOS_EXPORTAR,
  ],
  [PerfilUsuario.AUDITOR]: [
    Permissao.USUARIOS_VISUALIZAR,
    Permissao.UBS_VISUALIZAR,
    Permissao.DESPESAS_VISUALIZAR,
    Permissao.RELATORIOS_VISUALIZAR,
    Permissao.RELATORIOS_EXPORTAR,
    Permissao.RELATORIOS_AVANCADOS,
    Permissao.SISTEMA_AUDITAR,
  ],
  [PerfilUsuario.OPERADOR]: [
    Permissao.UBS_VISUALIZAR,
    Permissao.DESPESAS_VISUALIZAR,
    Permissao.DESPESAS_CRIAR,
    Permissao.DESPESAS_EDITAR,
  ],
  [PerfilUsuario.VISUALIZADOR]: [
    Permissao.UBS_VISUALIZAR,
    Permissao.DESPESAS_VISUALIZAR,
    Permissao.RELATORIOS_VISUALIZAR,
  ],
};

export class PermissaoService {
  static async inicializarPermissoesUsuario(usuarioId: string, perfil: PerfilUsuario, concedidaPor?: string) {
    const permissoes = PERMISSOES_POR_PERFIL[perfil];

    const permissoesData = permissoes.map(permissao => ({
      usuarioId,
      permissao,
      concedidaPor,
    }));

    await prisma.permissaoUsuario.createMany({
      data: permissoesData,
      skipDuplicates: true,
    });
  }

  static async verificarPermissao(usuarioId: string, permissao: Permissao): Promise<boolean> {
    const permissaoUsuario = await prisma.permissaoUsuario.findFirst({
      where: {
        usuarioId,
        permissao,
      },
    });

    return !!permissaoUsuario;
  }

  static async verificarMultiplasPermissoes(usuarioId: string, permissoes: Permissao[]): Promise<boolean> {
    const count = await prisma.permissaoUsuario.count({
      where: {
        usuarioId,
        permissao: {
          in: permissoes,
        },
      },
    });

    return count === permissoes.length;
  }

  static async verificarPeloMenosUmaPermissao(usuarioId: string, permissoes: Permissao[]): Promise<boolean> {
    const count = await prisma.permissaoUsuario.count({
      where: {
        usuarioId,
        permissao: {
          in: permissoes,
        },
      },
    });

    return count > 0;
  }

  static async listarPermissoesUsuario(usuarioId: string): Promise<Permissao[]> {
    const permissoes = await prisma.permissaoUsuario.findMany({
      where: { usuarioId },
      select: { permissao: true },
    });

    return permissoes.map(p => p.permissao);
  }

  static async concederPermissao(
    usuarioId: string,
    permissao: Permissao,
    concedidaPor: string
  ): Promise<void> {
    await prisma.permissaoUsuario.upsert({
      where: {
        usuarioId_permissao: {
          usuarioId,
          permissao,
        },
      },
      update: {
        concedidaPor,
      },
      create: {
        usuarioId,
        permissao,
        concedidaPor,
      },
    });
  }

  static async revogarPermissao(usuarioId: string, permissao: Permissao): Promise<void> {
    await prisma.permissaoUsuario.deleteMany({
      where: {
        usuarioId,
        permissao,
      },
    });
  }

  static async atualizarPermissoesUsuario(
    usuarioId: string,
    novasPermissoes: Permissao[],
    concedidaPor: string
  ): Promise<void> {
    // Remove todas as permissões atuais
    await prisma.permissaoUsuario.deleteMany({
      where: { usuarioId },
    });

    // Adiciona as novas permissões
    if (novasPermissoes.length > 0) {
      const permissoesData = novasPermissoes.map(permissao => ({
        usuarioId,
        permissao,
        concedidaPor,
      }));

      await prisma.permissaoUsuario.createMany({
        data: permissoesData,
      });
    }
  }

  static async sincronizarPermissoesPorPerfil(
    usuarioId: string,
    perfil: PerfilUsuario,
    concedidaPor: string
  ): Promise<void> {
    const permissoesPadrao = PERMISSOES_POR_PERFIL[perfil];
    await this.atualizarPermissoesUsuario(usuarioId, permissoesPadrao, concedidaPor);
  }

  static getPermissoesPorPerfil(perfil: PerfilUsuario): Permissao[] {
    return PERMISSOES_POR_PERFIL[perfil] || [];
  }

  static getTodasPermissoes(): Permissao[] {
    return Object.values(Permissao);
  }
}