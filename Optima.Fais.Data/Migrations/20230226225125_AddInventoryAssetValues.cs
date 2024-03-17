using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInventoryAssetValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AccumulDep",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrBkValue",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentAPC",
                table: "InventoryAsset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ResponsableWHTotals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Administration = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Division = table.Column<string>(nullable: true),
                    Initial = table.Column<int>(nullable: false),
                    InitialAccumulDep = table.Column<decimal>(nullable: false),
                    InitialCurrBkValue = table.Column<decimal>(nullable: false),
                    InitialCurrentAPC = table.Column<decimal>(nullable: false),
                    LastScan = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NotScanned = table.Column<int>(nullable: false),
                    NotScannedAccumulDep = table.Column<decimal>(nullable: false),
                    NotScannedCurrBkValue = table.Column<decimal>(nullable: false),
                    NotScannedCurrentAPC = table.Column<decimal>(nullable: false),
                    Procentage = table.Column<decimal>(nullable: false),
                    Room = table.Column<string>(nullable: true),
                    Scanned = table.Column<int>(nullable: false),
                    ScannedAccumulDep = table.Column<decimal>(nullable: false),
                    ScannedCurrBkValue = table.Column<decimal>(nullable: false),
                    ScannedCurrentAPC = table.Column<decimal>(nullable: false),
                    Temp = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsableWHTotals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamManagerWHTotals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Administration = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Division = table.Column<string>(nullable: true),
                    Initial = table.Column<int>(nullable: false),
                    InitialAccumulDep = table.Column<decimal>(nullable: false),
                    InitialCurrBkValue = table.Column<decimal>(nullable: false),
                    InitialCurrentAPC = table.Column<decimal>(nullable: false),
                    LastScan = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NotScanned = table.Column<int>(nullable: false),
                    NotScannedAccumulDep = table.Column<decimal>(nullable: false),
                    NotScannedCurrBkValue = table.Column<decimal>(nullable: false),
                    NotScannedCurrentAPC = table.Column<decimal>(nullable: false),
                    Procentage = table.Column<decimal>(nullable: false),
                    Room = table.Column<string>(nullable: true),
                    Scanned = table.Column<int>(nullable: false),
                    ScannedAccumulDep = table.Column<decimal>(nullable: false),
                    ScannedCurrBkValue = table.Column<decimal>(nullable: false),
                    ScannedCurrentAPC = table.Column<decimal>(nullable: false),
                    Temp = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamManagerWHTotals", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResponsableWHTotals");

            migrationBuilder.DropTable(
                name: "TeamManagerWHTotals");

            migrationBuilder.DropColumn(
                name: "AccumulDep",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "CurrBkValue",
                table: "InventoryAsset");

            migrationBuilder.DropColumn(
                name: "CurrentAPC",
                table: "InventoryAsset");
        }
    }
}
