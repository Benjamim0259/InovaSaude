using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InovaSaude.Blazor.Migrations
{
    /// <inheritdoc />
    public partial class AddApiExternasIntegrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apis_externas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoAutenticacao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ClientSecret = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TimeoutSegundos = table.Column<int>(type: "int", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UltimaSincronizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimaTentativa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimoErro = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TotalSincronizacoes = table.Column<int>(type: "int", nullable: false),
                    TotalErros = table.Column<int>(type: "int", nullable: false),
                    ConfiguracoesJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    UbsId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apis_externas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_apis_externas_ubs_UbsId",
                        column: x => x.UbsId,
                        principalTable: "ubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "estoque_farmacia",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomeMedicamento = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrincipioAtivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Concentracao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FormaFarmaceutica = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoHorus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Lote = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QuantidadeAtual = table.Column<int>(type: "int", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "int", nullable: false),
                    QuantidadeMaxima = table.Column<int>(type: "int", nullable: true),
                    Localizacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UltimaMovimentacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estoque_farmacia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "esus_pec_atendimentos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdEsus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CnsPaciente = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NomePaciente = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DataAtendimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoAtendimento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProcedimentosJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Cid10 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CnsProfissional = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    UbsId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_esus_pec_atendimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_esus_pec_atendimentos_ubs_UbsId",
                        column: x => x.UbsId,
                        principalTable: "ubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "horus_medicamentos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodigoHorus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrincipioAtivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Concentracao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FormaFarmaceutica = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuantidadeEstoque = table.Column<int>(type: "int", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "int", nullable: false),
                    UltimaAtualizacaoHorus = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UbsId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horus_medicamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_horus_medicamentos_ubs_UbsId",
                        column: x => x.UbsId,
                        principalTable: "ubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "nemesis_indicadores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodigoIndicador = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ValorNumerico = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValorTexto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PeriodoReferencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Meta = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentualAlcance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UbsId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nemesis_indicadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nemesis_indicadores_ubs_UbsId",
                        column: x => x.UbsId,
                        principalTable: "ubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "pedidos_medicamentos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NumeroPedido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UbsSolicitanteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsuarioCriacaoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataNecessidade = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Prioridade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UsuarioAprovacaoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DataAprovacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEntregaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DataEntrega = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRejeicao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos_medicamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pedidos_medicamentos_ubs_UbsSolicitanteId",
                        column: x => x.UbsSolicitanteId,
                        principalTable: "ubs",
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
                name: "logs_integracao_api",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApiExternaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MetodoHttp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    Sucesso = table.Column<bool>(type: "bit", nullable: false),
                    TempoRespostaMs = table.Column<long>(type: "bigint", nullable: true),
                    RequestPayload = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResponsePayload = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MensagemErro = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NumeroTentativa = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "itens_pedido_medicamento",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PedidoMedicamentoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomeMedicamento = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrincipioAtivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Concentracao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FormaFarmaceutica = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuantidadeSolicitada = table.Column<int>(type: "int", nullable: false),
                    QuantidadeAprovada = table.Column<int>(type: "int", nullable: true),
                    QuantidadeEntregue = table.Column<int>(type: "int", nullable: true),
                    Justificativa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CodigoHorus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itens_pedido_medicamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_itens_pedido_medicamento_pedidos_medicamentos_PedidoMedicamentoId",
                        column: x => x.PedidoMedicamentoId,
                        principalTable: "pedidos_medicamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movimentacoes_estoque",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EstoqueFarmaciaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    QuantidadeAnterior = table.Column<int>(type: "int", nullable: false),
                    QuantidadeApos = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PedidoMedicamentoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        name: "FK_movimentacoes_estoque_pedidos_medicamentos_PedidoMedicamentoId",
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
                name: "IX_apis_externas_UbsId",
                table: "apis_externas",
                column: "UbsId");

            migrationBuilder.CreateIndex(
                name: "IX_esus_pec_atendimentos_UbsId",
                table: "esus_pec_atendimentos",
                column: "UbsId");

            migrationBuilder.CreateIndex(
                name: "IX_horus_medicamentos_UbsId",
                table: "horus_medicamentos",
                column: "UbsId");

            migrationBuilder.CreateIndex(
                name: "IX_itens_pedido_medicamento_PedidoMedicamentoId",
                table: "itens_pedido_medicamento",
                column: "PedidoMedicamentoId");

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
                name: "IX_nemesis_indicadores_UbsId",
                table: "nemesis_indicadores",
                column: "UbsId");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_medicamentos_UbsSolicitanteId",
                table: "pedidos_medicamentos",
                column: "UbsSolicitanteId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "esus_pec_atendimentos");

            migrationBuilder.DropTable(
                name: "horus_medicamentos");

            migrationBuilder.DropTable(
                name: "itens_pedido_medicamento");

            migrationBuilder.DropTable(
                name: "logs_integracao_api");

            migrationBuilder.DropTable(
                name: "movimentacoes_estoque");

            migrationBuilder.DropTable(
                name: "nemesis_indicadores");

            migrationBuilder.DropTable(
                name: "apis_externas");

            migrationBuilder.DropTable(
                name: "estoque_farmacia");

            migrationBuilder.DropTable(
                name: "pedidos_medicamentos");
        }
    }
}
