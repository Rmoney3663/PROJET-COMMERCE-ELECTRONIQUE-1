using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NomAffaires",
                table: "PPVendeurs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PPVendeurs_NomAffaires",
                table: "PPVendeurs",
                column: "NomAffaires",
                unique: true,
                filter: "[NomAffaires] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PPVendeurs_NomAffaires",
                table: "PPVendeurs");

            migrationBuilder.AlterColumn<string>(
                name: "NomAffaires",
                table: "PPVendeurs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
