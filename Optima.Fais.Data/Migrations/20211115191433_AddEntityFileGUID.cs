using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEntityFileGUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "EntityFile",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuidAll",
                table: "EntityFile",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailSend",
                table: "EntityFile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_CostCenterId",
                table: "EntityFile",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_PartnerId",
                table: "EntityFile",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_CostCenter_CostCenterId",
                table: "EntityFile",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_Partner_PartnerId",
                table: "EntityFile",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_CostCenter_CostCenterId",
                table: "EntityFile");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_Partner_PartnerId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_CostCenterId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_PartnerId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "GuidAll",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "IsEmailSend",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "EntityFile");
        }
    }
}
