using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class messagerie2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Auteur",
                table: "PPMessages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Destinataire",
                table: "PPDestinatairesMessage",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_PPMessages_Auteur",
                table: "PPMessages",
                column: "Auteur");

            migrationBuilder.CreateIndex(
                name: "IX_PPDestinatairesMessage_Destinataire",
                table: "PPDestinatairesMessage",
                column: "Destinataire");

            migrationBuilder.AddForeignKey(
                name: "FK_PPDestinatairesMessage_AspNetUsers_Destinataire",
                table: "PPDestinatairesMessage",
                column: "Destinataire",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PPMessages_AspNetUsers_Auteur",
                table: "PPMessages",
                column: "Auteur",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPDestinatairesMessage_AspNetUsers_Destinataire",
                table: "PPDestinatairesMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_PPMessages_AspNetUsers_Auteur",
                table: "PPMessages");

            migrationBuilder.DropIndex(
                name: "IX_PPMessages_Auteur",
                table: "PPMessages");

            migrationBuilder.DropIndex(
                name: "IX_PPDestinatairesMessage_Destinataire",
                table: "PPDestinatairesMessage");

            migrationBuilder.AlterColumn<string>(
                name: "Auteur",
                table: "PPMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Destinataire",
                table: "PPDestinatairesMessage",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
