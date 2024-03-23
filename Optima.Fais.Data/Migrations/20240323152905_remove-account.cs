using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class removeaccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Account_AccountId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_Account_AccountId",
                table: "AssetAdmMD");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Account_AccountId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Account_AccountIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Account_AccountIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_Material_Account_AccountId",
                table: "Material");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Account_AccountId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_Account_AccountIdFinal",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_Account_AccountIdInitial",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Account_AccountId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderOp_Account_AccountIdFinal",
                table: "OrderOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderOp_Account_AccountIdInitial",
                table: "OrderOp");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropIndex(
                name: "IX_OrderOp_AccountIdFinal",
                table: "OrderOp");

            migrationBuilder.DropIndex(
                name: "IX_OrderOp_AccountIdInitial",
                table: "OrderOp");

            migrationBuilder.DropIndex(
                name: "IX_Order_AccountId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_AccountIdFinal",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_AccountIdInitial",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_Offer_AccountId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Material_AccountId",
                table: "Material");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_AccountIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_AccountIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_Budget_AccountId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_AccountId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_AccountId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "AccountIdFinal",
                table: "OrderOp");

            migrationBuilder.DropColumn(
                name: "AccountIdInitial",
                table: "OrderOp");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Material");

            migrationBuilder.DropColumn(
                name: "AccountIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "AccountIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Budget");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountIdFinal",
                table: "OrderOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountIdInitial",
                table: "OrderOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Material",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Budget",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Account",
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
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AccountIdFinal",
                table: "OrderOp",
                column: "AccountIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AccountIdInitial",
                table: "OrderOp",
                column: "AccountIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AccountId",
                table: "Order",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AccountIdFinal",
                table: "OfferOp",
                column: "AccountIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AccountIdInitial",
                table: "OfferOp",
                column: "AccountIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_AccountId",
                table: "Offer",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_AccountId",
                table: "Material",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AccountIdFinal",
                table: "BudgetOp",
                column: "AccountIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AccountIdInitial",
                table: "BudgetOp",
                column: "AccountIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AccountId",
                table: "Budget",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_AccountId",
                table: "AssetAdmMD",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AccountId",
                table: "Asset",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_CompanyId",
                table: "Account",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Account_AccountId",
                table: "Asset",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Account_AccountId",
                table: "AssetAdmMD",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Account_AccountId",
                table: "Budget",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Account_AccountIdFinal",
                table: "BudgetOp",
                column: "AccountIdFinal",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Account_AccountIdInitial",
                table: "BudgetOp",
                column: "AccountIdInitial",
                principalTable: "Account",
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
                name: "FK_Offer_Account_AccountId",
                table: "Offer",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_Account_AccountIdFinal",
                table: "OfferOp",
                column: "AccountIdFinal",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_Account_AccountIdInitial",
                table: "OfferOp",
                column: "AccountIdInitial",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Account_AccountId",
                table: "Order",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderOp_Account_AccountIdFinal",
                table: "OrderOp",
                column: "AccountIdFinal",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderOp_Account_AccountIdInitial",
                table: "OrderOp",
                column: "AccountIdInitial",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
