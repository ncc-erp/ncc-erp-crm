using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class AllowContractMileStoneIdNull_Invoice_20200721 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ContractMileStoneId",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ContractMileStoneId",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
