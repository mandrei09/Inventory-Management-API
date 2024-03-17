using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAmortizationNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KANSW",
                table: "AssetDepMDSync",
                newName: "DEP_FY_START");

            migrationBuilder.RenameColumn(
                name: "DEPY",
                table: "AssetDepMDSync",
                newName: "DEP_FY");

            migrationBuilder.RenameColumn(
                name: "DEPFY",
                table: "AssetDepMDSync",
                newName: "APC_FY_START");

            migrationBuilder.RenameColumn(
                name: "ANSWL",
                table: "AssetDepMDSync",
                newName: "ACQUISITION");

            migrationBuilder.AddColumn<string>(
                name: "ANLN1",
                table: "AssetDepMDSync",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ANLN2",
                table: "AssetDepMDSync",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ANLN1",
                table: "AssetDepMDSync");

            migrationBuilder.DropColumn(
                name: "ANLN2",
                table: "AssetDepMDSync");

            migrationBuilder.RenameColumn(
                name: "DEP_FY_START",
                table: "AssetDepMDSync",
                newName: "KANSW");

            migrationBuilder.RenameColumn(
                name: "DEP_FY",
                table: "AssetDepMDSync",
                newName: "DEPY");

            migrationBuilder.RenameColumn(
                name: "APC_FY_START",
                table: "AssetDepMDSync",
                newName: "DEPFY");

            migrationBuilder.RenameColumn(
                name: "ACQUISITION",
                table: "AssetDepMDSync",
                newName: "ANSWL");
        }
    }
}
