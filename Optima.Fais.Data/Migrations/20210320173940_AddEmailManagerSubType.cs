using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmailManagerSubType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "EmailManager",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferId",
                table: "EmailManager",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "EmailManager",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "EmailManager",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubTypeId",
                table: "EmailManager",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_BudgetId",
                table: "EmailManager",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_OfferId",
                table: "EmailManager",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_OrderId",
                table: "EmailManager",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_PartnerId",
                table: "EmailManager",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_SubTypeId",
                table: "EmailManager",
                column: "SubTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailManager_Budget_BudgetId",
                table: "EmailManager",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailManager_Offer_OfferId",
                table: "EmailManager",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailManager_Order_OrderId",
                table: "EmailManager",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailManager_Partner_PartnerId",
                table: "EmailManager",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailManager_SubType_SubTypeId",
                table: "EmailManager",
                column: "SubTypeId",
                principalTable: "SubType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailManager_Budget_BudgetId",
                table: "EmailManager");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailManager_Offer_OfferId",
                table: "EmailManager");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailManager_Order_OrderId",
                table: "EmailManager");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailManager_Partner_PartnerId",
                table: "EmailManager");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailManager_SubType_SubTypeId",
                table: "EmailManager");

            migrationBuilder.DropIndex(
                name: "IX_EmailManager_BudgetId",
                table: "EmailManager");

            migrationBuilder.DropIndex(
                name: "IX_EmailManager_OfferId",
                table: "EmailManager");

            migrationBuilder.DropIndex(
                name: "IX_EmailManager_OrderId",
                table: "EmailManager");

            migrationBuilder.DropIndex(
                name: "IX_EmailManager_PartnerId",
                table: "EmailManager");

            migrationBuilder.DropIndex(
                name: "IX_EmailManager_SubTypeId",
                table: "EmailManager");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "EmailManager");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "EmailManager");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "EmailManager");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "EmailManager");

            migrationBuilder.DropColumn(
                name: "SubTypeId",
                table: "EmailManager");
        }
    }
}
