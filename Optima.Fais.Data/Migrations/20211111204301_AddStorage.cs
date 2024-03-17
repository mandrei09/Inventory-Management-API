using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId2",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId3",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId4",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId5",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId6",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId7",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_EmployeeId2",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_EmployeeId3",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_EmployeeId4",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_EmployeeId5",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_EmployeeId6",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "EmployeeId2",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "EmployeeId3",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "EmployeeId4",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "EmployeeId5",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "EmployeeId6",
                table: "CostCenter");

            migrationBuilder.RenameColumn(
                name: "EmployeeId7",
                table: "CostCenter",
                newName: "StorageId");

            migrationBuilder.RenameIndex(
                name: "IX_CostCenter_EmployeeId7",
                table: "CostCenter",
                newName: "IX_CostCenter_StorageId");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Employee",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    ManagerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetCount = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DepartmentId = table.Column<int>(nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    InternalCode = table.Column<string>(maxLength: 30, nullable: false),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEmailSend = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotifyLast = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manager", x => x.ManagerId);
                    table.ForeignKey(
                        name: "FK_Manager_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Manager_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Plant",
                columns: table => new
                {
                    PlantId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plant", x => x.PlantId);
                    table.ForeignKey(
                        name: "FK_Plant_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ManagerId",
                table: "Employee",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_CompanyId",
                table: "Manager",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_DepartmentId",
                table: "Manager",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Plant_CompanyId",
                table: "Plant",
                column: "CompanyId");

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
                name: "FK_Employee_Manager_ManagerId",
                table: "Employee",
                column: "ManagerId",
                principalTable: "Manager",
                principalColumn: "ManagerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Storage_StorageId",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Manager_ManagerId",
                table: "Employee");

            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.DropTable(
                name: "Storage");

            migrationBuilder.DropTable(
                name: "Plant");

            migrationBuilder.DropIndex(
                name: "IX_Employee_ManagerId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Employee");

            migrationBuilder.RenameColumn(
                name: "StorageId",
                table: "CostCenter",
                newName: "EmployeeId7");

            migrationBuilder.RenameIndex(
                name: "IX_CostCenter_StorageId",
                table: "CostCenter",
                newName: "IX_CostCenter_EmployeeId7");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId2",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId3",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId4",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId5",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId6",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_EmployeeId2",
                table: "CostCenter",
                column: "EmployeeId2");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_EmployeeId3",
                table: "CostCenter",
                column: "EmployeeId3");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_EmployeeId4",
                table: "CostCenter",
                column: "EmployeeId4");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_EmployeeId5",
                table: "CostCenter",
                column: "EmployeeId5");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_EmployeeId6",
                table: "CostCenter",
                column: "EmployeeId6");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId2",
                table: "CostCenter",
                column: "EmployeeId2",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId3",
                table: "CostCenter",
                column: "EmployeeId3",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId4",
                table: "CostCenter",
                column: "EmployeeId4",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId5",
                table: "CostCenter",
                column: "EmployeeId5",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId6",
                table: "CostCenter",
                column: "EmployeeId6",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId7",
                table: "CostCenter",
                column: "EmployeeId7",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
