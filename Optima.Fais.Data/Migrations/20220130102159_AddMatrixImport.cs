using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMatrixImport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountL1",
                table: "Matrix",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountL2",
                table: "Matrix",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountL3",
                table: "Matrix",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountL4",
                table: "Matrix",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountS1",
                table: "Matrix",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountS2",
                table: "Matrix",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountS3",
                table: "Matrix",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeL1Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeL2Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeL3Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeL4Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeS1Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeS2Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeS3Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MatrixImport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdmCenter = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    AssetTypeCode = table.Column<string>(nullable: true),
                    AssetTypeName = table.Column<string>(nullable: true),
                    CompanyCode = table.Column<string>(maxLength: 450, nullable: true),
                    CompanyName = table.Column<string>(maxLength: 450, nullable: true),
                    CostCenterCode = table.Column<string>(nullable: true),
                    CostCenterName = table.Column<string>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true),
                    CountryName = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DepartmentCode = table.Column<string>(nullable: true),
                    DepartmentName = table.Column<string>(nullable: true),
                    DivisionCode = table.Column<string>(nullable: true),
                    DivisionName = table.Column<string>(nullable: true),
                    Imported = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    L1UserId = table.Column<string>(nullable: true),
                    L1UserSum = table.Column<decimal>(nullable: false),
                    L2UserId = table.Column<string>(nullable: true),
                    L2UserSum = table.Column<decimal>(nullable: false),
                    L3UserId = table.Column<string>(nullable: true),
                    L3UserSum = table.Column<decimal>(nullable: false),
                    L4UserId = table.Column<string>(nullable: true),
                    L4UserSum = table.Column<decimal>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    S1UserId = table.Column<string>(nullable: true),
                    S1UserSum = table.Column<decimal>(nullable: false),
                    S2UserId = table.Column<string>(nullable: true),
                    S2UserSum = table.Column<decimal>(nullable: false),
                    S3UserId = table.Column<string>(nullable: true),
                    S3UserSum = table.Column<decimal>(nullable: false),
                    Used = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatrixImport", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeL1Id",
                table: "Matrix",
                column: "EmployeeL1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeL2Id",
                table: "Matrix",
                column: "EmployeeL2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeL3Id",
                table: "Matrix",
                column: "EmployeeL3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeL4Id",
                table: "Matrix",
                column: "EmployeeL4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeS1Id",
                table: "Matrix",
                column: "EmployeeS1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeS2Id",
                table: "Matrix",
                column: "EmployeeS2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeS3Id",
                table: "Matrix",
                column: "EmployeeS3Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeL1Id",
                table: "Matrix",
                column: "EmployeeL1Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeL2Id",
                table: "Matrix",
                column: "EmployeeL2Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeL3Id",
                table: "Matrix",
                column: "EmployeeL3Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeL4Id",
                table: "Matrix",
                column: "EmployeeL4Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeS1Id",
                table: "Matrix",
                column: "EmployeeS1Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeS2Id",
                table: "Matrix",
                column: "EmployeeS2Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeS3Id",
                table: "Matrix",
                column: "EmployeeS3Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeL1Id",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeL2Id",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeL3Id",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeL4Id",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeS1Id",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeS2Id",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeS3Id",
                table: "Matrix");

            migrationBuilder.DropTable(
                name: "MatrixImport");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeL1Id",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeL2Id",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeL3Id",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeL4Id",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeS1Id",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeS2Id",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeS3Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AmountL1",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AmountL2",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AmountL3",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AmountL4",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AmountS1",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AmountS2",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AmountS3",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeL1Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeL2Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeL3Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeL4Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeS1Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeS2Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeS3Id",
                table: "Matrix");
        }
    }
}
