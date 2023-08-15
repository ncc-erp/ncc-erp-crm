using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class updateTblDeal_StartDate_LastFollow_PotentialType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DealType",
                table: "Deals");

            migrationBuilder.AddColumn<DateTime>(
                name: "DealLastFollow",
                table: "Deals",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DealStartDate",
                table: "Deals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Deals",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PotentialType",
                table: "Deals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DealLastFollow",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "DealStartDate",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "PotentialType",
                table: "Deals");

            migrationBuilder.AddColumn<int>(
                name: "DealType",
                table: "Deals",
                nullable: true);
        }
    }
}
