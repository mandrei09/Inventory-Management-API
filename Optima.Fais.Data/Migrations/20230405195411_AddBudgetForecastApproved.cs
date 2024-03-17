using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetForecastApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValOrderApproved",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValOrderPending",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValRequest",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValOrderApproved",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "ValOrderPending",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "ValRequest",
                table: "BudgetForecast");
        }
    }
}
