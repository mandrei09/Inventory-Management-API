using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddQuantityUsed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "QuantityUsed",
                table: "Order",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsed",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "QuantityOrder",
                table: "Budget",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "QuantityUsed",
                table: "Budget",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueOrder",
                table: "Budget",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueUsed",
                table: "Budget",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityUsed",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ValueUsed",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "QuantityOrder",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "QuantityUsed",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "ValueOrder",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "ValueUsed",
                table: "Budget");
        }
    }
}
