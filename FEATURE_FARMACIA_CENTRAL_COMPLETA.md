# ???? FARMÁCIA CENTRAL - SISTEMA COMPLETO IMPLEMENTADO!

## ? O QUE FOI CRIADO:

### ?? 1. MODELOS DE DADOS (4 Classes):

#### `PedidoMedicamento`
**Fluxo completo de pedidos das UBS**
- Número do pedido automático (PED-2025-000001)
- UBS solicitante
- Status: PENDENTE ? APROVADO ? ENTREGUE
- Prioridade: NORMAL, URGENTE, CRÍTICA
- Usuários: Criação, Aprovação, Entrega
- Datas: Pedido, Necessidade, Aprovação, Entrega

#### `ItemPedidoMedicamento`
**Itens individuais do pedido**
- Nome do medicamento
- Princípio ativo, concentração, forma farmacêutica
- Quantidade: Solicitada vs Aprovada vs Entregue
- Justificativa (quando aprovado < solicitado)
- Código HORUS (integração)

#### `EstoqueFarmacia`
**Controle de estoque da Farmácia Central**
- Medicamento completo (nome, princípio, concentração)
- Código HORUS, Lote, Validade
- Quantidade: Atual, Mínima, Máxima
- Localização física
- Status: DISPONIVEL, BLOQUEADO, VENCIDO, BAIXO_ESTOQUE
- Última movimentação

#### `MovimentacaoEstoque`
**Histórico completo de movimentações**
- Tipo: ENTRADA, SAIDA, AJUSTE, PERDA, VENCIMENTO
- Quantidade anterior e após
- Motivo e número do documento
- Pedido relacionado (se for saída)
- Usuário responsável

---

### ??? 2. SERVIÇOS (2 Classes):

#### `PedidoMedicamentoService`

**Funcionalidades:**
1. ? **Criar Pedido**
   - Geração automática de número
   - Múltiplos itens
   - Prioridade configurável

2. ? **Aprovar Pedido**
   - Verificação de estoque disponível
   - Ajuste de quantidades
   - Justificativa automática se < solicitado

3. ? **Rejeitar Pedido**
   - Motivo obrigatório
   - Histórico registrado

4. ? **Entregar Pedido**
   - Baixa automática no estoque
   - Movimentação registrada
   - Alerta se estoque baixo

5. ? **Estatísticas**
   - Total de pedidos
   - Por status (pendente, aprovado, entregue, rejeitado)
   - Tempo médio de aprovação

#### `EstoqueFarmaciaService`

**Funcionalidades:**
1. ? **Gerenciar Estoque**
   - Adicionar medicamentos (entrada)
   - Ajustar quantidades
   - Registrar perdas

2. ? **Alertas Automáticos**
   - Estoque baixo (quantidade <= mínima)
   - Próximos ao vencimento (90 dias)
   - Medicamentos vencidos

3. ? **Movimentações**
   - Histórico completo
   - Rastreabilidade total
 - Auditoria

4. ? **Estatísticas**
   - Total de medicamentos
   - Disponíveis vs Bloqueados
   - Próximos ao vencimento

---

## ?? FLUXO COMPLETO DE UM PEDIDO:

```
???????????????????????????????????????????????
?  1. UBS SOLICITA MEDICAMENTOS         ?
???????????????????????????????????????????????
?  - Enfermeiro acessa sistema                ?
?  - Cria pedido: PED-2025-000001 ?
?  - Adiciona itens:           ?
?    • Dipirona 500mg - 1000 comprimidos      ?
?    • Paracetamol 750mg - 500 comprimidos    ?
?  - Define prioridade: URGENTE ?
?  - Data necessidade: 05/02/2025             ?
?  - Status: PENDENTE ?        ?
???????????????????????????????????????????????
            ?
???????????????????????????????????????????????
?  2. FARMÁCIA CENTRAL RECEBE ?
???????????????????????????????????????????????
?  - Farmacêutico vê novo pedido       ?
?  - Verifica estoque disponível:        ?
?    • Dipirona: 5000 em estoque ?           ?
?    • Paracetamol: 300 em estoque ??         ?
?  - Aprova com ajuste:  ?
?    • Dipirona: 1000 (100%)            ?
?    • Paracetamol: 300 (60%)                 ?
?      Justificativa: "Estoque insuficiente"  ?
?  - Status: APROVADO ?    ?
???????????????????????????????????????????????
             ?
???????????????????????????????????????????????
?  3. SEPARAÇÃO E ENTREGA          ?
???????????????????????????????????????????????
?  - Auxiliar separa medicamentos             ?
?  - Marca como ENTREGUE    ?
?  - Sistema automaticamente:          ?
?    • Dá baixa no estoque: ?
?      - Dipirona: 5000 ? 4000  ?
?  - Paracetamol: 300 ? 0 ?? ZERADO!    ?
?    • Cria movimentações de SAIDA   ?
?    • Registra documento: PED-2025-000001    ?
?    • Alerta: Paracetamol em falta!   ?
?  - Status: ENTREGUE ??           ?
???????????????????????????????????????????????
 ?
???????????????????????????????????????????????
?  4. UBS RECEBE E CONFIRMA      ?
???????????????????????????????????????????????
?  - Enfermeiro vê pedido entregue            ?
?  - Confere quantidades recebidas            ?
?  - Atualiza estoque local da UBS      ?
?  - ? PROCESSO COMPLETO!           ?
???????????????????????????????????????????????
```

