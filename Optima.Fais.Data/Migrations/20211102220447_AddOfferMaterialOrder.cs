using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferMaterialOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrdersPrice",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrdersQuantity",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrdersValue",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceIni",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityIni",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueIni",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrdersPrice",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrdersQuantity",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrdersValue",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceIni",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityIni",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueIni",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdersPrice",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "OrdersQuantity",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "OrdersValue",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "PriceIni",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "QuantityIni",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ValueIni",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "OrdersPrice",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "OrdersQuantity",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "OrdersValue",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "PriceIni",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "QuantityIni",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "ValueIni",
                table: "OfferMaterial");
        }
    }
}
