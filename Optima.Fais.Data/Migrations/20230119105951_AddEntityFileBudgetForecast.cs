using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEntityFileBudgetForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetBaseOpId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetForecastId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_BudgetBaseOpId",
                table: "EntityFile",
                column: "BudgetBaseOpId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_BudgetForecastId",
                table: "EntityFile",
                column: "BudgetForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_BudgetBaseOp_BudgetBaseOpId",
                table: "EntityFile",
                column: "BudgetBaseOpId",
                principalTable: "BudgetBaseOp",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_BudgetForecast_BudgetForecastId",
                table: "EntityFile",
                column: "BudgetForecastId",
                principalTable: "BudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_BudgetBaseOp_BudgetBaseOpId",
                table: "EntityFile");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_BudgetForecast_BudgetForecastId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_BudgetBaseOpId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_BudgetForecastId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "BudgetBaseOpId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "BudgetForecastId",
                table: "EntityFile");
        }
    }
}
