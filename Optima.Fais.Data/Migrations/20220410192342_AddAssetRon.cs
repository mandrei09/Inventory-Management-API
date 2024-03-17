using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetRon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NetAmountRon",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmountRon",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountRon",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueInvRon",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRemRon",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetAmountRon",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TaxAmountRon",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TotalAmountRon",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ValueInvRon",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ValueRemRon",
                table: "Asset");
        }
    }
}
