using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddExpAccountRequireSN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FromClone",
                table: "TransferAssetSAP",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExpAccount",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireSN",
                table: "ExpAccount",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromClone",
                table: "TransferAssetSAP");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExpAccount");

            migrationBuilder.DropColumn(
                name: "RequireSN",
                table: "ExpAccount");
        }
    }
}
