using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddTransferInStockSAPCreateAssetSAP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreateAssetSAPId",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferInStockSAP_CreateAssetSAPId",
                table: "TransferInStockSAP",
                column: "CreateAssetSAPId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferInStockSAP_CreateAssetSAP_CreateAssetSAPId",
                table: "TransferInStockSAP",
                column: "CreateAssetSAPId",
                principalTable: "CreateAssetSAP",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferInStockSAP_CreateAssetSAP_CreateAssetSAPId",
                table: "TransferInStockSAP");

            migrationBuilder.DropIndex(
                name: "IX_TransferInStockSAP_CreateAssetSAPId",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "CreateAssetSAPId",
                table: "TransferInStockSAP");
        }
    }
}