---

## ?? RELATÓRIOS DE CUSTOS POR UBS:

### Agora você pode ver:

```csharp
// Total gasto por UBS
var despesasUBS = despesas
    .Where(d => d.UbsId == ubsId)
    .Sum(d => d.Valor);

// Despesas por categoria em cada UBS
var despesasPorCategoria = despesas
.Where(d => d.UbsId == ubsId)
    .GroupBy(d => d.Categoria.Nome)
    .Select(g => new {
        Categoria = g.Key,
        Total = g.Sum(d => d.Valor)
    });

// Ranking de UBS por gasto
var rankingUBS = despesas
    .GroupBy(d => d.Ubs.Nome)
    .Select(g => new {
        UBS = g.Key,
        TotalGasto = g.Sum(d => d.Valor),
        TotalDespesas = g.Count()
    })
    .OrderByDescending(x => x.TotalGasto);
```

### Exemplo de Relatório:

```
????????????????????????????????????????????
?CUSTOS POR UBS - JANEIRO/2025       ?
????????????????????????????????????????????
?       ?
?  ?? UBS CENTRAL             ?
?  ?? ?? Medicamentos: R$ 25.000,00       ?
?  ?? ?? Material Médico: R$ 15.000,00    ?
?  ?? ?? Contas Fixas: R$ 5.000,00        ?
?  ?? ?? Pessoal: R$ 80.000,00            ?
?  TOTAL: R$ 125.000,00         ?
?         ?
?  ?? UBS NORTE         ?
?  ?? ?? Medicamentos: R$ 18.000,00       ?
?  ?? ?? Material Médico: R$ 12.000,00    ?
?  ?? ?? Contas Fixas: R$ 4.500,00 ?
?  ?? ?? Pessoal: R$ 60.000,00   ?
?  TOTAL: R$ 94.500,00    ?
?            ?
?  ?? UBS SUL               ?
?  ?? ?? Medicamentos: R$ 22.000,00       ?
?  ?? ?? Material Médico: R$ 10.000,00    ?
??? ?? Contas Fixas: R$ 4.000,00        ?
?  ?? ?? Pessoal: R$ 70.000,00          ?
?  TOTAL: R$ 106.000,00         ?
?    ?
????????????????????????????????????????????

TOTAL GERAL: R$ 325.500,00
```

---

## ?? EXEMPLO DE USO - FARMÁCIA CENTRAL:

### 1. Adicionar Medicamento ao Estoque:

```csharp
await EstoqueFarmaciaService.AdicionarEstoqueAsync(
    nomeMedicamento: "Dipirona Sódica 500mg",
    quantidade: 10000,
    usuarioId: farmaceuticoId,
    principioAtivo: "Dipirona Sódica",
    concentracao: "500mg",
    formaFarmaceutica: "Comprimido",
    lote: "LOT-2025-001",
    dataValidade: new DateTime(2027, 12, 31),
    localizacao: "Prateleira A - Nível 2",
 quantidadeMinima: 1000,
    numeroDocumento: "NF-123456",
    motivo: "Compra mensal de medicamentos"
);
```

### 2. UBS Criar Pedido:

```csharp
var itens = new List<ItemPedidoMedicamento>
{
    new() {
        NomeMedicamento = "Dipirona Sódica 500mg",
        QuantidadeSolicitada = 1000
    },
    new() {
        NomeMedicamento = "Paracetamol 750mg",
   QuantidadeSolicitada = 500
    }
};

var pedido = await PedidoMedicamentoService.CriarPedidoAsync(
    ubsId: ubsCentralId,
    usuarioId: enfermeiroId,
    itens: itens,
    observacoes: "Necessário para campanha de vacinação",
    dataNecessidade: DateTime.Now.AddDays(7),
    prioridade: "URGENTE"
);

// Retorna: PED-2025-000001
```

### 3. Farmacêutico Aprovar:

