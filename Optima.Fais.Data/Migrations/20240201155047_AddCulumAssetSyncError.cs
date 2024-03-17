using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCulumAssetSyncError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetSyncErrors_Asset_AssetId",
                table: "AssetSyncErrors");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetSyncErrors_Error_ErrorId",
                table: "AssetSyncErrors");

            migrationBuilder.DropIndex(
                name: "IX_AssetSyncErrors_AssetId",
                table: "AssetSyncErrors");

            migrationBuilder.DropIndex(
                name: "IX_AssetSyncErrors_ErrorId",
                table: "AssetSyncErrors");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "AssetSyncErrors");

            migrationBuilder.AlterColumn<string>(
                name: "SyncDate",
                table: "AssetSyncErrors",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "AssetId",
                table: "AssetSyncErrors",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Error",
                table: "AssetSyncErrors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SyncCode",
                table: "AssetSyncErrors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                table: "AssetSyncErrors");

            migrationBuilder.DropColumn(
                name: "SyncCode",
                table: "AssetSyncErrors");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SyncDate",
                table: "AssetSyncErrors",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssetId",
                table: "AssetSyncErrors",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "AssetSyncErrors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSyncErrors_AssetId",
                table: "AssetSyncErrors",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSyncErrors_ErrorId",
                table: "AssetSyncErrors",
                column: "ErrorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetSyncErrors_Asset_AssetId",
                table: "AssetSyncErrors",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetSyncErrors_Error_ErrorId",
                table: "AssetSyncErrors",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
