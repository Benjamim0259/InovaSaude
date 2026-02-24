-- ============================================
-- MIGRATION: Adicionar Custos ao HORUS
-- Data: 2024
-- Descrição: Adiciona campos de custo, lote e validade aos medicamentos HORUS
-- ============================================

-- 1. Adicionar novos campos à tabela horus_medicamentos
ALTER TABLE horus_medicamentos 
ADD COLUMN IF NOT EXISTS "CustoUnitario" numeric(18,2) NOT NULL DEFAULT 0;

ALTER TABLE horus_medicamentos 
ADD COLUMN IF NOT EXISTS "Lote" character varying(50);

ALTER TABLE horus_medicamentos 
ADD COLUMN IF NOT EXISTS "DataValidade" timestamp with time zone;

-- 2. Criar índice para otimizar consultas por UBS
CREATE INDEX IF NOT EXISTS "IX_horus_medicamentos_UbsId_CustoUnitario" 
ON horus_medicamentos ("UbsId", "CustoUnitario");

-- 3. Criar índice para consultas de estoque baixo
CREATE INDEX IF NOT EXISTS "IX_horus_medicamentos_EstoqueBaixo" 
ON horus_medicamentos ("UbsId", "QuantidadeEstoque", "QuantidadeMinima")
WHERE "QuantidadeEstoque" <= "QuantidadeMinima";

-- 4. Comentários nas colunas (documentação)
COMMENT ON COLUMN horus_medicamentos."CustoUnitario" IS 'Custo unitário do medicamento em reais';
COMMENT ON COLUMN horus_medicamentos."Lote" IS 'Número do lote do medicamento';
COMMENT ON COLUMN horus_medicamentos."DataValidade" IS 'Data de validade do medicamento';

-- ============================================
-- DADOS DE EXEMPLO (opcional - apenas para testes)
-- ============================================

-- Atualizar alguns medicamentos com custos de exemplo
-- ATENÇÃO: Isso é apenas para testes. Em produção, os custos virão da API HORUS
/*
UPDATE horus_medicamentos 
SET "CustoUnitario" = 
    CASE 
        WHEN "Nome" ILIKE '%dipirona%' THEN 0.50
        WHEN "Nome" ILIKE '%paracetamol%' THEN 0.30
        WHEN "Nome" ILIKE '%ibuprofeno%' THEN 1.20
        WHEN "Nome" ILIKE '%amoxicilina%' THEN 2.50
        WHEN "Nome" ILIKE '%omeprazol%' THEN 0.80
        ELSE 1.00
    END
WHERE "CustoUnitario" = 0;
*/

-- ============================================
-- VERIFICAÇÕES
-- ============================================

-- Verificar se as colunas foram criadas
SELECT 
    column_name, 
    data_type, 
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'horus_medicamentos'
AND column_name IN ('CustoUnitario', 'Lote', 'DataValidade');

-- Verificar índices criados
SELECT 
    indexname, 
    indexdef
FROM pg_indexes
WHERE tablename = 'horus_medicamentos'
AND indexname LIKE '%Custo%' OR indexname LIKE '%EstoqueBaixo%';

-- Estatísticas da tabela
SELECT 
    COUNT(*) as total_medicamentos,
    COUNT(CASE WHEN "CustoUnitario" > 0 THEN 1 END) as medicamentos_com_custo,
    SUM("QuantidadeEstoque" * "CustoUnitario") as valor_total_estoque,
    COUNT(CASE WHEN "QuantidadeEstoque" <= "QuantidadeMinima" THEN 1 END) as medicamentos_estoque_baixo
FROM horus_medicamentos;

-- ============================================
-- ROLLBACK (caso precise desfazer)
-- ============================================
/*
-- ATENÇÃO: Isso remove os campos. Use apenas se realmente precisar reverter.

DROP INDEX IF EXISTS "IX_horus_medicamentos_UbsId_CustoUnitario";
DROP INDEX IF EXISTS "IX_horus_medicamentos_EstoqueBaixo";

ALTER TABLE horus_medicamentos DROP COLUMN IF EXISTS "CustoUnitario";
ALTER TABLE horus_medicamentos DROP COLUMN IF EXISTS "Lote";
ALTER TABLE horus_medicamentos DROP COLUMN IF EXISTS "DataValidade";
*/
