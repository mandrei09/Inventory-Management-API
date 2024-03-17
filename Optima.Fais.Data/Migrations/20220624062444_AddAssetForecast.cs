using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetForecastId",
                table: "Request",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetForecastId",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueAsset",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueOrder",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BudgetForecastId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_BudgetForecastId",
                table: "Request",
                column: "BudgetForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BudgetForecastId",
                table: "Order",
                column: "BudgetForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_BudgetForecastId",
                table: "Asset",
                column: "BudgetForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_BudgetForecast_BudgetForecastId",
                table: "Asset",
                column: "BudgetForecastId",
                principalTable: "BudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_BudgetForecast_BudgetForecastId",
                table: "Order",
                column: "BudgetForecastId",
                principalTable: "BudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_BudgetForecast_BudgetForecastId",
                table: "Request",
                column: "BudgetForecastId",
                principalTable: "BudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_BudgetForecast_BudgetForecastId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_BudgetForecast_BudgetForecastId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_BudgetForecast_BudgetForecastId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_BudgetForecastId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Order_BudgetForecastId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Asset_BudgetForecastId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "BudgetForecastId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "BudgetForecastId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ValueAsset",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "ValueOrder",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "BudgetForecastId",
                table: "Asset");
        }
    }
}
