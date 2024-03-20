using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class migr5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accountancy_InterCompany_InterCompanyId",
                table: "Accountancy");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_InterCompany_InterCompanyId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_InterCompany_InterCompanyId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_InterCompany_InterCompanyId",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_InterCompany_InterCompanyId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_InterCompany_InterCompanyIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_InterCompany_InterCompanyIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_InterCompany_InterCompanyId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryEN_InterCompanyEN_InterCompanyENId",
                table: "CategoryEN");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_InterCompany_InterCompanyId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_InterCompany_InterCompanyIdFinal",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_InterCompany_InterCompanyIdInitial",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_InterCompany_InterCompanyId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderOp_InterCompany_InterCompanyIdFinal",
                table: "OrderOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderOp_InterCompany_InterCompanyIdInitial",
                table: "OrderOp");

            migrationBuilder.DropForeignKey(
                name: "FK_Partner_InterCompany_InterCompanyId",
                table: "Partner");

            migrationBuilder.DropTable(
                name: "InterCompany");

            migrationBuilder.DropTable(
                name: "InterCompanyEN");

            migrationBuilder.DropIndex(
                name: "IX_Partner_InterCompanyId",
                table: "Partner");

            migrationBuilder.DropIndex(
                name: "IX_OrderOp_InterCompanyIdFinal",
                table: "OrderOp");

            migrationBuilder.DropIndex(
                name: "IX_OrderOp_InterCompanyIdInitial",
                table: "OrderOp");

            migrationBuilder.DropIndex(
                name: "IX_Order_InterCompanyId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_InterCompanyIdFinal",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_InterCompanyIdInitial",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_Offer_InterCompanyId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_CategoryEN_InterCompanyENId",
                table: "CategoryEN");

            migrationBuilder.DropIndex(
                name: "IX_Category_InterCompanyId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_InterCompanyIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_InterCompanyIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_Budget_InterCompanyId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_InterCompanyId",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_InterCompanyId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_InterCompanyId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Accountancy_InterCompanyId",
                table: "Accountancy");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Partner");

            migrationBuilder.DropColumn(
                name: "InterCompanyIdFinal",
                table: "OrderOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyIdInitial",
                table: "OrderOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "InterCompanyIdFinal",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyIdInitial",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "MatrixImport");

            migrationBuilder.DropColumn(
                name: "InterCompanyENId",
                table: "CategoryEN");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "InterCompanyIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "InterCompanyId",
                table: "Accountancy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Partner",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyIdFinal",
                table: "OrderOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyIdInitial",
                table: "OrderOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyIdFinal",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyIdInitial",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "MatrixImport",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyENId",
                table: "CategoryEN",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Category",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "AssetAdmMD",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterCompanyId",
                table: "Accountancy",
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
                name: "InterCompany",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: true),
                    AssetCategoryId = table.Column<int>(nullable: true),
                    AssetTypeId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ERPId = table.Column<int>(nullable: true),
                    ExpAccountId = table.Column<int>(nullable: true),
                    InterCompanyENId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterCompany_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterCompany_AssetCategory_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategory",
                        principalColumn: "AssetCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterCompany_AssetType_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetType",
                        principalColumn: "AssetTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterCompany_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterCompany_ExpAccount_ExpAccountId",
                        column: x => x.ExpAccountId,
                        principalTable: "ExpAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterCompany_InterCompanyEN_InterCompanyENId",
                        column: x => x.InterCompanyENId,
                        principalTable: "InterCompanyEN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partner_InterCompanyId",
                table: "Partner",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_InterCompanyIdFinal",
                table: "OrderOp",
                column: "InterCompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_InterCompanyIdInitial",
                table: "OrderOp",
                column: "InterCompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Order_InterCompanyId",
                table: "Order",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_InterCompanyIdFinal",
                table: "OfferOp",
                column: "InterCompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_InterCompanyIdInitial",
                table: "OfferOp",
                column: "InterCompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_InterCompanyId",
                table: "Offer",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEN_InterCompanyENId",
                table: "CategoryEN",
                column: "InterCompanyENId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_InterCompanyId",
                table: "Category",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_InterCompanyIdFinal",
                table: "BudgetOp",
                column: "InterCompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_InterCompanyIdInitial",
                table: "BudgetOp",
                column: "InterCompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_InterCompanyId",
                table: "Budget",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_InterCompanyId",
                table: "AssetOp",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_InterCompanyId",
                table: "AssetAdmMD",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InterCompanyId",
                table: "Asset",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accountancy_InterCompanyId",
                table: "Accountancy",
                column: "InterCompanyId");

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
                name: "IX_InterCompany_CompanyId",
                table: "InterCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_ExpAccountId",
                table: "InterCompany",
                column: "ExpAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompany_InterCompanyENId",
                table: "InterCompany",
                column: "InterCompanyENId");

            migrationBuilder.CreateIndex(
                name: "IX_InterCompanyEN_CompanyId",
                table: "InterCompanyEN",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accountancy_InterCompany_InterCompanyId",
                table: "Accountancy",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_InterCompany_InterCompanyId",
                table: "Asset",
                column: "InterCompanyId",
                principalTable: "InterCompany",
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
                name: "FK_AssetOp_InterCompany_InterCompanyId",
                table: "AssetOp",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_InterCompany_InterCompanyId",
                table: "Budget",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_InterCompany_InterCompanyIdFinal",
                table: "BudgetOp",
                column: "InterCompanyIdFinal",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_InterCompany_InterCompanyIdInitial",
                table: "BudgetOp",
                column: "InterCompanyIdInitial",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Category_InterCompany_InterCompanyId",
                table: "Category",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryEN_InterCompanyEN_InterCompanyENId",
                table: "CategoryEN",
                column: "InterCompanyENId",
                principalTable: "InterCompanyEN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_InterCompany_InterCompanyId",
                table: "Offer",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_InterCompany_InterCompanyIdFinal",
                table: "OfferOp",
                column: "InterCompanyIdFinal",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_InterCompany_InterCompanyIdInitial",
                table: "OfferOp",
                column: "InterCompanyIdInitial",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_InterCompany_InterCompanyId",
                table: "Order",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderOp_InterCompany_InterCompanyIdFinal",
                table: "OrderOp",
                column: "InterCompanyIdFinal",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderOp_InterCompany_InterCompanyIdInitial",
                table: "OrderOp",
                column: "InterCompanyIdInitial",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_InterCompany_InterCompanyId",
                table: "Partner",
                column: "InterCompanyId",
                principalTable: "InterCompany",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
