using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projeto_financeiro_mvc.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sobrenome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SenhaSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Preferences_Theme = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiracaoToken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrupoFamiliarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_GrupoFamiliar_GrupoFamiliarId",
                        column: x => x.GrupoFamiliarId,
                        principalTable: "GrupoFamiliar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    GrupoFamiliarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorias_GrupoFamiliar_GrupoFamiliarId",
                        column: x => x.GrupoFamiliarId,
                        principalTable: "GrupoFamiliar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Categorias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Banco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Agencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroConta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saldo = table.Column<double>(type: "float", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    GrupoFamiliarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contas_GrupoFamiliar_GrupoFamiliarId",
                        column: x => x.GrupoFamiliarId,
                        principalTable: "GrupoFamiliar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lancamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Previsao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Parcelas = table.Column<int>(type: "int", nullable: false),
                    Pago = table.Column<bool>(type: "bit", nullable: false),
                    ContaId = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    GrupoFamiliarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lancamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lancamentos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lancamentos_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lancamentos_GrupoFamiliar_GrupoFamiliarId",
                        column: x => x.GrupoFamiliarId,
                        principalTable: "GrupoFamiliar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lancamentos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recorrentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Previsao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Parcelas = table.Column<int>(type: "int", nullable: false),
                    Pago = table.Column<bool>(type: "bit", nullable: false),
                    IsRecorrente = table.Column<bool>(type: "bit", nullable: false),
                    ContaId = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    GrupoFamiliarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recorrentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recorrentes_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recorrentes_Contas_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Contas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recorrentes_GrupoFamiliar_GrupoFamiliarId",
                        column: x => x.GrupoFamiliarId,
                        principalTable: "GrupoFamiliar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recorrentes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transferencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataTransferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCompensacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContaOrigemId = table.Column<int>(type: "int", nullable: false),
                    ContaDestinoId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    GrupoFamiliarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transferencias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transferencias_Contas_ContaDestinoId",
                        column: x => x.ContaDestinoId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_Contas_ContaOrigemId",
                        column: x => x.ContaOrigemId,
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencias_GrupoFamiliar_GrupoFamiliarId",
                        column: x => x.GrupoFamiliarId,
                        principalTable: "GrupoFamiliar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transferencias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_GrupoFamiliarId",
                table: "Categorias",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UsuarioId",
                table: "Categorias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Contas_GrupoFamiliarId",
                table: "Contas",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Contas_UsuarioId",
                table: "Contas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_CategoriaId",
                table: "Lancamentos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_ContaId",
                table: "Lancamentos",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_GrupoFamiliarId",
                table: "Lancamentos",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_UsuarioId",
                table: "Lancamentos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrentes_CategoriaId",
                table: "Recorrentes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrentes_ContaId",
                table: "Recorrentes",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrentes_GrupoFamiliarId",
                table: "Recorrentes",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Recorrentes_UsuarioId",
                table: "Recorrentes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_CategoriaId",
                table: "Transferencias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_ContaDestinoId",
                table: "Transferencias",
                column: "ContaDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_ContaOrigemId",
                table: "Transferencias",
                column: "ContaOrigemId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_GrupoFamiliarId",
                table: "Transferencias",
                column: "GrupoFamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_UsuarioId",
                table: "Transferencias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GrupoFamiliarId",
                table: "Usuarios",
                column: "GrupoFamiliarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lancamentos");

            migrationBuilder.DropTable(
                name: "Recorrentes");

            migrationBuilder.DropTable(
                name: "Transferencias");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Contas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "GrupoFamiliar");
        }
    }
}
