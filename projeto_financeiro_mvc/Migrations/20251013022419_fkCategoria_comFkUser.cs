using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projeto_financeiro_mvc.Migrations
{
    /// <inheritdoc />
    public partial class fkCategoria_comFkUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrupoFamiliarId",
                table: "Categorias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Categorias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_GrupoFamiliarId",
                table: "Categorias",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UsuarioId",
                table: "Categorias",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_GrupoFamiliar_GrupoFamiliarId",
                table: "Categorias",
                column: "GrupoFamiliarId",
                principalTable: "GrupoFamiliar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioId",
                table: "Categorias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_GrupoFamiliar_GrupoFamiliarId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Usuarios_UsuarioId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_GrupoFamiliarId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UsuarioId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "GrupoFamiliarId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Categorias");
        }
    }
}
