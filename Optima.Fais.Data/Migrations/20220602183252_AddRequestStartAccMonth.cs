using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestStartAccMonth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StartAccMonthId",
                table: "Request",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_StartAccMonthId",
                table: "Request",
                column: "StartAccMonthId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AccMonth_StartAccMonthId",
                table: "Request",
                column: "StartAccMonthId",
                principalTable: "AccMonth",
                principalColumn: "AccMonthId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AccMonth_StartAccMonthId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_StartAccMonthId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "StartAccMonthId",
                table: "Request");
        }
    }
}
