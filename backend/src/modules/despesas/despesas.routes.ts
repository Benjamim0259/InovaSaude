import { Router } from 'express';
import { DespesasController } from './despesas.controller';
import { authMiddleware, authorize } from '../../shared/middlewares/auth.middleware';
import { validate } from '../../shared/middlewares/validation.middleware';
import {
  createDespesaSchema,
  updateDespesaSchema,
  listDespesasSchema,
} from './despesas.validation';

const router = Router();
const despesasController = new DespesasController();

// Todas as rotas de despesas requerem autenticação
router.use(authMiddleware);

router.get('/', validate(listDespesasSchema), despesasController.findAll);
router.get('/:id', despesasController.findById);
router.post(
  '/',
  authorize('ADMIN', 'COORDENADOR', 'GESTOR'),
  validate(createDespesaSchema),
  despesasController.create
);
router.put(
  '/:id',
  authorize('ADMIN', 'COORDENADOR', 'GESTOR'),
  validate(updateDespesaSchema),
  despesasController.update
);
router.delete('/:id', authorize('ADMIN', 'COORDENADOR'), despesasController.delete);

// Ações específicas
router.post('/:id/aprovar', authorize('ADMIN', 'GESTOR'), despesasController.aprovar);
router.post('/:id/rejeitar', authorize('ADMIN', 'GESTOR'), despesasController.rejeitar);
router.post('/:id/pagar', authorize('ADMIN', 'GESTOR'), despesasController.marcarComoPaga);

export default router;
