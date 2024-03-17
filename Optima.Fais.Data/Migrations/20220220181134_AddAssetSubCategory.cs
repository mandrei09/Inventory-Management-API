using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetSubCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_SubCategoryId",
                table: "AssetAdmMD",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_SubCategoryId",
                table: "Asset",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_SubCategory_SubCategoryId",
                table: "Asset",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_SubCategory_SubCategoryId",
                table: "AssetAdmMD",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_SubCategory_SubCategoryId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_SubCategory_SubCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_SubCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_SubCategoryId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Asset");
        }
    }
}
