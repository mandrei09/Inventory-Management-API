using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddPARTNERINTERCOMPANY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterCompany_Partner_PartnerId",
                table: "InterCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_Partner_Uom_UomId",
                table: "Partner");

            migrationBuilder.DropIndex(
                name: "IX_InterCompany_PartnerId",
                table: "InterCompany");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "InterCompany");

            migrationBuilder.RenameColumn(
                name: "UomId",
                table: "Partner",
                newName: "InterCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Partner_UomId",
                table: "Partner",
                newName: "IX_Partner_InterCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_InterCompany_InterCompanyId",
                table: "Partner",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partner_InterCompany_InterCompanyId",
                table: "Partner");

            migrationBuilder.RenameColumn(
                name: "InterCompanyId",
                table: "Partner",
                newName: "UomId");

            migrationBuilder.RenameIndex(
                name: "IX_Partner_InterCompanyId",
                table: "Partner",
                newName: "IX_Partner_UomId");

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "InterCompany",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_PartnerId",
                table: "InterCompany",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterCompany_Partner_PartnerId",
                table: "InterCompany",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_Uom_UomId",
                table: "Partner",
                column: "UomId",
                principalTable: "Uom",
                principalColumn: "UomId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
