using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEntityFileRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_RequestId",
                table: "EntityFile",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_Request_RequestId",
                table: "EntityFile",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_Request_RequestId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_RequestId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "EntityFile");
        }
    }
}
