import { Router } from 'express';
import { authMiddleware, authorize } from '../../shared/middlewares/auth.middleware';
import multer from 'multer';
import path from 'path';
import { config } from '../../config';

const router = Router();

// Configuração do multer para upload
const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    cb(null, config.upload.uploadDir);
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1e9);
    cb(null, file.fieldname + '-' + uniqueSuffix + path.extname(file.originalname));
  },
});

const upload = multer({
  storage,
  limits: {
    fileSize: config.upload.maxFileSize,
  },
  fileFilter: (req, file, cb) => {
    const allowedMimes = ['text/csv', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'];
    
    if (allowedMimes.includes(file.mimetype)) {
      cb(null, true);
    } else {
      cb(new Error('Tipo de arquivo não permitido. Apenas CSV e XLSX são aceitos.'));
    }
  },
});

// Todas as rotas de importação requerem autenticação
router.use(authMiddleware);

// Upload de arquivo para importação
router.post(
  '/upload',
  authorize('ADMIN', 'GESTOR'),
  upload.single('file'),
  async (req, res, next) => {
    try {
      if (!req.file) {
        return res.status(400).json({ error: 'Nenhum arquivo foi enviado' });
      }

      // TODO: Processar arquivo e importar dados
      // Esta é uma implementação básica que deve ser expandida

      res.json({
        message: 'Arquivo recebido com sucesso',
        filename: req.file.filename,
        originalname: req.file.originalname,
        size: req.file.size,
        status: 'AGUARDANDO_PROCESSAMENTO',
      });
    } catch (error) {
      next(error);
    }
  }
);

// Download de template
router.get('/template', async (req, res) => {
  // TODO: Retornar template CSV/XLSX para importação
  const template = {
    headers: [
      'descricao',
      'valor',
      'dataVencimento',
      'categoriaId',
      'tipo',
      'ubsId',
      'fornecedorId',
      'numeroNota',
      'observacoes',
    ],
    example: [
      'Material de limpeza',
      '150.00',
      '2024-01-31',
      'uuid-da-categoria',
      'VARIAVEL',
      'uuid-da-ubs',
      'uuid-do-fornecedor',
      'NF-12345',
      'Compra mensal',
    ],
  };

  res.json(template);
});

// Listar importações
router.get('/lotes', authorize('ADMIN', 'GESTOR'), async (req, res, next) => {
  try {
    // TODO: Implementar listagem de lotes de importação
    res.json({
      lotes: [],
      message: 'Funcionalidade em desenvolvimento',
    });
  } catch (error) {
    next(error);
  }
});

export default router;
