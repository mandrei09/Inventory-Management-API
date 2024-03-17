using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestExecutionRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndExecution",
                table: "Request",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartExecution",
                table: "Request",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndExecution",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "StartExecution",
                table: "Request");
        }
    }
}
