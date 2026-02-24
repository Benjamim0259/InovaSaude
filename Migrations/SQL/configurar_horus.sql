-- ============================================
-- CONFIGURAÇÃO DA INTEGRAÇÃO HORUS
-- ============================================

-- 1. INSERIR CONFIGURAÇÃO DA API HORUS
-- Substitua 'SEU_TOKEN_AQUI' pelo token real fornecido pelo DATASUS

INSERT INTO apis_externas (
    "Id", 
    "Nome", 
    "BaseUrl", 
    "TipoAutenticacao", 
    "Token", 
    "TimeoutSegundos", 
    "MaxRetries", 
    "Status",
    "TotalSincronizacoes",
    "TotalErros",
    "CreatedAt", 
    "UpdatedAt"
) VALUES (
    gen_random_uuid()::text,
    'HORUS',
    'https://horus.datasus.gov.br/api/v1',
    'Bearer',
    'SEU_TOKEN_AQUI',  -- 🔑 Substitua pelo token real
    30,
    3,
    'ATIVA',
    0,
    0,
    NOW(),
    NOW()
)
ON CONFLICT DO NOTHING;

-- 2. VERIFICAR SE A CONFIGURAÇÃO FOI CRIADA
SELECT 
    "Id",
    "Nome",
    "BaseUrl",
    "TipoAutenticacao",
    "Status",
    "UltimaSincronizacao",
    "TotalSincronizacoes",
    "TotalErros"
FROM apis_externas
WHERE "Nome" = 'HORUS';

-- ============================================
-- CONFIGURAÇÕES POR UBS (Opcional)
-- ============================================

-- Se cada UBS tiver um token/credencial diferente no HORUS:

/*
-- Para UBS específica
INSERT INTO apis_externas (
    "Id", 
    "Nome", 
    "BaseUrl", 
    "TipoAutenticacao", 
    "Token", 
    "TimeoutSegundos", 
    "MaxRetries", 
    "Status",
    "UbsId",  -- Vincula à UBS específica
    "CreatedAt", 
    "UpdatedAt"
) VALUES (
    gen_random_uuid()::text,
    'HORUS',
    'https://horus.datasus.gov.br/api/v1',
    'Bearer',
    'TOKEN_DA_UBS_CENTRO',
    30,
    3,
    'ATIVA',
    (SELECT "Id" FROM ubs WHERE "Codigo" = 'UBS001'),  -- ID da UBS
    NOW(),
    NOW()
);
*/

-- ============================================
-- OUTRAS INTEGRAÇÕES (e-SUS PEC, NEMESIS)
-- ============================================

-- e-SUS PEC (Prontuário Eletrônico do Cidadão)
INSERT INTO apis_externas (
    "Id", 
    "Nome", 
    "BaseUrl", 
    "TipoAutenticacao", 
    "Token", 
    "TimeoutSegundos", 
    "MaxRetries", 
    "Status",
    "CreatedAt", 
    "UpdatedAt"
) VALUES (
    gen_random_uuid()::text,
    'ESUS_PEC',
    'https://esus.saude.gov.br/api',
    'Bearer',
    'SEU_TOKEN_ESUS',
    30,
    3,
    'INATIVA',  -- Ativar quando configurado
    NOW(),
    NOW()
)
ON CONFLICT DO NOTHING;

-- NEMESIS (Sistema de Gestão Hospitalar)
INSERT INTO apis_externas (
    "Id", 
    "Nome", 
    "BaseUrl", 
    "TipoAutenticacao", 
    "Token", 
    "TimeoutSegundos", 
    "MaxRetries", 
    "Status",
    "CreatedAt", 
    "UpdatedAt"
) VALUES (
    gen_random_uuid()::text,
    'NEMESIS',
    'https://api.nemesis.saude.gov.br',
    'Bearer',
    'SEU_TOKEN_NEMESIS',
    30,
    3,
    'INATIVA',  -- Ativar quando configurado
    NOW(),
    NOW()
)
ON CONFLICT DO NOTHING;

-- ============================================
-- DADOS DE TESTE (Opcional)
-- ============================================

-- Se você quiser testar sem a API real, pode inserir dados de exemplo:

/*
-- Medicamentos de exemplo para teste
INSERT INTO horus_medicamentos (
    "Id",
    "CodigoHorus",
    "Nome",
    "PrincipioAtivo",
    "Concentracao",
    "FormaFarmaceutica",
    "QuantidadeEstoque",
    "QuantidadeMinima",
    "CustoUnitario",
    "Lote",
    "DataValidade",
    "UbsId",
    "UltimaAtualizacaoHorus",
    "CreatedAt",
    "UpdatedAt"
)
SELECT 
    gen_random_uuid()::text,
    'MED' || LPAD(gs::text, 6, '0'),
    CASE (RANDOM() * 10)::INT
        WHEN 0 THEN 'DIPIRONA SÓDICA'
        WHEN 1 THEN 'PARACETAMOL'
        WHEN 2 THEN 'IBUPROFENO'
        WHEN 3 THEN 'AMOXICILINA'
        WHEN 4 THEN 'OMEPRAZOL'
        WHEN 5 THEN 'LOSARTANA POTÁSSICA'
        WHEN 6 THEN 'METFORMINA'
        WHEN 7 THEN 'ENALAPRIL'
        WHEN 8 THEN 'SINVASTATINA'
        ELSE 'HIDROCLOROTIAZIDA'
    END,
    'Princípio Ativo ' || gs,
    '500mg',
    'COMPRIMIDO',
    (RANDOM() * 1000)::INT,  -- Estoque aleatório 0-1000
    50,  -- Mínimo
    (RANDOM() * 10 + 0.5)::numeric(18,2),  -- Custo entre 0.50 e 10.50
    'LOTE' || LPAD((RANDOM() * 999)::INT::text, 6, '0'),
    NOW() + (RANDOM() * 365 || ' days')::INTERVAL,  -- Validade aleatória até 1 ano
    u."Id",
    NOW(),
    NOW(),
    NOW()
FROM generate_series(1, 50) gs  -- 50 medicamentos
CROSS JOIN (SELECT "Id" FROM ubs LIMIT 5) u  -- Para cada uma das 5 primeiras UBS
ON CONFLICT DO NOTHING;
*/

-- ============================================
-- CONSULTAS ÚTEIS
-- ============================================

-- Ver status de todas as integrações
SELECT 
    "Nome",
    "Status",
    "UltimaSincronizacao",
    "TotalSincronizacoes",
    "TotalErros",
    "UltimoErro"
FROM apis_externas
ORDER BY "Nome";

-- Ver últimas 10 sincronizações
SELECT 
    a."Nome" as api,
    l."Endpoint",
    l."MetodoHttp",
    l."StatusCode",
    l."Sucesso",
    l."TempoRespostaMs",
    l."MensagemErro",
    l."CreatedAt"
FROM log_integracao_api l
JOIN apis_externas a ON l."ApiExternaId" = a."Id"
ORDER BY l."CreatedAt" DESC
LIMIT 10;

-- Ver medicamentos sincronizados por UBS
SELECT 
    u."Nome" as ubs,
    COUNT(m."Id") as total_medicamentos,
    SUM(m."QuantidadeEstoque") as quantidade_total,
    SUM(m."QuantidadeEstoque" * m."CustoUnitario") as custo_total,
    COUNT(CASE WHEN m."QuantidadeEstoque" <= m."QuantidadeMinima" THEN 1 END) as estoque_baixo
FROM horus_medicamentos m
JOIN ubs u ON m."UbsId" = u."Id"
GROUP BY u."Id", u."Nome"
ORDER BY custo_total DESC;
