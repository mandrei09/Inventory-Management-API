using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAdmAdmCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "Division",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "Department",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdmCenterId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Division_ActivityId",
                table: "Division",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_ActivityId",
                table: "Department",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_AdmCenterId",
                table: "AssetAdmMD",
                column: "AdmCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_RegionId",
                table: "AssetAdmMD",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_AdmCenter_AdmCenterId",
                table: "AssetAdmMD",
                column: "AdmCenterId",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Region_RegionId",
                table: "AssetAdmMD",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Activity_ActivityId",
                table: "Department",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Division_Activity_ActivityId",
                table: "Division",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_AdmCenter_AdmCenterId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Region_RegionId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Activity_ActivityId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Division_Activity_ActivityId",
                table: "Division");

            migrationBuilder.DropIndex(
                name: "IX_Division_ActivityId",
                table: "Division");

            migrationBuilder.DropIndex(
                name: "IX_Department_ActivityId",
                table: "Department");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_AdmCenterId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_RegionId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "AdmCenterId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "AssetAdmMD");
        }
    }
}
