-- Script para popular banco InovaSaúde com dados de teste
-- Execute no terminal do Render ou via psql conectado ao banco

-- Inserir Municípios
INSERT INTO "Municipios" ("Id", "Nome", "CodigoIbge", "Estado", "CreatedAt", "UpdatedAt") VALUES
('55000000-0000-0000-0000-000000000001', 'São Paulo', '3550308', 'SP', NOW(), NOW()),
('55000000-0000-0000-0000-000000000002', 'Rio de Janeiro', '3304557', 'RJ', NOW(), NOW());

-- Inserir UBS
INSERT INTO "UbsList" ("Id", "Nome", "Cnes", "Endereco", "Telefone", "Email", "CapacidadeAtendimento", "Status", "MunicipioId", "CreatedAt", "UpdatedAt") VALUES
('66000000-0000-0000-0000-000000000001', 'UBS Centro', '1234567', 'Rua Principal, 100', '(11) 9999-9999', 'ubs.centro@saude.sp.gov.br', 1000, 0, '55000000-0000-0000-0000-000000000001', NOW(), NOW()),
('66000000-0000-0000-0000-000000000002', 'UBS Jardim', '7654321', 'Av. Verde, 200', '(11) 8888-8888', 'ubs.jardim@saude.sp.gov.br', 800, 0, '55000000-0000-0000-0000-000000000001', NOW(), NOW());

-- Inserir Categorias
INSERT INTO "Categorias" ("Id", "Nome", "Descricao", "OrcamentoMensal", "CreatedAt", "UpdatedAt") VALUES
('77000000-0000-0000-0000-000000000001', 'Pessoal', 'Salários e encargos', 50000.00, NOW(), NOW()),
('77000000-0000-0000-0000-000000000002', 'Material de Consumo', 'Medicamentos e insumos', 15000.00, NOW(), NOW()),
('77000000-0000-0000-0000-000000000003', 'Equipamentos', 'Manutenção e compra', 20000.00, NOW(), NOW());

-- Inserir Usuários (Identity)
-- Senhas: Use PasswordHasher para gerar hash, mas para teste, insira hashes pré-gerados ou use API para registrar
-- Para MVP, registre via API ou insira manualmente com hash de 'senha123'

-- Exemplo de inserção (ajuste hashes):
-- INSERT INTO "AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "Nome", "Cpf", "MunicipioId", "UbsId") VALUES
-- ('user-id-1', 'admin@inovasaude.com.br', 'ADMIN@INOVASAUDE.COM.BR', 'admin@inovasaude.com.br', 'ADMIN@INOVASAUDE.COM.BR', true, 'AQAAAAEAACcQAAAAE...', 'stamp', 'stamp', NULL, false, false, NULL, true, 0, 'Admin', '12345678901', '55000000-0000-0000-0000-000000000001', NULL);

-- Para simplificar, use a API para registrar usuários ou gere hashes.

-- Inserir Despesas (após usuários)
-- INSERT INTO "Despesas" ("Id", "Descricao", "Valor", "Data", "Status", "Tipo", "UbsId", "CategoriaId", "UsuarioId", "CreatedAt", "UpdatedAt") VALUES
-- ('despesa-id-1', 'Compra de medicamentos', 5000.00, NOW(), 0, 0, '66000000-0000-0000-0000-000000000001', '77000000-0000-0000-0000-000000000002', 'user-id-1', NOW(), NOW());

COMMIT;