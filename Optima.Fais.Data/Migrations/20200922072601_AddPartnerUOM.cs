using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddPartnerUOM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "Partner",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocation_UomId",
                table: "PartnerLocation",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_UomId",
                table: "Partner",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_Uom_UomId",
                table: "Partner",
                column: "UomId",
                principalTable: "Uom",
                principalColumn: "UomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocation_Uom_UomId",
                table: "PartnerLocation",
                column: "UomId",
                principalTable: "Uom",
                principalColumn: "UomId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partner_Uom_UomId",
                table: "Partner");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocation_Uom_UomId",
                table: "PartnerLocation");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocation_UomId",
                table: "PartnerLocation");

            migrationBuilder.DropIndex(
                name: "IX_Partner_UomId",
                table: "Partner");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "Partner");
        }
    }
}
