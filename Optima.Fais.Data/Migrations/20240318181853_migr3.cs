using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class migr3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InFarLast",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "InFarSept",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "InitialFacility",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "InitialFacilityAccumulDep",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "InitialFacilityCurrBkValue",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "InitialFacilityCurrentAPC",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "InitialIT",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "InitialITAccumulDep",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "InitialITCurrBkValue",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "InitialITCurrentAPC",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedFacility",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedFacilityAccumulDep",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedFacilityCurrBkValue",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedFacilityCurrentAPC",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedIT",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedITAccumulDep",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedITCurrBkValue",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "NotScannedITCurrentAPC",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedFacility",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedFacilityAccumulDep",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedFacilityCurrBkValue",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedFacilityCurrentAPC",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedIT",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedITAccumulDep",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedITCurrBkValue",
                table: "AdministrationTotals");

            migrationBuilder.DropColumn(
                name: "ScannedITCurrentAPC",
                table: "AdministrationTotals");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InFarLast",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InFarSept",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InitialFacility",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialFacilityAccumulDep",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialFacilityCurrBkValue",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialFacilityCurrentAPC",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "InitialIT",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialITAccumulDep",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialITCurrBkValue",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialITCurrentAPC",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NotScannedFacility",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "NotScannedFacilityAccumulDep",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NotScannedFacilityCurrBkValue",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NotScannedFacilityCurrentAPC",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NotScannedIT",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "NotScannedITAccumulDep",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NotScannedITCurrBkValue",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NotScannedITCurrentAPC",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ScannedFacility",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ScannedFacilityAccumulDep",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ScannedFacilityCurrBkValue",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ScannedFacilityCurrentAPC",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ScannedIT",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ScannedITAccumulDep",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ScannedITCurrBkValue",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ScannedITCurrentAPC",
                table: "AdministrationTotals",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
