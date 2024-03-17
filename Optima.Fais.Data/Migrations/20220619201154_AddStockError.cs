using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddStockError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Imported",
                table: "Stock",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Validated",
                table: "Stock",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Stock_ErrorId",
                table: "Stock",
                column: "ErrorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Error_ErrorId",
                table: "Stock",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Error_ErrorId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_Stock_ErrorId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "Imported",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "Validated",
                table: "Stock");
        }
    }
}
