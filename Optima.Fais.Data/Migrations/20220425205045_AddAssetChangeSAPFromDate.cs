using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetChangeSAPFromDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FROM_DATE",
                table: "AssetChangeSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RESP_CCTR",
                table: "AssetChangeSAP",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FROM_DATE",
                table: "AssetChangeSAP");

            migrationBuilder.DropColumn(
                name: "RESP_CCTR",
                table: "AssetChangeSAP");
        }
    }
}
