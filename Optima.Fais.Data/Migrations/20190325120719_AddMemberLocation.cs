using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMemberLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Member1",
                table: "Location",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Member2",
                table: "Location",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Member3",
                table: "Location",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Member1",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Member2",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Member3",
                table: "Location");
        }
    }
}
