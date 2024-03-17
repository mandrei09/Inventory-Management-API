using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetOpBudgetBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Budget_BudgetId",
                table: "BudgetOp");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetId",
                table: "BudgetOp",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "BudgetBaseId",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_BudgetBaseId",
                table: "BudgetOp",
                column: "BudgetBaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_BudgetBase_BudgetBaseId",
                table: "BudgetOp",
                column: "BudgetBaseId",
                principalTable: "BudgetBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Budget_BudgetId",
                table: "BudgetOp",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_BudgetBase_BudgetBaseId",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Budget_BudgetId",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_BudgetBaseId",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "BudgetBaseId",
                table: "BudgetOp");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetId",
                table: "BudgetOp",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Budget_BudgetId",
                table: "BudgetOp",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
