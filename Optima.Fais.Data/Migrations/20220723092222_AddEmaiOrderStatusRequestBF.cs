using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmaiOrderStatusRequestBF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestBudgetForecastId",
                table: "EntityFile",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "EmailOrderStatus",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NeedBudgetEmailSend",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NeedContractEmailSend",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotNeedBudgetSync",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotNeedContractSync",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RequestBudgetForecastId",
                table: "EmailOrderStatus",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SyncNeedBudgetErrorCount",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncNeedContractErrorCount",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_ContractId",
                table: "EntityFile",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_RequestBudgetForecastId",
                table: "EntityFile",
                column: "RequestBudgetForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOrderStatus_ContractId",
                table: "EmailOrderStatus",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOrderStatus_RequestBudgetForecastId",
                table: "EmailOrderStatus",
                column: "RequestBudgetForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailOrderStatus_Contract_ContractId",
                table: "EmailOrderStatus",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailOrderStatus_RequestBudgetForecast_RequestBudgetForecastId",
                table: "EmailOrderStatus",
                column: "RequestBudgetForecastId",
                principalTable: "RequestBudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_Contract_ContractId",
                table: "EntityFile",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityFile_RequestBudgetForecast_RequestBudgetForecastId",
                table: "EntityFile",
                column: "RequestBudgetForecastId",
                principalTable: "RequestBudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailOrderStatus_Contract_ContractId",
                table: "EmailOrderStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailOrderStatus_RequestBudgetForecast_RequestBudgetForecastId",
                table: "EmailOrderStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_Contract_ContractId",
                table: "EntityFile");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityFile_RequestBudgetForecast_RequestBudgetForecastId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_ContractId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EntityFile_RequestBudgetForecastId",
                table: "EntityFile");

            migrationBuilder.DropIndex(
                name: "IX_EmailOrderStatus_ContractId",
                table: "EmailOrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_EmailOrderStatus_RequestBudgetForecastId",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "RequestBudgetForecastId",
                table: "EntityFile");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "NeedBudgetEmailSend",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "NeedContractEmailSend",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "NotNeedBudgetSync",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "NotNeedContractSync",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "RequestBudgetForecastId",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "SyncNeedBudgetErrorCount",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "SyncNeedContractErrorCount",
                table: "EmailOrderStatus");
        }
    }
}
