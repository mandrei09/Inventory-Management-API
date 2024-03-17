using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOrderRon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OrdersPriceRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrdersValueRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PreAmountRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceIniRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsPriceRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionsValueRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueIniRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueRon",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PreAmountRon",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceRon",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueFinRon",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueIniRon",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsedRon",
                table: "Order",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdersPriceRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "OrdersValueRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "PreAmountRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "PriceIniRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "PriceRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsPriceRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ReceptionsValueRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ValueIniRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ValueRon",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "PreAmountRon",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PriceRon",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ValueFinRon",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ValueIniRon",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ValueUsedRon",
                table: "Order");
        }
    }
}
