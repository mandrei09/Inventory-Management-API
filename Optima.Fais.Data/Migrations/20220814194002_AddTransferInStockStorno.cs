using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddTransferInStockStorno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Header_Text",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ref_Doc_No",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Storno",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Storno_Date",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Storno_Doc",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Storno_User",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Storno_Year",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WBS_ELEMENT",
                table: "AssetChangeSAP",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Header_Text",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "Ref_Doc_No",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "Storno",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "Storno_Date",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "Storno_Doc",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "Storno_User",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "Storno_Year",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "WBS_ELEMENT",
                table: "AssetChangeSAP");
        }
    }
}
