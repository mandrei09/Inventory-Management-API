using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOrderStartAccMonth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StartAccMonthId",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_StartAccMonthId",
                table: "Order",
                column: "StartAccMonthId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AccMonth_StartAccMonthId",
                table: "Order",
                column: "StartAccMonthId",
                principalTable: "AccMonth",
                principalColumn: "AccMonthId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AccMonth_StartAccMonthId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_StartAccMonthId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "StartAccMonthId",
                table: "Order");
        }
    }
}
