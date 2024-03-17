using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class _20240315 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InFarDec",
                table: "Asset",
                newName: "InFarLast");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.RenameColumn(
                name: "InFarLast",
                table: "Asset",
                newName: "InFarDec");
        }
    }
}
