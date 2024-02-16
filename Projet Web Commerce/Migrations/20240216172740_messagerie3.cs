using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class messagerie3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Transfemetteur",
                table: "PPMessages");

            migrationBuilder.AddColumn<string>(
                name: "Transmetteur",
                table: "PPMessages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MessageLu",
                table: "PPDestinatairesMessage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PPMessages_Transmetteur",
                table: "PPMessages",
                column: "Transmetteur");

            migrationBuilder.AddForeignKey(
                name: "FK_PPMessages_AspNetUsers_Transmetteur",
                table: "PPMessages",
                column: "Transmetteur",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPMessages_AspNetUsers_Transmetteur",
                table: "PPMessages");

            migrationBuilder.DropIndex(
                name: "IX_PPMessages_Transmetteur",
                table: "PPMessages");

            migrationBuilder.DropColumn(
                name: "Transmetteur",
                table: "PPMessages");

            migrationBuilder.DropColumn(
                name: "MessageLu",
                table: "PPDestinatairesMessage");

            migrationBuilder.AddColumn<string>(
                name: "Transfemetteur",
                table: "PPMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
