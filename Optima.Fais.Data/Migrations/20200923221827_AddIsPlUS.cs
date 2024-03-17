using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddIsPlUS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetRecoStateId",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfoMinus",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfoPlus",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMinus",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlus",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AssetRecoStateId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfoMinus",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfoPlus",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMinus",
                table: "AssetOp",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlus",
                table: "AssetOp",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InfoMinus",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfoPlus",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMinus",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlus",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_AssetRecoStateId",
                table: "InventoryAsset",
                column: "AssetRecoStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetRecoStateId",
                table: "AssetOp",
                column: "AssetRecoStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AppState_AssetRecoStateId",
                table: "AssetOp",
                column: "AssetRecoStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAsset_AppState_AssetRecoStateId",
                table: "InventoryAsset",
                column: "AssetRecoStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AppState_AssetRecoStateId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAsset_AppState_AssetRecoStateId",
                table: "InventoryAsset");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAsset_AssetRecoStateId",
                table: "InventoryAsset");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_AssetRecoStateId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "AssetRecoStateId",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "InfoMinus",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "InfoPlus",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "IsMinus",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "IsPlus",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "AssetRecoStateId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "InfoMinus",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "InfoPlus",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "IsMinus",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "IsPlus",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "InfoMinus",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "InfoPlus",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsMinus",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsPlus",
                table: "Asset");
        }
    }
}
