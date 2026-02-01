# 🚀 Guia Rápido - InovaSaúde Blazor

## Como Começar Agora

### 1️⃣ Iniciar o Sistema

```bash
cd InovaSaude.Blazor
dotnet run
```

A aplicação vai abrir em: **http://localhost:5163**

### 2️⃣ Fazer Login

Use as credenciais padrão:
- **Email:** admin@inovasaude.com.br
- **Senha:** Admin@123

### 3️⃣ Navegar pelo Sistema

Após o login, você terá acesso a:

#### 📊 Dashboard
- Visualize estatísticas gerais
- Total de UBS cadastradas
- Despesas do mês
- Despesas pendentes
- Últimas atividades do sistema

#### 💰 Despesas
- Cadastrar novas despesas
- Aprovar/Rejeitar despesas
- Anexar documentos
- Visualizar histórico

#### 🏥 UBS (Unidades Básicas de Saúde)
- Cadastrar UBS
- Definir orçamentos
- Atribuir coordenadores
- Gerenciar informações

#### 📈 Relatórios
- Despesas por período
- Despesas por UBS
- Despesas por categoria
- Exportar dados

## ⚙️ Configuração do Banco de Dados

### SQL Server Local

Se estiver usando SQL Server instalado localmente, a connection string padrão funciona:

```json
"Server=localhost;Database=InovaSaude;Trusted_Connection=True;TrustServerCertificate=True"
```

### SQL Server Express

Se estiver usando SQL Server Express, edite o `appsettings.json`:

```json
"Server=localhost\\SQLEXPRESS;Database=InovaSaude;Trusted_Connection=True;TrustServerCertificate=True"
```

### Criar o Banco de Dados

O sistema cria o banco automaticamente no primeiro startup. Mas você também pode criar manualmente:

```bash
# Aplicar migrations
dotnet ef database update

# Verificar status das migrations
dotnet ef migrations list
```

## 👥 Perfis de Usuário

O sistema possui 4 perfis:

1. **Administrador** - Acesso total ao sistema
2. **Coordenador** - Gerencia UBS específicas
3. **Gestor** - Aprova despesas
4. **Operador** - Registra despesas

## 📝 Funcionalidades Principais

### Cadastro de Despesa

1. Acesse "Despesas" no menu
2. Clique em "Nova Despesa"
3. Preencha os dados:
   - UBS
   - Categoria
   - Fornecedor
   - Valor
   - Data de Vencimento
   - Descrição
4. Anexe documentos (opcional)
5. Salve

### Aprovação de Despesa

1. Acesse "Despesas"
2. Filtre por "Pendentes"
3. Clique na despesa
4. Revise os detalhes
5. Clique em "Aprovar" ou "Rejeitar"
6. Adicione um comentário (opcional)

### Gerar Relatório

1. Acesse "Relatórios"
2. Selecione o tipo de relatório
3. Defina o período
4. Aplique filtros (UBS, Categoria, etc)
5. Clique em "Gerar"
6. Exporte para Excel ou PDF

## 🔐 Segurança

### Trocar Senha do Admin

1. Faça login como admin
2. Acesse "Meu Perfil"
3. Clique em "Alterar Senha"
4. Digite a senha atual e a nova senha
5. Confirme

### Criar Novo Usuário

1. Acesse "Configurações" → "Usuários"
2. Clique em "Novo Usuário"
3. Preencha os dados
4. Defina o perfil
5. Atribua permissões
6. Salve

## 🐛 Resolução de Problemas

### Erro: "Cannot connect to SQL Server"

**Solução:**
1. Verifique se o SQL Server está rodando
2. Teste a connection string
3. Verifique as credenciais

```bash
# Verificar serviço SQL Server
Get-Service MSSQLSERVER
```

### Erro: "Database does not exist"

**Solução:**
```bash
dotnet ef database update
```

### Erro: "Unable to resolve service for ApplicationDbContext"

**Solução:** Certifique-se que a connection string está configurada em `appsettings.json`

### Página em branco após login

**Solução:**
1. Limpe o cache do navegador
2. Pressione Ctrl+F5 para forçar reload
3. Tente em modo anônimo

## 📞 Atalhos de Teclado

- `Ctrl + K` → Pesquisa rápida
- `Ctrl + S` → Salvar formulário
- `Esc` → Fechar modal

## 💡 Dicas

1. **Use filtros** - Economize tempo usando os filtros avançados
2. **Exporte dados** - Mantenha backups regulares exportando relatórios
3. **Revise logs** - Monitore o log de auditoria regularmente
4. **Configure webhooks** - Automatize notificações de eventos importantes
5. **Gerencie permissões** - Atribua apenas as permissões necessárias

## 📚 Recursos Adicionais

- [Documentação Completa](README.md)
- [Estrutura do Projeto](PROJECT_STRUCTURE.md)
- [API Reference](API.md)

## 🆘 Precisa de Ajuda?

Entre em contato com o suporte técnico ou consulte a documentação completa.

---

**Última atualização:** Fevereiro 2026
