using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projeto_financeiro_mvc.Migrations
{
    /// <inheritdoc />
    public partial class fkCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Transferencias");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Recorrentes");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Lancamentos");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Transferencias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Recorrentes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Lancamentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_CategoriaId",
                table: "Transferencias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrentes_CategoriaId",
                table: "Recorrentes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_CategoriaId",
                table: "Lancamentos",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lancamentos_Categorias_CategoriaId",
                table: "Lancamentos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recorrentes_Categorias_CategoriaId",
                table: "Recorrentes",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transferencias_Categorias_CategoriaId",
                table: "Transferencias",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lancamentos_Categorias_CategoriaId",
                table: "Lancamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Recorrentes_Categorias_CategoriaId",
                table: "Recorrentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transferencias_Categorias_CategoriaId",
                table: "Transferencias");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Transferencias_CategoriaId",
                table: "Transferencias");

            migrationBuilder.DropIndex(
                name: "IX_Recorrentes_CategoriaId",
                table: "Recorrentes");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_CategoriaId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Transferencias");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Recorrentes");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Lancamentos");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Transferencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Recorrentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Lancamentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
