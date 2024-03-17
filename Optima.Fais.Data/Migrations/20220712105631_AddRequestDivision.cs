using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestDivision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Request",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_DivisionId",
                table: "Request",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DivisionId",
                table: "Order",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_DivisionId",
                table: "Offer",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Division_DivisionId",
                table: "Offer",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Division_DivisionId",
                table: "Order",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Division_DivisionId",
                table: "Request",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Division_DivisionId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Division_DivisionId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Division_DivisionId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_DivisionId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Order_DivisionId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Offer_DivisionId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Offer");
        }
    }
}
