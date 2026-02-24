# 📊 IMPLEMENTAÇÃO HORUS - CUSTOS DE MEDICAMENTOS POR UBS

## ✅ O QUE FOI IMPLEMENTADO

### 1. **Modelo de Dados Atualizado** 
**Arquivo:** `InovaSaude.Blazor/Models/Integrations/ApiExternaModels.cs`

Adicionados os seguintes campos ao modelo `HorusMedicamento`:
- ✅ `CustoUnitario` (decimal) - Custo unitário do medicamento
- ✅ `CustoTotal` (computed) - Quantidade em estoque * Custo unitário
- ✅ `Lote` (string) - Lote do medicamento
- ✅ `DataValidade` (DateTime) - Data de validade

### 2. **Serviço de Integração Expandido**
**Arquivo:** `InovaSaude.Blazor/Services/Integrations/ApiExternaServices.cs`

Novos métodos adicionados à classe `HorusIntegrationService`:

#### Métodos de Consulta:
- ✅ `ObterMedicamentosPorUbsAsync(string ubsId)` - Lista todos os medicamentos de uma UBS
- ✅ `ObterCustoTotalMedicamentosAsync(string ubsId)` - Calcula custo total de medicamentos
- ✅ `ObterResumoCustosAsync(string ubsId)` - Resumo detalhado de custos de uma UBS
- ✅ `ObterCustosPorTodasUbsAsync()` - Resumo de custos de todas as UBS

#### DTO de Resposta:
- ✅ `HorusCustoResumoDto` - Contém:
  - Total de medicamentos
  - Quantidade total em estoque
  - Custo total
  - Medicamentos com estoque baixo
  - Custo de medicamentos com estoque baixo
  - Última sincronização

### 3. **Interface Web - Página de Custos HORUS**
**Arquivo:** `InovaSaude.Blazor/Pages/HorusCustos.razor`

Página completa com:
- ✅ **Visão Geral**: Tabela com resumo de custos de todas as UBS
- ✅ **Detalhamento por UBS**: Cards com métricas e tabela de medicamentos
- ✅ **Filtros**: Seleção de UBS específica
- ✅ **Busca**: Pesquisa por nome, código HORUS ou princípio ativo
- ✅ **Sincronização**:
  - Sincronizar UBS individual
  - Sincronizar todas as UBS de uma vez
- ✅ **Indicadores Visuais**:
  - Status de estoque (Normal/Baixo)
  - Custos formatados em moeda
  - Data da última sincronização

### 4. **Integração no Menu**
**Arquivo:** `InovaSaude.Blazor/Shared/NavMenu.razor`

- ✅ Link "HORUS Custos" adicionado no menu de navegação
- ✅ Acessível apenas para usuários autenticados

---

## 🗄️ PRÓXIMOS PASSOS (MIGRAÇÃO DE BANCO)

Para ativar completamente a funcionalidade, você precisa executar a migration:

### Comando para criar migration:
```bash
cd InovaSaude.Blazor
dotnet ef migrations add AdicionarCustosHorusMedicamentos
```

### Comando para aplicar ao banco:
```bash
dotnet ef database update
```

### OU se estiver usando PostgreSQL em produção:
```sql
ALTER TABLE horus_medicamentos 
ADD COLUMN "CustoUnitario" numeric(18,2) NOT NULL DEFAULT 0,
ADD COLUMN "Lote" character varying(50),
ADD COLUMN "DataValidade" timestamp with time zone;
```

---

## 📝 COMO USAR

### 1. **Configurar API HORUS**
Primeiro, configure a integração HORUS no banco de dados:

```sql
INSERT INTO apis_externas (
    "Id", "Nome", "BaseUrl", "TipoAutenticacao", 
    "Token", "TimeoutSegundos", "MaxRetries", "Status",
    "CreatedAt", "UpdatedAt"
) VALUES (
    gen_random_uuid()::text,
    'HORUS',
    'https://horus.datasus.gov.br/api',
    'Bearer',
    'SEU_TOKEN_AQUI',
    30,
    3,
    'ATIVA',
    NOW(),
    NOW()
);
```

