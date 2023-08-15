using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class addTennantIdForAllEntitiesBusiness_151020201144 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "WorkflowTransitions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "WorkflowStatuses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Workflows",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UserTeams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UserTeamPermissions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UserAssignments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TransitionPermissions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Teams",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TeamPermissions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ProjectAssignee",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InvoiceUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InvoiceFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "InvoiceComments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "EntityAssignments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Deals",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "DealFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "DealComments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Contracts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ContractMileStones",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ContractFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Contacts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ClientContacts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Assignments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "WorkflowTransitions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "WorkflowStatuses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserTeams");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserTeamPermissions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserAssignments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TransitionPermissions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TeamPermissions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ProjectAssignee");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InvoiceUsers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InvoiceFiles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InvoiceComments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EntityAssignments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DealFiles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DealComments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ContractMileStones");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ContractFiles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ClientContacts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Assignments");
        }
    }
}
