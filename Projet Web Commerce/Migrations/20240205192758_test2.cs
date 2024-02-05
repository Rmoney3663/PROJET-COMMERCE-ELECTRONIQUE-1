using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PrixVente",
                table: "PPProduits",
                type: "smallmoney",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "smallmoney");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PrixVente",
                table: "PPProduits",
                type: "smallmoney",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "smallmoney",
                oldNullable: true);
        }
    }
}
