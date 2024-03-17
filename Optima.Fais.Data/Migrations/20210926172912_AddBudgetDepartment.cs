using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Budget",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budget_DepartmentId",
                table: "Budget",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_DivisionId",
                table: "Budget",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Department_DepartmentId",
                table: "Budget",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Division_DivisionId",
                table: "Budget",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Department_DepartmentId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Division_DivisionId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_DepartmentId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_DivisionId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Budget");
        }
    }
}
