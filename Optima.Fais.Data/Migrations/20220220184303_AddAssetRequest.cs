using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_RequestId",
                table: "AssetAdmMD",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_RequestId",
                table: "Asset",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Request_RequestId",
                table: "Asset",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Request_RequestId",
                table: "AssetAdmMD",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Request_RequestId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Request_RequestId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_RequestId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_RequestId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Asset");
        }
    }
}
