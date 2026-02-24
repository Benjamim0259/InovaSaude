using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InovaSaude.Blazor.Migrations
{
    /// <inheritdoc />
    public partial class InicialCompletoPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OldValues = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NewValues = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Changes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Metadata = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrcamentoMensal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Cor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Icone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "data_exports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ExportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Filters = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Data = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    RequestedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RecordCount = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_exports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "entity_versions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ChangedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ChangedByEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ChangeReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_versions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "estoque_farmacia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NomeMedicamento = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PrincipioAtivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Concentracao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FormaFarmaceutica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CodigoHorus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Lote = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DataValidade = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    QuantidadeAtual = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeMaxima = table.Column<int>(type: "integer", nullable: true),
                    Localizacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UltimaMovimentacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estoque_farmacia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "fornecedores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RazaoSocial = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NomeFantasia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InscricaoEstadual = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Contato = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "integrations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Configuration = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Settings = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LastSyncAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastSyncError = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SyncCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "system_events",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Data = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Acknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    AcknowledgedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "webhooks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Events = table.Column<int[]>(type: "integer[]", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Secret = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    Timeout = table.Column<int>(type: "integer", nullable: false),
                    Headers = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhooks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workflows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TriggerType = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TriggerConditions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "api_endpoints",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IntegrationId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RequestSchema = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ResponseSchema = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Headers = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    CallCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_endpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_api_endpoints_integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "integrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "external_syncs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IntegrationId = table.Column<string>(type: "text", nullable: false),
                    Direction = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RecordsTotal = table.Column<int>(type: "integer", nullable: false),
                    RecordsProcessed = table.Column<int>(type: "integer", nullable: false),
                    RecordsFailed = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SyncData = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    InitiatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_syncs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_external_syncs_integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "integrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "integration_logs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IntegrationId = table.Column<string>(type: "text", nullable: false),
                    Operation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestData = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ResponseData = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RecordsProcessed = table.Column<int>(type: "integer", nullable: true),
                    RecordsFailed = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    Metadata = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integration_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_integration_logs_integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "integrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IntegrationId = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RelatedEntityId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RelatedEntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentData = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Metadata = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_transactions_integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalTable: "integrations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "webhook_logs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    WebhookId = table.Column<string>(type: "text", nullable: false),
                    Event = table.Column<int>(type: "integer", nullable: false),
                    Payload = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhook_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_webhook_logs_webhooks_WebhookId",
                        column: x => x.WebhookId,
                        principalTable: "webhooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_instances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    WorkflowId = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    InitiatedBy = table.Column<string>(type: "text", nullable: false),
                    Context = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_instances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workflow_instances_workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_steps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    WorkflowId = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Configuration = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Conditions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TimeoutHours = table.Column<int>(type: "integer", nullable: true),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workflow_steps_workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workflow_step_instances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    WorkflowInstanceId = table.Column<string>(type: "text", nullable: false),
                    StepId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AssignedTo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CompletedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Action = table.Column<int>(type: "integer", nullable: true),
                    ActionData = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_step_instances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workflow_step_instances_workflow_instances_WorkflowInstance~",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "workflow_instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "anexos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DespesaId = table.Column<string>(type: "text", nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TipoArquivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tamanho = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "apis_externas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoAutenticacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Token = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ClientSecret = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TimeoutSegundos = table.Column<int>(type: "integer", nullable: false),
                    MaxRetries = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UltimaSincronizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UltimaTentativa = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UltimoErro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TotalSincronizacoes = table.Column<int>(type: "integer", nullable: false),
                    TotalErros = table.Column<int>(type: "integer", nullable: false),
                    ConfiguracoesJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    EsfId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apis_externas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "despesas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataPagamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CategoriaId = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EsfId = table.Column<string>(type: "text", nullable: false),
                    FornecedorId = table.Column<string>(type: "text", nullable: true),
                    UsuarioCriacaoId = table.Column<string>(type: "text", nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    NumeroNota = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NumeroEmpenho = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_despesas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_despesas_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_despesas_fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "fornecedores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "esf",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CoordenadorId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CapacidadeAtendimento = table.Column<int>(type: "integer", nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_esf", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "esus_pec_atendimentos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IdEsus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CnsPaciente = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    NomePaciente = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DataAtendimento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TipoAtendimento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProcedimentosJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Cid10 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CnsProfissional = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    EsfId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_esus_pec_atendimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_esus_pec_atendimentos_esf_EsfId",
                        column: x => x.EsfId,
                        principalTable: "esf",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "funcionarios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Salario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    EsfId = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funcionarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_funcionarios_esf_EsfId",
                        column: x => x.EsfId,
                        principalTable: "esf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "horus_medicamentos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CodigoHorus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PrincipioAtivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Concentracao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FormaFarmaceutica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuantidadeEstoque = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "integer", nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Lote = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DataValidade = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UltimaAtualizacaoHorus = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EsfId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horus_medicamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_horus_medicamentos_esf_EsfId",
                        column: x => x.EsfId,
                        principalTable: "esf",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "nemesis_indicadores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CodigoIndicador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ValorNumerico = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    ValorTexto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PeriodoReferencia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Meta = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    PercentualAlcance = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    EsfId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nemesis_indicadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nemesis_indicadores_esf_EsfId",
                        column: x => x.EsfId,
                        principalTable: "esf",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Perfil = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EsfId = table.Column<string>(type: "text", nullable: true),
                    UltimoAcesso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_esf_EsfId",
                        column: x => x.EsfId,
                        principalTable: "esf",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "historico_despesas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DespesaId = table.Column<string>(type: "text", nullable: false),
                    StatusAnterior = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StatusNovo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: true),
                    Observacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_despesas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historico_despesas_despesas_DespesaId",
                        column: x => x.DespesaId,
                        principalTable: "despesas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historico_despesas_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "importacao_lotes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TotalRegistros = table.Column<int>(type: "integer", nullable: false),
                    RegistrosProcessados = table.Column<int>(type: "integer", nullable: false),
                    RegistrosErro = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    Erros = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_importacao_lotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_importacao_lotes_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "importacoes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Erro = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    TotalRegistros = table.Column<int>(type: "integer", nullable: true),
                    RegistrosProcessados = table.Column<int>(type: "integer", nullable: true),
                    RegistrosErro = table.Column<int>(type: "integer", nullable: true),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_importacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_importacoes_usuarios_CriadoPor",
                        column: x => x.CriadoPor,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "logs_auditoria",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: true),
                    Acao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntidadeId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DadosAntigos = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DadosNovos = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_auditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_logs_auditoria_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "logs_integracao_api",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApiExternaId = table.Column<string>(type: "text", nullable: false),
                    Endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MetodoHttp = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    Sucesso = table.Column<bool>(type: "boolean", nullable: false),
                    TempoRespostaMs = table.Column<long>(type: "bigint", nullable: true),
                    RequestPayload = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ResponsePayload = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MensagemErro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NumeroTentativa = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_integracao_api", x => x.Id);
                    table.ForeignKey(
                        name: "FK_logs_integracao_api_apis_externas_ApiExternaId",
                        column: x => x.ApiExternaId,
                        principalTable: "apis_externas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_logs_integracao_api_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "pedidos_medicamentos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NumeroPedido = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EsfSolicitanteId = table.Column<string>(type: "text", nullable: false),
                    UsuarioCriacaoId = table.Column<string>(type: "text", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataNecessidade = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Prioridade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UsuarioAprovacaoId = table.Column<string>(type: "text", nullable: true),
                    DataAprovacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioEntregaId = table.Column<string>(type: "text", nullable: true),
                    DataEntrega = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MotivoRejeicao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos_medicamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pedidos_medicamentos_esf_EsfSolicitanteId",
                        column: x => x.EsfSolicitanteId,
                        principalTable: "esf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pedidos_medicamentos_usuarios_UsuarioAprovacaoId",
                        column: x => x.UsuarioAprovacaoId,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_pedidos_medicamentos_usuarios_UsuarioCriacaoId",
                        column: x => x.UsuarioCriacaoId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pedidos_medicamentos_usuarios_UsuarioEntregaId",
                        column: x => x.UsuarioEntregaId,
                        principalTable: "usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "permissoes_usuario",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    Permissao = table.Column<int>(type: "integer", nullable: false),
                    ConcedidaEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ConcedidaPor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissoes_usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_permissoes_usuario_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tokens_recuperacao_senha",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiradoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UtilizadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens_recuperacao_senha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tokens_recuperacao_senha_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "importacao_itens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ImportacaoId = table.Column<string>(type: "text", nullable: false),
                    DadosOriginais = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Erro = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EntidadeCriadaId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EntidadeTipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_importacao_itens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_importacao_itens_importacoes_ImportacaoId",
                        column: x => x.ImportacaoId,
                        principalTable: "importacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "itens_pedido_medicamento",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PedidoMedicamentoId = table.Column<string>(type: "text", nullable: false),
                    NomeMedicamento = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PrincipioAtivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Concentracao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FormaFarmaceutica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuantidadeSolicitada = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeAprovada = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeEntregue = table.Column<int>(type: "integer", nullable: true),
                    Justificativa = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CodigoHorus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itens_pedido_medicamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_itens_pedido_medicamento_pedidos_medicamentos_PedidoMedicam~",
                        column: x => x.PedidoMedicamentoId,
                        principalTable: "pedidos_medicamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movimentacoes_estoque",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EstoqueFarmaciaId = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeAnterior = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeApos = table.Column<int>(type: "integer", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PedidoMedicamentoId = table.Column<string>(type: "text", nullable: true),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimentacoes_estoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movimentacoes_estoque_estoque_farmacia_EstoqueFarmaciaId",
                        column: x => x.EstoqueFarmaciaId,
                        principalTable: "estoque_farmacia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movimentacoes_estoque_pedidos_medicamentos_PedidoMedicament~",
                        column: x => x.PedidoMedicamentoId,
                        principalTable: "pedidos_medicamentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_movimentacoes_estoque_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_anexos_DespesaId",
                table: "anexos",
                column: "DespesaId");

            migrationBuilder.CreateIndex(
                name: "IX_api_endpoints_IntegrationId",
                table: "api_endpoints",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_apis_externas_EsfId",
                table: "apis_externas",
                column: "EsfId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_Action_CreatedAt",
                table: "audit_logs",
                columns: new[] { "Action", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_CreatedAt",
                table: "audit_logs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EntityType_EntityId",
                table: "audit_logs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_UserId_CreatedAt",
                table: "audit_logs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_despesas_CategoriaId",
                table: "despesas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_DataVencimento",
                table: "despesas",
                column: "DataVencimento");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_EsfId",
                table: "despesas",
                column: "EsfId");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_FornecedorId",
                table: "despesas",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_despesas_UsuarioCriacaoId",
                table: "despesas",
                column: "UsuarioCriacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_esf_Codigo",
                table: "esf",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_esf_CoordenadorId",
                table: "esf",
                column: "CoordenadorId");

            migrationBuilder.CreateIndex(
                name: "IX_esus_pec_atendimentos_EsfId",
                table: "esus_pec_atendimentos",
                column: "EsfId");

            migrationBuilder.CreateIndex(
                name: "IX_external_syncs_IntegrationId",
                table: "external_syncs",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_fornecedores_Cnpj",
                table: "fornecedores",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_funcionarios_EsfId",
                table: "funcionarios",
                column: "EsfId");

            migrationBuilder.CreateIndex(
                name: "IX_historico_despesas_DespesaId",
                table: "historico_despesas",
                column: "DespesaId");

            migrationBuilder.CreateIndex(
                name: "IX_historico_despesas_UsuarioId",
                table: "historico_despesas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_horus_medicamentos_EsfId",
                table: "horus_medicamentos",
                column: "EsfId");

            migrationBuilder.CreateIndex(
                name: "IX_importacao_itens_ImportacaoId",
                table: "importacao_itens",
                column: "ImportacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_importacao_lotes_UsuarioId",
                table: "importacao_lotes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_importacoes_CriadoPor",
                table: "importacoes",
                column: "CriadoPor");

            migrationBuilder.CreateIndex(
                name: "IX_integration_logs_IntegrationId",
                table: "integration_logs",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_itens_pedido_medicamento_PedidoMedicamentoId",
                table: "itens_pedido_medicamento",
                column: "PedidoMedicamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_CreatedAt",
                table: "logs_auditoria",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_Entidade",
                table: "logs_auditoria",
                column: "Entidade");

            migrationBuilder.CreateIndex(
                name: "IX_logs_auditoria_UsuarioId",
                table: "logs_auditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_logs_integracao_api_ApiExternaId",
                table: "logs_integracao_api",
                column: "ApiExternaId");

            migrationBuilder.CreateIndex(
                name: "IX_logs_integracao_api_UsuarioId",
                table: "logs_integracao_api",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_movimentacoes_estoque_EstoqueFarmaciaId",
                table: "movimentacoes_estoque",
                column: "EstoqueFarmaciaId");

            migrationBuilder.CreateIndex(
                name: "IX_movimentacoes_estoque_PedidoMedicamentoId",
                table: "movimentacoes_estoque",
                column: "PedidoMedicamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_movimentacoes_estoque_UsuarioId",
                table: "movimentacoes_estoque",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_nemesis_indicadores_EsfId",
                table: "nemesis_indicadores",
                column: "EsfId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_IntegrationId",
                table: "payment_transactions",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transactions_TransactionId",
                table: "payment_transactions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_medicamentos_EsfSolicitanteId",
                table: "pedidos_medicamentos",
                column: "EsfSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_medicamentos_UsuarioAprovacaoId",
                table: "pedidos_medicamentos",
                column: "UsuarioAprovacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_medicamentos_UsuarioCriacaoId",
                table: "pedidos_medicamentos",
                column: "UsuarioCriacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_medicamentos_UsuarioEntregaId",
                table: "pedidos_medicamentos",
                column: "UsuarioEntregaId");

            migrationBuilder.CreateIndex(
                name: "IX_permissoes_usuario_UsuarioId_Permissao",
                table: "permissoes_usuario",
                columns: new[] { "UsuarioId", "Permissao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_system_events_EventType_CreatedAt",
                table: "system_events",
                columns: new[] { "EventType", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tokens_recuperacao_senha_Token",
                table: "tokens_recuperacao_senha",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tokens_recuperacao_senha_UsuarioId",
                table: "tokens_recuperacao_senha",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_EsfId",
                table: "usuarios",
                column: "EsfId");

            migrationBuilder.CreateIndex(
                name: "IX_webhook_logs_Event_CreatedAt",
                table: "webhook_logs",
                columns: new[] { "Event", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_webhook_logs_WebhookId",
                table: "webhook_logs",
                column: "WebhookId");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_instances_EntityType_EntityId",
                table: "workflow_instances",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_workflow_instances_WorkflowId",
                table: "workflow_instances",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_step_instances_AssignedTo",
                table: "workflow_step_instances",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_step_instances_WorkflowInstanceId",
                table: "workflow_step_instances",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_workflow_steps_WorkflowId",
                table: "workflow_steps",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_anexos_despesas_DespesaId",
                table: "anexos",
                column: "DespesaId",
                principalTable: "despesas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_apis_externas_esf_EsfId",
                table: "apis_externas",
                column: "EsfId",
                principalTable: "esf",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_esf_EsfId",
                table: "despesas",
                column: "EsfId",
                principalTable: "esf",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_despesas_usuarios_UsuarioCriacaoId",
                table: "despesas",
                column: "UsuarioCriacaoId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_esf_usuarios_CoordenadorId",
                table: "esf",
                column: "CoordenadorId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_esf_EsfId",
                table: "usuarios");

            migrationBuilder.DropTable(
                name: "anexos");

            migrationBuilder.DropTable(
                name: "api_endpoints");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "data_exports");

            migrationBuilder.DropTable(
                name: "entity_versions");

            migrationBuilder.DropTable(
                name: "esus_pec_atendimentos");

            migrationBuilder.DropTable(
                name: "external_syncs");

            migrationBuilder.DropTable(
                name: "funcionarios");

            migrationBuilder.DropTable(
                name: "historico_despesas");

            migrationBuilder.DropTable(
                name: "horus_medicamentos");

            migrationBuilder.DropTable(
                name: "importacao_itens");

            migrationBuilder.DropTable(
                name: "importacao_lotes");

            migrationBuilder.DropTable(
                name: "integration_logs");

            migrationBuilder.DropTable(
                name: "itens_pedido_medicamento");

            migrationBuilder.DropTable(
                name: "logs_auditoria");

            migrationBuilder.DropTable(
                name: "logs_integracao_api");

            migrationBuilder.DropTable(
                name: "movimentacoes_estoque");

            migrationBuilder.DropTable(
                name: "nemesis_indicadores");

            migrationBuilder.DropTable(
                name: "payment_transactions");

            migrationBuilder.DropTable(
                name: "permissoes_usuario");

            migrationBuilder.DropTable(
                name: "system_events");

            migrationBuilder.DropTable(
                name: "tokens_recuperacao_senha");

            migrationBuilder.DropTable(
                name: "webhook_logs");

            migrationBuilder.DropTable(
                name: "workflow_step_instances");

            migrationBuilder.DropTable(
                name: "workflow_steps");

            migrationBuilder.DropTable(
                name: "despesas");

            migrationBuilder.DropTable(
                name: "importacoes");

            migrationBuilder.DropTable(
                name: "apis_externas");

            migrationBuilder.DropTable(
                name: "estoque_farmacia");

            migrationBuilder.DropTable(
                name: "pedidos_medicamentos");

            migrationBuilder.DropTable(
                name: "integrations");

            migrationBuilder.DropTable(
                name: "webhooks");

            migrationBuilder.DropTable(
                name: "workflow_instances");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "fornecedores");

            migrationBuilder.DropTable(
                name: "workflows");

            migrationBuilder.DropTable(
                name: "esf");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
