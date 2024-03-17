using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetInvPlusSAPCreateAssetSAP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InvPlus",
                table: "CreateAssetSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CreateAssetSAPId",
                table: "AssetInvPlusSAP",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvPlusSAP_CreateAssetSAPId",
                table: "AssetInvPlusSAP",
                column: "CreateAssetSAPId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetInvPlusSAP_CreateAssetSAP_CreateAssetSAPId",
                table: "AssetInvPlusSAP",
                column: "CreateAssetSAPId",
                principalTable: "CreateAssetSAP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetInvPlusSAP_CreateAssetSAP_CreateAssetSAPId",
                table: "AssetInvPlusSAP");

            migrationBuilder.DropIndex(
                name: "IX_AssetInvPlusSAP_CreateAssetSAPId",
                table: "AssetInvPlusSAP");

            migrationBuilder.DropColumn(
                name: "InvPlus",
                table: "CreateAssetSAP");

            migrationBuilder.DropColumn(
                name: "CreateAssetSAPId",
                table: "AssetInvPlusSAP");
        }
    }
}
