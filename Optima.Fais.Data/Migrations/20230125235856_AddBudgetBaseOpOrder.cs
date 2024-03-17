using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetBaseOpOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "BudgetBaseOp",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_OrderId",
                table: "BudgetBaseOp",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetBaseOp_Order_OrderId",
                table: "BudgetBaseOp",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetBaseOp_Order_OrderId",
                table: "BudgetBaseOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetBaseOp_OrderId",
                table: "BudgetBaseOp");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "BudgetBaseOp");
        }
    }
}
