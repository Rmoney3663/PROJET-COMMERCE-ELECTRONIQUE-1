using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class fixedphone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelephoneVendeurs",
                table: "TelephoneVendeurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TelephoneClients",
                table: "TelephoneClients");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TelephoneVendeurs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TelephoneClients",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "NoProvince",
                table: "PPVendeurs",
                type: "char(2)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelephoneVendeurs",
                table: "TelephoneVendeurs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelephoneClients",
                table: "TelephoneClients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TelephoneVendeurs_NoVendeur",
                table: "TelephoneVendeurs",
                column: "NoVendeur");

            migrationBuilder.CreateIndex(
                name: "IX_TelephoneClients_NoClient",
                table: "TelephoneClients",
                column: "NoClient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelephoneVendeurs",
                table: "TelephoneVendeurs");

            migrationBuilder.DropIndex(
                name: "IX_TelephoneVendeurs_NoVendeur",
                table: "TelephoneVendeurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TelephoneClients",
                table: "TelephoneClients");

            migrationBuilder.DropIndex(
                name: "IX_TelephoneClients_NoClient",
                table: "TelephoneClients");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TelephoneVendeurs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TelephoneClients");

            migrationBuilder.AlterColumn<string>(
                name: "NoProvince",
                table: "PPVendeurs",
                type: "char(2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelephoneVendeurs",
                table: "TelephoneVendeurs",
                column: "NoVendeur");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelephoneClients",
                table: "TelephoneClients",
                column: "NoClient");
        }
    }
}
