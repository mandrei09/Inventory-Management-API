using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBadgeColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BadgeColor",
                table: "InvState",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeIcon",
                table: "InvState",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeColor",
                table: "AssetState",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeIcon",
                table: "AssetState",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeColor",
                table: "AppState",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeIcon",
                table: "AppState",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadgeColor",
                table: "InvState");

            migrationBuilder.DropColumn(
                name: "BadgeIcon",
                table: "InvState");

            migrationBuilder.DropColumn(
                name: "BadgeColor",
                table: "AssetState");

            migrationBuilder.DropColumn(
                name: "BadgeIcon",
                table: "AssetState");

            migrationBuilder.DropColumn(
                name: "BadgeColor",
                table: "AppState");

            migrationBuilder.DropColumn(
                name: "BadgeIcon",
                table: "AppState");
        }
    }
}
