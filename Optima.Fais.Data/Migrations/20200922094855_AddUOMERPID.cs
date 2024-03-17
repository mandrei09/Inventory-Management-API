using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddUOMERPID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocation_Uom_UomId",
                table: "PartnerLocation");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocation_UomId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "PartnerLocation");

            migrationBuilder.AddColumn<int>(
                name: "ERPId",
                table: "Uom",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ERPInitialId",
                table: "Uom",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerLocationId",
                table: "Uom",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uom_PartnerLocationId",
                table: "Uom",
                column: "PartnerLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Uom_PartnerLocation_PartnerLocationId",
                table: "Uom",
                column: "PartnerLocationId",
                principalTable: "PartnerLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uom_PartnerLocation_PartnerLocationId",
                table: "Uom");

            migrationBuilder.DropIndex(
                name: "IX_Uom_PartnerLocationId",
                table: "Uom");

            migrationBuilder.DropColumn(
                name: "ERPId",
                table: "Uom");

            migrationBuilder.DropColumn(
                name: "ERPInitialId",
                table: "Uom");

            migrationBuilder.DropColumn(
                name: "PartnerLocationId",
                table: "Uom");

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocation_UomId",
                table: "PartnerLocation",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocation_Uom_UomId",
                table: "PartnerLocation",
                column: "UomId",
                principalTable: "Uom",
                principalColumn: "UomId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
