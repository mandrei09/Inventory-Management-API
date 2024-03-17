using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestBFMaterialCostCenterMaxQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxQuantity",
                table: "RequestBFMaterialCostCenter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "RequestBFMaterialCostCenter",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValueRon",
                table: "RequestBFMaterialCostCenter",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxQuantity",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropColumn(
                name: "MaxValueRon",
                table: "RequestBFMaterialCostCenter");
        }
    }
}
