using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCommittee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "RouteChildren",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Route",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "CostCenter",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId7",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Committees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministrationId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Document1 = table.Column<string>(nullable: true),
                    Document2 = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    EmployeeId2 = table.Column<int>(nullable: true),
                    EmployeeId3 = table.Column<int>(nullable: true),
                    EmployeeId4 = table.Column<int>(nullable: true),
                    EmployeeId5 = table.Column<int>(nullable: true),
                    EmployeeId6 = table.Column<int>(nullable: true),
                    EmployeeId7 = table.Column<int>(nullable: true),
                    InventoryId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    RoomId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Committees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Committees_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Employee_EmployeeId2",
                        column: x => x.EmployeeId2,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Employee_EmployeeId3",
                        column: x => x.EmployeeId3,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Employee_EmployeeId4",
                        column: x => x.EmployeeId4,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Employee_EmployeeId5",
                        column: x => x.EmployeeId5,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Employee_EmployeeId6",
                        column: x => x.EmployeeId6,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Employee_EmployeeId7",
                        column: x => x.EmployeeId7,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committees_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_EmployeeId",
                table: "CostCenter",
                column: "EmployeeId");

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

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_EmployeeId7",
                table: "CostCenter",
                column: "EmployeeId7");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_AdministrationId",
                table: "Committees",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_CostCenterId",
                table: "Committees",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_EmployeeId",
                table: "Committees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_EmployeeId2",
                table: "Committees",
                column: "EmployeeId2");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_EmployeeId3",
                table: "Committees",
                column: "EmployeeId3");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_EmployeeId4",
                table: "Committees",
                column: "EmployeeId4");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_EmployeeId5",
                table: "Committees",
                column: "EmployeeId5");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_EmployeeId6",
                table: "Committees",
                column: "EmployeeId6");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_EmployeeId7",
                table: "Committees",
                column: "EmployeeId7");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_InventoryId",
                table: "Committees",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_RoomId",
                table: "Committees",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId",
                table: "CostCenter",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Employee_EmployeeId",
                table: "CostCenter");

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

            migrationBuilder.DropTable(
                name: "Committees");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_EmployeeId",
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

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_EmployeeId7",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "RouteChildren");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
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

            migrationBuilder.DropColumn(
                name: "EmployeeId7",
                table: "CostCenter");
        }
    }
}
