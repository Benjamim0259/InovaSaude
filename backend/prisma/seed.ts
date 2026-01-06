import { PrismaClient } from '@prisma/client';
import bcrypt from 'bcrypt';

const prisma = new PrismaClient();

async function main() {
  console.log('🌱 Iniciando seed do banco de dados...');

  // Limpar dados existentes (apenas em desenvolvimento)
  if (process.env.NODE_ENV === 'development') {
    console.log('🧹 Limpando dados existentes...');
    await prisma.logAuditoria.deleteMany();
    await prisma.historicoDespesa.deleteMany();
    await prisma.anexo.deleteMany();
    await prisma.despesa.deleteMany();
    await prisma.categoria.deleteMany();
    await prisma.fornecedor.deleteMany();
    await prisma.uBS.deleteMany();
    await prisma.usuario.deleteMany();
  }

  // Criar usuário admin
  console.log('👤 Criando usuário admin...');
  const senhaHash = await bcrypt.hash('admin123', 10);
  
  const admin = await prisma.usuario.create({
    data: {
      nome: 'Administrador',
      email: 'admin@inovasaude.com.br',
      senhaHash,
      perfil: 'ADMIN',
      status: 'ATIVO',
      telefone: '(11) 99999-9999',
    },
  });

  // Criar categorias
  console.log('📁 Criando categorias...');
  const categorias = await Promise.all([
    prisma.categoria.create({
      data: {
        nome: 'Pessoal',
        descricao: 'Despesas com pessoal e folha de pagamento',
        tipo: 'PESSOAL',
        orcamentoMensal: 50000,
        cor: '#3b82f6',
        icone: 'users',
      },
    }),
    prisma.categoria.create({
      data: {
        nome: 'Material de Consumo',
        descricao: 'Materiais de uso diário e consumíveis',
        tipo: 'MATERIAL',
        orcamentoMensal: 15000,
        cor: '#10b981',
        icone: 'package',
      },
    }),
    prisma.categoria.create({
      data: {
        nome: 'Serviços',
        descricao: 'Serviços terceirizados',
        tipo: 'SERVICO',
        orcamentoMensal: 10000,
        cor: '#f59e0b',
        icone: 'briefcase',
      },
    }),
    prisma.categoria.create({
      data: {
        nome: 'Equipamentos',
        descricao: 'Compra e manutenção de equipamentos',
        tipo: 'EQUIPAMENTO',
        orcamentoMensal: 20000,
        cor: '#8b5cf6',
        icone: 'monitor',
      },
    }),
    prisma.categoria.create({
      data: {
        nome: 'Infraestrutura',
        descricao: 'Manutenção de infraestrutura',
        tipo: 'INFRAESTRUTURA',
        orcamentoMensal: 25000,
        cor: '#ef4444',
        icone: 'building',
      },
    }),
  ]);

  // Criar UBS
  console.log('🏥 Criando UBS...');
  const ubs1 = await prisma.uBS.create({
    data: {
      nome: 'UBS Centro',
      codigo: 'UBS-001',
      endereco: 'Rua Principal, 100',
      bairro: 'Centro',
      cep: '12345-678',
      telefone: '(11) 3333-1111',
      email: 'ubs.centro@municipio.gov.br',
      status: 'ATIVA',
      capacidadeAtendimento: 1000,
    },
  });

  const ubs2 = await prisma.uBS.create({
    data: {
      nome: 'UBS Jardim das Flores',
      codigo: 'UBS-002',
      endereco: 'Av. das Flores, 500',
      bairro: 'Jardim das Flores',
      cep: '12345-999',
      telefone: '(11) 3333-2222',
      email: 'ubs.flores@municipio.gov.br',
      status: 'ATIVA',
      capacidadeAtendimento: 800,
    },
  });

  // Criar coordenadores
  console.log('👥 Criando coordenadores...');
  const coordenador1 = await prisma.usuario.create({
    data: {
      nome: 'Maria Silva',
      email: 'maria.silva@inovasaude.com.br',
      senhaHash: await bcrypt.hash('senha123', 10),
      perfil: 'COORDENADOR',
      status: 'ATIVO',
      telefone: '(11) 98888-1111',
      ubsId: ubs1.id,
    },
  });

  const coordenador2 = await prisma.usuario.create({
    data: {
      nome: 'João Santos',
      email: 'joao.santos@inovasaude.com.br',
      senhaHash: await bcrypt.hash('senha123', 10),
      perfil: 'COORDENADOR',
      status: 'ATIVO',
      telefone: '(11) 98888-2222',
      ubsId: ubs2.id,
    },
  });

  // Atualizar UBS com coordenadores
  await prisma.uBS.update({
    where: { id: ubs1.id },
    data: { coordenadorId: coordenador1.id },
  });

  await prisma.uBS.update({
    where: { id: ubs2.id },
    data: { coordenadorId: coordenador2.id },
  });

  // Criar gestor
  console.log('👨‍💼 Criando gestor...');
  const gestor = await prisma.usuario.create({
    data: {
      nome: 'Carlos Oliveira',
      email: 'carlos.oliveira@inovasaude.com.br',
      senhaHash: await bcrypt.hash('senha123', 10),
      perfil: 'GESTOR',
      status: 'ATIVO',
      telefone: '(11) 98888-3333',
    },
  });

  // Criar fornecedores
  console.log('🏢 Criando fornecedores...');
  const fornecedor1 = await prisma.fornecedor.create({
    data: {
      razaoSocial: 'Materiais Médicos Ltda',
      nomeFantasia: 'MedSupply',
      cnpj: '12.345.678/0001-90',
      endereco: 'Rua Comercial, 200',
      cidade: 'São Paulo',
      estado: 'SP',
      telefone: '(11) 4444-1111',
      email: 'contato@medsupply.com.br',
      status: 'ATIVO',
    },
  });

  const fornecedor2 = await prisma.fornecedor.create({
    data: {
      razaoSocial: 'Serviços de Limpeza Silva LTDA',
      nomeFantasia: 'LimpClin',
      cnpj: '98.765.432/0001-10',
      endereco: 'Av. Industrial, 1500',
      cidade: 'São Paulo',
      estado: 'SP',
      telefone: '(11) 4444-2222',
      email: 'contato@limpclin.com.br',
      status: 'ATIVO',
    },
  });

  // Criar despesas de exemplo
  console.log('💰 Criando despesas de exemplo...');
  await prisma.despesa.create({
    data: {
      descricao: 'Material de limpeza - Janeiro',
      valor: 1250.00,
      dataVencimento: new Date('2024-01-31'),
      categoriaId: categorias[1].id,
      tipo: 'VARIAVEL',
      status: 'PENDENTE',
      ubsId: ubs1.id,
      fornecedorId: fornecedor2.id,
      usuarioCriacaoId: coordenador1.id,
      numeroNota: 'NF-2024-001',
      observacoes: 'Compra mensal de materiais de limpeza',
    },
  });

  await prisma.despesa.create({
    data: {
      descricao: 'Manutenção equipamentos médicos',
      valor: 3500.00,
      dataVencimento: new Date('2024-01-15'),
      categoriaId: categorias[3].id,
      tipo: 'EVENTUAL',
      status: 'APROVADA',
      ubsId: ubs2.id,
      fornecedorId: fornecedor1.id,
      usuarioCriacaoId: coordenador2.id,
      usuarioAprovacaoId: gestor.id,
      dataAprovacao: new Date(),
      numeroNota: 'NF-2024-002',
      observacoes: 'Manutenção preventiva anual',
    },
  });

  console.log('✅ Seed concluído com sucesso!');
  console.log('\n📋 Usuários criados:');
  console.log('  Admin: admin@inovasaude.com.br / admin123');
  console.log('  Coordenador 1: maria.silva@inovasaude.com.br / senha123');
  console.log('  Coordenador 2: joao.santos@inovasaude.com.br / senha123');
  console.log('  Gestor: carlos.oliveira@inovasaude.com.br / senha123');
}

main()
  .catch((e) => {
    console.error('❌ Erro ao executar seed:', e);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });
