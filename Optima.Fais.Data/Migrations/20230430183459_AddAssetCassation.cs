using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetCassation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cassation",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "CassationQuantity",
                table: "Asset",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "CassationValue",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CassationValueRon",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cassation",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "CassationQuantity",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "CassationValue",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "CassationValueRon",
                table: "Asset");
        }
    }
}
