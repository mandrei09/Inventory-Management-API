using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferOrderType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderTypeId",
                table: "OrderMaterial",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WIP",
                table: "OrderMaterial",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WIP",
                table: "Order",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderTypeId",
                table: "OfferMaterial",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WIP",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderTypeId",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WIP",
                table: "Offer",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_OrderTypeId",
                table: "OrderMaterial",
                column: "OrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferMaterial_OrderTypeId",
                table: "OfferMaterial",
                column: "OrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_OrderTypeId",
                table: "Offer",
                column: "OrderTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_OrderType_OrderTypeId",
                table: "Offer",
                column: "OrderTypeId",
                principalTable: "OrderType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferMaterial_OrderType_OrderTypeId",
                table: "OfferMaterial",
                column: "OrderTypeId",
                principalTable: "OrderType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderMaterial_OrderType_OrderTypeId",
                table: "OrderMaterial",
                column: "OrderTypeId",
                principalTable: "OrderType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_OrderType_OrderTypeId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferMaterial_OrderType_OrderTypeId",
                table: "OfferMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderMaterial_OrderType_OrderTypeId",
                table: "OrderMaterial");

            migrationBuilder.DropIndex(
                name: "IX_OrderMaterial_OrderTypeId",
                table: "OrderMaterial");

            migrationBuilder.DropIndex(
                name: "IX_OfferMaterial_OrderTypeId",
                table: "OfferMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Offer_OrderTypeId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "OrderTypeId",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "WIP",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "WIP",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderTypeId",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "WIP",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "OrderTypeId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "WIP",
                table: "Offer");
        }
    }
}
