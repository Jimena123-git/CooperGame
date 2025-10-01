using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CooperGame.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jugadores",
                columns: table => new
                {
                    IdJugador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugadores", x => x.IdJugador);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    IdPartida = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TiempoTotal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    IdJugador = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.IdPartida);
                    table.ForeignKey(
                        name: "FK_Partidas_Jugadores_IdJugador",
                        column: x => x.IdJugador,
                        principalTable: "Jugadores",
                        principalColumn: "IdJugador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recursos",
                columns: table => new
                {
                    IdRecurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Meta = table.Column<int>(type: "int", nullable: true),
                    CantidadRecolectada = table.Column<int>(type: "int", nullable: false),
                    IdPartida = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recursos", x => x.IdRecurso);
                    table.ForeignKey(
                        name: "FK_Recursos_Partidas_IdPartida",
                        column: x => x.IdPartida,
                        principalTable: "Partidas",
                        principalColumn: "IdPartida",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resultados",
                columns: table => new
                {
                    IdResultado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CantidadResultadoPorJugador = table.Column<int>(type: "int", nullable: false),
                    IdJugador = table.Column<int>(type: "int", nullable: false),
                    IdPartida = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resultados", x => x.IdResultado);
                    table.ForeignKey(
                        name: "FK_Resultados_Jugadores_IdJugador",
                        column: x => x.IdJugador,
                        principalTable: "Jugadores",
                        principalColumn: "IdJugador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resultados_Partidas_IdPartida",
                        column: x => x.IdPartida,
                        principalTable: "Partidas",
                        principalColumn: "IdPartida",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_IdJugador",
                table: "Partidas",
                column: "IdJugador");

            migrationBuilder.CreateIndex(
                name: "IX_Recursos_IdPartida",
                table: "Recursos",
                column: "IdPartida");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_IdJugador",
                table: "Resultados",
                column: "IdJugador");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_IdPartida",
                table: "Resultados",
                column: "IdPartida");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recursos");

            migrationBuilder.DropTable(
                name: "Resultados");

            migrationBuilder.DropTable(
                name: "Partidas");

            migrationBuilder.DropTable(
                name: "Jugadores");
        }
    }
}
