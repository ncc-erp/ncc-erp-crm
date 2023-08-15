using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class changeManmonthToFloatfromDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "ManMonth",
                table: "InvoiceUsers",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ManMonth",
                table: "InvoiceUsers",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
