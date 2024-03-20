using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class removesubcategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_SubCategory_SubCategoryId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_SubCategory_SubCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_SubCategoryEN_SubCategoryENId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_SubCategory_SubCategoryId",
                table: "Material");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropTable(
                name: "SubCategoryEN");

            migrationBuilder.DropIndex(
                name: "IX_Material_SubCategoryENId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_SubCategoryId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_SubCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_SubCategoryId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "SubCategoryENId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Asset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryENId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubCategory",
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
                    table.PrimaryKey("PK_SubCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategory_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCategoryEN",
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
                    table.PrimaryKey("PK_SubCategoryEN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategoryEN_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Material_SubCategoryENId",
                table: "Material",
                column: "SubCategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_SubCategoryId",
                table: "Material",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_SubCategoryId",
                table: "AssetAdmMD",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_SubCategoryId",
                table: "Asset",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CompanyId",
                table: "SubCategory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryEN_CompanyId",
                table: "SubCategoryEN",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_SubCategory_SubCategoryId",
                table: "Asset",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_SubCategory_SubCategoryId",
                table: "AssetAdmMD",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_SubCategoryEN_SubCategoryENId",
                table: "Material",
                column: "SubCategoryENId",
                principalTable: "SubCategoryEN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_SubCategory_SubCategoryId",
                table: "Material",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
