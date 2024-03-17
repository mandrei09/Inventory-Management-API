using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddContractAmountRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLast",
                table: "Rate",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Multiplier",
                table: "Rate",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountRon",
                table: "ContractAmount",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountRonRem",
                table: "ContractAmount",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountRonUsed",
                table: "ContractAmount",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountUsed",
                table: "ContractAmount",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "ContractAmount",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RateRonId",
                table: "ContractAmount",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractAmount_RateId",
                table: "ContractAmount",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAmount_RateRonId",
                table: "ContractAmount",
                column: "RateRonId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ContractId",
                table: "Asset",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Contract_ContractId",
                table: "Asset",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractAmount_Rate_RateId",
                table: "ContractAmount",
                column: "RateId",
                principalTable: "Rate",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractAmount_Rate_RateRonId",
                table: "ContractAmount",
                column: "RateRonId",
                principalTable: "Rate",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Contract_ContractId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractAmount_Rate_RateId",
                table: "ContractAmount");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractAmount_Rate_RateRonId",
                table: "ContractAmount");

            migrationBuilder.DropIndex(
                name: "IX_ContractAmount_RateId",
                table: "ContractAmount");

            migrationBuilder.DropIndex(
                name: "IX_ContractAmount_RateRonId",
                table: "ContractAmount");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ContractId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "IsLast",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "Multiplier",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "AmountRon",
                table: "ContractAmount");

            migrationBuilder.DropColumn(
                name: "AmountRonRem",
                table: "ContractAmount");

            migrationBuilder.DropColumn(
                name: "AmountRonUsed",
                table: "ContractAmount");

            migrationBuilder.DropColumn(
                name: "AmountUsed",
                table: "ContractAmount");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "ContractAmount");

            migrationBuilder.DropColumn(
                name: "RateRonId",
                table: "ContractAmount");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Asset");
        }
    }
}
