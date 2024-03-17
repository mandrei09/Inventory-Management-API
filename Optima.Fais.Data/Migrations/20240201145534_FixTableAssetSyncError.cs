using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class FixTableAssetSyncError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetSyncErrors_Error_ErrorIdId",
                table: "AssetSyncErrors");

            migrationBuilder.RenameColumn(
                name: "ErrorIdId",
                table: "AssetSyncErrors",
                newName: "ErrorId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetSyncErrors_ErrorIdId",
                table: "AssetSyncErrors",
                newName: "IX_AssetSyncErrors_ErrorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetSyncErrors_Error_ErrorId",
                table: "AssetSyncErrors",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetSyncErrors_Error_ErrorId",
                table: "AssetSyncErrors");

            migrationBuilder.RenameColumn(
                name: "ErrorId",
                table: "AssetSyncErrors",
                newName: "ErrorIdId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetSyncErrors_ErrorId",
                table: "AssetSyncErrors",
                newName: "IX_AssetSyncErrors_ErrorIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetSyncErrors_Error_ErrorIdId",
                table: "AssetSyncErrors",
                column: "ErrorIdId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
