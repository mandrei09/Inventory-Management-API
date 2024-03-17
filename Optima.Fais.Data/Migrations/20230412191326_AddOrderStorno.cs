using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOrderStorno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "ValueOrderApproved",
            //    table: "BudgetForecast");

            //migrationBuilder.DropColumn(
            //    name: "ValueOrderPending",
            //    table: "BudgetForecast");

            //migrationBuilder.DropColumn(
            //    name: "ValueRequest",
            //    table: "BudgetForecast");

            migrationBuilder.AddColumn<bool>(
                name: "Storno",
                table: "RequestBFMaterialCostCenter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StornoQuantity",
                table: "RequestBFMaterialCostCenter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "StornoValue",
                table: "RequestBFMaterialCostCenter",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "Storno",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StornoQuantity",
                table: "Asset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "StornoValue",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Storno",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropColumn(
                name: "StornoQuantity",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropColumn(
                name: "StornoValue",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropColumn(
                name: "Storno",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "StornoQuantity",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "StornoValue",
                table: "Asset");

            //migrationBuilder.AddColumn<decimal>(
            //    name: "ValueOrderApproved",
            //    table: "BudgetForecast",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "ValueOrderPending",
            //    table: "BudgetForecast",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "ValueRequest",
            //    table: "BudgetForecast",
            //    nullable: false,
            //    defaultValue: 0m);
        }
    }
}
