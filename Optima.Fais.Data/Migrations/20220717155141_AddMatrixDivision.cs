using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMatrixDivision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Area_AreaId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_AssetType_AssetTypeId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Company_CompanyId",
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

            migrationBuilder.AlterColumn<int>(
                name: "ProjectTypeId",
                table: "Matrix",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Matrix",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Matrix",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CostCenterId",
                table: "Matrix",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Matrix",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "AssetTypeId",
                table: "Matrix",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Matrix",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Matrix",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_DivisionId",
                table: "Matrix",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Area_AreaId",
                table: "Matrix",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "AreaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_AssetType_AssetTypeId",
                table: "Matrix",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Company_CompanyId",
                table: "Matrix",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
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
                name: "FK_Matrix_Division_DivisionId",
                table: "Matrix",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Area_AreaId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_AssetType_AssetTypeId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Company_CompanyId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_CostCenter_CostCenterId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Country_CountryId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Division_DivisionId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Project_ProjectId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_ProjectType_ProjectTypeId",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_DivisionId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Matrix");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectTypeId",
                table: "Matrix",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Matrix",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Matrix",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CostCenterId",
                table: "Matrix",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Matrix",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssetTypeId",
                table: "Matrix",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Matrix",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Area_AreaId",
                table: "Matrix",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "AreaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_AssetType_AssetTypeId",
                table: "Matrix",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Company_CompanyId",
                table: "Matrix",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_CostCenter_CostCenterId",
                table: "Matrix",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Country_CountryId",
                table: "Matrix",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Project_ProjectId",
                table: "Matrix",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_ProjectType_ProjectTypeId",
                table: "Matrix",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
