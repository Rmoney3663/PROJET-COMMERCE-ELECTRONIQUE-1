using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class messagerie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PPMessages",
                columns: table => new
                {
                    NoMessage = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sujet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Auteur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeMessage = table.Column<int>(type: "int", nullable: false),
                    PieceJointe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Transfemetteur = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPMessages", x => x.NoMessage);
                });

            migrationBuilder.CreateTable(
                name: "PPDestinatairesMessage",
                columns: table => new
                {
                    DestinataireId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoMessage = table.Column<int>(type: "int", nullable: false),
                    Destinataire = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPDestinatairesMessage", x => x.DestinataireId);
                    table.ForeignKey(
                        name: "FK_PPDestinatairesMessage_PPMessages_NoMessage",
                        column: x => x.NoMessage,
                        principalTable: "PPMessages",
                        principalColumn: "NoMessage",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPDestinatairesMessage_NoMessage",
                table: "PPDestinatairesMessage",
                column: "NoMessage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPDestinatairesMessage");

            migrationBuilder.DropTable(
                name: "PPMessages");
        }
    }
}
