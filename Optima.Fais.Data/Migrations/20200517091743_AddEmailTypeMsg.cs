using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmailTypeMsg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomMsg",
                table: "EmailType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FooterMsg",
                table: "EmailType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderMsg",
                table: "EmailType",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomMsg",
                table: "EmailType");

            migrationBuilder.DropColumn(
                name: "FooterMsg",
                table: "EmailType");

            migrationBuilder.DropColumn(
                name: "HeaderMsg",
                table: "EmailType");
        }
    }
}
