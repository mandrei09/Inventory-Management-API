using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmployeeAssetCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetCount",
                table: "Room",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssetCount",
                table: "Region",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssetCount",
                table: "Model",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssetCount",
                table: "Location",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssetCount",
                table: "Employee",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Model");

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "AssetCount",
                table: "Employee");
        }
    }
}
