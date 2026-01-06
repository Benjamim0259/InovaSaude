import { UBSRepository } from './ubs.repository';
import { AppError } from '../../shared/middlewares/error.middleware';
import logger from '../../config/logger';

const ubsRepository = new UBSRepository();

export class UBSService {
  async create(data: any) {
    // Verifica se código já existe
    const ubsExistente = await ubsRepository.findByCodigo(data.codigo);
    if (ubsExistente) {
      throw new AppError('Código de UBS já cadastrado', 400);
    }

    const ubs = await ubsRepository.create({
      nome: data.nome,
      codigo: data.codigo,
      endereco: data.endereco,
      bairro: data.bairro,
      cep: data.cep,
      telefone: data.telefone,
      email: data.email,
      capacidadeAtendimento: data.capacidadeAtendimento,
      observacoes: data.observacoes,
      ...(data.coordenadorId && {
        coordenador: {
          connect: { id: data.coordenadorId },
        },
      }),
    });

    logger.info(`UBS criada: ${ubs.id} - ${ubs.nome}`);

    return ubs;
  }

  async findById(id: string) {
    const ubs = await ubsRepository.findById(id);

    if (!ubs) {
      throw new AppError('UBS não encontrada', 404);
    }

    return ubs;
  }

  async findAll(filters?: any) {
    return await ubsRepository.findAll(filters);
  }

  async update(id: string, data: any) {
    const ubsExistente = await ubsRepository.findById(id);

    if (!ubsExistente) {
      throw new AppError('UBS não encontrada', 404);
    }

    // Se alterando código, verifica se já existe
    if (data.codigo && data.codigo !== ubsExistente.codigo) {
      const ubsComCodigo = await ubsRepository.findByCodigo(data.codigo);
      if (ubsComCodigo) {
        throw new AppError('Código de UBS já cadastrado', 400);
      }
    }

    const updateData: any = {};

    if (data.nome) updateData.nome = data.nome;
    if (data.codigo) updateData.codigo = data.codigo;
    if (data.endereco !== undefined) updateData.endereco = data.endereco;
    if (data.bairro !== undefined) updateData.bairro = data.bairro;
    if (data.cep !== undefined) updateData.cep = data.cep;
    if (data.telefone !== undefined) updateData.telefone = data.telefone;
    if (data.email !== undefined) updateData.email = data.email;
    if (data.status) updateData.status = data.status;
    if (data.capacidadeAtendimento !== undefined) {
      updateData.capacidadeAtendimento = data.capacidadeAtendimento;
    }
    if (data.observacoes !== undefined) updateData.observacoes = data.observacoes;

    if (data.coordenadorId !== undefined) {
      if (data.coordenadorId === null) {
        updateData.coordenador = { disconnect: true };
      } else {
        updateData.coordenador = { connect: { id: data.coordenadorId } };
      }
    }

    const ubsAtualizada = await ubsRepository.update(id, updateData);

    logger.info(`UBS atualizada: ${id}`);

    return ubsAtualizada;
  }

  async delete(id: string) {
    const ubs = await ubsRepository.findById(id);

    if (!ubs) {
      throw new AppError('UBS não encontrada', 404);
    }

    // Verifica se há despesas associadas
    if (ubs._count.despesas > 0) {
      throw new AppError(
        'Não é possível excluir UBS com despesas cadastradas. Desative a UBS ao invés disso.',
        400
      );
    }

    await ubsRepository.delete(id);

    logger.info(`UBS deletada: ${id}`);

    return { message: 'UBS deletada com sucesso' };
  }
}
