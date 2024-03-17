using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEntityFileOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_OrderId",
                table: "EntityFile",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_Order_OrderId",
                table: "EntityFile",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_Order_OrderId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_OrderId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "EntityFile");
        }
    }
}
