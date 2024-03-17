using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class MIGR20240222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WFHCheck_BudgetManager_BudgetManagerId",
                table: "WFHCheck");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetManagerId",
                table: "WFHCheck",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_WFHCheck_BudgetManager_BudgetManagerId",
                table: "WFHCheck",
                column: "BudgetManagerId",
                principalTable: "BudgetManager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WFHCheck_BudgetManager_BudgetManagerId",
                table: "WFHCheck");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetManagerId",
                table: "WFHCheck",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WFHCheck_BudgetManager_BudgetManagerId",
                table: "WFHCheck",
                column: "BudgetManagerId",
                principalTable: "BudgetManager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
