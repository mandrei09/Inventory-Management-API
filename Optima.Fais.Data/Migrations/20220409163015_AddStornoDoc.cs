using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddStornoDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "STORNO",
                table: "RetireAssetSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STORNO_DATE",
                table: "RetireAssetSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STORNO_DOC",
                table: "RetireAssetSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STORNO_REASON",
                table: "RetireAssetSAP",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BELNR",
                table: "Error",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BUKRS",
                table: "Error",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GJAHR",
                table: "Error",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STORNO",
                table: "RetireAssetSAP");

            migrationBuilder.DropColumn(
                name: "STORNO_DATE",
                table: "RetireAssetSAP");

            migrationBuilder.DropColumn(
                name: "STORNO_DOC",
                table: "RetireAssetSAP");

            migrationBuilder.DropColumn(
                name: "STORNO_REASON",
                table: "RetireAssetSAP");

            migrationBuilder.DropColumn(
                name: "BELNR",
                table: "Error");

            migrationBuilder.DropColumn(
                name: "BUKRS",
                table: "Error");

            migrationBuilder.DropColumn(
                name: "GJAHR",
                table: "Error");
        }
    }
}
