using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetComponentInvState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<int>(
                name: "InvStateId",
                table: "AssetComponentOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvStateId",
                table: "AssetComponent",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponentOp_InvStateId",
                table: "AssetComponentOp",
                column: "InvStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponent_InvStateId",
                table: "AssetComponent",
                column: "InvStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetComponent_InvState_InvStateId",
                table: "AssetComponent",
                column: "InvStateId",
                principalTable: "InvState",
                principalColumn: "InvStateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetComponentOp_InvState_InvStateId",
                table: "AssetComponentOp",
                column: "InvStateId",
                principalTable: "InvState",
                principalColumn: "InvStateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetComponent_InvState_InvStateId",
                table: "AssetComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetComponentOp_InvState_InvStateId",
                table: "AssetComponentOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetComponentOp_InvStateId",
                table: "AssetComponentOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetComponent_InvStateId",
                table: "AssetComponent");

            migrationBuilder.DropColumn(
                name: "InvStateId",
                table: "AssetComponentOp");

            migrationBuilder.DropColumn(
                name: "InvStateId",
                table: "AssetComponent");
        }
    }
}
