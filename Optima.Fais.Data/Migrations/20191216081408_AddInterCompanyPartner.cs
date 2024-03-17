using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInterCompanyPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterCompany_Partner_PartnerId",
                table: "InterCompany");

            migrationBuilder.DropIndex(
                name: "IX_InterCompany_PartnerId",
                table: "InterCompany");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "InterCompany");
        }
    }
}
