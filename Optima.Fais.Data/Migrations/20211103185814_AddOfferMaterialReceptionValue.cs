using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferMaterialReceptionValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsPrice",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsQuantity",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsValue",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsPrice",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsQuantity",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsValue",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceptionsPrice",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsQuantity",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsValue",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsPrice",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsQuantity",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsValue",
                table: "OfferMaterial");
        }
    }
}
