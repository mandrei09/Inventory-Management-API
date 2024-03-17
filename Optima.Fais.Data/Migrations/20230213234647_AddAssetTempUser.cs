using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetTempUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempUserId",
                table: "InventoryAsset",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InInventory",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TempUserId",
                table: "Asset",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_TempUserId",
                table: "InventoryAsset",
                column: "TempUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_TempUserId",
                table: "Asset",
                column: "TempUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AspNetUsers_TempUserId",
                table: "Asset",
                column: "TempUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAsset_AspNetUsers_TempUserId",
                table: "InventoryAsset",
                column: "TempUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AspNetUsers_TempUserId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAsset_AspNetUsers_TempUserId",
                table: "InventoryAsset");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAsset_TempUserId",
                table: "InventoryAsset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_TempUserId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TempUserId",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "InInventory",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TempUserId",
                table: "Asset");
        }
    }
}
