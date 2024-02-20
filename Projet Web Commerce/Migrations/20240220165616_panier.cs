using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class panier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "NoPanier",
                table: "PPArticlesEnPanier",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NoPanier",
                table: "PPArticlesEnPanier",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
