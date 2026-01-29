-- CreateTable
CREATE TABLE "usuarios" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "nome" TEXT NOT NULL,
    "email" TEXT NOT NULL,
    "senha_hash" TEXT NOT NULL,
    "perfil" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'ATIVO',
    "telefone" TEXT,
    "ubs_id" TEXT,
    "ultimo_acesso" DATETIME,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" DATETIME NOT NULL,
    CONSTRAINT "usuarios_ubs_id_fkey" FOREIGN KEY ("ubs_id") REFERENCES "ubs" ("id") ON DELETE SET NULL ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "permissoes_usuario" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "usuario_id" TEXT NOT NULL,
    "permissao" TEXT NOT NULL,
    "concedida_em" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "concedida_por" TEXT,
    CONSTRAINT "permissoes_usuario_usuario_id_fkey" FOREIGN KEY ("usuario_id") REFERENCES "usuarios" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "ubs" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "nome" TEXT NOT NULL,
    "codigo" TEXT NOT NULL,
    "endereco" TEXT,
    "bairro" TEXT,
    "cep" TEXT,
    "telefone" TEXT,
    "email" TEXT,
    "coordenador_id" TEXT,
    "status" TEXT NOT NULL DEFAULT 'ATIVA',
    "capacidade_atendimento" INTEGER,
    "observacoes" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" DATETIME NOT NULL,
    CONSTRAINT "ubs_coordenador_id_fkey" FOREIGN KEY ("coordenador_id") REFERENCES "usuarios" ("id") ON DELETE SET NULL ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "fornecedores" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "razao_social" TEXT NOT NULL,
    "nome_fantasia" TEXT,
    "cnpj" TEXT NOT NULL,
    "inscricao_estadual" TEXT,
    "endereco" TEXT,
    "bairro" TEXT,
    "cidade" TEXT,
    "estado" TEXT,
    "cep" TEXT,
    "telefone" TEXT,
    "email" TEXT,
    "contato" TEXT,
    "status" TEXT NOT NULL DEFAULT 'ATIVO',
    "observacoes" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "categorias" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "nome" TEXT NOT NULL,
    "descricao" TEXT,
    "tipo" TEXT NOT NULL,
    "orcamento_mensal" REAL,
    "cor" TEXT,
    "icone" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "despesas" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "descricao" TEXT NOT NULL,
    "valor" REAL NOT NULL,
    "data_vencimento" DATETIME,
    "data_pagamento" DATETIME,
    "categoria_id" TEXT NOT NULL,
    "tipo" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'PENDENTE',
    "ubs_id" TEXT NOT NULL,
    "fornecedor_id" TEXT,
    "usuario_criacao_id" TEXT NOT NULL,
    "usuario_aprovacao_id" TEXT,
    "data_aprovacao" DATETIME,
    "observacoes" TEXT,
    "numero_nota" TEXT,
    "numero_empenho" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" DATETIME NOT NULL,
    CONSTRAINT "despesas_categoria_id_fkey" FOREIGN KEY ("categoria_id") REFERENCES "categorias" ("id") ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT "despesas_ubs_id_fkey" FOREIGN KEY ("ubs_id") REFERENCES "ubs" ("id") ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT "despesas_fornecedor_id_fkey" FOREIGN KEY ("fornecedor_id") REFERENCES "fornecedores" ("id") ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT "despesas_usuario_criacao_id_fkey" FOREIGN KEY ("usuario_criacao_id") REFERENCES "usuarios" ("id") ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT "despesas_usuario_aprovacao_id_fkey" FOREIGN KEY ("usuario_aprovacao_id") REFERENCES "usuarios" ("id") ON DELETE SET NULL ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "anexos" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "despesa_id" TEXT NOT NULL,
    "nome_arquivo" TEXT NOT NULL,
    "caminho_arquivo" TEXT NOT NULL,
    "tipo_arquivo" TEXT NOT NULL,
    "tamanho" INTEGER NOT NULL,
    "descricao" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "anexos_despesa_id_fkey" FOREIGN KEY ("despesa_id") REFERENCES "despesas" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "historico_despesas" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "despesa_id" TEXT NOT NULL,
    "status_anterior" TEXT,
    "status_novo" TEXT NOT NULL,
    "usuario_id" TEXT,
    "observacao" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "historico_despesas_despesa_id_fkey" FOREIGN KEY ("despesa_id") REFERENCES "despesas" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "logs_auditoria" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "usuario_id" TEXT,
    "acao" TEXT NOT NULL,
    "entidade" TEXT NOT NULL,
    "entidade_id" TEXT,
    "dados_antigos" TEXT,
    "dadosNovos" TEXT,
    "ip" TEXT,
    "user_agent" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "logs_auditoria_usuario_id_fkey" FOREIGN KEY ("usuario_id") REFERENCES "usuarios" ("id") ON DELETE SET NULL ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "tokens_recuperacao_senha" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "usuario_id" TEXT NOT NULL,
    "token" TEXT NOT NULL,
    "expirado_em" DATETIME NOT NULL,
    "utilizado_em" DATETIME,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "tokens_recuperacao_senha_usuario_id_fkey" FOREIGN KEY ("usuario_id") REFERENCES "usuarios" ("id") ON DELETE RESTRICT ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "importacao_lotes" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "nome_arquivo" TEXT NOT NULL,
    "total_registros" INTEGER NOT NULL,
    "registros_processados" INTEGER NOT NULL,
    "registros_erro" INTEGER NOT NULL,
    "status" TEXT NOT NULL,
    "usuario_id" TEXT NOT NULL,
    "erros" TEXT,
    "created_at" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updated_at" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "webhooks" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "name" TEXT NOT NULL,
    "description" TEXT NOT NULL,
    "url" TEXT NOT NULL,
    "events" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'ACTIVE',
    "secret" TEXT,
    "retryCount" INTEGER NOT NULL DEFAULT 3,
    "timeout" INTEGER NOT NULL DEFAULT 5000,
    "headers" TEXT,
    "createdBy" TEXT NOT NULL,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "webhook_logs" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "webhookId" TEXT NOT NULL,
    "event" TEXT NOT NULL,
    "payload" TEXT NOT NULL,
    "statusCode" INTEGER NOT NULL,
    "response" TEXT,
    "error" TEXT,
    "retryCount" INTEGER NOT NULL DEFAULT 0,
    "success" BOOLEAN NOT NULL DEFAULT false,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "webhook_logs_webhookId_fkey" FOREIGN KEY ("webhookId") REFERENCES "webhooks" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "workflows" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "name" TEXT NOT NULL,
    "description" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'DRAFT',
    "triggerType" TEXT NOT NULL DEFAULT 'MANUAL',
    "entityType" TEXT NOT NULL,
    "triggerConditions" TEXT,
    "version" INTEGER NOT NULL DEFAULT 1,
    "createdBy" TEXT NOT NULL,
    "updatedBy" TEXT,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "workflow_steps" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "workflowId" TEXT NOT NULL,
    "order" INTEGER NOT NULL,
    "name" TEXT NOT NULL,
    "description" TEXT,
    "type" TEXT NOT NULL,
    "configuration" TEXT NOT NULL,
    "conditions" TEXT,
    "assignedTo" TEXT,
    "timeoutHours" INTEGER,
    "required" BOOLEAN NOT NULL DEFAULT true,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "workflow_steps_workflowId_fkey" FOREIGN KEY ("workflowId") REFERENCES "workflows" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "workflow_instances" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "workflowId" TEXT NOT NULL,
    "entityType" TEXT NOT NULL,
    "entityId" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'PENDING',
    "initiatedBy" TEXT NOT NULL,
    "context" TEXT,
    "completedAt" DATETIME,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "workflow_instances_workflowId_fkey" FOREIGN KEY ("workflowId") REFERENCES "workflows" ("id") ON DELETE RESTRICT ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "workflow_step_instances" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "workflowInstanceId" TEXT NOT NULL,
    "stepId" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'PENDING',
    "assignedTo" TEXT,
    "completedBy" TEXT,
    "comments" TEXT,
    "action" TEXT,
    "actionData" TEXT,
    "startedAt" DATETIME,
    "completedAt" DATETIME,
    "dueDate" DATETIME,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "workflow_step_instances_workflowInstanceId_fkey" FOREIGN KEY ("workflowInstanceId") REFERENCES "workflow_instances" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "workflow_rules" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "name" TEXT NOT NULL,
    "description" TEXT NOT NULL,
    "entityType" TEXT NOT NULL,
    "triggerEvent" TEXT NOT NULL,
    "conditions" TEXT NOT NULL,
    "workflowId" TEXT NOT NULL,
    "active" BOOLEAN NOT NULL DEFAULT true,
    "priority" INTEGER NOT NULL DEFAULT 1,
    "createdBy" TEXT NOT NULL,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "audit_logs" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "action" TEXT NOT NULL,
    "entityType" TEXT NOT NULL,
    "entityId" TEXT,
    "userId" TEXT,
    "userEmail" TEXT,
    "userName" TEXT,
    "oldValues" TEXT,
    "newValues" TEXT,
    "changes" TEXT,
    "ipAddress" TEXT,
    "userAgent" TEXT,
    "sessionId" TEXT,
    "severity" TEXT NOT NULL DEFAULT 'LOW',
    "description" TEXT,
    "metadata" TEXT,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- CreateTable
