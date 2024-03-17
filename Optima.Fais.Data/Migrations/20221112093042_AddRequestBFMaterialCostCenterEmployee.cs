using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestBFMaterialCostCenterEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "RequestBFMaterialCostCenter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_EmployeeId",
                table: "RequestBFMaterialCostCenter",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBFMaterialCostCenter_Employee_EmployeeId",
                table: "RequestBFMaterialCostCenter",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestBFMaterialCostCenter_Employee_EmployeeId",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropIndex(
                name: "IX_RequestBFMaterialCostCenter_EmployeeId",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "RequestBFMaterialCostCenter");
        }
    }
}
