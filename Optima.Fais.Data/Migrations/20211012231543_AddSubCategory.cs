using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddSubCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpAccountId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryENId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubTypeId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyENId",
                table: "InterCompany",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InterCompanyEN",
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
                    table.PrimaryKey("PK_InterCompanyEN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterCompanyEN_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryEN",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InterCompanyENId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryEN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryEN_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryEN_InterCompanyEN_InterCompanyENId",
                        column: x => x.InterCompanyENId,
                        principalTable: "InterCompanyEN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryENId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InterCompanyId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_CategoryEN_CategoryENId",
                        column: x => x.CategoryENId,
                        principalTable: "CategoryEN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Category_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Category_InterCompany_InterCompanyId",
                        column: x => x.InterCompanyId,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCategoryEN",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryENId = table.Column<int>(nullable: true),
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
                        name: "FK_SubCategoryEN_CategoryEN_CategoryENId",
                        column: x => x.CategoryENId,
                        principalTable: "CategoryEN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCategoryEN_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryENId = table.Column<int>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
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
                        name: "FK_SubCategory_CategoryEN_CategoryENId",
                        column: x => x.CategoryENId,
                        principalTable: "CategoryEN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCategory_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Material_AccountId",
                table: "Material",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_AssetCategoryId",
                table: "Material",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_ExpAccountId",
                table: "Material",
                column: "ExpAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_SubCategoryENId",
                table: "Material",
                column: "SubCategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_SubCategoryId",
                table: "Material",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_SubTypeId",
                table: "Material",
                column: "SubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_InterCompanyENId",
                table: "InterCompany",
                column: "InterCompanyENId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CategoryENId",
                table: "Category",
                column: "CategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId",
                table: "Category",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_InterCompanyId",
                table: "Category",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEN_CompanyId",
                table: "CategoryEN",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEN_InterCompanyENId",
                table: "CategoryEN",
                column: "InterCompanyENId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompanyEN_CompanyId",
                table: "InterCompanyEN",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryENId",
                table: "SubCategory",
                column: "CategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CompanyId",
                table: "SubCategory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryEN_CategoryENId",
                table: "SubCategoryEN",
                column: "CategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryEN_CompanyId",
                table: "SubCategoryEN",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterCompany_InterCompanyEN_InterCompanyENId",
                table: "InterCompany",
                column: "InterCompanyENId",
                principalTable: "InterCompanyEN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_Account_AccountId",
                table: "Material",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_AssetCategory_AssetCategoryId",
                table: "Material",
                column: "AssetCategoryId",
                principalTable: "AssetCategory",
                principalColumn: "AssetCategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_ExpAccount_ExpAccountId",
                table: "Material",
                column: "ExpAccountId",
                principalTable: "ExpAccount",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Material_SubType_SubTypeId",
                table: "Material",
                column: "SubTypeId",
                principalTable: "SubType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterCompany_InterCompanyEN_InterCompanyENId",
                table: "InterCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_Account_AccountId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_AssetCategory_AssetCategoryId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_ExpAccount_ExpAccountId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_SubCategoryEN_SubCategoryENId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_SubCategory_SubCategoryId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_SubType_SubTypeId",
                table: "Material");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropTable(
                name: "SubCategoryEN");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "CategoryEN");

            migrationBuilder.DropTable(
                name: "InterCompanyEN");

            migrationBuilder.DropIndex(
                name: "IX_Material_AccountId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_AssetCategoryId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_ExpAccountId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_SubCategoryENId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_SubCategoryId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_Material_SubTypeId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_InterCompany_InterCompanyENId",
                table: "InterCompany");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "ExpAccountId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "SubCategoryENId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "SubTypeId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "InterCompanyENId",
                table: "InterCompany");
        }
    }
}
