using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class add_column_parentId_to_table_DealComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "DealComments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "DealComments");
        }
    }
}
