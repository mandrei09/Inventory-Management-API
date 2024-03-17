using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestBFMaterialCostCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReqBFMCostCenterId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestBFMaterialCostCenter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppStateId = table.Column<int>(nullable: true),
                    BudgetForecastTimeStamp = table.Column<decimal>(nullable: false),
                    BudgetValueNeed = table.Column<decimal>(nullable: false),
                    CostCenterId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    OfferMaterialId = table.Column<int>(nullable: true),
                    OrderId = table.Column<int>(nullable: true),
                    OrderMaterialId = table.Column<int>(nullable: true),
                    PreAmount = table.Column<decimal>(nullable: false),
                    PreAmountRon = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PriceRon = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    QuantityRem = table.Column<int>(nullable: false),
                    ReceptionsPrice = table.Column<decimal>(nullable: false),
                    ReceptionsPriceRon = table.Column<decimal>(nullable: false),
                    ReceptionsQuantity = table.Column<decimal>(nullable: false),
                    ReceptionsValue = table.Column<decimal>(nullable: false),
                    ReceptionsValueRon = table.Column<decimal>(nullable: false),
                    RequestBudgetForecastMaterialId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    ValueRem = table.Column<decimal>(nullable: false),
                    ValueRemRon = table.Column<decimal>(nullable: false),
                    ValueRon = table.Column<decimal>(nullable: false),
                    WIP = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestBFMaterialCostCenter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestBFMaterialCostCenter_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBFMaterialCostCenter_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestBFMaterialCostCenter_OfferMaterial_OfferMaterialId",
                        column: x => x.OfferMaterialId,
                        principalTable: "OfferMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBFMaterialCostCenter_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBFMaterialCostCenter_OrderMaterial_OrderMaterialId",
                        column: x => x.OrderMaterialId,
                        principalTable: "OrderMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBFMaterialCostCenter_RequestBudgetForecastMaterial_RequestBudgetForecastMaterialId",
                        column: x => x.RequestBudgetForecastMaterialId,
                        principalTable: "RequestBudgetForecastMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ReqBFMCostCenterId",
                table: "Asset",
                column: "ReqBFMCostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_AppStateId",
                table: "RequestBFMaterialCostCenter",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_CostCenterId",
                table: "RequestBFMaterialCostCenter",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_OfferMaterialId",
                table: "RequestBFMaterialCostCenter",
                column: "OfferMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_OrderId",
                table: "RequestBFMaterialCostCenter",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_OrderMaterialId",
                table: "RequestBFMaterialCostCenter",
                column: "OrderMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBFMaterialCostCenter_RequestBudgetForecastMaterialId",
                table: "RequestBFMaterialCostCenter",
                column: "RequestBudgetForecastMaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_RequestBFMaterialCostCenter_ReqBFMCostCenterId",
                table: "Asset",
                column: "ReqBFMCostCenterId",
                principalTable: "RequestBFMaterialCostCenter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_RequestBFMaterialCostCenter_ReqBFMCostCenterId",
                table: "Asset");

            migrationBuilder.DropTable(
                name: "RequestBFMaterialCostCenter");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ReqBFMCostCenterId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "ReqBFMCostCenterId",
                table: "Asset");
        }
    }
}
