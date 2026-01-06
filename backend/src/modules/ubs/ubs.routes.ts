import { Router } from 'express';
import { UBSController } from './ubs.controller';
import { authMiddleware, authorize } from '../../shared/middlewares/auth.middleware';
import { validate } from '../../shared/middlewares/validation.middleware';
import { createUBSSchema, updateUBSSchema } from './ubs.validation';

const router = Router();
const ubsController = new UBSController();

// Todas as rotas de UBS requerem autenticação
router.use(authMiddleware);

router.get('/', ubsController.findAll);
router.get('/:id', ubsController.findById);
router.post('/', authorize('ADMIN', 'GESTOR'), validate(createUBSSchema), ubsController.create);
router.put(
  '/:id',
  authorize('ADMIN', 'GESTOR'),
  validate(updateUBSSchema),
  ubsController.update
);
router.delete('/:id', authorize('ADMIN'), ubsController.delete);

export default router;
