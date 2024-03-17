using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "Request",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Request",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_AssetTypeId",
                table: "Request",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProjectTypeId",
                table: "Request",
                column: "ProjectTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AssetType_AssetTypeId",
                table: "Request",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProjectType_ProjectTypeId",
                table: "Request",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AssetType_AssetTypeId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProjectType_ProjectTypeId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_AssetTypeId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_ProjectTypeId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Request");
        }
    }
}
