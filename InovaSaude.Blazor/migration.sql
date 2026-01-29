IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [audit_logs] (
    [Id] nvarchar(450) NOT NULL,
    [Action] nvarchar(100) NOT NULL,
    [EntityType] nvarchar(100) NOT NULL,
    [EntityId] nvarchar(255) NULL,
    [UserId] nvarchar(255) NULL,
    [UserEmail] nvarchar(255) NULL,
    [UserName] nvarchar(255) NULL,
    [OldValues] nvarchar(4000) NULL,
    [NewValues] nvarchar(4000) NULL,
    [Changes] nvarchar(2000) NULL,
    [IpAddress] nvarchar(45) NULL,
    [UserAgent] nvarchar(500) NULL,
    [SessionId] nvarchar(255) NULL,
    [Severity] nvarchar(20) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Metadata] nvarchar(2000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_audit_logs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [categorias] (
    [Id] nvarchar(450) NOT NULL,
    [Nome] nvarchar(255) NOT NULL,
    [Descricao] nvarchar(1000) NULL,
    [Tipo] nvarchar(50) NOT NULL,
    [OrcamentoMensal] decimal(18,2) NULL,
    [Cor] nvarchar(50) NULL,
    [Icone] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_categorias] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [data_exports] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [ExportType] nvarchar(50) NOT NULL,
    [Filters] nvarchar(2000) NOT NULL,
    [Data] nvarchar(4000) NOT NULL,
    [FileUrl] nvarchar(1000) NULL,
    [FileSize] bigint NOT NULL,
    [RequestedBy] nvarchar(255) NOT NULL,
    [RequestedByEmail] nvarchar(255) NULL,
    [CompletedAt] datetime2 NULL,
    [RecordCount] int NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_data_exports] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [entity_versions] (
    [Id] nvarchar(450) NOT NULL,
    [EntityType] nvarchar(100) NOT NULL,
    [EntityId] nvarchar(255) NOT NULL,
    [Version] int NOT NULL,
    [Data] nvarchar(4000) NOT NULL,
    [ChangedBy] nvarchar(255) NOT NULL,
    [ChangedByEmail] nvarchar(255) NULL,
    [ChangeReason] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_entity_versions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [fornecedores] (
    [Id] nvarchar(450) NOT NULL,
    [RazaoSocial] nvarchar(255) NOT NULL,
    [NomeFantasia] nvarchar(255) NULL,
    [Cnpj] nvarchar(20) NOT NULL,
    [InscricaoEstadual] nvarchar(20) NULL,
    [Endereco] nvarchar(500) NULL,
    [Bairro] nvarchar(100) NULL,
    [Cidade] nvarchar(100) NULL,
    [Estado] nvarchar(2) NULL,
    [Cep] nvarchar(20) NULL,
    [Telefone] nvarchar(20) NULL,
    [Email] nvarchar(255) NULL,
    [Contato] nvarchar(100) NULL,
    [Status] nvarchar(20) NOT NULL,
    [Observacoes] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_fornecedores] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [integrations] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Configuration] nvarchar(2000) NOT NULL,
    [Settings] nvarchar(2000) NULL,
    [CreatedBy] nvarchar(255) NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [LastSyncAt] datetime2 NULL,
    [LastSyncError] nvarchar(1000) NULL,
    [SyncCount] int NOT NULL,
    [ErrorCount] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_integrations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [system_events] (
    [Id] nvarchar(450) NOT NULL,
    [EventType] nvarchar(100) NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Severity] nvarchar(20) NOT NULL,
    [Data] nvarchar(2000) NULL,
    [Source] nvarchar(255) NULL,
    [Acknowledged] bit NOT NULL,
    [AcknowledgedBy] nvarchar(255) NULL,
    [AcknowledgedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_system_events] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [webhooks] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Url] nvarchar(1000) NOT NULL,
    [Events] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [Secret] nvarchar(255) NULL,
    [RetryCount] int NOT NULL,
    [Timeout] int NOT NULL,
    [Headers] nvarchar(2000) NULL,
    [CreatedBy] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_webhooks] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [workflows] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Status] int NOT NULL,
    [TriggerType] int NOT NULL,
    [EntityType] nvarchar(100) NOT NULL,
    [TriggerConditions] nvarchar(2000) NULL,
    [Version] int NOT NULL,
    [CreatedBy] nvarchar(max) NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_workflows] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [api_endpoints] (
    [Id] nvarchar(450) NOT NULL,
    [IntegrationId] nvarchar(450) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Method] nvarchar(10) NOT NULL,
    [Path] nvarchar(500) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [RequestSchema] nvarchar(2000) NULL,
    [ResponseSchema] nvarchar(2000) NULL,
    [Headers] nvarchar(1000) NULL,
    [Active] bit NOT NULL,
    [CallCount] int NOT NULL,
    [ErrorCount] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_api_endpoints] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_api_endpoints_integrations_IntegrationId] FOREIGN KEY ([IntegrationId]) REFERENCES [integrations] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [external_syncs] (
    [Id] nvarchar(450) NOT NULL,
    [IntegrationId] nvarchar(450) NOT NULL,
    [Direction] nvarchar(20) NOT NULL,
    [EntityType] nvarchar(100) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [RecordsTotal] int NOT NULL,
    [RecordsProcessed] int NOT NULL,
    [RecordsFailed] int NOT NULL,
    [ErrorMessage] nvarchar(1000) NULL,
    [SyncData] nvarchar(4000) NULL,
    [InitiatedBy] nvarchar(255) NOT NULL,
    [CompletedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_external_syncs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_external_syncs_integrations_IntegrationId] FOREIGN KEY ([IntegrationId]) REFERENCES [integrations] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [integration_logs] (
    [Id] nvarchar(450) NOT NULL,
    [IntegrationId] nvarchar(450) NOT NULL,
    [Operation] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [RequestData] nvarchar(4000) NULL,
    [ResponseData] nvarchar(4000) NULL,
    [ErrorMessage] nvarchar(1000) NULL,
    [RecordsProcessed] int NULL,
    [RecordsFailed] int NULL,
    [Duration] bigint NULL,
    [Metadata] nvarchar(2000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_integration_logs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_integration_logs_integrations_IntegrationId] FOREIGN KEY ([IntegrationId]) REFERENCES [integrations] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [payment_transactions] (
    [Id] nvarchar(450) NOT NULL,
    [IntegrationId] nvarchar(450) NULL,
    [Provider] nvarchar(50) NOT NULL,
    [TransactionId] nvarchar(255) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Description] nvarchar(500) NULL,
    [RelatedEntityId] nvarchar(255) NULL,
    [RelatedEntityType] nvarchar(100) NULL,
    [PaymentData] nvarchar(2000) NULL,
    [Metadata] nvarchar(2000) NULL,
    [ProcessedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_payment_transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_payment_transactions_integrations_IntegrationId] FOREIGN KEY ([IntegrationId]) REFERENCES [integrations] ([Id])
);
GO

CREATE TABLE [webhook_logs] (
    [Id] nvarchar(450) NOT NULL,
    [WebhookId] nvarchar(450) NOT NULL,
    [Event] int NOT NULL,
    [Payload] nvarchar(4000) NOT NULL,
    [StatusCode] int NOT NULL,
    [Response] nvarchar(4000) NULL,
    [Error] nvarchar(1000) NULL,
    [RetryCount] int NOT NULL,
    [Success] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_webhook_logs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_webhook_logs_webhooks_WebhookId] FOREIGN KEY ([WebhookId]) REFERENCES [webhooks] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [workflow_instances] (
    [Id] nvarchar(450) NOT NULL,
    [WorkflowId] nvarchar(450) NOT NULL,
    [EntityType] nvarchar(100) NOT NULL,
    [EntityId] nvarchar(255) NOT NULL,
    [Status] int NOT NULL,
    [InitiatedBy] nvarchar(max) NOT NULL,
    [Context] nvarchar(2000) NULL,
    [CompletedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_workflow_instances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_workflow_instances_workflows_WorkflowId] FOREIGN KEY ([WorkflowId]) REFERENCES [workflows] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [workflow_steps] (
    [Id] nvarchar(450) NOT NULL,
    [WorkflowId] nvarchar(450) NOT NULL,
    [Order] int NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Type] int NOT NULL,
    [Configuration] nvarchar(2000) NOT NULL,
    [Conditions] nvarchar(2000) NULL,
    [AssignedTo] nvarchar(255) NULL,
    [TimeoutHours] int NULL,
    [Required] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_workflow_steps] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_workflow_steps_workflows_WorkflowId] FOREIGN KEY ([WorkflowId]) REFERENCES [workflows] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [workflow_step_instances] (
    [Id] nvarchar(450) NOT NULL,
    [WorkflowInstanceId] nvarchar(450) NOT NULL,
    [StepId] nvarchar(255) NOT NULL,
    [Status] int NOT NULL,
    [AssignedTo] nvarchar(255) NULL,
    [CompletedBy] nvarchar(255) NULL,
    [Comments] nvarchar(1000) NULL,
    [Action] int NULL,
    [ActionData] nvarchar(2000) NULL,
    [StartedAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [DueDate] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_workflow_step_instances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_workflow_step_instances_workflow_instances_WorkflowInstanceId] FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [workflow_instances] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [anexos] (
    [Id] nvarchar(450) NOT NULL,
    [DespesaId] nvarchar(450) NOT NULL,
    [NomeArquivo] nvarchar(255) NOT NULL,
    [CaminhoArquivo] nvarchar(1000) NOT NULL,
    [TipoArquivo] nvarchar(100) NOT NULL,
    [Tamanho] bigint NOT NULL,
    [Descricao] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_anexos] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [despesas] (
    [Id] nvarchar(450) NOT NULL,
    [Descricao] nvarchar(1000) NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    [DataVencimento] datetime2 NULL,
    [DataPagamento] datetime2 NULL,
    [CategoriaId] nvarchar(450) NOT NULL,
    [Tipo] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [UbsId] nvarchar(450) NOT NULL,
    [FornecedorId] nvarchar(450) NULL,
    [UsuarioCriacaoId] nvarchar(450) NOT NULL,
    [UsuarioAprovacaoId] nvarchar(450) NULL,
    [DataAprovacao] datetime2 NULL,
    [Observacoes] nvarchar(1000) NULL,
    [NumeroNota] nvarchar(50) NULL,
    [NumeroEmpenho] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_despesas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_despesas_categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [categorias] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_despesas_fornecedores_FornecedorId] FOREIGN KEY ([FornecedorId]) REFERENCES [fornecedores] ([Id])
);
GO

