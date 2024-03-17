using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddPermissionRoleType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PermissionTypeId",
                table: "PermissionRole",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRole_PermissionTypeId",
                table: "PermissionRole",
                column: "PermissionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_PermissionType_PermissionTypeId",
                table: "PermissionRole",
                column: "PermissionTypeId",
                principalTable: "PermissionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_PermissionType_PermissionTypeId",
                table: "PermissionRole");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRole_PermissionTypeId",
                table: "PermissionRole");

            migrationBuilder.DropColumn(
                name: "PermissionTypeId",
                table: "PermissionRole");
        }
    }
}
