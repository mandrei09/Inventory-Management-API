using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class InvCommitteeMemberInventoryPlanId4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryPlanId",
                table: "InvCommitteeMember",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvCommitteeMember_InventoryPlanId",
                table: "InvCommitteeMember",
                column: "InventoryPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvCommitteeMember_InventoryPlan_InventoryPlanId",
                table: "InvCommitteeMember",
                column: "InventoryPlanId",
                principalTable: "InventoryPlan",
                principalColumn: "InventoryPlanId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvCommitteeMember_InventoryPlan_InventoryPlanId",
                table: "InvCommitteeMember");

            migrationBuilder.DropIndex(
                name: "IX_InvCommitteeMember_InventoryPlanId",
                table: "InvCommitteeMember");

            migrationBuilder.DropColumn(
                name: "InventoryPlanId",
                table: "InvCommitteeMember");
        }
    }
}
