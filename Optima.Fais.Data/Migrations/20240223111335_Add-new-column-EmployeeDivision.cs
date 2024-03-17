using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddnewcolumnEmployeeDivision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "EmployeeDivision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDivision_DepartmentId",
                table: "EmployeeDivision",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDivision_Department_DepartmentId",
                table: "EmployeeDivision",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDivision_Department_DepartmentId",
                table: "EmployeeDivision");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDivision_DepartmentId",
                table: "EmployeeDivision");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "EmployeeDivision");
        }
    }
}
