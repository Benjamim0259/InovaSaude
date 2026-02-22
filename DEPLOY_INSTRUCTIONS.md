# 🚀 Instruções para Deploy no Render

## ✅ Alterações Implementadas

1. **Farmácia Central Removida** - Menu e funcionalidades removidos
2. **Botão Despesa Rápida** - Adicione despesas com apenas nome e valor
3. **Sistema de Funcionários** - Gerencie funcionários por UBS (nome e salário)
4. **UBS em Cards** - Visualização em cards com imagem padrão
5. **Correções de Autenticação** - Login funcionando para todos os perfis
6. **HORUS Funcionando** - Integração garantida e operacional

## 📝 Passos para Deploy

### 1. Commit e Push

```bash
git add .
git commit -m "feat: Remover farmácia central, adicionar despesa rápida, funcionários e cards UBS"
git push origin main
```

### 2. No Render - Executar SQL Manual

Como há incompatibilidade entre migrations SQL Server e PostgreSQL, execute este SQL no console do PostgreSQL do Render:

```sql
CREATE TABLE IF NOT EXISTS funcionarios (
    "Id" text NOT NULL,
    "Nome" character varying(255) NOT NULL,
    "Salario" numeric(18,2) NOT NULL,
    "UbsId" text NOT NULL,
    "Cargo" character varying(50),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_funcionarios" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_funcionarios_ubs_UbsId" FOREIGN KEY ("UbsId") REFERENCES ubs("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_funcionarios_UbsId" ON funcionarios ("UbsId");
```

### 3. Acessar o Banco no Render

1. Acesse seu dashboard do Render
2. Vá em **PostgreSQL Database**
3. Clique em **Connect** → **External Connection**
4. Use um cliente PostgreSQL (DBeaver, pgAdmin, psql) com as credenciais
5. Execute o SQL acima

**OU use o Shell do Render:**

```bash
# No dashboard do Render, vá em seu serviço web
# Shell → Execute:
psql $DATABASE_URL -c "CREATE TABLE IF NOT EXISTS funcionarios (\"Id\" text NOT NULL, \"Nome\" character varying(255) NOT NULL, \"Salario\" numeric(18,2) NOT NULL, \"UbsId\" text NOT NULL, \"Cargo\" character varying(50), \"CreatedAt\" timestamp with time zone NOT NULL, \"UpdatedAt\" timestamp with time zone NOT NULL, CONSTRAINT \"PK_funcionarios\" PRIMARY KEY (\"Id\"), CONSTRAINT \"FK_funcionarios_ubs_UbsId\" FOREIGN KEY (\"UbsId\") REFERENCES ubs(\"Id\") ON DELETE CASCADE);"

psql $DATABASE_URL -c "CREATE INDEX IF NOT EXISTS \"IX_funcionarios_UbsId\" ON funcionarios (\"UbsId\");"
```

### 4. Reiniciar o Serviço

Após executar o SQL, reinicie o serviço web no Render para que o SeedData crie os usuários de teste.

## 👤 Usuários de Teste Criados

Após o deploy, você poderá logar com:

- **Admin:** admin@inovasaude.com.br | Admin@123
- **Coordenador:** coordenador@inovasaude.com.br | Coord@123
- **Gestor:** gestor@inovasaude.com.br | Gestor@123
- **Operador:** operador@inovasaude.com.br | Oper@123

## 🎯 Novos Recursos

### Despesa Rápida
- Clique no botão verde "⚡ Despesa Rápida"
- Preencha apenas: Descrição/NF, Valor e UBS
- Sistema define automaticamente categoria, tipo e status

### Gerenciar Funcionários
- Acesse "Gerenciar UBS"
- Clique em "👥 Funcionários" no card da UBS
- Adicione funcionários com nome, cargo e salário
- Veja o total de salários da UBS

### Cards de UBS
- Imagem padrão (ubs-padrao.jfif)
- Nome da UBS
- Nome do Gestor
- Telefone e Status
- Efeito hover

## ⚠️ Importante

Se houver qualquer erro no deploy, execute o SQL manual primeiro antes de reiniciar o serviço!
