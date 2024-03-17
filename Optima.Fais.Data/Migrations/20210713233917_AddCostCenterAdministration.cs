using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCostCenterAdministration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdministrationId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_AdministrationId",
                table: "CostCenter",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_LocationId",
                table: "CostCenter",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Administration_AdministrationId",
                table: "CostCenter",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "AdministrationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Location_LocationId",
                table: "CostCenter",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Administration_AdministrationId",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Location_LocationId",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_AdministrationId",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_LocationId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "AdministrationId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CostCenter");
        }
    }
}
