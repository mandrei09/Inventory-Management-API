using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequestBudgetForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetForecastId",
                table: "OrderMaterial",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReqBFMaterialId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestBudgetForecast",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: false),
                    AppStateId = table.Column<int>(nullable: true),
                    BudgetForecastId = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    ContractId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Materials = table.Column<string>(nullable: true),
                    MaxQuantity = table.Column<int>(nullable: false),
                    MaxValue = table.Column<decimal>(nullable: false),
                    MaxValueRon = table.Column<decimal>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NeedBudget = table.Column<bool>(nullable: false),
                    NeedContract = table.Column<bool>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PriceRon = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    RequestId = table.Column<int>(nullable: true),
                    TotalOrderQuantity = table.Column<int>(nullable: false),
                    TotalOrderValue = table.Column<decimal>(nullable: false),
                    TotalOrderValueRon = table.Column<decimal>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    ValueRon = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestBudgetForecast", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecast_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecast_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecast_BudgetForecast_BudgetForecastId",
                        column: x => x.BudgetForecastId,
                        principalTable: "BudgetForecast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecast_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecast_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecast_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestBudgetForecastMaterial",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppStateId = table.Column<int>(nullable: true),
                    BudgetForecastTimeStamp = table.Column<decimal>(nullable: false),
                    BudgetValueNeed = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MaterialId = table.Column<int>(nullable: false),
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
                    RequestBudgetForecastId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    ValueRem = table.Column<decimal>(nullable: false),
                    ValueRemRon = table.Column<decimal>(nullable: false),
                    ValueRon = table.Column<decimal>(nullable: false),
                    WIP = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestBudgetForecastMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecastMaterial_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecastMaterial_Material_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Material",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecastMaterial_OfferMaterial_OfferMaterialId",
                        column: x => x.OfferMaterialId,
                        principalTable: "OfferMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecastMaterial_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecastMaterial_OrderMaterial_OrderMaterialId",
                        column: x => x.OrderMaterialId,
                        principalTable: "OrderMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestBudgetForecastMaterial_RequestBudgetForecast_RequestBudgetForecastId",
                        column: x => x.RequestBudgetForecastId,
                        principalTable: "RequestBudgetForecast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterial_BudgetForecastId",
                table: "OrderMaterial",
                column: "BudgetForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ReqBFMaterialId",
                table: "Asset",
                column: "ReqBFMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecast_AccMonthId",
                table: "RequestBudgetForecast",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecast_AppStateId",
                table: "RequestBudgetForecast",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecast_BudgetForecastId",
                table: "RequestBudgetForecast",
                column: "BudgetForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecast_BudgetManagerId",
                table: "RequestBudgetForecast",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecast_ContractId",
                table: "RequestBudgetForecast",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecast_RequestId",
                table: "RequestBudgetForecast",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecastMaterial_AppStateId",
                table: "RequestBudgetForecastMaterial",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecastMaterial_MaterialId",
                table: "RequestBudgetForecastMaterial",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecastMaterial_OfferMaterialId",
                table: "RequestBudgetForecastMaterial",
                column: "OfferMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecastMaterial_OrderId",
                table: "RequestBudgetForecastMaterial",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecastMaterial_OrderMaterialId",
                table: "RequestBudgetForecastMaterial",
                column: "OrderMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestBudgetForecastMaterial_RequestBudgetForecastId",
                table: "RequestBudgetForecastMaterial",
                column: "RequestBudgetForecastId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_RequestBudgetForecastMaterial_ReqBFMaterialId",
                table: "Asset",
                column: "ReqBFMaterialId",
                principalTable: "RequestBudgetForecastMaterial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderMaterial_BudgetForecast_BudgetForecastId",
                table: "OrderMaterial",
                column: "BudgetForecastId",
                principalTable: "BudgetForecast",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_RequestBudgetForecastMaterial_ReqBFMaterialId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderMaterial_BudgetForecast_BudgetForecastId",
                table: "OrderMaterial");

            migrationBuilder.DropTable(
                name: "RequestBudgetForecastMaterial");

            migrationBuilder.DropTable(
                name: "RequestBudgetForecast");

            migrationBuilder.DropIndex(
                name: "IX_OrderMaterial_BudgetForecastId",
                table: "OrderMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Asset_ReqBFMaterialId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "BudgetForecastId",
                table: "OrderMaterial");

            migrationBuilder.DropColumn(
                name: "ReqBFMaterialId",
                table: "Asset");
        }
    }
}
