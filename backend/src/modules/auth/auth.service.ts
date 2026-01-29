import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import crypto from 'crypto';
import prisma from '../../config/database';
import { config } from '../../config';
import logger from '../../config/logger';
import { AppError } from '../../shared/middlewares/error.middleware';
import { emailService } from '../../shared/services/email.service';

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
      config.jwt.secret as jwt.Secret,
      { expiresIn: config.jwt.expiresIn as jwt.SignOptions['expiresIn'] }
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
      config.jwt.secret as jwt.Secret,
      { expiresIn: config.jwt.expiresIn as jwt.SignOptions['expiresIn'] }
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

    // Gerar token seguro
    const token = crypto.randomBytes(32).toString('hex');
    const expiradoEm = new Date(Date.now() + 60 * 60 * 1000); // 1 hora

    // Salvar token no banco
    await prisma.tokenRecuperacaoSenha.create({
      data: {
        usuarioId: usuario.id,
        token,
        expiradoEm,
      },
    });

    try {
      // Enviar email com link de recuperação
      await emailService.enviarEmailRecuperacaoSenha(
        email,
        token,
        usuario.nome
      );
      logger.info(`Email de recuperação de senha enviado para: ${email}`);
    } catch (error) {
      logger.error(`Erro ao enviar email para ${email}:`, error);
      // Não lançar erro para não revelar problemas de email
    }

    return { message: 'Se o email existir, você receberá instruções de recuperação' };
  }

  async resetPassword(token: string, novaSenha: string) {
    // Validar token
    const tokenRecord = await prisma.tokenRecuperacaoSenha.findUnique({
      where: { token },
      include: { usuario: true },
    });

    if (!tokenRecord) {
      throw new AppError('Token de recuperação inválido', 400);
    }

    // Verificar se token expirou
    if (new Date() > tokenRecord.expiradoEm) {
      throw new AppError('Token de recuperação expirado', 400);
    }

    // Verificar se token já foi usado
    if (tokenRecord.utilizadoEm) {
      throw new AppError('Este token já foi utilizado', 400);
    }

    // Atualizar senha
    const senhaHash = await bcrypt.hash(novaSenha, config.bcrypt.rounds);
    
    await prisma.usuario.update({
      where: { id: tokenRecord.usuarioId },
      data: { senhaHash },
    });

    // Marcar token como utilizado
    await prisma.tokenRecuperacaoSenha.update({
      where: { id: tokenRecord.id },
      data: { utilizadoEm: new Date() },
    });

    logger.info(`Senha resetada para usuário: ${tokenRecord.usuario.email}`);

    return { message: 'Senha atualizada com sucesso' };
  }

  async getUserById(id: string) {
    const usuario = await prisma.usuario.findUnique({
      where: { id },
      select: {
        id: true,
        nome: true,
        email: true,
        perfil: true,
        status: true,
        telefone: true,
        ubsId: true,
        ultimoAcesso: true,
        createdAt: true,
        updatedAt: true,
      },
    });

    return usuario;
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
