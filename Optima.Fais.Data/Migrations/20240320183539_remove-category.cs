using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class removecategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Category_CategoryId",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_CategoryEN_CategoryENId",
                table: "SubCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Category_CategoryId",
                table: "SubCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategoryEN_CategoryEN_CategoryENId",
                table: "SubCategoryEN");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "CategoryEN");

            migrationBuilder.DropIndex(
                name: "IX_SubCategoryEN_CategoryENId",
                table: "SubCategoryEN");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CategoryENId",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_Stock_CategoryId",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "CategoryENId",
                table: "SubCategoryEN");

            migrationBuilder.DropColumn(
                name: "CategoryENId",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Stock");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryENId",
                table: "SubCategoryEN",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryENId",
                table: "SubCategory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "SubCategory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Stock",
                nullable: true);

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
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryEN_CategoryENId",
                table: "SubCategoryEN",
                column: "CategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryENId",
                table: "SubCategory",
                column: "CategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_CategoryId",
                table: "Stock",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CategoryENId",
                table: "Category",
                column: "CategoryENId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId",
                table: "Category",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEN_CompanyId",
                table: "CategoryEN",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Category_CategoryId",
                table: "Stock",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_CategoryEN_CategoryENId",
                table: "SubCategory",
                column: "CategoryENId",
                principalTable: "CategoryEN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Category_CategoryId",
                table: "SubCategory",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategoryEN_CategoryEN_CategoryENId",
                table: "SubCategoryEN",
                column: "CategoryENId",
                principalTable: "CategoryEN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
