using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCCAssetCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AssetCount",
                table: "Division",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssetCount",
                table: "Department",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssetCount",
                table: "CostCenter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssetCount",
                table: "Administration",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Administration");
        }
    }
}
