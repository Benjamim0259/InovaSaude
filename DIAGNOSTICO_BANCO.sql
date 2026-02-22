-- ============================================
-- DIAGNÓSTICO E CORREÇÃO COMPLETA DO BANCO
-- Execute estes comandos NO RENDER SHELL
-- ============================================

-- ===== PASSO 1: VERIFICAR ESTRUTURA ATUAL =====
\dt

-- ===== PASSO 2: VERIFICAR SE FUNCIONARIOS EXISTE =====
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name = 'funcionarios';

-- ===== PASSO 3: CRIAR TABELA FUNCIONARIOS SE NÃO EXISTIR =====
CREATE TABLE IF NOT EXISTS funcionarios (
    "Id" text NOT NULL,
    "Nome" character varying(255) NOT NULL,
    "Salario" numeric(18,2) NOT NULL,
    "UbsId" text NOT NULL,
    "Cargo" character varying(50),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_funcionarios" PRIMARY KEY ("Id")
);

-- ===== PASSO 4: VERIFICAR SE UBS EXISTE =====
SELECT "Id", "Nome", "Codigo" FROM ubs LIMIT 5;

-- ===== PASSO 5: ADICIONAR FOREIGN KEY FUNCIONARIOS -> UBS =====
-- Só execute se a tabela funcionarios já existir mas não tiver a FK
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'FK_funcionarios_ubs_UbsId'
    ) THEN
        ALTER TABLE funcionarios 
        ADD CONSTRAINT "FK_funcionarios_ubs_UbsId" 
        FOREIGN KEY ("UbsId") REFERENCES ubs("Id") ON DELETE CASCADE;
    END IF;
END $$;

-- ===== PASSO 6: CRIAR ÍNDICE =====
CREATE INDEX IF NOT EXISTS "IX_funcionarios_UbsId" ON funcionarios ("UbsId");

-- ===== PASSO 7: VERIFICAR USUÁRIOS =====
SELECT "Id", "Nome", "Email", "Perfil", "Status" FROM usuarios;

-- ===== PASSO 8: SE NÃO HOUVER USUÁRIOS, CRIAR ADMIN =====
INSERT INTO usuarios ("Id", "Nome", "Email", "SenhaHash", "Perfil", "Status", "CreatedAt", "UpdatedAt")
SELECT 
    gen_random_uuid()::text,
    'Administrador',
    'admin@inovasaude.com.br',
    '$2a$11$YourBCryptHashHere', -- Será substituído pelo SeedData.cs
    'ADMIN',
    'ATIVO',
    NOW(),
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM usuarios WHERE "Email" = 'admin@inovasaude.com.br');

-- ===== PASSO 9: VERIFICAR CATEGORIAS =====
SELECT "Id", "Nome" FROM categorias LIMIT 5;

-- ===== PASSO 10: SE NÃO HOUVER CATEGORIAS, CRIAR UMA BÁSICA =====
INSERT INTO categorias ("Id", "Nome", "Tipo", "Descricao", "OrcamentoMensal", "Cor", "Icone", "CreatedAt", "UpdatedAt")
SELECT 
    gen_random_uuid()::text,
    'Outros',
    'DESPESA',
    'Outras despesas não categorizadas',
    10000.00,
    '#6c757d',
    '📋',
    NOW(),
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM categorias WHERE "Nome" = 'Outros');

-- ===== PASSO 11: VERIFICAR TODAS AS TABELAS NECESSÁRIAS =====
SELECT 
    table_name,
    (SELECT COUNT(*) FROM information_schema.columns WHERE table_name = t.table_name) as column_count
FROM information_schema.tables t
WHERE table_schema = 'public'
AND table_name IN ('usuarios', 'ubs', 'funcionarios', 'categorias', 'despesas')
ORDER BY table_name;

-- ===== PASSO 12: TESTAR INSERÇÃO DE FUNCIONÁRIO (APÓS CRIAR TABELA) =====
-- Substitua o UbsId pelo ID de uma UBS existente
/*
INSERT INTO funcionarios ("Id", "Nome", "Salario", "UbsId", "Cargo", "CreatedAt", "UpdatedAt")
VALUES (
    gen_random_uuid()::text,
    'Funcionário Teste',
    3500.00,
    (SELECT "Id" FROM ubs LIMIT 1),
    'Enfermeiro',
    NOW(),
    NOW()
);
*/

-- ===== PASSO 13: VERIFICAR SE FUNCIONOU =====
SELECT 
    f."Nome" as "Funcionario",
    f."Cargo",
    f."Salario",
    u."Nome" as "UBS"
FROM funcionarios f
LEFT JOIN ubs u ON f."UbsId" = u."Id";
