using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class add_column_TemplateID_to_table_CampaignEmailTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TemplateId",
                table: "CampaignEmailTemplates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "CampaignEmailTemplates");
        }
    }
}
