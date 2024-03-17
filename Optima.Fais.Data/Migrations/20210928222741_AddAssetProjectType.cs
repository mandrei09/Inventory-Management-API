using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Asset",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_ProjectTypeId",
                table: "AssetAdmMD",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ProjectTypeId",
                table: "Asset",
                column: "ProjectTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_ProjectType_ProjectTypeId",
                table: "Asset",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_ProjectType_ProjectTypeId",
                table: "AssetAdmMD",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_ProjectType_ProjectTypeId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_ProjectType_ProjectTypeId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_ProjectTypeId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ProjectTypeId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Asset");
        }
    }
}
