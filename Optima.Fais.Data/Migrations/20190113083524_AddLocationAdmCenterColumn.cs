using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddLocationAdmCenterColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdmCenterId",
                table: "Location",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_AdmCenterId",
                table: "Location",
                column: "AdmCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_AdmCenter_AdmCenterId",
                table: "Location",
                column: "AdmCenterId",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_AdmCenter_AdmCenterId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_AdmCenterId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "AdmCenterId",
                table: "Location");
        }
    }
}
