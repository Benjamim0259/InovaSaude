# 🚀 GUIA RÁPIDO - HORUS Custos

## O que foi implementado?

✅ **Sistema completo de custos de medicamentos HORUS por UBS**

O sistema agora:
1. Sincroniza medicamentos da API HORUS do DATASUS
2. Armazena custos unitários e totais
3. Calcula automaticamente valor do estoque
4. Identifica medicamentos com estoque baixo
5. Exibe relatórios visuais por UBS

---

## 📝 Passos para Ativar

### 1️⃣ Atualizar o Banco de Dados
Execute o script SQL:
```bash
psql -d seu_banco -f Migrations/SQL/add_horus_custos.sql
```

Ou copie e cole o conteúdo do arquivo no pgAdmin.

### 2️⃣ Configurar Credenciais HORUS
Execute:
```bash
psql -d seu_banco -f Migrations/SQL/configurar_horus.sql
```

**IMPORTANTE:** Edite o arquivo e substitua `SEU_TOKEN_AQUI` pelo token real do DATASUS.

### 3️⃣ Acessar a Página
1. Execute o projeto: `dotnet run`
2. Faça login no sistema
3. Clique em **"HORUS Custos"** no menu
4. Pronto! 🎉

---

## 🎯 Como Usar

### Visualizar Todas as UBS
- Deixe o filtro vazio
- Você verá uma tabela com resumo de custos de todas as UBS

### Ver Detalhes de Uma UBS
- Selecione a UBS no filtro
- Você verá:
  - 📊 Cards com métricas
  - 📋 Tabela completa de medicamentos
  - 💰 Custos unitários e totais
  - ⚠️ Alertas de estoque baixo

### Sincronizar Medicamentos
**Opção 1:** Botão "Sincronizar Todas UBS" (no topo)
**Opção 2:** Ícone de sincronização em cada linha da tabela

### Buscar Medicamentos
Digite no campo de busca:
- Nome do medicamento
- Código HORUS
- Princípio ativo

---

## 📊 Informações Disponíveis

### Por UBS:
- Total de medicamentos diferentes
- Quantidade total em estoque
- **Custo total do estoque**
- Medicamentos com estoque baixo
- Custo dos medicamentos com estoque baixo

### Por Medicamento:
- Código HORUS
- Nome e princípio ativo
- Quantidade em estoque
- Estoque mínimo
- **Custo unitário**
- **Custo total** (calculado)
- Data de validade
- Status (Normal/Estoque Baixo)

---

## ⚠️ Importante

### Sobre a API HORUS
- A URL base configurada é: `https://horus.datasus.gov.br/api/v1`
- Você precisa obter credenciais no DATASUS
- Verifique a documentação oficial da API HORUS

### Custos dos Medicamentos
- Os custos vêm da API HORUS
- Se a API não retornar custos, o valor será R$ 0,00
- Você pode atualizar custos manualmente no banco

### Performance
- Sincronizar todas as UBS pode demorar
- Recomendado: Configure um job para sincronizar à noite
- A página carrega rápido após a primeira sincronização

---

## 🔍 Resolução de Problemas

### Erro ao Sincronizar
1. Verifique se o token está correto em `apis_externas`
2. Confirme se a URL da API está acessível
3. Veja os logs em `log_integracao_api`

### Custos em Zero
- A API HORUS pode não retornar custos para todos medicamentos
- Atualize manualmente:
```sql
UPDATE horus_medicamentos 
SET "CustoUnitario" = 1.50 
WHERE "CodigoHorus" = 'MED123456';
```

### Dados Não Aparecem
- Primeiro, sincronize pelo menos uma UBS
- Recarregue a página
- Verifique se há medicamentos no banco:
```sql
SELECT COUNT(*) FROM horus_medicamentos;
```

---

## 📁 Arquivos Criados/Modificados

### Novos Arquivos:
- ✅ `InovaSaude.Blazor/Pages/HorusCustos.razor` - Interface web
- ✅ `Migrations/SQL/add_horus_custos.sql` - Script de migração
- ✅ `Migrations/SQL/configurar_horus.sql` - Script de configuração
- ✅ `HORUS_IMPLEMENTACAO.md` - Documentação completa

### Arquivos Modificados:
- ✅ `InovaSaude.Blazor/Models/Integrations/ApiExternaModels.cs` - Modelo atualizado
- ✅ `InovaSaude.Blazor/Services/Integrations/ApiExternaServices.cs` - Novos métodos
- ✅ `InovaSaude.Blazor/Shared/NavMenu.razor` - Link no menu

---

## 📞 Precisa de Ajuda?

### Consulte:
1. `HORUS_IMPLEMENTACAO.md` - Documentação técnica completa
2. Logs do sistema em `log_integracao_api`
3. Status das APIs em `apis_externas`

### Comandos Úteis:
```sql
-- Ver status da integração HORUS
SELECT * FROM apis_externas WHERE "Nome" = 'HORUS';

-- Ver últimos erros
SELECT * FROM log_integracao_api 
WHERE "Sucesso" = false 
ORDER BY "CreatedAt" DESC 
LIMIT 5;

-- Ver resumo de medicamentos
SELECT 
    u."Nome",
    COUNT(m."Id") as medicamentos,
    SUM(m."CustoTotal") as custo_total
FROM horus_medicamentos m
JOIN ubs u ON m."UbsId" = u."Id"
GROUP BY u."Nome";
```

---

**Pronto! Agora você pode gerenciar os custos de medicamentos HORUS de todas as UBS! 🎉**
