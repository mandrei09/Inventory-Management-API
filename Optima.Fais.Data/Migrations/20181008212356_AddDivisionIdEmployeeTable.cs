using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddDivisionIdEmployeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Location_LocationId",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Employee",
                newName: "DivisionId");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_LocationId",
                table: "Employee",
                newName: "IX_Employee_DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Division_DivisionId",
                table: "Employee",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Division_DivisionId",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "DivisionId",
                table: "Employee",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_DivisionId",
                table: "Employee",
                newName: "IX_Employee_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Location_LocationId",
                table: "Employee",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
