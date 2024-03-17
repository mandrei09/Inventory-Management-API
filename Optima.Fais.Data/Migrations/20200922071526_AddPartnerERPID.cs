using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddPartnerERPID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ERPId",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ERPId",
                table: "Partner",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ERPId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "ERPId",
                table: "Partner");
        }
    }
}
