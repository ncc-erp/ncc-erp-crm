using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class update_DealEntity_add_ContactId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContactID",
                table: "Deals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_ContactID",
                table: "Deals",
                column: "ContactID");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Contacts_ContactID",
                table: "Deals",
                column: "ContactID",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Contacts_ContactID",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_ContactID",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ContactID",
                table: "Deals");
        }
    }
}
