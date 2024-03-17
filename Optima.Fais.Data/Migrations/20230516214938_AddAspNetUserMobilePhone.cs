using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAspNetUserMobilePhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MobilePhoneId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MobilePhoneId",
                table: "AspNetUsers",
                column: "MobilePhoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MobilePhone_MobilePhoneId",
                table: "AspNetUsers",
                column: "MobilePhoneId",
                principalTable: "MobilePhone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MobilePhone_MobilePhoneId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MobilePhoneId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MobilePhoneId",
                table: "AspNetUsers");
        }
    }
}
