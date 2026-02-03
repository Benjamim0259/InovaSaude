# ?? INTEGRAÇÃO COM APIs EXTERNAS - COMPLETO!

## ? O QUE FOI IMPLEMENTADO:

### ?? 1. Modelos de Dados

**Arquivo:** `Models/Integrations/ApiExternaModels.cs`

#### Classes Criadas:

1. **`ApiExterna`** - Configuração das APIs
   - Nome, URL, Autenticação
   - Status, Logs, Métricas
   - Suporte a: Bearer, API Key, OAuth2, Basic Auth

2. **`LogIntegracaoApi`** - Histórico completo
   - Endpoint, Método HTTP, Status Code
   - Tempo de resposta
   - Payload Request/Response
   - Retry tracking

3. **`HorusMedicamento`** - Dados do HORUS
   - Código, Nome, Princípio Ativo
   - Estoque atual e mínimo
   - Sincronização automática

4. **`EsusPecAtendimento`** - Dados do e-SUS PEC
   - CNS do paciente
   - Data, Tipo, Procedimentos
   - CID-10, Profissional

5. **`NemesisIndicador`** - Dados do NEMESIS
   - Código e Nome do indicador
   - Valor numérico/texto
   - Meta e % de alcance

---

### ??? 2. Serviços de Integração

**Arquivo:** `Services/Integrations/ApiExternaServices.cs`

#### `ApiExternaServiceBase` (Classe Base)

**Features:**
- ? Configuração dinâmica de autenticação
- ? Retry automático com exponential backoff
- ? Logging completo de todas as requisições
- ? Atualização automática de status
- ? Métricas de sucesso/erro

**Métodos Principais:**
```csharp
- ObterConfiguracaoAsync() // Busca config por API e UBS
- ConfigurarAutenticacao() // Bearer, ApiKey, OAuth2, Basic
- ExecutarRequisicaoAsync() // Com retry e logging
- RegistrarLogAsync() // Salva histórico completo
- AtualizarStatusApiAsync() // Atualiza métricas
```

#### `HorusIntegrationService`

**Funcionalidades:**
- ? Sincronizar estoque de medicamentos
- ? Identificar medicamentos com estoque baixo
- ? Alertas automáticos

**Endpoints:**
```
GET /api/v1/medicamentos/estoque
```

#### `EsusPecIntegrationService`

**Funcionalidades:**
- ? Sincronizar atendimentos por período
- ? Estatísticas de atendimentos
- ? Pacientes únicos
- ? Atendimentos por tipo

**Endpoints:**
```
GET /api/v1/atendimentos?dataInicio={data}&dataFim={data}
```

#### `NemesisIntegrationService`

**Funcionalidades:**
- ? Sincronizar indicadores
- ? Identificar indicadores fora da meta
- ? Cálculo automático de % de alcance

**Endpoints:**
```
GET /api/v1/indicadores?periodo={periodo}
```

---

### ?? 3. Interface de Gerenciamento

**Arquivo:** `Pages/IntegracoesExternas.razor`

#### Features da Tela:

1. **Cards Visuais por API**
   - ?? HORUS (Verde farmacêutico)
   - ?? e-SUS PEC (Azul hospitalar)
   - ?? NEMESIS (Laranja indicadores)

2. **Status em Tempo Real**
   - ? ATIVA (verde)
   - ?? INATIVA (cinza)
- ? ERRO (vermelho)

3. **Métricas Visuais**
   - Total de sincronizações bem-sucedidas
   - Total de erros
   - Última sincronização
   - Último erro (se houver)

4. **Ações Rápidas**
   - ?? Sincronizar agora
   - ?? Editar configuração
   - ?? Ver logs detalhados

5. **Guia de Integração**
   - Descrição de cada API
   - Funcionalidades disponíveis
   - Dados que são sincronizados

---

## ?? SEGURANÇA:

### Autenticação Suportada:

1. **Bearer Token**
   ```
   Authorization: Bearer {token}
   ```

2. **API Key**
   ```
   X-API-Key: {key}
   ```

3. **OAuth 2.0**
   ```
   Client ID + Client Secret
   ```

4. **Basic Auth**
   ```
   Authorization: Basic base64(username:password)
   ```

### Proteções:

- ? Tokens criptografados no banco
- ? Secrets nunca aparecem nos logs
- ? Timeout configurável
- ? Máximo de retries

---

## ?? FLUXO DE SINCRONIZAÇÃO:

### Exemplo: Sincronizar HORUS

```
1. USUÁRIO CLICA EM "SINCRONIZAR"
   ?
2. BUSCAR CONFIGURAÇÃO DA API
   ?
3. CONFIGURAR AUTENTICAÇÃO
   (Bearer/ApiKey/OAuth2/Basic)
 ?
4. FAZER REQUISIÇÃO
   GET /api/v1/medicamentos/estoque
   ?
5. RETRY AUTOMÁTICO SE ERRO 5XX
   (Exponential backoff: 2s, 4s, 8s)
   ?
6. REGISTRAR LOG
   - Tempo de resposta
   - Status code
   - Payload (truncado)
   ?
7. PROCESSAR DADOS
   - Atualizar medicamentos existentes
   - Criar novos
   ?
8. ATUALIZAR MÉTRICAS
 - Total sincronizações++
   - Última sincronização = Agora
   - Status = ATIVA
   ?
9. ? SUCESSO!
```

---

## ??? ESTRUTURA NO BANCO:

### Tabelas Criadas:

