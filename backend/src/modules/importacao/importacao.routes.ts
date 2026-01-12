import { Router } from 'express';
import { authMiddleware, authorize } from '../../shared/middlewares/auth.middleware';
import multer from 'multer';
import path from 'path';
import { config } from '../../config';
import prisma from '../../config/database';
import logger from '../../config/logger';
import XLSX from 'xlsx';
import fs from 'fs';

const router = Router();

// Configuração do multer para upload
const storage = multer.diskStorage({
  destination: (_req, _file, cb) => {
    cb(null, config.upload.uploadDir);
  },
  filename: (_req, file, cb) => {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1e9);
    cb(null, file.fieldname + '-' + uniqueSuffix + path.extname(file.originalname));
  },
});

const upload = multer({
  storage,
  limits: {
    fileSize: config.upload.maxFileSize,
  },
  fileFilter: (_req, file, cb) => {
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

      const usuario = (req as any).user;
      const filePath = req.file.path;
      
      // Ler arquivo
      const workbook = XLSX.readFile(filePath);
      const sheetName = workbook.SheetNames[0];
      const worksheet = workbook.Sheets[sheetName];
      const data = XLSX.utils.sheet_to_json(worksheet);

      let registrosProcessados = 0;
      let registrosErro = 0;
      const erros: string[] = [];

      // Processar cada linha
      for (let i = 0; i < data.length; i++) {
        try {
          const row = data[i] as any;

          // Validar campos obrigatórios
          if (!row.descricao || !row.valor || !row.categoriaId || !row.ubsId) {
            registrosErro++;
            erros.push(`Linha ${i + 2}: Campos obrigatórios ausentes`);
            continue;
          }

          // Criar despesa
          await prisma.despesa.create({
            data: {
              descricao: String(row.descricao),
              valor: parseFloat(row.valor),
              dataVencimento: row.dataVencimento ? new Date(row.dataVencimento) : undefined,
              categoriaId: String(row.categoriaId),
              tipo: row.tipo || 'VARIAVEL',
              ubsId: String(row.ubsId),
              fornecedorId: row.fornecedorId ? String(row.fornecedorId) : undefined,
              usuarioCriacaoId: usuario.id,
              numeroNota: row.numeroNota ? String(row.numeroNota) : undefined,
              observacoes: row.observacoes ? String(row.observacoes) : undefined,
            },
          });

          registrosProcessados++;
        } catch (error: any) {
          registrosErro++;
          erros.push(`Linha ${i + 2}: ${error.message}`);
        }
      }

      // Salvar lote de importação
      const lote = await prisma.importacaoLote.create({
        data: {
          nomeArquivo: req.file.originalname,
          totalRegistros: data.length,
          registrosProcessados,
          registrosErro,
          status: registrosErro === 0 ? 'CONCLUIDO' : 'CONCLUIDO_COM_ERROS',
          usuarioId: usuario.id,
          erros: erros.length > 0 ? JSON.stringify(erros) : undefined,
        },
      });

      logger.info(`Importação concluída: ${registrosProcessados}/${data.length} registros`);

      // Remover arquivo após processamento
      fs.unlinkSync(filePath);

      res.json({
        message: 'Arquivo importado com sucesso',
        lote: {
          id: lote.id,
          nomeArquivo: lote.nomeArquivo,
          totalRegistros: lote.totalRegistros,
          registrosProcessados: lote.registrosProcessados,
          registrosErro: lote.registrosErro,
          status: lote.status,
        },
        erros: erros,
      });
    } catch (error) {
      next(error);
    }
  }
);

// Download de template
router.get('/template', async (_req, res) => {
  try {
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
      exemplo: [
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

    // Criar arquivo Excel de template
    const workbook = XLSX.utils.book_new();
    const worksheet = XLSX.utils.aoa_to_sheet([
      template.headers,
      template.exemplo,
    ]);
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Despesas');

    // Configurar largura das colunas
    worksheet['!cols'] = [
      { wch: 25 },
      { wch: 12 },
      { wch: 15 },
      { wch: 25 },
      { wch: 12 },
      { wch: 25 },
      { wch: 25 },
      { wch: 15 },
      { wch: 25 },
    ];

    // Enviar arquivo
    res.setHeader('Content-Disposition', 'attachment; filename="template-despesas.xlsx"');
    res.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
    
    const buffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'buffer' });
    res.send(buffer);
  } catch (error) {
    res.status(500).json({ error: 'Erro ao gerar template' });
  }
});

// Listar importações
router.get('/lotes', authorize('ADMIN', 'GESTOR'), async (req, res, next) => {
  try {
    const { page = 1, limit = 10 } = req.query;
    const skip = (Number(page) - 1) * Number(limit);

    const [lotes, total] = await Promise.all([
      prisma.importacaoLote.findMany({
        skip,
        take: Number(limit),
        orderBy: { createdAt: 'desc' },
      }),
      prisma.importacaoLote.count(),
    ]);

    res.json({
      data: lotes,
      total,
      page: Number(page),
      limit: Number(limit),
    });
  } catch (error) {
    next(error);
  }
});

// Obter detalhes de um lote
router.get('/lotes/:id', authorize('ADMIN', 'GESTOR'), async (req, res, next) => {
  try {
    const lote = await prisma.importacaoLote.findUnique({
      where: { id: req.params.id },
    });

    if (!lote) {
      return res.status(404).json({ error: 'Lote não encontrado' });
    }

    const erros = lote.erros ? JSON.parse(lote.erros) : [];

    res.json({
      ...lote,
      erros,
    });
  } catch (error) {
    next(error);
  }
});

export default router;
