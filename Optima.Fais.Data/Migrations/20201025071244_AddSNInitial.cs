using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddSNInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowLabelInitial",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SNInitial",
                table: "InventoryAsset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowLabelInitial",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "SNInitial",
                table: "InventoryAsset");
        }
    }
}
