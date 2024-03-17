using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddDivisionDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Division",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Division_DepartmentId",
                table: "Division",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Division_Department_DepartmentId",
                table: "Division",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Division_Department_DepartmentId",
                table: "Division");

            migrationBuilder.DropIndex(
                name: "IX_Division_DepartmentId",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Division");
        }
    }
}
