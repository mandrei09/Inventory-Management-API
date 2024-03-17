using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInsuranceCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InsuranceCategoryId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InsuranceCategoryId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InsuranceCategory",
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
                    table.PrimaryKey("PK_InsuranceCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceCategory_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_InsuranceCategoryId",
                table: "AssetAdmMD",
                column: "InsuranceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InsuranceCategoryId",
                table: "Asset",
                column: "InsuranceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceCategory_CompanyId",
                table: "InsuranceCategory",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_InsuranceCategory_InsuranceCategoryId",
                table: "Asset",
                column: "InsuranceCategoryId",
                principalTable: "InsuranceCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_InsuranceCategory_InsuranceCategoryId",
                table: "AssetAdmMD",
                column: "InsuranceCategoryId",
                principalTable: "InsuranceCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_InsuranceCategory_InsuranceCategoryId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_InsuranceCategory_InsuranceCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropTable(
                name: "InsuranceCategory");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_InsuranceCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_InsuranceCategoryId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "InsuranceCategoryId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "InsuranceCategoryId",
                table: "Asset");
        }
    }
}
