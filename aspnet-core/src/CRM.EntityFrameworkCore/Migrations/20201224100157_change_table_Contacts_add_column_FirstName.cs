using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class change_table_Contacts_add_column_FirstName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Contacts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Contacts");
        }
    }
}