CREATE TABLE [historico_despesas] (
    [Id] nvarchar(450) NOT NULL,
    [DespesaId] nvarchar(450) NOT NULL,
    [StatusAnterior] nvarchar(20) NULL,
    [StatusNovo] nvarchar(20) NOT NULL,
    [UsuarioId] nvarchar(450) NULL,
    [Observacao] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_historico_despesas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_historico_despesas_despesas_DespesaId] FOREIGN KEY ([DespesaId]) REFERENCES [despesas] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [importacao_itens] (
    [Id] nvarchar(450) NOT NULL,
    [ImportacaoId] nvarchar(450) NOT NULL,
    [DadosOriginais] nvarchar(4000) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Erro] nvarchar(1000) NULL,
    [EntidadeCriadaId] nvarchar(100) NULL,
    [EntidadeTipo] nvarchar(50) NULL,
    [CriadoEm] datetime2 NOT NULL,
    CONSTRAINT [PK_importacao_itens] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [importacao_lotes] (
    [Id] nvarchar(450) NOT NULL,
    [NomeArquivo] nvarchar(255) NOT NULL,
    [TotalRegistros] int NOT NULL,
    [RegistrosProcessados] int NOT NULL,
    [RegistrosErro] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [UsuarioId] nvarchar(450) NOT NULL,
    [Erros] nvarchar(4000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_importacao_lotes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [importacoes] (
    [Id] nvarchar(450) NOT NULL,
    [NomeArquivo] nvarchar(255) NOT NULL,
    [Tipo] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Descricao] nvarchar(1000) NULL,
    [Erro] nvarchar(4000) NULL,
    [TotalRegistros] int NULL,
    [RegistrosProcessados] int NULL,
    [RegistrosErro] int NULL,
    [CriadoPor] nvarchar(450) NULL,
    [CriadoEm] datetime2 NOT NULL,
    [AtualizadoEm] datetime2 NULL,
    CONSTRAINT [PK_importacoes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [logs_auditoria] (
    [Id] nvarchar(450) NOT NULL,
    [UsuarioId] nvarchar(450) NULL,
    [Acao] nvarchar(100) NOT NULL,
    [Entidade] nvarchar(100) NOT NULL,
    [EntidadeId] nvarchar(255) NULL,
    [DadosAntigos] nvarchar(4000) NULL,
    [DadosNovos] nvarchar(4000) NULL,
    [Ip] nvarchar(45) NULL,
    [UserAgent] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_logs_auditoria] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [permissoes_usuario] (
    [Id] nvarchar(450) NOT NULL,
    [UsuarioId] nvarchar(450) NOT NULL,
    [Permissao] int NOT NULL,
    [ConcedidaEm] datetime2 NOT NULL,
    [ConcedidaPor] nvarchar(255) NULL,
    CONSTRAINT [PK_permissoes_usuario] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [tokens_recuperacao_senha] (
    [Id] nvarchar(450) NOT NULL,
    [UsuarioId] nvarchar(450) NOT NULL,
    [Token] nvarchar(255) NOT NULL,
    [ExpiradoEm] datetime2 NOT NULL,
    [UtilizadoEm] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_tokens_recuperacao_senha] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ubs] (
    [Id] nvarchar(450) NOT NULL,
    [Nome] nvarchar(255) NOT NULL,
    [Codigo] nvarchar(50) NOT NULL,
    [Endereco] nvarchar(500) NULL,
    [Bairro] nvarchar(100) NULL,
    [Cep] nvarchar(20) NULL,
    [Telefone] nvarchar(20) NULL,
    [Email] nvarchar(255) NULL,
    [CoordenadorId] nvarchar(450) NULL,
    [Status] nvarchar(20) NOT NULL,
    [CapacidadeAtendimento] int NULL,
    [Observacoes] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ubs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [usuarios] (
    [Id] nvarchar(450) NOT NULL,
    [Nome] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [SenhaHash] nvarchar(255) NOT NULL,
    [Perfil] int NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Telefone] nvarchar(20) NULL,
    [UbsId] nvarchar(450) NULL,
    [UltimoAcesso] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_usuarios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_usuarios_ubs_UbsId] FOREIGN KEY ([UbsId]) REFERENCES [ubs] ([Id])
);
GO

CREATE INDEX [IX_anexos_DespesaId] ON [anexos] ([DespesaId]);
GO

CREATE INDEX [IX_api_endpoints_IntegrationId] ON [api_endpoints] ([IntegrationId]);
GO

CREATE INDEX [IX_audit_logs_Action_CreatedAt] ON [audit_logs] ([Action], [CreatedAt]);
GO

CREATE INDEX [IX_audit_logs_CreatedAt] ON [audit_logs] ([CreatedAt]);
GO

CREATE INDEX [IX_audit_logs_EntityType_EntityId] ON [audit_logs] ([EntityType], [EntityId]);
GO

CREATE INDEX [IX_audit_logs_UserId_CreatedAt] ON [audit_logs] ([UserId], [CreatedAt]);
GO

CREATE INDEX [IX_despesas_CategoriaId] ON [despesas] ([CategoriaId]);
GO

CREATE INDEX [IX_despesas_DataVencimento] ON [despesas] ([DataVencimento]);
GO

CREATE INDEX [IX_despesas_FornecedorId] ON [despesas] ([FornecedorId]);
GO

CREATE INDEX [IX_despesas_Status] ON [despesas] ([Status]);
GO

CREATE INDEX [IX_despesas_UbsId] ON [despesas] ([UbsId]);
GO

CREATE INDEX [IX_despesas_UsuarioAprovacaoId] ON [despesas] ([UsuarioAprovacaoId]);
GO

CREATE INDEX [IX_despesas_UsuarioCriacaoId] ON [despesas] ([UsuarioCriacaoId]);
GO

CREATE INDEX [IX_external_syncs_IntegrationId] ON [external_syncs] ([IntegrationId]);
GO

CREATE UNIQUE INDEX [IX_fornecedores_Cnpj] ON [fornecedores] ([Cnpj]);
GO

CREATE INDEX [IX_historico_despesas_DespesaId] ON [historico_despesas] ([DespesaId]);
GO

CREATE INDEX [IX_historico_despesas_UsuarioId] ON [historico_despesas] ([UsuarioId]);
GO

CREATE INDEX [IX_importacao_itens_ImportacaoId] ON [importacao_itens] ([ImportacaoId]);
GO

CREATE INDEX [IX_importacao_lotes_UsuarioId] ON [importacao_lotes] ([UsuarioId]);
GO

CREATE INDEX [IX_importacoes_CriadoPor] ON [importacoes] ([CriadoPor]);
GO

CREATE INDEX [IX_integration_logs_IntegrationId] ON [integration_logs] ([IntegrationId]);
GO

CREATE INDEX [IX_logs_auditoria_CreatedAt] ON [logs_auditoria] ([CreatedAt]);
GO

CREATE INDEX [IX_logs_auditoria_Entidade] ON [logs_auditoria] ([Entidade]);
GO

CREATE INDEX [IX_logs_auditoria_UsuarioId] ON [logs_auditoria] ([UsuarioId]);
GO

CREATE INDEX [IX_payment_transactions_IntegrationId] ON [payment_transactions] ([IntegrationId]);
GO

CREATE INDEX [IX_payment_transactions_TransactionId] ON [payment_transactions] ([TransactionId]);
GO

CREATE UNIQUE INDEX [IX_permissoes_usuario_UsuarioId_Permissao] ON [permissoes_usuario] ([UsuarioId], [Permissao]);
GO

CREATE INDEX [IX_system_events_EventType_CreatedAt] ON [system_events] ([EventType], [CreatedAt]);
GO

CREATE UNIQUE INDEX [IX_tokens_recuperacao_senha_Token] ON [tokens_recuperacao_senha] ([Token]);
GO

CREATE INDEX [IX_tokens_recuperacao_senha_UsuarioId] ON [tokens_recuperacao_senha] ([UsuarioId]);
GO

CREATE UNIQUE INDEX [IX_ubs_Codigo] ON [ubs] ([Codigo]);
GO

CREATE INDEX [IX_ubs_CoordenadorId] ON [ubs] ([CoordenadorId]);
GO

CREATE UNIQUE INDEX [IX_usuarios_Email] ON [usuarios] ([Email]);
GO

CREATE INDEX [IX_usuarios_UbsId] ON [usuarios] ([UbsId]);
GO

CREATE INDEX [IX_webhook_logs_Event_CreatedAt] ON [webhook_logs] ([Event], [CreatedAt]);
GO

CREATE INDEX [IX_webhook_logs_WebhookId] ON [webhook_logs] ([WebhookId]);
GO

CREATE INDEX [IX_workflow_instances_EntityType_EntityId] ON [workflow_instances] ([EntityType], [EntityId]);
GO

CREATE INDEX [IX_workflow_instances_WorkflowId] ON [workflow_instances] ([WorkflowId]);
GO

CREATE INDEX [IX_workflow_step_instances_AssignedTo] ON [workflow_step_instances] ([AssignedTo]);
GO

CREATE INDEX [IX_workflow_step_instances_WorkflowInstanceId] ON [workflow_step_instances] ([WorkflowInstanceId]);
GO

CREATE INDEX [IX_workflow_steps_WorkflowId] ON [workflow_steps] ([WorkflowId]);
GO

ALTER TABLE [anexos] ADD CONSTRAINT [FK_anexos_despesas_DespesaId] FOREIGN KEY ([DespesaId]) REFERENCES [despesas] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [despesas] ADD CONSTRAINT [FK_despesas_ubs_UbsId] FOREIGN KEY ([UbsId]) REFERENCES [ubs] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [despesas] ADD CONSTRAINT [FK_despesas_usuarios_UsuarioAprovacaoId] FOREIGN KEY ([UsuarioAprovacaoId]) REFERENCES [usuarios] ([Id]) ON DELETE SET NULL;
GO

ALTER TABLE [despesas] ADD CONSTRAINT [FK_despesas_usuarios_UsuarioCriacaoId] FOREIGN KEY ([UsuarioCriacaoId]) REFERENCES [usuarios] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [historico_despesas] ADD CONSTRAINT [FK_historico_despesas_usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [usuarios] ([Id]);
GO

ALTER TABLE [importacao_itens] ADD CONSTRAINT [FK_importacao_itens_importacoes_ImportacaoId] FOREIGN KEY ([ImportacaoId]) REFERENCES [importacoes] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [importacao_lotes] ADD CONSTRAINT [FK_importacao_lotes_usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [usuarios] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [importacoes] ADD CONSTRAINT [FK_importacoes_usuarios_CriadoPor] FOREIGN KEY ([CriadoPor]) REFERENCES [usuarios] ([Id]);
GO

ALTER TABLE [logs_auditoria] ADD CONSTRAINT [FK_logs_auditoria_usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [usuarios] ([Id]) ON DELETE SET NULL;
GO

ALTER TABLE [permissoes_usuario] ADD CONSTRAINT [FK_permissoes_usuario_usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [usuarios] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [tokens_recuperacao_senha] ADD CONSTRAINT [FK_tokens_recuperacao_senha_usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [usuarios] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [ubs] ADD CONSTRAINT [FK_ubs_usuarios_CoordenadorId] FOREIGN KEY ([CoordenadorId]) REFERENCES [usuarios] ([Id]) ON DELETE SET NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260129215314_InitialCreate', N'8.0.10');
GO

COMMIT;
GO

