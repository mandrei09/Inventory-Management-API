using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOrderProce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Order",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "QuantityRem",
                table: "Order",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Offer",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Offer",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "QuantityRem",
                table: "Offer",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Budget",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Budget",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "QuantityRem",
                table: "Budget",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "QuantityRem",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "QuantityRem",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "QuantityRem",
                table: "Budget");
        }
    }
}
