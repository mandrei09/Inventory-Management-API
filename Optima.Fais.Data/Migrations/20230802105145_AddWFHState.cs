using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddWFHState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWFH",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WFHStateId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asset_WFHStateId",
                table: "Asset",
                column: "WFHStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AppState_WFHStateId",
                table: "Asset",
                column: "WFHStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AppState_WFHStateId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_WFHStateId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsWFH",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "WFHStateId",
                table: "Asset");
        }
    }
}
