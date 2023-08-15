using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CRM.Migrations
{
    public partial class Update_Deal_Project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DealId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClientId",
                table: "Deals",
                nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ClosingDate",
            //    table: "Deals",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Deals",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DealId",
                table: "Projects",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_ClientId",
                table: "Deals",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Clients_ClientId",
                table: "Deals",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Deals_DealId",
                table: "Projects",
                column: "DealId",
                principalTable: "Deals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Clients_ClientId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Deals_DealId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DealId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Deals_ClientId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "DealId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ClosingDate",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Deals");
        }
    }
}
