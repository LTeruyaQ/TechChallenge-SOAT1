using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class Orcamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataEnvioOrcamento",
                table: "OrdensSevico");

            migrationBuilder.DropColumn(
                name: "Orcamento",
                table: "OrdensSevico");

            migrationBuilder.AddColumn<Guid>(
                name: "OrcamentoId",
                table: "OrdensSevico",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orcamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdemServicoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamentos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdensSevico_OrcamentoId",
                table: "OrdensSevico",
                column: "OrcamentoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdensSevico_Orcamentos_OrcamentoId",
                table: "OrdensSevico",
                column: "OrcamentoId",
                principalTable: "Orcamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdensSevico_Orcamentos_OrcamentoId",
                table: "OrdensSevico");

            migrationBuilder.DropTable(
                name: "Orcamentos");

            migrationBuilder.DropIndex(
                name: "IX_OrdensSevico_OrcamentoId",
                table: "OrdensSevico");

            migrationBuilder.DropColumn(
                name: "OrcamentoId",
                table: "OrdensSevico");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEnvioOrcamento",
                table: "OrdensSevico",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Orcamento",
                table: "OrdensSevico",
                type: "numeric",
                nullable: true);
        }
    }
}
