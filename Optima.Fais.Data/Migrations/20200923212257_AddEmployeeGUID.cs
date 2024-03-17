﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmployeeGUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Employee",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Employee");

        }
    }
}