CREATE TABLE "entity_versions" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "entityType" TEXT NOT NULL,
    "entityId" TEXT NOT NULL,
    "version" INTEGER NOT NULL,
    "data" TEXT NOT NULL,
    "changedBy" TEXT NOT NULL,
    "changedByEmail" TEXT,
    "changeReason" TEXT,
    "isActive" BOOLEAN NOT NULL DEFAULT false,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- CreateTable
CREATE TABLE "system_events" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "eventType" TEXT NOT NULL,
    "title" TEXT NOT NULL,
    "description" TEXT,
    "severity" TEXT NOT NULL DEFAULT 'LOW',
    "data" TEXT,
    "source" TEXT,
    "acknowledged" BOOLEAN NOT NULL DEFAULT false,
    "acknowledgedBy" TEXT,
    "acknowledgedAt" DATETIME,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- CreateTable
CREATE TABLE "data_exports" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "name" TEXT NOT NULL,
    "description" TEXT,
    "exportType" TEXT NOT NULL,
    "filters" TEXT NOT NULL,
    "data" TEXT NOT NULL,
    "fileUrl" TEXT,
    "fileSize" BIGINT NOT NULL,
    "requestedBy" TEXT NOT NULL,
    "requestedByEmail" TEXT,
    "completedAt" DATETIME,
    "recordCount" INTEGER,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- CreateTable
