using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class Telephone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelephoneClients");

            migrationBuilder.DropTable(
                name: "TelephoneVendeurs");

            migrationBuilder.AddColumn<string>(
                name: "Tel1",
                table: "PPVendeurs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tel2",
                table: "PPVendeurs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tel1",
                table: "PPClients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tel2",
                table: "PPClients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tel1",
                table: "PPVendeurs");

            migrationBuilder.DropColumn(
                name: "Tel2",
                table: "PPVendeurs");

            migrationBuilder.DropColumn(
                name: "Tel1",
                table: "PPClients");

            migrationBuilder.DropColumn(
                name: "Tel2",
                table: "PPClients");

            migrationBuilder.CreateTable(
                name: "TelephoneClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelephoneClients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelephoneClients_PPClients_NoClient",
                        column: x => x.NoClient,
                        principalTable: "PPClients",
                        principalColumn: "NoClient",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TelephoneVendeurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelephoneVendeurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelephoneVendeurs_PPVendeurs_NoVendeur",
                        column: x => x.NoVendeur,
                        principalTable: "PPVendeurs",
                        principalColumn: "NoVendeur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelephoneClients_NoClient",
                table: "TelephoneClients",
                column: "NoClient");

            migrationBuilder.CreateIndex(
                name: "IX_TelephoneVendeurs_NoVendeur",
                table: "TelephoneVendeurs",
                column: "NoVendeur");
        }
    }
}
