using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddCostCenterArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_ArticleId",
                table: "CostCenter",
                column: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Article_ArticleId",
                table: "CostCenter",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Article_ArticleId",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_ArticleId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "CostCenter");
        }
    }
}
