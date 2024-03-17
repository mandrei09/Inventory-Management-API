using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Request",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WBS_ELEMENT",
                table: "AcquisitionAssetSAP",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_OwnerId",
                table: "Request",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Employee_OwnerId",
                table: "Request",
                column: "OwnerId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Employee_OwnerId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_OwnerId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "WBS_ELEMENT",
                table: "AcquisitionAssetSAP");
        }
    }
}
