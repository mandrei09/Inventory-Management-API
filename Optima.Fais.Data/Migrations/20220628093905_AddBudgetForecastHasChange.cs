using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetForecastHasChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasChangeApril",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeAugust",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeDecember",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeFebruary",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeJanuary",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeJuly",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeJune",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeMarch",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeMay",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeNovember",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeOctomber",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChangeSeptember",
                table: "BudgetForecast",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasChangeApril",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeAugust",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeDecember",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeFebruary",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeJanuary",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeJuly",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeJune",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeMarch",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeMay",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeNovember",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeOctomber",
                table: "BudgetForecast");

            migrationBuilder.DropColumn(
                name: "HasChangeSeptember",
                table: "BudgetForecast");
        }
    }
}
