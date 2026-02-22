-- Migration manual para adicionar tabela funcionarios
-- Execute este SQL diretamente no banco PostgreSQL do Render

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
