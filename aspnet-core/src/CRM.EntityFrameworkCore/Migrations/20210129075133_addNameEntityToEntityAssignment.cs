using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class addNameEntityToEntityAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameOfEntity",
                table: "EntityAssignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignments_CreatorUserId",
                table: "UserAssignments",
                column: "CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_AbpUsers_CreatorUserId",
                table: "UserAssignments",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_AbpUsers_CreatorUserId",
                table: "UserAssignments");

            migrationBuilder.DropIndex(
                name: "IX_UserAssignments_CreatorUserId",
                table: "UserAssignments");

            migrationBuilder.DropColumn(
                name: "NameOfEntity",
                table: "EntityAssignments");
        }
    }
}
