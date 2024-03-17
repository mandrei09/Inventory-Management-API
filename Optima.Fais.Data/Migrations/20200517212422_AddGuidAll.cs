using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddGuidAll : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailSend",
                table: "Employee",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "EmailType",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GuidAll",
                table: "EmailManager",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailSend",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "EmailType");

            migrationBuilder.DropColumn(
                name: "GuidAll",
                table: "EmailManager");
        }
    }
}
