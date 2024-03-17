using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCostCenterFinished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowLabelList",
                table: "CostCenter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BookAfter",
                table: "CostCenter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BookBefore",
                table: "CostCenter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinished",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InventoryList",
                table: "CostCenter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "CostCenter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PvBook",
                table: "CostCenter",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AllowLabelList",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "BookAfter",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "BookBefore",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "DateFinished",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "InventoryList",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "PvBook",
                table: "CostCenter");
        }
    }
}
