using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetOPTransferTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetNatureFinalId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetNatureIdFinal",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetNatureIdInitial",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetNatureInitialId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetManagerFinalId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetManagerIdFinal",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetManagerIdInitial",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetManagerInitialId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DimensionFinalId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DimensionIdFinal",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DimensionIdInitial",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DimensionInitialId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectFinalId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectIdFinal",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectIdInitial",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectInitialId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetNatureFinalId",
                table: "AssetOp",
                column: "AssetNatureFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetNatureInitialId",
                table: "AssetOp",
                column: "AssetNatureInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_BudgetManagerFinalId",
                table: "AssetOp",
                column: "BudgetManagerFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_BudgetManagerInitialId",
                table: "AssetOp",
                column: "BudgetManagerInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_DimensionFinalId",
                table: "AssetOp",
                column: "DimensionFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_DimensionInitialId",
                table: "AssetOp",
                column: "DimensionInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_ProjectFinalId",
                table: "AssetOp",
                column: "ProjectFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_ProjectInitialId",
                table: "AssetOp",
                column: "ProjectInitialId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AssetNature_AssetNatureFinalId",
                table: "AssetOp",
                column: "AssetNatureFinalId",
                principalTable: "AssetNature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AssetNature_AssetNatureInitialId",
                table: "AssetOp",
                column: "AssetNatureInitialId",
                principalTable: "AssetNature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_BudgetManager_BudgetManagerFinalId",
                table: "AssetOp",
                column: "BudgetManagerFinalId",
                principalTable: "BudgetManager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_BudgetManager_BudgetManagerInitialId",
                table: "AssetOp",
                column: "BudgetManagerInitialId",
                principalTable: "BudgetManager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Dimension_DimensionFinalId",
                table: "AssetOp",
                column: "DimensionFinalId",
                principalTable: "Dimension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Dimension_DimensionInitialId",
                table: "AssetOp",
                column: "DimensionInitialId",
                principalTable: "Dimension",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Project_ProjectFinalId",
                table: "AssetOp",
                column: "ProjectFinalId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Project_ProjectInitialId",
                table: "AssetOp",
                column: "ProjectInitialId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AssetNature_AssetNatureFinalId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AssetNature_AssetNatureInitialId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_BudgetManager_BudgetManagerFinalId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_BudgetManager_BudgetManagerInitialId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_Dimension_DimensionFinalId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_Dimension_DimensionInitialId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_Project_ProjectFinalId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_Project_ProjectInitialId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_AssetNatureFinalId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_AssetNatureInitialId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_BudgetManagerFinalId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_BudgetManagerInitialId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_DimensionFinalId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_DimensionInitialId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_ProjectFinalId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_ProjectInitialId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "AssetNatureFinalId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "AssetNatureIdFinal",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "AssetNatureIdInitial",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "AssetNatureInitialId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "BudgetManagerFinalId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "BudgetManagerIdFinal",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "BudgetManagerIdInitial",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "BudgetManagerInitialId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "DimensionFinalId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "DimensionIdFinal",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "DimensionIdInitial",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "DimensionInitialId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "ProjectFinalId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "ProjectIdFinal",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "ProjectIdInitial",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "ProjectInitialId",
                table: "AssetOp");

        }
    }
}
