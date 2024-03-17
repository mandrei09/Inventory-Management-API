using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetDatesColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PODate",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceptionDate",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovalDate",
                table: "Asset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "PODate",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ReceptionDate",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "RemovalDate",
                table: "Asset");
        }
    }
}
