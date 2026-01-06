import express, { Application } from 'express';
import cors from 'cors';
import helmet from 'helmet';
import rateLimit from 'express-rate-limit';
import { config } from './config';
import logger from './config/logger';
import { errorHandler, notFoundHandler } from './shared/middlewares/error.middleware';

// Routes
import authRoutes from './modules/auth/auth.routes';
import despesasRoutes from './modules/despesas/despesas.routes';
import ubsRoutes from './modules/ubs/ubs.routes';
import usuariosRoutes from './modules/usuarios/usuarios.routes';
import relatoriosRoutes from './modules/relatorios/relatorios.routes';
import importacaoRoutes from './modules/importacao/importacao.routes';

const app: Application = express();

// Security middleware
app.use(helmet());
app.use(
  cors({
    origin: config.cors.origin,
    credentials: true,
  })
);

// Rate limiting
const limiter = rateLimit({
  windowMs: config.rateLimit.windowMs,
  max: config.rateLimit.max,
  message: 'Muitas requisições deste IP, tente novamente mais tarde.',
});
app.use('/api/', limiter);

// Body parser
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Health check
app.get('/health', (req, res) => {
  res.json({
    status: 'ok',
    timestamp: new Date().toISOString(),
    uptime: process.uptime(),
    environment: config.env,
  });
});

// API Routes
app.use('/api/auth', authRoutes);
app.use('/api/despesas', despesasRoutes);
app.use('/api/ubs', ubsRoutes);
app.use('/api/usuarios', usuariosRoutes);
app.use('/api/relatorios', relatoriosRoutes);
app.use('/api/importacao', importacaoRoutes);

// Error handling
app.use(notFoundHandler);
app.use(errorHandler);

// Start server
const PORT = config.port;

app.listen(PORT, () => {
  logger.info(`🚀 Servidor rodando na porta ${PORT}`);
  logger.info(`📊 Ambiente: ${config.env}`);
  logger.info(`🔗 Health check: http://localhost:${PORT}/health`);
});

// Graceful shutdown
process.on('SIGTERM', () => {
  logger.info('SIGTERM recebido, encerrando servidor...');
  process.exit(0);
});

process.on('SIGINT', () => {
  logger.info('SIGINT recebido, encerrando servidor...');
  process.exit(0);
});

export default app;
