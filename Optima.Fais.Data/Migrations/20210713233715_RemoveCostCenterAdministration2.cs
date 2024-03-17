using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class RemoveCostCenterAdministration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administration_CostCenter_CostCenterId",
                table: "Administration");

            //migrationBuilder.DropIndex(
            //    name: "IX_Administration_CostCenterId",
            //    table: "Administration");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "Administration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "Administration",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administration_CostCenterId",
                table: "Administration",
                column: "CostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Administration_CostCenter_CostCenterId",
                table: "Administration",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