CREATE TABLE "integrations" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "name" TEXT NOT NULL,
    "description" TEXT NOT NULL,
    "type" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'INACTIVE',
    "configuration" TEXT NOT NULL,
    "settings" TEXT,
    "createdBy" TEXT NOT NULL,
    "updatedBy" TEXT,
    "lastSyncAt" DATETIME,
    "lastSyncError" TEXT,
    "syncCount" INTEGER NOT NULL DEFAULT 0,
    "errorCount" INTEGER NOT NULL DEFAULT 0,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL
);

-- CreateTable
CREATE TABLE "integration_logs" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "integrationId" TEXT NOT NULL,
    "operation" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'PENDING',
    "requestData" TEXT,
    "responseData" TEXT,
    "errorMessage" TEXT,
    "recordsProcessed" INTEGER,
    "recordsFailed" INTEGER,
    "duration" BIGINT,
    "metadata" TEXT,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "integration_logs_integrationId_fkey" FOREIGN KEY ("integrationId") REFERENCES "integrations" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "payment_transactions" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "integrationId" TEXT,
    "provider" TEXT NOT NULL,
    "transactionId" TEXT NOT NULL,
    "status" TEXT NOT NULL,
    "amount" REAL NOT NULL,
    "currency" TEXT NOT NULL DEFAULT 'BRL',
    "description" TEXT,
    "relatedEntityId" TEXT,
    "relatedEntityType" TEXT,
    "paymentData" TEXT,
    "metadata" TEXT,
    "processedAt" DATETIME,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "payment_transactions_integrationId_fkey" FOREIGN KEY ("integrationId") REFERENCES "integrations" ("id") ON DELETE SET NULL ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "external_syncs" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "integrationId" TEXT NOT NULL,
    "direction" TEXT NOT NULL,
    "entityType" TEXT NOT NULL,
    "status" TEXT NOT NULL DEFAULT 'PENDING',
    "recordsTotal" INTEGER NOT NULL DEFAULT 0,
    "recordsProcessed" INTEGER NOT NULL DEFAULT 0,
    "recordsFailed" INTEGER NOT NULL DEFAULT 0,
    "errorMessage" TEXT,
    "syncData" TEXT,
    "initiatedBy" TEXT NOT NULL,
    "completedAt" DATETIME,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "external_syncs_integrationId_fkey" FOREIGN KEY ("integrationId") REFERENCES "integrations" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateTable
CREATE TABLE "api_endpoints" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "integrationId" TEXT NOT NULL,
    "name" TEXT NOT NULL,
    "method" TEXT NOT NULL,
    "path" TEXT NOT NULL,
    "description" TEXT,
    "requestSchema" TEXT,
    "responseSchema" TEXT,
    "headers" TEXT,
    "active" BOOLEAN NOT NULL DEFAULT true,
    "callCount" INTEGER NOT NULL DEFAULT 0,
    "errorCount" INTEGER NOT NULL DEFAULT 0,
    "createdAt" DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" DATETIME NOT NULL,
    CONSTRAINT "api_endpoints_integrationId_fkey" FOREIGN KEY ("integrationId") REFERENCES "integrations" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

-- CreateIndex
CREATE UNIQUE INDEX "usuarios_email_key" ON "usuarios"("email");

-- CreateIndex
CREATE UNIQUE INDEX "permissoes_usuario_usuario_id_permissao_key" ON "permissoes_usuario"("usuario_id", "permissao");

-- CreateIndex
CREATE UNIQUE INDEX "ubs_codigo_key" ON "ubs"("codigo");

