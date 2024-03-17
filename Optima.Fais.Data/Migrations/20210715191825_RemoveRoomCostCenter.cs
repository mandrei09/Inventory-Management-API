using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class RemoveRoomCostCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_CostCenter_CostCenterId",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_CostCenterId",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "Room");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "Room",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Room_CostCenterId",
                table: "Room",
                column: "CostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_CostCenter_CostCenterId",
                table: "Room",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
