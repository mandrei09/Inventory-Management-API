using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmployeeLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Employee",
                nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "InvStateId",
            //    table: "AssetInv",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_LocationId",
                table: "Employee",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Location_LocationId",
                table: "Employee",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Location_LocationId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_LocationId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Employee");

            //migrationBuilder.DropColumn(
            //    name: "InvStateId",
            //    table: "AssetInv");
        }
    }
}
