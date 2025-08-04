using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projeto_financeiro_mvc.Migrations
{
    /// <inheritdoc />
    public partial class update_databaseRecorrente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fixo",
                table: "Lancamentos",
                newName: "Recorrente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Recorrente",
                table: "Lancamentos",
                newName: "Fixo");
        }
    }
}
