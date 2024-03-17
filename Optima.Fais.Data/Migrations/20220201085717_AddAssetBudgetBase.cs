using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetBudgetBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetBaseId",
                table: "Request",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetBaseId",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetBaseId",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BudgetBaseId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_BudgetBaseId",
                table: "Request",
                column: "BudgetBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BudgetBaseId",
                table: "Order",
                column: "BudgetBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_BudgetBaseId",
                table: "Offer",
                column: "BudgetBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_ProjectTypeId",
                table: "Matrix",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_BudgetBaseId",
                table: "Asset",
                column: "BudgetBaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_BudgetBase_BudgetBaseId",
                table: "Asset",
                column: "BudgetBaseId",
                principalTable: "BudgetBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_ProjectType_ProjectTypeId",
                table: "Matrix",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_BudgetBase_BudgetBaseId",
                table: "Offer",
                column: "BudgetBaseId",
                principalTable: "BudgetBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_BudgetBase_BudgetBaseId",
                table: "Order",
                column: "BudgetBaseId",
                principalTable: "BudgetBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_BudgetBase_BudgetBaseId",
                table: "Request",
                column: "BudgetBaseId",
                principalTable: "BudgetBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_BudgetBase_BudgetBaseId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_ProjectType_ProjectTypeId",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_BudgetBase_BudgetBaseId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_BudgetBase_BudgetBaseId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_BudgetBase_BudgetBaseId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_BudgetBaseId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Order_BudgetBaseId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Offer_BudgetBaseId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_ProjectTypeId",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_Asset_BudgetBaseId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "BudgetBaseId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "BudgetBaseId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "BudgetBaseId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "BudgetBaseId",
                table: "Asset");
        }
    }
}
