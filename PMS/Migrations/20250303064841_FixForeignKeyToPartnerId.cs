using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyToPartnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_AspNetUsers_PartnerEmail",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_PartnerEmail",
                table: "Offers");

            migrationBuilder.AlterColumn<string>(
                name: "PartnerEmail",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "PartnerId",
                table: "Offers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_PartnerId",
                table: "Offers",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_AspNetUsers_PartnerId",
                table: "Offers",
                column: "PartnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_AspNetUsers_PartnerId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_PartnerId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Offers");

            migrationBuilder.AlterColumn<string>(
                name: "PartnerEmail",
                table: "Offers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_PartnerEmail",
                table: "Offers",
                column: "PartnerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_AspNetUsers_PartnerEmail",
                table: "Offers",
                column: "PartnerEmail",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
