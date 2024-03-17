using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBrandPattern : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imei1Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imei2Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imei3Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imei4Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imei5Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Serial1Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Serial2Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Serial3Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Serial4Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Serial5Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag1Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag2Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag3Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag4Pattern",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag5Pattern",
                table: "Brand",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imei1Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Imei2Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Imei3Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Imei4Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Imei5Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Serial1Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Serial2Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Serial3Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Serial4Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Serial5Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Tag1Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Tag2Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Tag3Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Tag4Pattern",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Tag5Pattern",
                table: "Brand");
        }
    }
}
