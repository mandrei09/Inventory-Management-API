using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMissingColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageCount",
                table: "Location",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsuranceCategoryId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowLabel",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_RoomId",
                table: "EntityFile",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_CompanyId",
                table: "AssetOp",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_InsuranceCategoryId",
                table: "AssetOp",
                column: "InsuranceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_InterCompanyId",
                table: "AssetOp",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_UomId",
                table: "AssetOp",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Company_CompanyId",
                table: "AssetOp",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_InsuranceCategory_InsuranceCategoryId",
                table: "AssetOp",
                column: "InsuranceCategoryId",
                principalTable: "InsuranceCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_InterCompany_InterCompanyId",
                table: "AssetOp",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Uom_UomId",
                table: "AssetOp",
                column: "UomId",
                principalTable: "Uom",
                principalColumn: "UomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_Room_RoomId",
                table: "EntityFile",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_Company_CompanyId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_InsuranceCategory_InsuranceCategoryId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_InterCompany_InterCompanyId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_Uom_UomId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_Room_RoomId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_RoomId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_CompanyId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_InsuranceCategoryId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_InterCompanyId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_UomId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "ImageCount",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "InsuranceCategoryId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "AllowLabel",
                table: "Asset");
        }
    }
}
