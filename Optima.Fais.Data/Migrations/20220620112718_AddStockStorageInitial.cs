using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddStockStorageInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlantActualId",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlantInitialId",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageId",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageInitialId",
                table: "Stock",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stock_PlantActualId",
                table: "Stock",
                column: "PlantActualId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_PlantInitialId",
                table: "Stock",
                column: "PlantInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_StorageId",
                table: "Stock",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_StorageInitialId",
                table: "Stock",
                column: "StorageInitialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Plant_PlantActualId",
                table: "Stock",
                column: "PlantActualId",
                principalTable: "Plant",
                principalColumn: "PlantId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Plant_PlantInitialId",
                table: "Stock",
                column: "PlantInitialId",
                principalTable: "Plant",
                principalColumn: "PlantId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Storage_StorageId",
                table: "Stock",
                column: "StorageId",
                principalTable: "Storage",
                principalColumn: "StorageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Storage_StorageInitialId",
                table: "Stock",
                column: "StorageInitialId",
                principalTable: "Storage",
                principalColumn: "StorageId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Plant_PlantActualId",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Plant_PlantInitialId",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Storage_StorageId",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Storage_StorageInitialId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_Stock_PlantActualId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_Stock_PlantInitialId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_Stock_StorageId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_Stock_StorageInitialId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "PlantActualId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "PlantInitialId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "StorageInitialId",
                table: "Stock");
        }
    }
}
