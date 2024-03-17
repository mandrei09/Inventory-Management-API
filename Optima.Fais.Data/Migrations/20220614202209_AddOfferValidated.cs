using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferValidated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Validated",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Validated",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "Validated",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "Validated",
                table: "OfferMaterial");
        }
    }
}
