using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInventoryAssetImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageCount",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReconcile",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemp",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TempName",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TempReco",
                table: "InventoryAsset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCount",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "IsReconcile",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "IsTemp",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "TempName",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "TempReco",
                table: "InventoryAsset");

        }
    }
}
