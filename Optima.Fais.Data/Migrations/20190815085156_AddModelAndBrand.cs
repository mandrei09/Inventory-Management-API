using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddModelAndBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModelId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModelId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brand_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Model",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Model", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Model_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_BrandId",
                table: "AssetAdmMD",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_ModelId",
                table: "AssetAdmMD",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_BrandId",
                table: "Asset",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ModelId",
                table: "Asset",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_CompanyId",
                table: "Brand",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Model_CompanyId",
                table: "Model",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Brand_BrandId",
                table: "Asset",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Model_ModelId",
                table: "Asset",
                column: "ModelId",
                principalTable: "Model",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Brand_BrandId",
                table: "AssetAdmMD",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Model_ModelId",
                table: "AssetAdmMD",
                column: "ModelId",
                principalTable: "Model",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Brand_BrandId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Model_ModelId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Brand_BrandId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Model_ModelId",
                table: "AssetAdmMD");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Model");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_BrandId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_ModelId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_BrandId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ModelId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Asset");
        }
    }
}
