using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddConfigValueRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "ConfigValue",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfigValue_RoleId",
                table: "ConfigValue",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigValue_AspNetRoles_RoleId",
                table: "ConfigValue",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigValue_AspNetRoles_RoleId",
                table: "ConfigValue");

            migrationBuilder.DropIndex(
                name: "IX_ConfigValue_RoleId",
                table: "ConfigValue");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "ConfigValue");

        }
    }
}
