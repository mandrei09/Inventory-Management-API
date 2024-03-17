using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class RenameAssetDepMDColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValueTrIn",
                table: "AssetDepMD",
                newName: "Transfer");

            migrationBuilder.RenameColumn(
                name: "ValueTr",
                table: "AssetDepMD",
                newName: "Retirement");

            migrationBuilder.RenameColumn(
                name: "ValueRetIn",
                table: "AssetDepMD",
                newName: "DepTransfer");

            migrationBuilder.RenameColumn(
                name: "ValueRet",
                table: "AssetDepMD",
                newName: "DepRetirement");

            migrationBuilder.RenameColumn(
                name: "ValueRemIn",
                table: "AssetDepMD",
                newName: "DepPostCap");

            migrationBuilder.RenameColumn(
                name: "ValueRem",
                table: "AssetDepMD",
                newName: "PosCap");

            migrationBuilder.RenameColumn(
                name: "ValueInvIn",
                table: "AssetDepMD",
                newName: "DepFYStart");

            migrationBuilder.RenameColumn(
                name: "ValueInv",
                table: "AssetDepMD",
                newName: "DepForYear");

            migrationBuilder.RenameColumn(
                name: "ValueDepYTDIn",
                table: "AssetDepMD",
                newName: "CurrBkValue");

            migrationBuilder.RenameColumn(
                name: "ValueDepYTD",
                table: "AssetDepMD",
                newName: "CurrentAPC");

            migrationBuilder.RenameColumn(
                name: "ValueDepPUIn",
                table: "AssetDepMD",
                newName: "Acquisition");

            migrationBuilder.RenameColumn(
                name: "ValueDepPU",
                table: "AssetDepMD",
                newName: "BkValFYStart");

            migrationBuilder.RenameColumn(
                name: "ValueDepIn",
                table: "AssetDepMD",
                newName: "APCFYStart");

            migrationBuilder.RenameColumn(
                name: "ValueDep",
                table: "AssetDepMD",
                newName: "AccumulDep");

            migrationBuilder.RenameColumn(
                name: "DepPeriodRemIn",
                table: "AssetDepMD",
                newName: "UsefulLife");

            migrationBuilder.RenameColumn(
                name: "DepPeriodRem",
                table: "AssetDepMD",
                newName: "TotLifeInpPeriods");

            migrationBuilder.RenameColumn(
                name: "DepPeriodMonth",
                table: "AssetDepMD",
                newName: "RemLifeInPeriods");

            migrationBuilder.RenameColumn(
                name: "DepPeriod",
                table: "AssetDepMD",
                newName: "ExpLifeInPeriods");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsefulLife",
                table: "AssetDepMD",
                newName: "DepPeriodRemIn");

            migrationBuilder.RenameColumn(
                name: "Transfer",
                table: "AssetDepMD",
                newName: "ValueTrIn");

            migrationBuilder.RenameColumn(
                name: "TotLifeInpPeriods",
                table: "AssetDepMD",
                newName: "DepPeriodRem");

            migrationBuilder.RenameColumn(
                name: "Retirement",
                table: "AssetDepMD",
                newName: "ValueTr");

            migrationBuilder.RenameColumn(
                name: "RemLifeInPeriods",
                table: "AssetDepMD",
                newName: "DepPeriodMonth");

            migrationBuilder.RenameColumn(
                name: "PosCap",
                table: "AssetDepMD",
                newName: "ValueRem");

            migrationBuilder.RenameColumn(
                name: "ExpLifeInPeriods",
                table: "AssetDepMD",
                newName: "DepPeriod");

            migrationBuilder.RenameColumn(
                name: "DepTransfer",
                table: "AssetDepMD",
                newName: "ValueRetIn");

            migrationBuilder.RenameColumn(
                name: "DepRetirement",
                table: "AssetDepMD",
                newName: "ValueRet");

            migrationBuilder.RenameColumn(
                name: "DepPostCap",
                table: "AssetDepMD",
                newName: "ValueRemIn");

            migrationBuilder.RenameColumn(
                name: "DepForYear",
                table: "AssetDepMD",
                newName: "ValueInv");

            migrationBuilder.RenameColumn(
                name: "DepFYStart",
                table: "AssetDepMD",
                newName: "ValueInvIn");

            migrationBuilder.RenameColumn(
                name: "CurrentAPC",
                table: "AssetDepMD",
                newName: "ValueDepYTD");

            migrationBuilder.RenameColumn(
                name: "CurrBkValue",
                table: "AssetDepMD",
                newName: "ValueDepYTDIn");

            migrationBuilder.RenameColumn(
                name: "BkValFYStart",
                table: "AssetDepMD",
                newName: "ValueDepPU");

            migrationBuilder.RenameColumn(
                name: "Acquisition",
                table: "AssetDepMD",
                newName: "ValueDepPUIn");

            migrationBuilder.RenameColumn(
                name: "AccumulDep",
                table: "AssetDepMD",
                newName: "ValueDep");

            migrationBuilder.RenameColumn(
                name: "APCFYStart",
                table: "AssetDepMD",
                newName: "ValueDepIn");
        }
    }
}
