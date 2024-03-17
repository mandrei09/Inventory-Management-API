using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetIsCLone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BudgetValueNeed",
                table: "Request",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsClone",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSAPClone",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "TOTAL_AMOUNT",
                table: "AcquisitionAssetSAP",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "TAX_AMOUNT",
                table: "AcquisitionAssetSAP",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "NET_AMOUNT",
                table: "AcquisitionAssetSAP",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "EXCH_RATE",
                table: "AcquisitionAssetSAP",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetValueNeed",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "IsClone",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsSAPClone",
                table: "Asset");

            migrationBuilder.AlterColumn<decimal>(
                name: "TOTAL_AMOUNT",
                table: "AcquisitionAssetSAP",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TAX_AMOUNT",
                table: "AcquisitionAssetSAP",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NET_AMOUNT",
                table: "AcquisitionAssetSAP",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EXCH_RATE",
                table: "AcquisitionAssetSAP",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");
        }
    }
}
