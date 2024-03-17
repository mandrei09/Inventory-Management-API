using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "CostCenter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InvestSupport",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WriteUps",
                table: "AssetDepMD",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgreementNo",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubNo",
                table: "Asset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Material_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_RegionId",
                table: "CostCenter",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_DivisionId",
                table: "AssetAdmMD",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_DivisionId",
                table: "Asset",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_MaterialId",
                table: "Asset",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_CompanyId",
                table: "Material",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Division_DivisionId",
                table: "Asset",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Material_MaterialId",
                table: "Asset",
                column: "MaterialId",
                principalTable: "Material",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Division_DivisionId",
                table: "AssetAdmMD",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenter_Region_RegionId",
                table: "CostCenter",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Division_DivisionId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Material_MaterialId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Division_DivisionId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Region_RegionId",
                table: "CostCenter");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropIndex(
                name: "IX_CostCenter_RegionId",
                table: "CostCenter");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_DivisionId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_DivisionId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_MaterialId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "CostCenter");

            migrationBuilder.DropColumn(
                name: "InvestSupport",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "WriteUps",
                table: "AssetDepMD");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "AgreementNo",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "SubNo",
                table: "Asset");
        }
    }
}
