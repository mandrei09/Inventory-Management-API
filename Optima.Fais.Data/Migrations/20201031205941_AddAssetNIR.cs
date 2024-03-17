using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetNIR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntityTypeId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedUser",
                table: "Asset",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NIRDate",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NIRNumber",
                table: "Asset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PIFDate",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PIFNumber",
                table: "Asset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_EntityTypeId",
                table: "AssetOp",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_CreatedUser",
                table: "Asset",
                column: "CreatedUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AspNetUsers_CreatedUser",
                table: "Asset",
                column: "CreatedUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_EntityType_EntityTypeId",
                table: "AssetOp",
                column: "EntityTypeId",
                principalTable: "EntityType",
                principalColumn: "EntityTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AspNetUsers_CreatedUser",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_EntityType_EntityTypeId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_EntityTypeId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_Asset_CreatedUser",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "EntityTypeId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "NIRDate",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "NIRNumber",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "PIFDate",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "PIFNumber",
                table: "Asset");
        }
    }
}
