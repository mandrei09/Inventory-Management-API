using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddProjectInterComapny : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InterCompany",
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
                    table.PrimaryKey("PK_InterCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterCompany_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project",
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
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_InterCompanyId",
                table: "AssetAdmMD",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_ProjectId",
                table: "AssetAdmMD",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InterCompanyId",
                table: "Asset",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ProjectId",
                table: "Asset",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_CompanyId",
                table: "InterCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId",
                table: "Project",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_InterCompany_InterCompanyId",
                table: "Asset",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Project_ProjectId",
                table: "Asset",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_InterCompany_InterCompanyId",
                table: "AssetAdmMD",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Project_ProjectId",
                table: "AssetAdmMD",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_InterCompany_InterCompanyId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Project_ProjectId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_InterCompany_InterCompanyId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Project_ProjectId",
                table: "AssetAdmMD");

            migrationBuilder.DropTable(
                name: "InterCompany");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_InterCompanyId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_ProjectId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_InterCompanyId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ProjectId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Asset");
        }
    }
}
