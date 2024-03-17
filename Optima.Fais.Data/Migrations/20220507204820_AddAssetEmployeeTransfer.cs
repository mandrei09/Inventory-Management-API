using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetEmployeeTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "AssetOp",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "EmployeeTransferId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Asset",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Asset_EmployeeTransferId",
                table: "Asset",
                column: "EmployeeTransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Employee_EmployeeTransferId",
                table: "Asset",
                column: "EmployeeTransferId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Employee_EmployeeTransferId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_EmployeeTransferId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "EmployeeTransferId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Asset");
        }
    }
}
