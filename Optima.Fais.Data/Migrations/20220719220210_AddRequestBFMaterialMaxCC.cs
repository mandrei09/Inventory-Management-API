using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestBFMaterialMaxCC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxQuantity",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValueRon",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalCostCenterQuantity",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCostCenterValue",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCostCenterValueRon",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Multiple",
                table: "RequestBFMaterialCostCenter",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxQuantity",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "MaxValueRon",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "TotalCostCenterQuantity",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "TotalCostCenterValue",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "TotalCostCenterValueRon",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "Multiple",
                table: "RequestBFMaterialCostCenter");
        }
    }
}
