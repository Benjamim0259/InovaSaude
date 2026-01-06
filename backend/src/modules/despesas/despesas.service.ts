import { DespesasRepository } from './despesas.repository';
import { AppError } from '../../shared/middlewares/error.middleware';
import logger from '../../config/logger';

const despesasRepository = new DespesasRepository();

export class DespesasService {
  async create(data: any, usuarioId: string) {
    try {
      const despesa = await despesasRepository.create({
        descricao: data.descricao,
        valor: data.valor,
        dataVencimento: data.dataVencimento ? new Date(data.dataVencimento) : undefined,
        dataPagamento: data.dataPagamento ? new Date(data.dataPagamento) : undefined,
        tipo: data.tipo,
        status: 'PENDENTE',
        observacoes: data.observacoes,
        numeroNota: data.numeroNota,
        numeroEmpenho: data.numeroEmpenho,
        categoria: {
          connect: { id: data.categoriaId },
        },
        ubs: {
          connect: { id: data.ubsId },
        },
        usuarioCriacao: {
          connect: { id: usuarioId },
        },
        ...(data.fornecedorId && {
          fornecedor: {
            connect: { id: data.fornecedorId },
          },
        }),
      });

      // Cria histórico
      await despesasRepository.createHistorico({
        despesaId: despesa.id,
        statusNovo: 'PENDENTE',
        usuarioId,
        observacao: 'Despesa criada',
      });

      logger.info(`Despesa criada: ${despesa.id} por usuário ${usuarioId}`);

      return despesa;
    } catch (error: any) {
      logger.error('Erro ao criar despesa:', error);
      throw new AppError('Erro ao criar despesa: ' + error.message, 500);
    }
  }

  async findById(id: string) {
    const despesa = await despesasRepository.findById(id);

    if (!despesa) {
      throw new AppError('Despesa não encontrada', 404);
    }

    return despesa;
  }

  async findAll(filters: any) {
    return await despesasRepository.findAll(filters);
  }

  async update(id: string, data: any, usuarioId: string) {
    const despesaExistente = await despesasRepository.findById(id);

    if (!despesaExistente) {
      throw new AppError('Despesa não encontrada', 404);
    }

    const updateData: any = {};

    if (data.descricao) updateData.descricao = data.descricao;
    if (data.valor) updateData.valor = data.valor;
    if (data.dataVencimento) updateData.dataVencimento = new Date(data.dataVencimento);
    if (data.dataPagamento) updateData.dataPagamento = new Date(data.dataPagamento);
    if (data.tipo) updateData.tipo = data.tipo;
    if (data.observacoes !== undefined) updateData.observacoes = data.observacoes;
    if (data.numeroNota !== undefined) updateData.numeroNota = data.numeroNota;
    if (data.numeroEmpenho !== undefined) updateData.numeroEmpenho = data.numeroEmpenho;

    if (data.categoriaId) {
      updateData.categoria = { connect: { id: data.categoriaId } };
    }

    if (data.fornecedorId) {
      updateData.fornecedor = { connect: { id: data.fornecedorId } };
    }

    // Atualiza status se fornecido
    if (data.status && data.status !== despesaExistente.status) {
      updateData.status = data.status;

      if (data.status === 'APROVADA') {
        updateData.dataAprovacao = new Date();
        updateData.usuarioAprovacao = { connect: { id: usuarioId } };
      }

      // Cria histórico de mudança de status
      await despesasRepository.createHistorico({
        despesaId: id,
        statusAnterior: despesaExistente.status,
        statusNovo: data.status,
        usuarioId,
        observacao: data.observacaoStatus || 'Status atualizado',
      });
    }

    const despesaAtualizada = await despesasRepository.update(id, updateData);

    logger.info(`Despesa atualizada: ${id} por usuário ${usuarioId}`);

    return despesaAtualizada;
  }

  async delete(id: string, usuarioId: string) {
    const despesa = await despesasRepository.findById(id);

    if (!despesa) {
      throw new AppError('Despesa não encontrada', 404);
    }

    await despesasRepository.delete(id);

    logger.info(`Despesa deletada: ${id} por usuário ${usuarioId}`);

    return { message: 'Despesa deletada com sucesso' };
  }

  async aprovar(id: string, usuarioId: string) {
    return await this.update(id, { status: 'APROVADA' }, usuarioId);
  }

  async rejeitar(id: string, usuarioId: string, observacao?: string) {
    return await this.update(
      id,
      { status: 'REJEITADA', observacaoStatus: observacao },
      usuarioId
    );
  }

  async marcarComoPaga(id: string, usuarioId: string, dataPagamento?: Date) {
    return await this.update(
      id,
      {
        status: 'PAGA',
        dataPagamento: dataPagamento || new Date(),
      },
      usuarioId
    );
  }
}
