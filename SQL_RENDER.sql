-- ====================================
-- SQL PARA EXECUTAR NO RENDER (SE NECESSÁRIO)
-- ====================================

-- 1. CRIAR TABELA FUNCIONARIOS
-- Execute isso se a tabela não for criada automaticamente
CREATE TABLE IF NOT EXISTS funcionarios (
    "Id" text NOT NULL,
    "Nome" character varying(255) NOT NULL,
    "Salario" numeric(18,2) NOT NULL,
    "UbsId" text NOT NULL,
    "Cargo" character varying(50),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_funcionarios" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_funcionarios_ubs_UbsId" FOREIGN KEY ("UbsId") 
        REFERENCES ubs("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_funcionarios_UbsId" ON funcionarios ("UbsId");

-- 2. VERIFICAR SE TABELA FOI CRIADA
SELECT * FROM funcionarios LIMIT 5;

-- 3. VERIFICAR USUÁRIOS CRIADOS
SELECT "Id", "Nome", "Email", "Perfil", "Status" FROM usuarios;

-- 4. CRIAR USUÁRIO ADICIONAL (SE NECESSÁRIO)
-- Exemplo: Criar um novo gestor
/*
INSERT INTO usuarios ("Id", "Nome", "Email", "SenhaHash", "Perfil", "Status", "CreatedAt", "UpdatedAt")
VALUES (
    gen_random_uuid()::text,
    'Novo Gestor',
    'gestor2@inovasaude.com.br',
    '$2a$11$exemplo_hash_bcrypt_aqui',  -- Use BCrypt online para gerar
    'GESTOR',
    'ATIVO',
    NOW(),
    NOW()
);
*/

-- 5. VERIFICAR UBS CRIADAS
SELECT "Id", "Nome", "Codigo", "Status" FROM ubs;

-- 6. LISTAR FUNCIONÁRIOS POR UBS
SELECT f."Nome", f."Cargo", f."Salario", u."Nome" as "NomeUBS"
FROM funcionarios f
INNER JOIN ubs u ON f."UbsId" = u."Id"
ORDER BY u."Nome", f."Nome";

-- 7. VERIFICAR DESPESAS
SELECT "Id", "Descricao", "Valor", "Status", "CreatedAt" 
FROM despesas 
ORDER BY "CreatedAt" DESC 
LIMIT 10;

-- 8. LIMPAR DADOS DE TESTE (CUIDADO!)
-- Execute apenas se quiser resetar tudo
/*
TRUNCATE TABLE funcionarios CASCADE;
DELETE FROM despesas WHERE "Status" = 'PENDENTE';
*/

-- ====================================
-- COMO EXECUTAR NO RENDER
-- ====================================

-- Opção 1: Via Render Shell
-- 1. Vá no dashboard do Render
-- 2. Clique no seu serviço web
-- 3. Clique em "Shell" no menu superior
-- 4. Execute: psql $DATABASE_URL
-- 5. Cole os comandos SQL acima

-- Opção 2: Via Cliente PostgreSQL Externo (DBeaver, pgAdmin, etc)
-- 1. No Render Dashboard → PostgreSQL Database → Connect
-- 2. Copie as credenciais (External Connection)
-- 3. Conecte com seu cliente favorito
-- 4. Execute os comandos SQL
