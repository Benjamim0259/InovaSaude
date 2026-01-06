import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import prisma from '../../config/database';
import { config } from '../../config';
import logger from '../../config/logger';
import { AppError } from '../../shared/middlewares/error.middleware';

export class AuthService {
  async login(email: string, senha: string) {
    const usuario = await prisma.usuario.findUnique({
      where: { email },
      include: { ubs: true },
    });

    if (!usuario) {
      throw new AppError('Credenciais inválidas', 401);
    }

    if (usuario.status !== 'ATIVO') {
      throw new AppError('Usuário inativo ou bloqueado', 401);
    }

    const senhaValida = await bcrypt.compare(senha, usuario.senhaHash);

    if (!senhaValida) {
      throw new AppError('Credenciais inválidas', 401);
    }

    // Atualiza último acesso
    await prisma.usuario.update({
      where: { id: usuario.id },
      data: { ultimoAcesso: new Date() },
    });

    const token = jwt.sign(
      {
        id: usuario.id,
        email: usuario.email,
        perfil: usuario.perfil,
        ubsId: usuario.ubsId,
      },
      config.jwt.secret,
      { expiresIn: config.jwt.expiresIn }
    );

    logger.info(`Usuário ${usuario.email} autenticado com sucesso`);

    return {
      token,
      usuario: {
        id: usuario.id,
        nome: usuario.nome,
        email: usuario.email,
        perfil: usuario.perfil,
        ubs: usuario.ubs,
      },
    };
  }

  async register(data: {
    nome: string;
    email: string;
    senha: string;
    perfil: string;
    ubsId?: string;
  }) {
    const usuarioExistente = await prisma.usuario.findUnique({
      where: { email: data.email },
    });

    if (usuarioExistente) {
      throw new AppError('Email já cadastrado', 400);
    }

    const senhaHash = await bcrypt.hash(data.senha, config.bcrypt.rounds);

    const usuario = await prisma.usuario.create({
      data: {
        nome: data.nome,
        email: data.email,
        senhaHash,
        perfil: data.perfil as any,
        ubsId: data.ubsId,
      },
      include: { ubs: true },
    });

    logger.info(`Novo usuário criado: ${usuario.email}`);

    const token = jwt.sign(
      {
        id: usuario.id,
        email: usuario.email,
        perfil: usuario.perfil,
        ubsId: usuario.ubsId,
      },
      config.jwt.secret,
      { expiresIn: config.jwt.expiresIn }
    );

    return {
      token,
      usuario: {
        id: usuario.id,
        nome: usuario.nome,
        email: usuario.email,
        perfil: usuario.perfil,
        ubs: usuario.ubs,
      },
    };
  }

  async forgotPassword(email: string) {
    const usuario = await prisma.usuario.findUnique({
      where: { email },
    });

    if (!usuario) {
      // Não revela se o email existe por segurança
      return { message: 'Se o email existir, você receberá instruções de recuperação' };
    }

    // TODO: Implementar envio de email com token de recuperação
    logger.info(`Recuperação de senha solicitada para: ${email}`);

    return { message: 'Se o email existir, você receberá instruções de recuperação' };
  }

  async resetPassword(token: string, novaSenha: string) {
    // TODO: Implementar lógica de validação de token e reset de senha
    throw new AppError('Funcionalidade em desenvolvimento', 501);
  }

  async validateToken(token: string) {
    try {
      const decoded = jwt.verify(token, config.jwt.secret);
      return decoded;
    } catch (error) {
      throw new AppError('Token inválido', 401);
    }
  }
}
