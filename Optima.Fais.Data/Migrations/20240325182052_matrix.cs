using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class matrix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_AssetType_AssetTypeId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_CostCenter_CostCenterId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Country_CountryId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Project_ProjectId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_ProjectType_ProjectTypeId",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_AssetTypeId",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_CostCenterId",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_CountryId",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_ProjectId",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_ProjectTypeId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL1",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL2",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL3",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL4",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityS1",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityS2",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityS3",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL1",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityL2",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityL3",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityL4",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityS1",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityS2",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityS3",
                table: "EmailOrderStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL1",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL2",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL3",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL4",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS1",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS2",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS3",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL1",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL2",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL3",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL4",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS1",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS2",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS3",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_AssetTypeId",
                table: "Matrix",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_CostCenterId",
                table: "Matrix",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_CountryId",
                table: "Matrix",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_ProjectId",
                table: "Matrix",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_ProjectTypeId",
                table: "Matrix",
                column: "ProjectTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_AssetType_AssetTypeId",
                table: "Matrix",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_CostCenter_CostCenterId",
                table: "Matrix",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Country_CountryId",
                table: "Matrix",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Project_ProjectId",
                table: "Matrix",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_ProjectType_ProjectTypeId",
                table: "Matrix",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
