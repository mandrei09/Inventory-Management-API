using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetDictionaryItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DictionaryItemId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asset_DictionaryItemId",
                table: "Asset",
                column: "DictionaryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_DictionaryItem_DictionaryItemId",
                table: "Asset",
                column: "DictionaryItemId",
                principalTable: "DictionaryItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_DictionaryItem_DictionaryItemId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_DictionaryItemId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "DictionaryItemId",
                table: "Asset");
        }
    }
}
