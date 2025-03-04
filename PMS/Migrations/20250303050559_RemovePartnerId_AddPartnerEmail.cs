using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class RemovePartnerId_AddPartnerEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
