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
  
  await prisma.usuario.create({
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
    prisma.categoria.create({
      data: {
        nome: 'Medicamentos',
        descricao: 'Compra de medicamentos e drogas',
        tipo: 'MATERIAL',
        orcamentoMensal: 35000,
        cor: '#06b6d4',
        icone: 'pill',
      },
    }),
    prisma.categoria.create({
      data: {
        nome: 'Utilidades Públicas',
        descricao: 'Água, luz, internet e telefone',
        tipo: 'SERVICO',
        orcamentoMensal: 8000,
        cor: '#ec4899',
        icone: 'zap',
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

  const ubs3 = await prisma.uBS.create({
    data: {
      nome: 'UBS Vila Esperança',
      codigo: 'UBS-003',
      endereco: 'Rua Esperança, 250',
      bairro: 'Vila Esperança',
      cep: '12346-000',
      telefone: '(11) 3333-3333',
      email: 'ubs.esperanca@municipio.gov.br',
      status: 'ATIVA',
      capacidadeAtendimento: 650,
    },
  });

  const ubs4 = await prisma.uBS.create({
    data: {
      nome: 'UBS Alto do Morro',
      codigo: 'UBS-004',
      endereco: 'Av. do Morro, 1000',
      bairro: 'Alto do Morro',
      cep: '12346-111',
      telefone: '(11) 3333-4444',
      email: 'ubs.morro@municipio.gov.br',
      status: 'ATIVA',
      capacidadeAtendimento: 900,
    },
  });

  const ubs5 = await prisma.uBS.create({
    data: {
      nome: 'UBS São Benedito',
      codigo: 'UBS-005',
      endereco: 'Rua São Benedito, 750',
      bairro: 'São Benedito',
      cep: '12346-222',
      telefone: '(11) 3333-5555',
      email: 'ubs.benedito@municipio.gov.br',
      status: 'ATIVA',
      capacidadeAtendimento: 700,
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

  const coordenador3 = await prisma.usuario.create({
    data: {
      nome: 'Ana Costa',
      email: 'ana.costa@inovasaude.com.br',
      senhaHash: await bcrypt.hash('senha123', 10),
      perfil: 'COORDENADOR',
      status: 'ATIVO',
      telefone: '(11) 98888-3333',
      ubsId: ubs3.id,
    },
  });

  const coordenador4 = await prisma.usuario.create({
    data: {
      nome: 'Roberto Ferreira',
      email: 'roberto.ferreira@inovasaude.com.br',
      senhaHash: await bcrypt.hash('senha123', 10),
      perfil: 'COORDENADOR',
      status: 'ATIVO',
      telefone: '(11) 98888-4444',
      ubsId: ubs4.id,
    },
  });

  const coordenador5 = await prisma.usuario.create({
    data: {
      nome: 'Juliana Mendes',
      email: 'juliana.mendes@inovasaude.com.br',
      senhaHash: await bcrypt.hash('senha123', 10),
      perfil: 'COORDENADOR',
      status: 'ATIVO',
      telefone: '(11) 98888-5555',
      ubsId: ubs5.id,
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

  await prisma.uBS.update({
    where: { id: ubs3.id },
    data: { coordenadorId: coordenador3.id },
  });

  await prisma.uBS.update({
    where: { id: ubs4.id },
    data: { coordenadorId: coordenador4.id },
  });

  await prisma.uBS.update({
    where: { id: ubs5.id },
    data: { coordenadorId: coordenador5.id },
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

  // Criar auditor
  console.log('🔍 Criando auditor...');
  await prisma.usuario.create({
    data: {
      nome: 'Patricia Ribeiro',
      email: 'patricia.ribeiro@inovasaude.com.br',
      senhaHash: await bcrypt.hash('senha123', 10),
      perfil: 'AUDITOR',
      status: 'ATIVO',
      telefone: '(11) 98888-6666',
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

  await prisma.despesa.create({
    data: {
      descricao: 'Medicamentos essenciais',
      valor: 5000.00,
      dataVencimento: new Date('2024-02-05'),
      categoriaId: categorias[5].id,
      tipo: 'FIXA',
      status: 'PAGA',
      ubsId: ubs3.id,
      fornecedorId: fornecedor1.id,
      usuarioCriacaoId: coordenador3.id,
      usuarioAprovacaoId: gestor.id,
      dataAprovacao: new Date('2024-01-30'),
      dataPagamento: new Date('2024-02-02'),
      numeroNota: 'NF-2024-003',
      observacoes: 'Compra trimestral de medicamentos',
    },
  });

  await prisma.despesa.create({
    data: {
      descricao: 'Água e energia - Janeiro',
      valor: 2800.00,
      dataVencimento: new Date('2024-02-10'),
      categoriaId: categorias[6].id,
      tipo: 'FIXA',
      status: 'PENDENTE',
      ubsId: ubs4.id,
      usuarioCriacaoId: coordenador4.id,
      numeroNota: 'UTIL-2024-001',
      observacoes: 'Despesa com utilidades públicas',
    },
  });

  await prisma.despesa.create({
    data: {
      descricao: 'Contratação de serviço de limpeza',
      valor: 4200.00,
      dataVencimento: new Date('2024-02-20'),
      categoriaId: categorias[2].id,
      tipo: 'FIXA',
      status: 'APROVADA',
      ubsId: ubs5.id,
      fornecedorId: fornecedor2.id,
      usuarioCriacaoId: coordenador5.id,
      usuarioAprovacaoId: gestor.id,
      dataAprovacao: new Date(),
      numeroNota: 'NF-2024-004',
      observacoes: 'Serviço mensal de limpeza e higienização',
    },
  });

  await prisma.despesa.create({
    data: {
      descricao: 'Impressoras e scanners',
      valor: 8500.00,
      dataVencimento: new Date('2024-02-15'),
      categoriaId: categorias[3].id,
      tipo: 'EVENTUAL',
      status: 'PENDENTE',
      ubsId: ubs1.id,
      fornecedorId: fornecedor1.id,
      usuarioCriacaoId: coordenador1.id,
      numeroNota: 'NF-2024-005',
      observacoes: 'Equipamentos para modernizar centro administrativo',
    },
  });

  await prisma.despesa.create({
    data: {
      descricao: 'Materiais de escritório',
      valor: 950.00,
      dataVencimento: new Date('2024-02-25'),
      categoriaId: categorias[1].id,
      tipo: 'VARIAVEL',
      status: 'APROVADA',
      ubsId: ubs2.id,
      fornecedorId: fornecedor2.id,
      usuarioCriacaoId: coordenador2.id,
      usuarioAprovacaoId: gestor.id,
      dataAprovacao: new Date(),
      numeroNota: 'NF-2024-006',
      observacoes: 'Reposição de estoque de materiais',
    },
  });

  await prisma.despesa.create({
    data: {
      descricao: 'Reforma da ala de pediatria',
      valor: 15000.00,
      dataVencimento: new Date('2024-03-15'),
      categoriaId: categorias[4].id,
      tipo: 'EVENTUAL',
      status: 'PENDENTE',
      ubsId: ubs3.id,
      usuarioCriacaoId: coordenador3.id,
      numeroNota: 'NF-2024-007',
      observacoes: 'Obra de manutenção estrutural e pintura',
    },
  });

  console.log('✅ Seed concluído com sucesso!');
  console.log('\n📋 Usuários criados:');
  console.log('  Admin: admin@inovasaude.com.br / admin123');
  console.log('  Coordenador 1: maria.silva@inovasaude.com.br / senha123');
  console.log('  Coordenador 2: joao.santos@inovasaude.com.br / senha123');
  console.log('  Coordenador 3: ana.costa@inovasaude.com.br / senha123');
  console.log('  Coordenador 4: roberto.ferreira@inovasaude.com.br / senha123');
  console.log('  Coordenador 5: juliana.mendes@inovasaude.com.br / senha123');
  console.log('  Gestor: carlos.oliveira@inovasaude.com.br / senha123');
  console.log('  Auditor: patricia.ribeiro@inovasaude.com.br / senha123');
  console.log('\n🏥 UBS criadas: 5');
  console.log('\n📁 Categorias criadas: 7');
  console.log('\n💰 Despesas de exemplo: 8');
  console.log('\n🚀 Sistema pronto para teste!')
}

main()
  .catch((e) => {
    console.error('❌ Erro ao executar seed:', e);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });
