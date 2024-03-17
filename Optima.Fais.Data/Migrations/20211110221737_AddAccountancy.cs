using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAccountancy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountRem",
                table: "ContractAmount",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Accountancy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    AssetCategoryId = table.Column<int>(nullable: false),
                    AssetTypeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ExpAccountId = table.Column<int>(nullable: false),
                    InterCompanyId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accountancy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accountancy_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accountancy_AssetCategory_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategory",
                        principalColumn: "AssetCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accountancy_AssetType_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetType",
                        principalColumn: "AssetTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accountancy_ExpAccount_ExpAccountId",
                        column: x => x.ExpAccountId,
                        principalTable: "ExpAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accountancy_InterCompany_InterCompanyId",
                        column: x => x.InterCompanyId,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accountancy_AccountId",
                table: "Accountancy",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accountancy_AssetCategoryId",
                table: "Accountancy",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Accountancy_AssetTypeId",
                table: "Accountancy",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accountancy_ExpAccountId",
                table: "Accountancy",
                column: "ExpAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accountancy_InterCompanyId",
                table: "Accountancy",
                column: "InterCompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accountancy");

            migrationBuilder.DropColumn(
                name: "AmountRem",
                table: "ContractAmount");
        }
    }
}
