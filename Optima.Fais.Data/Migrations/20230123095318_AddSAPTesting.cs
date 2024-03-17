using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddSAPTesting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "TransferInStockSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "TransferAssetSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "RetireAssetSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "CreateAssetSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "AssetInvPlusSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "AssetInvMinusSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "AssetChangeSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTesting",
                table: "AcquisitionAssetSAP",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "TransferAssetSAP");

            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "RetireAssetSAP");

            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "CreateAssetSAP");

            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "AssetInvPlusSAP");

            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "AssetInvMinusSAP");

            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "AssetChangeSAP");

            migrationBuilder.DropColumn(
                name: "IsTesting",
                table: "AcquisitionAssetSAP");
        }
    }
}
