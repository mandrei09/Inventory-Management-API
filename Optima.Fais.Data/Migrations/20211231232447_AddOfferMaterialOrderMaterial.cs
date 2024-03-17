using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferMaterialOrderMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfferMaterialId",
                table: "OrderMaterial",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "OrderMaterial",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferMaterialId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderMaterialId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceptionPrice",
                table: "Asset",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AssetDepMDSync",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ANBTR_R = table.Column<decimal>(nullable: false),
                    ANBTR_T = table.Column<decimal>(nullable: false),
                    ANSWL = table.Column<decimal>(nullable: false),
                    AccMonthId = table.Column<int>(nullable: false),
                    AccSystemId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DEPFY = table.Column<decimal>(nullable: false),
                    DEPRET = table.Column<decimal>(nullable: false),
                    DEPTRANS = table.Column<decimal>(nullable: false),
                    DEPY = table.Column<decimal>(nullable: false),
                    InvNo = table.Column<string>(maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsImported = table.Column<bool>(nullable: false),
                    KANSW = table.Column<decimal>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NDABJ = table.Column<string>(nullable: true),
                    NDJAR = table.Column<string>(nullable: true),
                    NDPER = table.Column<string>(nullable: true),
                    SubNumber = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDepMDSync", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDepMDSync_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDepMDSync_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDepMDSync_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDepMDSync_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ErrorType",
                columns: table => new
                {
                    ErrorTypeId = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_ErrorType", x => x.ErrorTypeId);
                    table.ForeignKey(
                        name: "FK_ErrorType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Error",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ErrorTypeId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Error", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Error_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Error_ErrorType_ErrorTypeId",
                        column: x => x.ErrorTypeId,
                        principalTable: "ErrorType",
                        principalColumn: "ErrorTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Error_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_OfferMaterialId",
                table: "OrderMaterial",
                column: "OfferMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_RequestId",
                table: "OrderMaterial",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OfferMaterialId",
                table: "Asset",
                column: "OfferMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OrderMaterialId",
                table: "Asset",
                column: "OrderMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMDSync_AccMonthId",
                table: "AssetDepMDSync",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMDSync_AccSystemId",
                table: "AssetDepMDSync",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMDSync_BudgetManagerId",
                table: "AssetDepMDSync",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMDSync_CompanyId",
                table: "AssetDepMDSync",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Error_AssetId",
                table: "Error",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Error_ErrorTypeId",
                table: "Error",
                column: "ErrorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Error_UserId",
                table: "Error",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorType_CompanyId",
                table: "ErrorType",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_OfferMaterial_OfferMaterialId",
                table: "Asset",
                column: "OfferMaterialId",
                principalTable: "OfferMaterial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_OrderMaterial_OrderMaterialId",
                table: "Asset",
                column: "OrderMaterialId",
                principalTable: "OrderMaterial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderMaterial_OfferMaterial_OfferMaterialId",
                table: "OrderMaterial",
                column: "OfferMaterialId",
                principalTable: "OfferMaterial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderMaterial_Request_RequestId",
                table: "OrderMaterial",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_OfferMaterial_OfferMaterialId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_OrderMaterial_OrderMaterialId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderMaterial_OfferMaterial_OfferMaterialId",
                table: "OrderMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderMaterial_Request_RequestId",
                table: "OrderMaterial");

            migrationBuilder.DropTable(
                name: "AssetDepMDSync");

            migrationBuilder.DropTable(
                name: "Error");

            migrationBuilder.DropTable(
                name: "ErrorType");

            migrationBuilder.DropIndex(
                name: "IX_OrderMaterial_OfferMaterialId",
                table: "OrderMaterial");

            migrationBuilder.DropIndex(
                name: "IX_OrderMaterial_RequestId",
                table: "OrderMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Asset_OfferMaterialId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_OrderMaterialId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "OfferMaterialId",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "OfferMaterialId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "OrderMaterialId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ReceptionPrice",
                table: "Asset");
        }
    }
}
