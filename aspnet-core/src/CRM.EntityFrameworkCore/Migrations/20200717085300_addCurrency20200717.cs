using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class addCurrency20200717 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "InvoiceUsers",
                newName: "Position");

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "ContractValue",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Contracts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<float>(
                name: "Value",
                table: "ContractMileStones",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "ContractMileStones",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ContractMileStones");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "InvoiceUsers",
                newName: "Role");

            migrationBuilder.AlterColumn<long>(
                name: "ContractValue",
                table: "Contracts",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "ContractMileStones",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
