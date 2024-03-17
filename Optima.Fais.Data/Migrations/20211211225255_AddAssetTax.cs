using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaxId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderText",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Tax",
                columns: table => new
                {
                    TaxId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax", x => x.TaxId);
                    table.ForeignKey(
                        name: "FK_Tax_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_TaxId",
                table: "AssetOp",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_TaxId",
                table: "Asset",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_CompanyId",
                table: "Tax",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Tax_TaxId",
                table: "Asset",
                column: "TaxId",
                principalTable: "Tax",
                principalColumn: "TaxId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Tax_TaxId",
                table: "AssetOp",
                column: "TaxId",
                principalTable: "Tax",
                principalColumn: "TaxId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Tax_TaxId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_Tax_TaxId",
                table: "AssetOp");

            migrationBuilder.DropTable(
                name: "Tax");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_TaxId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_Asset_TaxId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "HeaderText",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Asset");
        }
    }
}
