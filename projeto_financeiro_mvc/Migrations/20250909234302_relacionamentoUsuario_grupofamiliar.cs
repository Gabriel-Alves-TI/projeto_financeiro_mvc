using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projeto_financeiro_mvc.Migrations
{
    /// <inheritdoc />
    public partial class relacionamentoUsuario_grupofamiliar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrupoFamiliarId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GrupoFamiliarId",
                table: "Transferencias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Transferencias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GrupoFamiliarId",
                table: "Recorrentes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Recorrentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GrupoFamiliarId",
                table: "Lancamentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Lancamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GrupoFamiliarId",
                table: "Contas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Contas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GrupoFamiliar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoFamiliar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GrupoFamiliarId",
                table: "Usuarios",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_GrupoFamiliarId",
                table: "Transferencias",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_UsuarioId",
                table: "Transferencias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrentes_GrupoFamiliarId",
                table: "Recorrentes",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrentes_UsuarioId",
                table: "Recorrentes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_GrupoFamiliarId",
                table: "Lancamentos",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_UsuarioId",
                table: "Lancamentos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Contas_GrupoFamiliarId",
                table: "Contas",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Contas_UsuarioId",
                table: "Contas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contas_GrupoFamiliar_GrupoFamiliarId",
                table: "Contas",
                column: "GrupoFamiliarId",
                principalTable: "GrupoFamiliar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contas_Usuarios_UsuarioId",
                table: "Contas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_GrupoFamiliar_GrupoFamiliarId",
                table: "Lancamentos",
                column: "GrupoFamiliarId",
                principalTable: "GrupoFamiliar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_Usuarios_UsuarioId",
                table: "Lancamentos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recorrentes_GrupoFamiliar_GrupoFamiliarId",
                table: "Recorrentes",
                column: "GrupoFamiliarId",
                principalTable: "GrupoFamiliar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recorrentes_Usuarios_UsuarioId",
                table: "Recorrentes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transferencias_GrupoFamiliar_GrupoFamiliarId",
                table: "Transferencias",
                column: "GrupoFamiliarId",
                principalTable: "GrupoFamiliar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transferencias_Usuarios_UsuarioId",
                table: "Transferencias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_GrupoFamiliar_GrupoFamiliarId",
                table: "Usuarios",
                column: "GrupoFamiliarId",
                principalTable: "GrupoFamiliar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contas_GrupoFamiliar_GrupoFamiliarId",
                table: "Contas");

            migrationBuilder.DropForeignKey(
                name: "FK_Contas_Usuarios_UsuarioId",
                table: "Contas");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_GrupoFamiliar_GrupoFamiliarId",
                table: "Lancamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_Usuarios_UsuarioId",
                table: "Lancamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Recorrentes_GrupoFamiliar_GrupoFamiliarId",
                table: "Recorrentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recorrentes_Usuarios_UsuarioId",
                table: "Recorrentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transferencias_GrupoFamiliar_GrupoFamiliarId",
                table: "Transferencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Transferencias_Usuarios_UsuarioId",
                table: "Transferencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_GrupoFamiliar_GrupoFamiliarId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "GrupoFamiliar");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_GrupoFamiliarId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Transferencias_GrupoFamiliarId",
                table: "Transferencias");

            migrationBuilder.DropIndex(
                name: "IX_Transferencias_UsuarioId",
                table: "Transferencias");

            migrationBuilder.DropIndex(
                name: "IX_Recorrentes_GrupoFamiliarId",
                table: "Recorrentes");

            migrationBuilder.DropIndex(
                name: "IX_Recorrentes_UsuarioId",
                table: "Recorrentes");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_GrupoFamiliarId",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_UsuarioId",
                table: "Lancamentos");

            migrationBuilder.DropIndex(
                name: "IX_Contas_GrupoFamiliarId",
                table: "Contas");

            migrationBuilder.DropIndex(
                name: "IX_Contas_UsuarioId",
                table: "Contas");

            migrationBuilder.DropColumn(
                name: "GrupoFamiliarId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GrupoFamiliarId",
                table: "Transferencias");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Transferencias");

            migrationBuilder.DropColumn(
                name: "GrupoFamiliarId",
                table: "Recorrentes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Recorrentes");

            migrationBuilder.DropColumn(
                name: "GrupoFamiliarId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "GrupoFamiliarId",
                table: "Contas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Contas");
        }
    }
}
