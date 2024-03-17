using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetNatureAssetTypeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "BudgetManager",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "AssetNature",
                nullable: true);

         

            migrationBuilder.CreateIndex(
                name: "IX_BudgetManager_UomId",
                table: "BudgetManager",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNature_AssetTypeId",
                table: "AssetNature",
                column: "AssetTypeId");

           

            migrationBuilder.AddForeignKey(
                name: "FK_AssetNature_AssetType_AssetTypeId",
                table: "AssetNature",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetManager_Uom_UomId",
                table: "BudgetManager",
                column: "UomId",
                principalTable: "Uom",
                principalColumn: "UomId",
                onDelete: ReferentialAction.Restrict);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetNature_AssetType_AssetTypeId",
                table: "AssetNature");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetManager_Uom_UomId",
                table: "BudgetManager");

           

            migrationBuilder.DropIndex(
                name: "IX_BudgetManager_UomId",
                table: "BudgetManager");

            migrationBuilder.DropIndex(
                name: "IX_AssetNature_AssetTypeId",
                table: "AssetNature");

          

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "BudgetManager");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "AssetNature");

         
        }
    }
}
