using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestBudgetForecastNeedBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NeedBudgetValue",
                table: "RequestBudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NeedContractValue",
                table: "RequestBudgetForecast",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedBudgetValue",
                table: "RequestBudgetForecast");

            migrationBuilder.DropColumn(
                name: "NeedContractValue",
                table: "RequestBudgetForecast");
        }
    }
}
