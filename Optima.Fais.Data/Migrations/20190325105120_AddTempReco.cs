using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddTempReco : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            //migrationBuilder.AddColumn<string>(
            //    name: "TempName",
            //    table: "Asset",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TempReco",
            //    table: "Asset",
            //    nullable: true);

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "TempName",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TempReco",
                table: "Asset");
        }
    }
}
