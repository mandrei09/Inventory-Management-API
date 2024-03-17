using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAppState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "AppStateId",
                table: "EmailManager",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "EmailManager",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppStateId",
                table: "AssetComponent",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "AssetComponent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppStateId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_AppStateId",
                table: "EmailManager",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponent_AppStateId",
                table: "AssetComponent",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AppStateId",
                table: "Asset",
                column: "AppStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AppState_AppStateId",
                table: "Asset",
                column: "AppStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetComponent_AppState_AppStateId",
                table: "AssetComponent",
                column: "AppStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailManager_AppState_AppStateId",
                table: "EmailManager",
                column: "AppStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AppState_AppStateId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetComponent_AppState_AppStateId",
                table: "AssetComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailManager_AppState_AppStateId",
                table: "EmailManager");

            migrationBuilder.DropIndex(
                name: "IX_EmailManager_AppStateId",
                table: "EmailManager");

            migrationBuilder.DropIndex(
                name: "IX_AssetComponent_AppStateId",
                table: "AssetComponent");

            migrationBuilder.DropIndex(
                name: "IX_Asset_AppStateId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "AppStateId",
                table: "EmailManager");

            migrationBuilder.DropColumn(
                name: "Info",
                table: "EmailManager");

            migrationBuilder.DropColumn(
                name: "AppStateId",
                table: "AssetComponent");

            migrationBuilder.DropColumn(
                name: "Info",
                table: "AssetComponent");

            migrationBuilder.DropColumn(
                name: "AppStateId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Info",
                table: "Asset");
        }
    }
}
