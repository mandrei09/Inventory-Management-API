using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAccuntancySubCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accountancy_InterCompany_InterCompanyId",
                table: "Accountancy");

            migrationBuilder.AlterColumn<int>(
                name: "InterCompanyId",
                table: "Accountancy",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Accountancy",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accountancy_SubCategoryId",
                table: "Accountancy",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accountancy_InterCompany_InterCompanyId",
                table: "Accountancy",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accountancy_SubCategory_SubCategoryId",
                table: "Accountancy",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accountancy_InterCompany_InterCompanyId",
                table: "Accountancy");

            migrationBuilder.DropForeignKey(
                name: "FK_Accountancy_SubCategory_SubCategoryId",
                table: "Accountancy");

            migrationBuilder.DropIndex(
                name: "IX_Accountancy_SubCategoryId",
                table: "Accountancy");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Accountancy");

            migrationBuilder.AlterColumn<int>(
                name: "InterCompanyId",
                table: "Accountancy",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accountancy_InterCompany_InterCompanyId",
                table: "Accountancy",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
