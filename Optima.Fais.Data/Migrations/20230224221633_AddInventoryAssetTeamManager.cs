using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInventoryAssetTeamManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<string>(
                name: "InventoryResponsableId",
                table: "InventoryAsset",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InventoryTeamManagerId",
                table: "InventoryAsset",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_InventoryResponsableId",
                table: "InventoryAsset",
                column: "InventoryResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_InventoryTeamManagerId",
                table: "InventoryAsset",
                column: "InventoryTeamManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAsset_AspNetUsers_InventoryResponsableId",
                table: "InventoryAsset",
                column: "InventoryResponsableId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAsset_AspNetUsers_InventoryTeamManagerId",
                table: "InventoryAsset",
                column: "InventoryTeamManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAsset_AspNetUsers_InventoryResponsableId",
                table: "InventoryAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAsset_AspNetUsers_InventoryTeamManagerId",
                table: "InventoryAsset");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAsset_InventoryResponsableId",
                table: "InventoryAsset");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAsset_InventoryTeamManagerId",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "InventoryResponsableId",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "InventoryTeamManagerId",
                table: "InventoryAsset");
        }
    }
}
