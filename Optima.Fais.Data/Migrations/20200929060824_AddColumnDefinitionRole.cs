using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddColumnDefinitionRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "ColumnDefinition",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColumnDefinition_RoleId",
                table: "ColumnDefinition",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColumnDefinition_AspNetRoles_RoleId",
                table: "ColumnDefinition",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColumnDefinition_AspNetRoles_RoleId",
                table: "ColumnDefinition");

            migrationBuilder.DropIndex(
                name: "IX_ColumnDefinition_RoleId",
                table: "ColumnDefinition");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "ColumnDefinition");
        }
    }
}
