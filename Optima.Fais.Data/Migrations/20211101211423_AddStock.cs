using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppStateId",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmailManagerId",
                table: "OfferMaterial",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaterialTypeId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "InterCompany",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryId",
                table: "InterCompany",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "InterCompany",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpAccountId",
                table: "InterCompany",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaterialType",
                columns: table => new
                {
                    MaterialTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_MaterialType", x => x.MaterialTypeId);
                    table.ForeignKey(
                        name: "FK_MaterialType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderMaterial",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppStateId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MaterialId = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMaterial_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderMaterial_Material_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderMaterial_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrandId = table.Column<int>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 200, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Last_Incoming_Date = table.Column<DateTime>(nullable: true),
                    LongName = table.Column<string>(maxLength: 400, nullable: false),
                    MaterialId = table.Column<int>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 400, nullable: false),
                    PartnerId = table.Column<int>(nullable: true),
                    Plant = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: false),
                    Storage_Location = table.Column<string>(nullable: true),
                    UM = table.Column<string>(nullable: true),
                    UomId = table.Column<int>(nullable: true),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stock_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stock_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stock_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stock_Material_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stock_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stock_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferMaterial_AppStateId",
                table: "OfferMaterial",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferMaterial_EmailManagerId",
                table: "OfferMaterial",
                column: "EmailManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_MaterialTypeId",
                table: "Material",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_AccountId",
                table: "InterCompany",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_AssetCategoryId",
                table: "InterCompany",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_AssetTypeId",
                table: "InterCompany",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_ExpAccountId",
                table: "InterCompany",
                column: "ExpAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_StockId",
                table: "Asset",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialType_CompanyId",
                table: "MaterialType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_AppStateId",
                table: "OrderMaterial",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_MaterialId",
                table: "OrderMaterial",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_OrderId",
                table: "OrderMaterial",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_BrandId",
                table: "Stock",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_CategoryId",
                table: "Stock",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_CompanyId",
                table: "Stock",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_MaterialId",
                table: "Stock",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_PartnerId",
                table: "Stock",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_UomId",
                table: "Stock",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Stock_StockId",
                table: "Asset",
                column: "StockId",
                principalTable: "Stock",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterCompany_Account_AccountId",
                table: "InterCompany",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterCompany_AssetCategory_AssetCategoryId",
                table: "InterCompany",
                column: "AssetCategoryId",
                principalTable: "AssetCategory",
                principalColumn: "AssetCategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterCompany_AssetType_AssetTypeId",
                table: "InterCompany",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterCompany_ExpAccount_ExpAccountId",
                table: "InterCompany",
                column: "ExpAccountId",
                principalTable: "ExpAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Material_MaterialType_MaterialTypeId",
                table: "Material",
                column: "MaterialTypeId",
                principalTable: "MaterialType",
                principalColumn: "MaterialTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferMaterial_AppState_AppStateId",
                table: "OfferMaterial",
                column: "AppStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferMaterial_EmailManager_EmailManagerId",
                table: "OfferMaterial",
                column: "EmailManagerId",
                principalTable: "EmailManager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Stock_StockId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_InterCompany_Account_AccountId",
                table: "InterCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_InterCompany_AssetCategory_AssetCategoryId",
                table: "InterCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_InterCompany_AssetType_AssetTypeId",
                table: "InterCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_InterCompany_ExpAccount_ExpAccountId",
                table: "InterCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_MaterialType_MaterialTypeId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferMaterial_AppState_AppStateId",
                table: "OfferMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferMaterial_EmailManager_EmailManagerId",
                table: "OfferMaterial");

            migrationBuilder.DropTable(
                name: "MaterialType");

            migrationBuilder.DropTable(
                name: "OrderMaterial");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropIndex(
                name: "IX_OfferMaterial_AppStateId",
                table: "OfferMaterial");

            migrationBuilder.DropIndex(
                name: "IX_OfferMaterial_EmailManagerId",
                table: "OfferMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Material_MaterialTypeId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_InterCompany_AccountId",
                table: "InterCompany");

            migrationBuilder.DropIndex(
                name: "IX_InterCompany_AssetCategoryId",
                table: "InterCompany");

            migrationBuilder.DropIndex(
                name: "IX_InterCompany_AssetTypeId",
                table: "InterCompany");

            migrationBuilder.DropIndex(
                name: "IX_InterCompany_ExpAccountId",
                table: "InterCompany");

            migrationBuilder.DropIndex(
                name: "IX_Asset_StockId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "AppStateId",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "EmailManagerId",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "MaterialTypeId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "InterCompany");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "InterCompany");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "InterCompany");

            migrationBuilder.DropColumn(
                name: "ExpAccountId",
                table: "InterCompany");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Asset");
        }
    }
}
