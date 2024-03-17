using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMaterialEAN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EAN",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Material",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "Material",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Material",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "EAN",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Material");
        }
    }
}
