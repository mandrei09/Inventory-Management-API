using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAdmCenterEmployeeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "AdmCenter",
                nullable: true);

          
            migrationBuilder.CreateIndex(
                name: "IX_AdmCenter_EmployeeId",
                table: "AdmCenter",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmCenter_Employee_EmployeeId",
                table: "AdmCenter",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmCenter_Employee_EmployeeId",
                table: "AdmCenter");

            migrationBuilder.DropIndex(
                name: "IX_AdmCenter_EmployeeId",
                table: "AdmCenter");


            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AdmCenter");
        }
    }
}