```csharp
var quantidadesAprovadas = new Dictionary<string, int>
{
    { item1Id, 1000 },  // Dipirona - aprovado 100%
    { item2Id, 300 }    // Paracetamol - aprovado 60% (estoque baixo)
};

await PedidoMedicamentoService.AprovarPedidoAsync(
    pedidoId: pedido.Id,
    usuarioId: farmaceuticoId,
    quantidadesAprovadas: quantidadesAprovadas
);
```

### 4. Entregar e Dar Baixa:

```csharp
await PedidoMedicamentoService.EntregarPedidoAsync(
    pedidoId: pedido.Id,
    usuarioId: auxiliarFarmaciaId
);

// Automático:
// - Baixa no estoque
// - Movimentação de SAIDA
// - Alerta se estoque baixo
```

### 5. Alertas Automáticos:

```csharp
// Estoque baixo
var estoqueBaixo = await EstoqueFarmaciaService.ObterEstoqueBaixoAsync();
// Retorna: Paracetamol (0 unidades) ??

// Próximos ao vencimento
var proximosVencimento = await EstoqueFarmaciaService
    .ObterProximosVencimentoAsync(diasAlerta: 90);
// Retorna: Insulina (vence em 45 dias) ??
```

---

## ??? ESTRUTURA NO BANCO:

### Tabelas Criadas:

```sql
-- Pedidos de medicamentos
pedidos_medicamentos (
    Id, NumeroPedido, UbsSolicitanteId,
    UsuarioCriacaoId, DataPedido, DataNecessidade,
    Status, Prioridade, Observacoes,
    UsuarioAprovacaoId, DataAprovacao,
    UsuarioEntregaId, DataEntrega,
    MotivoRejeicao
)

-- Itens dos pedidos
itens_pedido_medicamento (
    Id, PedidoMedicamentoId, NomeMedicamento,
    PrincipioAtivo, Concentracao, FormaFarmaceutica,
    QuantidadeSolicitada, QuantidadeAprovada,
    QuantidadeEntregue, Justificativa, CodigoHorus
)

-- Estoque da Farmácia Central
estoque_farmacia (
    Id, NomeMedicamento, PrincipioAtivo,
    Concentracao, FormaFarmaceutica, CodigoHorus,
 Lote, DataValidade, QuantidadeAtual,
    QuantidadeMinima, QuantidadeMaxima,
    Localizacao, Status, UltimaMovimentacao
)

-- Movimentações de estoque
movimentacoes_estoque (
    Id, EstoqueFarmaciaId, Tipo,
    Quantidade, QuantidadeAnterior, QuantidadeApos,
    Motivo, NumeroDocumento, PedidoMedicamentoId,
    UsuarioId
)
```

---

## ?? PRÓXIMOS PASSOS:

### 1. Atualizar ApplicationDbContext:

```csharp
// Adicionar no ApplicationDbContext.cs:
public DbSet<PedidoMedicamento> PedidosMedicamentos { get; set; }
public DbSet<ItemPedidoMedicamento> ItensPedidoMedicamento { get; set; }
public DbSet<EstoqueFarmacia> EstoqueFarmacia { get; set; }
public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
```

### 2. Registrar Serviços no Program.cs:

```csharp
// Adicionar:
builder.Services.AddScoped<PedidoMedicamentoService>();
builder.Services.AddScoped<EstoqueFarmaciaService>();
```

### 3. Criar Migration:

```bash
dotnet ef migrations add AddFarmaciaCentral
```

### 4. Criar UBS Especial "Farmácia Central":

```csharp
// No SeedData.cs ou manual:
var farmaciaCentral = new UBS
{
    Nome = "Farmácia Central",
    Codigo = "FARM-CENTRAL",
    Endereco = "Rua da Farmácia, 100",
    Bairro = "Centro",
    Cep = "00000-000",
    Status = "ATIVA",
    Observacoes = "UBS especial responsável pelo gerenciamento " +
 "centralizado de medicamentos para todas as unidades"
};
```

---

## ?? STATUS FINAL:

? **4 Modelos criados** (Pedido, Item, Estoque, Movimentação)  
? **2 Serviços implementados** (Pedido, Estoque)  
? **Sistema completo de pedidos** UBS ? Farmácia  
? **Controle de estoque** com alertas  
? **Rastreabilidade total** (movimentações)  
? **Aprovação inteligente** (verifica estoque)  
? **Baixa automática** ao entregar  
? **Relatórios de custos** por UBS  
? **Pendente:** Interface web (páginas Razor)  

---

**PRÓXIMO PASSO: Vou criar as páginas web para gerenciar tudo isso visualmente! ??**

Quer que eu crie agora as páginas para:
1. ?? **Listar e criar pedidos** (para UBS)
2. ? **Aprovar pedidos** (para Farmácia Central)
3. ?? **Gerenciar estoque** (Farmácia Central)
4. ?? **Dashboard de custos** por UBS
