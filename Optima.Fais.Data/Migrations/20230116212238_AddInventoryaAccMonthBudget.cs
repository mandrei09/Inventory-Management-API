using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInventoryaAccMonthBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "WFHCheck",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccMonthBudgetId",
                table: "Inventory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WFHCheck_AssetId",
                table: "WFHCheck",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_AccMonthBudgetId",
                table: "Inventory",
                column: "AccMonthBudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_AccMonth_AccMonthBudgetId",
                table: "Inventory",
                column: "AccMonthBudgetId",
                principalTable: "AccMonth",
                principalColumn: "AccMonthId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WFHCheck_Asset_AssetId",
                table: "WFHCheck",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_AccMonth_AccMonthBudgetId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_WFHCheck_Asset_AssetId",
                table: "WFHCheck");

            migrationBuilder.DropIndex(
                name: "IX_WFHCheck_AssetId",
                table: "WFHCheck");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_AccMonthBudgetId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "WFHCheck");

            migrationBuilder.DropColumn(
                name: "AccMonthBudgetId",
                table: "Inventory");
        }
    }
}
