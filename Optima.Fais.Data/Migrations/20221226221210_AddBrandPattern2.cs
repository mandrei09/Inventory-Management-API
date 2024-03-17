using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBrandPattern2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber1Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber2Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber3Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber4Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber5Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imei",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Asset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber1Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "PhoneNumber2Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "PhoneNumber3Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "PhoneNumber4Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "PhoneNumber5Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Imei",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Asset");
        }
    }
}
