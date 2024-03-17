using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRBFValueUsed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsed",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsedRon",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsed",
                table: "RequestBudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsedRon",
                table: "RequestBudgetForecast",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueUsed",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "ValueUsedRon",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "ValueUsed",
                table: "RequestBudgetForecast");

            migrationBuilder.DropColumn(
                name: "ValueUsedRon",
                table: "RequestBudgetForecast");
        }
    }
}
