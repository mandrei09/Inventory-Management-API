using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetChangeSAPInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DOC_YEAR",
                table: "AssetChangeSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "INVOICE",
                table: "AssetChangeSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MAT_DOC",
                table: "AssetChangeSAP",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DOC_YEAR",
                table: "AssetChangeSAP");

            migrationBuilder.DropColumn(
                name: "INVOICE",
                table: "AssetChangeSAP");

            migrationBuilder.DropColumn(
                name: "MAT_DOC",
                table: "AssetChangeSAP");
        }
    }
}
