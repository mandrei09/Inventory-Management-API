using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class removestorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Storage_StorageId",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Storage_StorageId",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Storage_StorageInitialId",
                table: "Stock");

            migrationBuilder.DropTable(
                name: "EmployeeStorage");

            migrationBuilder.DropTable(
                name: "Storage");

            migrationBuilder.DropIndex(
                name: "IX_Stock_StorageId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_Stock_StorageInitialId",
                table: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_StorageId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "StorageInitialId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "CostCenter");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StorageId",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageInitialId",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Storage",
                columns: table => new
                {
                    StorageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    PlantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storage", x => x.StorageId);
                    table.ForeignKey(
                        name: "FK_Storage_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Storage_Plant_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plant",
                        principalColumn: "PlantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    StorageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeStorage_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeStorage_Storage_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storage",
                        principalColumn: "StorageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stock_StorageId",
                table: "Stock",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_StorageInitialId",
                table: "Stock",
                column: "StorageInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_StorageId",
                table: "CostCenter",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStorage_EmployeeId",
                table: "EmployeeStorage",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeStorage_StorageId",
                table: "EmployeeStorage",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_CompanyId",
                table: "Storage",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_PlantId",
                table: "Storage",
                column: "PlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Storage_StorageId",
                table: "CostCenter",
                column: "StorageId",
                principalTable: "Storage",
                principalColumn: "StorageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Storage_StorageId",
                table: "Stock",
                column: "StorageId",
                principalTable: "Storage",
                principalColumn: "StorageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Storage_StorageInitialId",
                table: "Stock",
                column: "StorageInitialId",
                principalTable: "Storage",
                principalColumn: "StorageId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
