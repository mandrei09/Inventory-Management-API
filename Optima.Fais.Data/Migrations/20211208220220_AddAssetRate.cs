using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NetAmount",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Asset_RateId",
                table: "Asset",
                column: "RateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Rate_RateId",
                table: "Asset",
                column: "RateId",
                principalTable: "Rate",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Rate_RateId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_RateId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "NetAmount",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Asset");
        }
    }
}
