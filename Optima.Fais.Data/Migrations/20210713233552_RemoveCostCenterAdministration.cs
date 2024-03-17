using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class RemoveCostCenterAdministration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_CostCenter_CostCenterId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_CostCenterId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "Location");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "Location",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_CostCenterId",
                table: "Location",
                column: "CostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_CostCenter_CostCenterId",
                table: "Location",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
