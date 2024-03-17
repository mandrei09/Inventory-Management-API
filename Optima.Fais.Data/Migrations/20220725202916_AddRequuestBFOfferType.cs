using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequuestBFOfferType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfferTypeId",
                table: "RequestBudgetForecastMaterial",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferTypeId",
                table: "RequestBudgetForecast",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferTypeId",
                table: "RequestBFMaterialCostCenter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecastMaterial_OfferTypeId",
                table: "RequestBudgetForecastMaterial",
                column: "OfferTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecast_OfferTypeId",
                table: "RequestBudgetForecast",
                column: "OfferTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_OfferTypeId",
                table: "RequestBFMaterialCostCenter",
                column: "OfferTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBFMaterialCostCenter_OfferType_OfferTypeId",
                table: "RequestBFMaterialCostCenter",
                column: "OfferTypeId",
                principalTable: "OfferType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBudgetForecast_OfferType_OfferTypeId",
                table: "RequestBudgetForecast",
                column: "OfferTypeId",
                principalTable: "OfferType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestBudgetForecastMaterial_OfferType_OfferTypeId",
                table: "RequestBudgetForecastMaterial",
                column: "OfferTypeId",
                principalTable: "OfferType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestBFMaterialCostCenter_OfferType_OfferTypeId",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestBudgetForecast_OfferType_OfferTypeId",
                table: "RequestBudgetForecast");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestBudgetForecastMaterial_OfferType_OfferTypeId",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropIndex(
                name: "IX_RequestBudgetForecastMaterial_OfferTypeId",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropIndex(
                name: "IX_RequestBudgetForecast_OfferTypeId",
                table: "RequestBudgetForecast");

            migrationBuilder.DropIndex(
                name: "IX_RequestBFMaterialCostCenter_OfferTypeId",
                table: "RequestBFMaterialCostCenter");

            migrationBuilder.DropColumn(
                name: "OfferTypeId",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "OfferTypeId",
                table: "RequestBudgetForecast");

            migrationBuilder.DropColumn(
                name: "OfferTypeId",
                table: "RequestBFMaterialCostCenter");
        }
    }
}