-- CreateIndex
CREATE UNIQUE INDEX "fornecedores_cnpj_key" ON "fornecedores"("cnpj");

-- CreateIndex
CREATE INDEX "despesas_ubs_id_idx" ON "despesas"("ubs_id");

-- CreateIndex
CREATE INDEX "despesas_categoria_id_idx" ON "despesas"("categoria_id");

-- CreateIndex
CREATE INDEX "despesas_fornecedor_id_idx" ON "despesas"("fornecedor_id");

-- CreateIndex
CREATE INDEX "despesas_status_idx" ON "despesas"("status");

-- CreateIndex
CREATE INDEX "despesas_data_vencimento_idx" ON "despesas"("data_vencimento");

-- CreateIndex
CREATE INDEX "anexos_despesa_id_idx" ON "anexos"("despesa_id");

-- CreateIndex
CREATE INDEX "historico_despesas_despesa_id_idx" ON "historico_despesas"("despesa_id");

-- CreateIndex
CREATE INDEX "logs_auditoria_usuario_id_idx" ON "logs_auditoria"("usuario_id");

-- CreateIndex
CREATE INDEX "logs_auditoria_entidade_idx" ON "logs_auditoria"("entidade");

-- CreateIndex
CREATE INDEX "logs_auditoria_created_at_idx" ON "logs_auditoria"("created_at");

-- CreateIndex
CREATE UNIQUE INDEX "tokens_recuperacao_senha_token_key" ON "tokens_recuperacao_senha"("token");

-- CreateIndex
CREATE INDEX "tokens_recuperacao_senha_usuario_id_idx" ON "tokens_recuperacao_senha"("usuario_id");

-- CreateIndex
CREATE INDEX "tokens_recuperacao_senha_token_idx" ON "tokens_recuperacao_senha"("token");

-- CreateIndex
CREATE INDEX "webhook_logs_webhookId_idx" ON "webhook_logs"("webhookId");

-- CreateIndex
CREATE INDEX "webhook_logs_event_idx" ON "webhook_logs"("event");

-- CreateIndex
CREATE INDEX "webhook_logs_createdAt_idx" ON "webhook_logs"("createdAt");

-- CreateIndex
CREATE INDEX "workflow_steps_workflowId_idx" ON "workflow_steps"("workflowId");

-- CreateIndex
CREATE INDEX "workflow_instances_workflowId_idx" ON "workflow_instances"("workflowId");

-- CreateIndex
CREATE INDEX "workflow_instances_entityType_idx" ON "workflow_instances"("entityType");

-- CreateIndex
CREATE INDEX "workflow_instances_entityId_idx" ON "workflow_instances"("entityId");

-- CreateIndex
CREATE INDEX "workflow_step_instances_workflowInstanceId_idx" ON "workflow_step_instances"("workflowInstanceId");

-- CreateIndex
CREATE INDEX "workflow_step_instances_assignedTo_idx" ON "workflow_step_instances"("assignedTo");

-- CreateIndex
CREATE INDEX "workflow_rules_entityType_idx" ON "workflow_rules"("entityType");

-- CreateIndex
CREATE INDEX "workflow_rules_triggerEvent_idx" ON "workflow_rules"("triggerEvent");

-- CreateIndex
CREATE INDEX "audit_logs_entityType_entityId_idx" ON "audit_logs"("entityType", "entityId");

-- CreateIndex
CREATE INDEX "audit_logs_userId_createdAt_idx" ON "audit_logs"("userId", "createdAt");

-- CreateIndex
CREATE INDEX "audit_logs_action_createdAt_idx" ON "audit_logs"("action", "createdAt");

-- CreateIndex
CREATE INDEX "audit_logs_createdAt_idx" ON "audit_logs"("createdAt");

-- CreateIndex
CREATE INDEX "entity_versions_entityType_entityId_version_idx" ON "entity_versions"("entityType", "entityId", "version");

-- CreateIndex
CREATE UNIQUE INDEX "entity_versions_entityType_entityId_version_key" ON "entity_versions"("entityType", "entityId", "version");

-- CreateIndex
CREATE INDEX "system_events_eventType_createdAt_idx" ON "system_events"("eventType", "createdAt");

-- CreateIndex
CREATE INDEX "integration_logs_integrationId_idx" ON "integration_logs"("integrationId");

-- CreateIndex
CREATE INDEX "payment_transactions_integrationId_idx" ON "payment_transactions"("integrationId");

-- CreateIndex
CREATE INDEX "payment_transactions_transactionId_idx" ON "payment_transactions"("transactionId");

-- CreateIndex
CREATE INDEX "external_syncs_integrationId_idx" ON "external_syncs"("integrationId");

-- CreateIndex
CREATE INDEX "api_endpoints_integrationId_idx" ON "api_endpoints"("integrationId");
