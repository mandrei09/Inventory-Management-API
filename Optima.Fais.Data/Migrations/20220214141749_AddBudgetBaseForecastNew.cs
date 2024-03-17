using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetBaseForecastNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetMonthBase_BudgetBaseId",
                table: "BudgetMonthBase");

            migrationBuilder.DropIndex(
                name: "IX_BudgetForecast_BudgetBaseId",
                table: "BudgetForecast");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonthBase_BudgetBaseId",
                table: "BudgetMonthBase",
                column: "BudgetBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetForecast_BudgetBaseId",
                table: "BudgetForecast",
                column: "BudgetBaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BudgetMonthBase_BudgetBaseId",
                table: "BudgetMonthBase");

            migrationBuilder.DropIndex(
                name: "IX_BudgetForecast_BudgetBaseId",
                table: "BudgetForecast");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonthBase_BudgetBaseId",
                table: "BudgetMonthBase",
                column: "BudgetBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetForecast_BudgetBaseId",
                table: "BudgetForecast",
                column: "BudgetBaseId",
                unique: true);
        }
    }
}
