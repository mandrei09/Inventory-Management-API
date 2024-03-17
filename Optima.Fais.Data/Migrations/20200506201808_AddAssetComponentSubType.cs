using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetComponentSubType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<int>(
                name: "SubTypeId",
                table: "AssetComponent",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_AssetComponent_SubTypeId",
                table: "AssetComponent",
                column: "SubTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetComponent_SubType_SubTypeId",
                table: "AssetComponent",
                column: "SubTypeId",
                principalTable: "SubType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetComponent_SubType_SubTypeId",
                table: "AssetComponent");

        
            migrationBuilder.DropIndex(
                name: "IX_AssetComponent_SubTypeId",
                table: "AssetComponent");

            migrationBuilder.DropColumn(
                name: "SubTypeId",
                table: "AssetComponent");
        }
    }
}
