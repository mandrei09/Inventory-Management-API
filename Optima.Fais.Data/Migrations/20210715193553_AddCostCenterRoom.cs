using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCostCenterRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_RoomId",
                table: "CostCenter",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Room_RoomId",
                table: "CostCenter",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Room_RoomId",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_RoomId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "CostCenter");
        }
    }
}
