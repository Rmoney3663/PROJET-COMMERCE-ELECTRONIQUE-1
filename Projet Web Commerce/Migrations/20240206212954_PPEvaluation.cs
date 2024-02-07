using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class PPEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PPEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    NoProduit = table.Column<int>(type: "int", nullable: false),
                    Cote = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateMAJ = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPEvaluations_PPClients_NoClient",
                        column: x => x.NoClient,
                        principalTable: "PPClients",
                        principalColumn: "NoClient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPEvaluations_PPProduits_NoProduit",
                        column: x => x.NoProduit,
                        principalTable: "PPProduits",
                        principalColumn: "NoProduit",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPEvaluations_NoClient",
                table: "PPEvaluations",
                column: "NoClient");

            migrationBuilder.CreateIndex(
                name: "IX_PPEvaluations_NoProduit",
                table: "PPEvaluations",
                column: "NoProduit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPEvaluations");
        }
    }
}
