using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class remove_expaccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_ExpAccount_ExpAccountId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_ExpAccount_ExpAccountId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_ExpAccount_ExpAccountId",
                table: "Material");

            migrationBuilder.DropTable(
                name: "ExpAccount");

            migrationBuilder.DropIndex(
                name: "IX_Material_ExpAccountId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_ExpAccountId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ExpAccountId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ExpAccountId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "ExpAccountId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "ExpAccountId",
                table: "Asset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpAccountId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpAccountId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpAccountId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    RequireSN = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpAccount_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Material_ExpAccountId",
                table: "Material",
                column: "ExpAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_ExpAccountId",
                table: "AssetAdmMD",
                column: "ExpAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ExpAccountId",
                table: "Asset",
                column: "ExpAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpAccount_CompanyId",
                table: "ExpAccount",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_ExpAccount_ExpAccountId",
                table: "Asset",
                column: "ExpAccountId",
                principalTable: "ExpAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_ExpAccount_ExpAccountId",
                table: "AssetAdmMD",
                column: "ExpAccountId",
                principalTable: "ExpAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_ExpAccount_ExpAccountId",
                table: "Material",
                column: "ExpAccountId",
                principalTable: "ExpAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
