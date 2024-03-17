using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetForecast_YTD_YTG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValueAssetYTD",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueAssetYTG",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueAssetYTD",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "ValueAssetYTG",
                table: "BudgetForecast");
        }
    }
}