### 2. **Acessar a Página**
- Faça login no sistema
- Clique em **"HORUS Custos"** no menu lateral
- A página será carregada em: `/horus-custos`

### 3. **Sincronizar Medicamentos**
- **Opção 1**: Clique em "Sincronizar Todas UBS" (botão no topo)
- **Opção 2**: Na tabela de resumo, clique no ícone de sincronização de uma UBS específica

### 4. **Visualizar Custos**
- **Visão Geral**: Deixe o filtro vazio para ver todas as UBS
- **Detalhes de UBS**: Selecione uma UBS no filtro para ver:
  - Cards com métricas (total medicamentos, quantidade, custo total, estoque baixo)
  - Tabela completa de medicamentos com custos

### 5. **Buscar Medicamentos**
- No campo de busca, digite:
  - Nome do medicamento
  - Código HORUS
  - Princípio ativo

---

## 🔗 FLUXO DE DADOS

```
1. API HORUS (DATASUS)
   ↓ (Sincronização)
2. HorusIntegrationService
   ↓ (Salva no banco)
3. Tabela: horus_medicamentos
   ↓ (Consulta)
4. HorusCustos.razor (Interface)
   ↓ (Exibe)
5. Usuário visualiza custos por UBS
```

---

## 📊 MÉTRICAS DISPONÍVEIS

Para cada UBS, você pode ver:
1. **Total de Medicamentos**: Quantidade de itens diferentes
2. **Quantidade Total**: Soma de unidades em estoque
3. **Custo Total**: Valor total do estoque (Qtd × Custo Unit.)
4. **Medicamentos em Estoque Baixo**: Quantidade abaixo do mínimo
5. **Custo de Estoque Baixo**: Valor dos medicamentos com estoque baixo
6. **Última Sincronização**: Data/hora da última atualização

Para cada medicamento:
- Código HORUS
- Nome
- Princípio Ativo
- Quantidade em Estoque
- Estoque Mínimo
- Custo Unitário
- **Custo Total** (calculado automaticamente)
- Data de Validade
- Status (Normal/Estoque Baixo)

---

## ⚠️ OBSERVAÇÕES IMPORTANTES

### Autenticação na API HORUS
O sistema está configurado para usar autenticação Bearer Token. Você precisa:
1. Obter credenciais no DATASUS/HORUS
2. Configurar o token na tabela `apis_externas`
3. A URL base pode precisar ser ajustada conforme a documentação oficial

### Custos dos Medicamentos
Os custos unitários virão da API HORUS. Se a API não fornecer custos:
- O campo `CustoUnitario` será `0` por padrão
- Você pode atualizar manualmente no banco de dados
- Ou implementar uma tela de manutenção de custos

### Performance
- A sincronização de todas as UBS pode demorar se houver muitas UBS
- Considere implementar jobs agendados (background) para sincronização automática
- Use cache se necessário para consultas frequentes

---

## 🚀 MELHORIAS FUTURAS SUGERIDAS

1. **Agendamento Automático**: Job para sincronizar HORUS diariamente
2. **Alertas**: Notificações quando medicamentos estão com estoque baixo
3. **Histórico**: Rastrear alterações de custos ao longo do tempo
4. **Previsão**: Calcular previsão de compras baseado no consumo
5. **Exportação**: Gerar relatórios em PDF/Excel
6. **Dashboard**: Widget no dashboard principal com resumo HORUS

---

## 📞 SUPORTE

Se tiver dúvidas sobre a implementação:
1. Verifique os logs de integração na tabela `log_integracao_api`
2. Confira o status da API na tabela `apis_externas`
3. Teste a sincronização de uma UBS por vez primeiro

---

**Status:** ✅ Implementação concluída
**Data:** 2024
**Desenvolvedor:** GitHub Copilot
