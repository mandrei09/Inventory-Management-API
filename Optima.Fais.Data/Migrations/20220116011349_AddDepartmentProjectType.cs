using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddDepartmentProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "ProjectType",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectType_DepartmentId",
                table: "ProjectType",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectType_Department_DepartmentId",
                table: "ProjectType",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectType_Department_DepartmentId",
                table: "ProjectType");

            migrationBuilder.DropIndex(
                name: "IX_ProjectType_DepartmentId",
                table: "ProjectType");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ProjectType");
        }
    }
}
