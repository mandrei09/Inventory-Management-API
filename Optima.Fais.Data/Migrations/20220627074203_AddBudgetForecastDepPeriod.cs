using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetForecastDepPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DepPeriod",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DepPeriodRem",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StartMonthId",
                table: "BudgetForecast",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetForecast_StartMonthId",
                table: "BudgetForecast",
                column: "StartMonthId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetForecast_AccMonth_StartMonthId",
                table: "BudgetForecast",
                column: "StartMonthId",
                principalTable: "AccMonth",
                principalColumn: "AccMonthId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetForecast_AccMonth_StartMonthId",
                table: "BudgetForecast");

            migrationBuilder.DropIndex(
                name: "IX_BudgetForecast_StartMonthId",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "DepPeriod",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "DepPeriodRem",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "StartMonthId",
                table: "BudgetForecast");
        }
    }
}
