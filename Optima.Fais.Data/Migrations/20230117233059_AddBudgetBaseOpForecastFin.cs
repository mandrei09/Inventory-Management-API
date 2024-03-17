using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetBaseOpForecastFin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetForecastFinId",
                table: "BudgetBaseOp",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetForecastFinId",
                table: "BudgetBaseOp",
                column: "BudgetForecastFinId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetBaseOp_BudgetForecast_BudgetForecastFinId",
                table: "BudgetBaseOp",
                column: "BudgetForecastFinId",
                principalTable: "BudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetBaseOp_BudgetForecast_BudgetForecastFinId",
                table: "BudgetBaseOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetBaseOp_BudgetForecastFinId",
                table: "BudgetBaseOp");

            migrationBuilder.DropColumn(
                name: "BudgetForecastFinId",
                table: "BudgetBaseOp");
        }
    }
}
