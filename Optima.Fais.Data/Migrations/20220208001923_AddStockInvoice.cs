using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddStockInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EAN",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Invoice",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TempRecoSerialNumber",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetRecoStateId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TempSerialNumber",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AssetRecoStateId",
                table: "Asset",
                column: "AssetRecoStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AppState_AssetRecoStateId",
                table: "Asset",
                column: "AssetRecoStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AppState_AssetRecoStateId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_AssetRecoStateId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "EAN",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "Invoice",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "TempRecoSerialNumber",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "AssetRecoStateId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TempSerialNumber",
                table: "Asset");
        }
    }
}
