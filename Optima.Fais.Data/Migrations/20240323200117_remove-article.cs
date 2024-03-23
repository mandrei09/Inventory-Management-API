using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class removearticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Article_ArticleId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Article_ArticleId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Article_ArticleId",
                table: "CostCenter");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_ArticleId",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_ArticleId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ArticleId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Asset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Article_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_ArticleId",
                table: "CostCenter",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_ArticleId",
                table: "AssetAdmMD",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ArticleId",
                table: "Asset",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_CompanyId",
                table: "Article",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Article_ArticleId",
                table: "Asset",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Article_ArticleId",
                table: "AssetAdmMD",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Article_ArticleId",
                table: "CostCenter",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
