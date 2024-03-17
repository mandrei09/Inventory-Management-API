using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferRon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrdersPriceRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrdersValueRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PreAmountRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceIniRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsPriceRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsValueRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueIniRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRon",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PreAmountRon",
                table: "Offer",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceRon",
                table: "Offer",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueFinRon",
                table: "Offer",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueIniRon",
                table: "Offer",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsedRon",
                table: "Offer",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdersPriceRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "OrdersValueRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "PreAmountRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "PriceIniRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "PriceRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsPriceRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsValueRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "ValueIniRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "ValueRon",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "PreAmountRon",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "PriceRon",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ValueFinRon",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ValueIniRon",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ValueUsedRon",
                table: "Offer");
        }
    }
}
