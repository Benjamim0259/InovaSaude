-- CreateTable "tokens_recuperacao_senha"
CREATE TABLE IF NOT EXISTS "tokens_recuperacao_senha" (
    "id" TEXT NOT NULL,
    "usuario_id" TEXT NOT NULL,
    "token" TEXT NOT NULL,
    "expirado_em" TIMESTAMP(3) NOT NULL,
    "utilizado_em" TIMESTAMP(3),
    "created_at" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "tokens_recuperacao_senha_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE UNIQUE INDEX "tokens_recuperacao_senha_token_key" ON "tokens_recuperacao_senha"("token");

-- CreateIndex
CREATE INDEX "tokens_recuperacao_senha_usuario_id_idx" ON "tokens_recuperacao_senha"("usuario_id");

-- CreateIndex
CREATE INDEX "tokens_recuperacao_senha_token_idx" ON "tokens_recuperacao_senha"("token");
