using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetDepMDTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepPeriodRemIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDepIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDepPUIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDepYTDIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueInvIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRemIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRet",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRetIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueTr",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueTrIn",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRet",
                table: "AssetDep",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRetIn",
                table: "AssetDep",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueTr",
                table: "AssetDep",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueTrIn",
                table: "AssetDep",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepPeriodRemIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueDepIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueDepPUIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueDepYTDIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueInvIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueRemIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueRet",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueRetIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueTr",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueTrIn",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "ValueRet",
                table: "AssetDep");

            migrationBuilder.DropColumn(
                name: "ValueRetIn",
                table: "AssetDep");

            migrationBuilder.DropColumn(
                name: "ValueTr",
                table: "AssetDep");

            migrationBuilder.DropColumn(
                name: "ValueTrIn",
                table: "AssetDep");
        }
    }
}
