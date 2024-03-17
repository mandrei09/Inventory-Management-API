using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInventoryPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryPlan",
                columns: table => new
                {
                    InventoryPlanId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InventoryId = table.Column<int>(nullable: false),
                    InvCommitteeId = table.Column<int>(nullable: false),
                    AdministrationId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateFinished = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    DateStarted = table.Column<DateTime>(type: "datetime2(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryPlan", x => x.InventoryPlanId);
                    table.ForeignKey(
                        name: "FK_InventoryPlan_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryPlan_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryPlan_InvCommittee_InvCommitteeId",
                        column: x => x.InvCommitteeId,
                        principalTable: "InvCommittee",
                        principalColumn: "InvCommitteeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryPlan_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryPlan_AdministrationId",
                table: "InventoryPlan",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryPlan_CostCenterId",
                table: "InventoryPlan",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryPlan_InvCommitteeId",
                table: "InventoryPlan",
                column: "InvCommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryPlan_InventoryId",
                table: "InventoryPlan",
                column: "InventoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryPlan");
        }
    }
}
