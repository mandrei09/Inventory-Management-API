using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInventoryBudgetManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetManagerId",
                table: "Inventory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccMonthId",
                table: "BudgetMonthBase",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccMonthId",
                table: "BudgetForecast",
                nullable: true);
           

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_BudgetManagerId",
                table: "Inventory",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonthBase_AccMonthId",
                table: "BudgetMonthBase",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetForecast_AccMonthId",
                table: "BudgetForecast",
                column: "AccMonthId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetForecast_AccMonth_AccMonthId",
                table: "BudgetForecast",
                column: "AccMonthId",
                principalTable: "AccMonth",
                principalColumn: "AccMonthId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetMonthBase_AccMonth_AccMonthId",
                table: "BudgetMonthBase",
                column: "AccMonthId",
                principalTable: "AccMonth",
                principalColumn: "AccMonthId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_BudgetManager_BudgetManagerId",
                table: "Inventory",
                column: "BudgetManagerId",
                principalTable: "BudgetManager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetForecast_AccMonth_AccMonthId",
                table: "BudgetForecast");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetMonthBase_AccMonth_AccMonthId",
                table: "BudgetMonthBase");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_BudgetManager_BudgetManagerId",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_BudgetManagerId",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_BudgetMonthBase_AccMonthId",
                table: "BudgetMonthBase");

            migrationBuilder.DropIndex(
                name: "IX_BudgetForecast_AccMonthId",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "BudgetManagerId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "AccMonthId",
                table: "BudgetMonthBase");

            migrationBuilder.DropColumn(
                name: "AccMonthId",
                table: "BudgetForecast");
        }
    }
}
