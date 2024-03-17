using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddSAPCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "SAPCode",
                table: "Asset",
                nullable: true);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "SAPCode",
                table: "Asset");

          
        }
    }
}
