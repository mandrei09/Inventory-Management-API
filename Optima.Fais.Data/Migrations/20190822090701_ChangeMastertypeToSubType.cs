using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class ChangeMastertypeToSubType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_MasterType_MasterTypeId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_MasterType_MasterTypeId",
                table: "AssetAdmMD");

            migrationBuilder.RenameColumn(
                name: "MasterTypeId",
                table: "AssetAdmMD",
                newName: "SubTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetAdmMD_MasterTypeId",
                table: "AssetAdmMD",
                newName: "IX_AssetAdmMD_SubTypeId");

            migrationBuilder.RenameColumn(
                name: "MasterTypeId",
                table: "Asset",
                newName: "SubTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Asset_MasterTypeId",
                table: "Asset",
                newName: "IX_Asset_SubTypeId");


            //migrationBuilder.AddForeignKey(
            //    name: "FK_Asset_SubType_SubTypeId",
            //    table: "Asset",
            //    column: "SubTypeId",
            //    principalTable: "SubType",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_AssetAdmMD_SubType_SubTypeId",
            //    table: "AssetAdmMD",
            //    column: "SubTypeId",
            //    principalTable: "SubType",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_SubType_SubTypeId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_SubType_SubTypeId",
                table: "AssetAdmMD");

            migrationBuilder.RenameColumn(
                name: "SubTypeId",
                table: "AssetAdmMD",
                newName: "MasterTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetAdmMD_SubTypeId",
                table: "AssetAdmMD",
                newName: "IX_AssetAdmMD_MasterTypeId");

            migrationBuilder.RenameColumn(
                name: "SubTypeId",
                table: "Asset",
                newName: "MasterTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Asset_SubTypeId",
                table: "Asset",
                newName: "IX_Asset_MasterTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_MasterType_MasterTypeId",
                table: "Asset",
                column: "MasterTypeId",
                principalTable: "MasterType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_MasterType_MasterTypeId",
                table: "AssetAdmMD",
                column: "MasterTypeId",
                principalTable: "MasterType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
