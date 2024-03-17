using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCOstCenterDivision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_DivisionId",
                table: "CostCenter",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Division_DivisionId",
                table: "CostCenter",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Division_DivisionId",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_DivisionId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "CostCenter");
        }
    }
}
