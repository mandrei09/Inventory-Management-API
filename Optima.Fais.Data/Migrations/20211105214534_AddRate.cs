using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "OrderMaterial",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "OfferMaterial",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rate",
                columns: table => new
                {
                    RateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    UomId = table.Column<int>(nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rate", x => x.RateId);
                    table.ForeignKey(
                        name: "FK_Rate_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rate_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rate_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_RateId",
                table: "OrderMaterial",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_RateId",
                table: "Order",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferMaterial_RateId",
                table: "OfferMaterial",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_RateId",
                table: "Offer",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_Rate_AccMonthId",
                table: "Rate",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Rate_CompanyId",
                table: "Rate",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Rate_UomId",
                table: "Rate",
                column: "UomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Rate_RateId",
                table: "Offer",
                column: "RateId",
                principalTable: "Rate",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferMaterial_Rate_RateId",
                table: "OfferMaterial",
                column: "RateId",
                principalTable: "Rate",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Rate_RateId",
                table: "Order",
                column: "RateId",
                principalTable: "Rate",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderMaterial_Rate_RateId",
                table: "OrderMaterial",
                column: "RateId",
                principalTable: "Rate",
                principalColumn: "RateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Rate_RateId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferMaterial_Rate_RateId",
                table: "OfferMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Rate_RateId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderMaterial_Rate_RateId",
                table: "OrderMaterial");

            migrationBuilder.DropTable(
                name: "Rate");

            migrationBuilder.DropIndex(
                name: "IX_OrderMaterial_RateId",
                table: "OrderMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Order_RateId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_OfferMaterial_RateId",
                table: "OfferMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Offer_RateId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "Offer");
        }
    }
}
