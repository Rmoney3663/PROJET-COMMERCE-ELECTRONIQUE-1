using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class Produit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NombreItems",
                table: "PPProduits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreItems",
                table: "PPProduits");
        }
    }
}
