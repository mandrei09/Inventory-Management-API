using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddDivisionLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Division",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Division_LocationId",
                table: "Division",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Division_Location_LocationId",
                table: "Division",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Division_Location_LocationId",
                table: "Division");

            migrationBuilder.DropIndex(
                name: "IX_Division_LocationId",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Division");
        }
    }
}
