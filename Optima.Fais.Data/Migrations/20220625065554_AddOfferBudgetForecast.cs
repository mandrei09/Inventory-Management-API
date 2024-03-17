using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferBudgetForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetForecastId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offer_BudgetForecastId",
                table: "Offer",
                column: "BudgetForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_BudgetForecast_BudgetForecastId",
                table: "Offer",
                column: "BudgetForecastId",
                principalTable: "BudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_BudgetForecast_BudgetForecastId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_BudgetForecastId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "BudgetForecastId",
                table: "Offer");
        }
    }
}