```sql
-- Configurações das APIs
api_externas (
 Id, Nome, BaseUrl, TipoAutenticacao,
    Token, ClientId, ClientSecret,
    Status, UltimaSincronizacao, TotalSincronizacoes,
    TotalErros, UltimoErro, UbsId
)

-- Logs de todas as requisições
log_integracao_api (
    Id, ApiExternaId, Endpoint, MetodoHttp,
    StatusCode, Sucesso, TempoRespostaMs,
    RequestPayload, ResponsePayload,
    MensagemErro, NumeroTentativa, UsuarioId
)

-- Dados sincronizados do HORUS
horus_medicamentos (
    Id, CodigoHorus, Nome, PrincipioAtivo,
    Concentracao, FormaFarmaceutica,
    QuantidadeEstoque, QuantidadeMinima,
    UltimaAtualizacaoHorus, UbsId
)

-- Dados sincronizados do e-SUS PEC
esus_pec_atendimentos (
    Id, IdEsus, CnsPaciente, NomePaciente,
    DataAtendimento, TipoAtendimento,
    ProcedimentosJson, Cid10,
    CnsProfissional, UbsId
)

-- Dados sincronizados do NEMESIS
nemesis_indicadores (
    Id, CodigoIndicador, Nome,
    ValorNumerico, ValorTexto,
    PeriodoReferencia, Meta,
    PercentualAlcance, UbsId
)
```

---

## ?? PRÓXIMOS PASSOS:

### 1. Adicionar ao ApplicationDbContext:

```csharp
// Em ApplicationDbContext.cs
public DbSet<ApiExterna> ApisExternas { get; set; }
public DbSet<LogIntegracaoApi> LogsIntegracaoApi { get; set; }
public DbSet<HorusMedicamento> HorusMedicamentos { get; set; }
public DbSet<EsusPecAtendimento> EsusPecAtendimentos { get; set; }
public DbSet<NemesisIndicador> NemesisIndicadores { get; set; }
```

### 2. Registrar Serviços no Program.cs:

```csharp
// Adicionar HttpClient
builder.Services.AddHttpClient();

// Registrar serviços de integração
builder.Services.AddScoped<HorusIntegrationService>();
builder.Services.AddScoped<EsusPecIntegrationService>();
builder.Services.AddScoped<NemesisIntegrationService>();
```

### 3. Criar Migration:

```bash
cd InovaSaude.Blazor
dotnet ef migrations add AddApiExternasIntegrations
```

### 4. Adicionar ao Menu:

```razor
<!-- Em NavMenu.razor -->
<div class="nav-item px-3">
    <NavLink class="nav-link" href="integracoes-externas">
        <span class="oi oi-cloud-upload" aria-hidden="true"></span> APIs Externas
    </NavLink>
</div>
```

---

## ?? CASOS DE USO:

### 1. Configurar HORUS:

```
1. Acesse "APIs Externas"
2. Clique em "Nova Integração"
3. Selecione "HORUS (Farmácia)"
4. Preencha:
   - URL: https://horus.datasus.gov.br/api
   - Tipo: Bearer Token
   - Token: {seu_token}
   - Timeout: 30s
   - Max Retries: 3
5. Clique em "Salvar"
6. Clique em "Sincronizar"
7. ? Medicamentos sincronizados!
```

### 2. Ver Medicamentos com Estoque Baixo:

```csharp
var medicamentosBaixos = await HorusService
    .ObterMedicamentosEstoqueBaixoAsync(ubsId);

// Criar alerta automático
// Enviar notificação para gestor
```

### 3. Obter Estatísticas de Atendimentos:

```csharp
var stats = await EsusPecService
    .ObterEstatisticasAsync(dataInicio, dataFim, ubsId);

Console.WriteLine($"Total: {stats.TotalAtendimentos}");
Console.WriteLine($"Pacientes Únicos: {stats.PacientesUnicos}");
```

### 4. Monitorar Indicadores:

```csharp
var indicadoresForaDaMeta = await NemesisService
    .ObterIndicadoresForaDaMetaAsync("2025-01", ubsId);

// Gerar relatório
// Alertar coordenador
```

---

## ?? BENEFÍCIOS:

| Feature | Benefício |
|---------|-----------|
| **Sincronização Automática** | Dados sempre atualizados |
| **Retry Inteligente** | Resiliência a falhas temporárias |
| **Logs Detalhados** | Troubleshooting fácil |
| **Múltiplas Autenticações** | Flexibilidade |
| **Métricas em Tempo Real** | Monitoramento de saúde |
| **Por UBS** | Configurações específicas |

---

## ?? DASHBOARD SUGERIDO:

```
???????????????????????????????????????????
?  APIs Externas - Status Geral    ?
???????????????????????????????????????????
?      ?
?  ?? HORUS       ?
?  Status: ? ATIVA    ?
?  Última Sync: 30/01/2025 14:30         ?
?  Sucessos: 150 | Erros: 2              ?
?  [Sincronizar] [Configurar] [Logs]    ?
?            ?
?  ?? e-SUS PEC       ?
?  Status: ? ATIVA    ?
?  Última Sync: 30/01/2025 14:25         ?
?  Sucessos: 89 | Erros: 0    ?
?  [Sincronizar] [Configurar] [Logs]    ?
?   ?
?  ?? NEMESIS        ?
?  Status: ?? INATIVA         ?
?  Última Sync: 28/01/2025 10:00         ?
?  Sucessos: 45 | Erros: 3         ?
?  [Sincronizar] [Configurar] [Logs]  ?
?           ?
???????????????????????????????????????????
```

---

## ?? STATUS:

? **Modelos criados**  
? **Serviços implementados**  
? **Página de gerenciamento**  
? **Logging completo**  
? **Retry automático**  
? **Métricas em tempo real**  
? **Pendente:** Migration + Registro no Program.cs

---

**PRÓXIMO PASSO: Vou completar a implementação com migration e registro dos serviços!** ??
