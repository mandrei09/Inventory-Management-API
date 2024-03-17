using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMEmailOrderStatusSkip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmployeeL1EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeL2EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeL3EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeL4EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeS1EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeS2EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeS3EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeL1EmailSkip",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeL2EmailSkip",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeL3EmailSkip",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeL4EmailSkip",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeS1EmailSkip",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeS2EmailSkip",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeS3EmailSkip",
                table: "EmailOrderStatus");
        }
    }
}
