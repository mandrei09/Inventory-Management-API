using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAccMonthIdInventoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccMonthId",
                table: "Inventory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_AccMonthId",
                table: "Inventory",
                column: "AccMonthId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_AccMonth_AccMonthId",
                table: "Inventory",
                column: "AccMonthId",
                principalTable: "AccMonth",
                principalColumn: "AccMonthId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_AccMonth_AccMonthId",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_AccMonthId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "AccMonthId",
                table: "Inventory");
        }
    }
}
