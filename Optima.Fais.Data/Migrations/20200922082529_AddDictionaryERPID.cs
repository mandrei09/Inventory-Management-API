using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddDictionaryERPID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "DictionaryType",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ERPId",
                table: "DictionaryType",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "DictionaryItem",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ERPId",
                table: "DictionaryItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name2",
                table: "DictionaryItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name3",
                table: "DictionaryItem",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryType_CompanyId",
                table: "DictionaryType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryItem_CompanyId",
                table: "DictionaryItem",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryItem_Company_CompanyId",
                table: "DictionaryItem",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryType_Company_CompanyId",
                table: "DictionaryType",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryItem_Company_CompanyId",
                table: "DictionaryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryType_Company_CompanyId",
                table: "DictionaryType");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryType_CompanyId",
                table: "DictionaryType");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryItem_CompanyId",
                table: "DictionaryItem");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "DictionaryType");

            migrationBuilder.DropColumn(
                name: "ERPId",
                table: "DictionaryType");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "DictionaryItem");

            migrationBuilder.DropColumn(
                name: "ERPId",
                table: "DictionaryItem");

            migrationBuilder.DropColumn(
                name: "Name2",
                table: "DictionaryItem");

            migrationBuilder.DropColumn(
                name: "Name3",
                table: "DictionaryItem");
        }
    }
}
