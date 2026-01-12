import nodemailer from 'nodemailer';
import { config } from '../../config';
import logger from '../../config/logger';

interface EmailOptions {
  to: string;
  subject: string;
  html: string;
  text?: string;
}

export class EmailService {
  private transporter: nodemailer.Transporter;

  constructor() {
    this.transporter = nodemailer.createTransport({
      service: process.env.EMAIL_SERVICE || 'gmail',
      host: process.env.EMAIL_HOST,
      port: parseInt(process.env.EMAIL_PORT || '587'),
      secure: process.env.EMAIL_SECURE === 'true',
      auth: {
        user: process.env.EMAIL_USER,
        pass: process.env.EMAIL_PASSWORD,
      },
    });
  }

  async enviarEmail(opcoes: EmailOptions): Promise<void> {
    try {
      const info = await this.transporter.sendMail({
        from: process.env.EMAIL_FROM || 'noreply@inovasaude.com.br',
        to: opcoes.to,
        subject: opcoes.subject,
        html: opcoes.html,
        text: opcoes.text,
      });

      logger.info(`Email enviado para ${opcoes.to}: ${info.response}`);
    } catch (error) {
      logger.error(`Erro ao enviar email para ${opcoes.to}:`, error);
      throw error;
    }
  }

  async enviarEmailRecuperacaoSenha(
    email: string,
    token: string,
    nome: string
  ): Promise<void> {
    const linkRecuperacao = `${process.env.FRONTEND_URL}/reset-password?token=${token}`;

    const html = `
      <!DOCTYPE html>
      <html>
        <head>
          <meta charset="utf-8">
          <style>
            body { font-family: Arial, sans-serif; color: #333; }
            .container { max-width: 600px; margin: 0 auto; padding: 20px; }
            .header { background-color: #007bff; color: white; padding: 20px; border-radius: 5px 5px 0 0; }
            .content { background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }
            .button { 
              display: inline-block;
              background-color: #007bff;
              color: white;
              padding: 12px 30px;
              text-decoration: none;
              border-radius: 5px;
              margin: 20px 0;
            }
            .footer { font-size: 12px; color: #666; margin-top: 20px; border-top: 1px solid #ddd; padding-top: 10px; }
          </style>
        </head>
        <body>
          <div class="container">
            <div class="header">
              <h1>InovaSaúde - Recuperação de Senha</h1>
            </div>
            <div class="content">
              <p>Olá ${nome},</p>
              <p>Você solicitou a recuperação de senha para sua conta no InovaSaúde.</p>
              <p>Clique no botão abaixo para resetar sua senha:</p>
              <a href="${linkRecuperacao}" class="button">Resetar Senha</a>
              <p>Ou copie e cole o link no seu navegador:</p>
              <p>${linkRecuperacao}</p>
              <p>Este link expira em 1 hora.</p>
              <p>Se você não solicitou esta recuperação, ignore este email.</p>
              <div class="footer">
                <p>© 2026 InovaSaúde. Todos os direitos reservados.</p>
              </div>
            </div>
          </div>
        </body>
      </html>
    `;

    await this.enviarEmail({
      to: email,
      subject: 'InovaSaúde - Recuperação de Senha',
      html,
      text: `Você solicitou a recuperação de senha. Use este link para resetar sua senha: ${linkRecuperacao}. Este link expira em 1 hora.`,
    });
  }

  async enviarEmailNovoUsuario(
    email: string,
    nome: string,
    senha: string
  ): Promise<void> {
    const html = `
      <!DOCTYPE html>
      <html>
        <head>
          <meta charset="utf-8">
          <style>
            body { font-family: Arial, sans-serif; color: #333; }
            .container { max-width: 600px; margin: 0 auto; padding: 20px; }
            .header { background-color: #28a745; color: white; padding: 20px; border-radius: 5px 5px 0 0; }
            .content { background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }
            .credentials { background-color: #fff3cd; padding: 15px; border-radius: 5px; margin: 20px 0; }
            .button { 
              display: inline-block;
              background-color: #28a745;
              color: white;
              padding: 12px 30px;
              text-decoration: none;
              border-radius: 5px;
              margin: 20px 0;
            }
            .footer { font-size: 12px; color: #666; margin-top: 20px; border-top: 1px solid #ddd; padding-top: 10px; }
          </style>
        </head>
        <body>
          <div class="container">
            <div class="header">
              <h1>Bem-vindo ao InovaSaúde!</h1>
            </div>
            <div class="content">
              <p>Olá ${nome},</p>
              <p>Sua conta foi criada com sucesso no InovaSaúde - Sistema de Análise e Gerenciamento de Gastos por UBS.</p>
              <div class="credentials">
                <p><strong>Email:</strong> ${email}</p>
                <p><strong>Senha Temporária:</strong> ${senha}</p>
              </div>
              <p>Você pode fazer login em:</p>
              <a href="${process.env.FRONTEND_URL}/login" class="button">Acessar InovaSaúde</a>
              <p><strong>Importante:</strong> Recomendamos trocar sua senha ao primeiro acesso.</p>
              <p>Se tiver dúvidas, entre em contato com o administrador do sistema.</p>
              <div class="footer">
                <p>© 2026 InovaSaúde. Todos os direitos reservados.</p>
              </div>
            </div>
          </div>
        </body>
      </html>
    `;

    await this.enviarEmail({
      to: email,
      subject: 'InovaSaúde - Bem-vindo! Credenciais de Acesso',
      html,
      text: `Bem-vindo ao InovaSaúde! Email: ${email}, Senha: ${senha}. Faça login em ${process.env.FRONTEND_URL}/login e altere sua senha.`,
    });
  }

  async enviarEmailNotificacaoDespesa(
    email: string,
    usuario: string,
    descricao: string,
    status: string
  ): Promise<void> {
    const html = `
      <!DOCTYPE html>
      <html>
        <head>
          <meta charset="utf-8">
          <style>
            body { font-family: Arial, sans-serif; color: #333; }
            .container { max-width: 600px; margin: 0 auto; padding: 20px; }
            .header { background-color: #007bff; color: white; padding: 20px; border-radius: 5px 5px 0 0; }
            .content { background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; }
            .info { background-color: #e7f3ff; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0; }
            .footer { font-size: 12px; color: #666; margin-top: 20px; border-top: 1px solid #ddd; padding-top: 10px; }
          </style>
        </head>
        <body>
          <div class="container">
            <div class="header">
              <h1>InovaSaúde - Notificação de Despesa</h1>
            </div>
            <div class="content">
              <p>Olá ${usuario},</p>
              <p>Houve uma atualização em uma despesa do seu sistema.</p>
              <div class="info">
                <p><strong>Descrição:</strong> ${descricao}</p>
                <p><strong>Status Atual:</strong> ${status}</p>
              </div>
              <p>Acesse o sistema para mais detalhes.</p>
              <div class="footer">
                <p>© 2026 InovaSaúde. Todos os direitos reservados.</p>
              </div>
            </div>
          </div>
        </body>
      </html>
    `;

    await this.enviarEmail({
      to: email,
      subject: `InovaSaúde - Atualização de Despesa: ${status}`,
      html,
    });
  }
}

export const emailService = new EmailService();
