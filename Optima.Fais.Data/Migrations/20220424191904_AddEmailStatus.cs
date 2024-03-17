using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmailStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppStateId = table.Column<int>(nullable: true),
                    AssetId = table.Column<int>(nullable: true),
                    AssetOpId = table.Column<int>(nullable: true),
                    BudgetBaseId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    CostCenterIdFinal = table.Column<int>(nullable: true),
                    CostCenterIdInitial = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DocumentNumber = table.Column<int>(nullable: false),
                    DstEmployeeEmailSend = table.Column<bool>(nullable: false),
                    DstEmployeeValidateAt = table.Column<DateTime>(nullable: true),
                    DstEmployeeValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    DstManagerEmailSend = table.Column<bool>(nullable: false),
                    DstManagerValidateAt = table.Column<DateTime>(nullable: true),
                    DstManagerValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmailSend = table.Column<bool>(nullable: false),
                    EmailTypeId = table.Column<int>(nullable: false),
                    EmployeeIdFinal = table.Column<int>(nullable: true),
                    EmployeeIdInitial = table.Column<int>(nullable: true),
                    ErrorId = table.Column<int>(nullable: true),
                    Exported = table.Column<bool>(nullable: false),
                    FinalValidateAt = table.Column<DateTime>(nullable: true),
                    FinalValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    GuidAll = table.Column<Guid>(nullable: false),
                    Info = table.Column<string>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotCompletedSync = table.Column<bool>(nullable: false),
                    NotDstEmployeeSync = table.Column<bool>(nullable: false),
                    NotDstManagerSync = table.Column<bool>(nullable: false),
                    NotSrcEmployeeSync = table.Column<bool>(nullable: false),
                    NotSrcManagerSync = table.Column<bool>(nullable: false),
                    NotSync = table.Column<bool>(nullable: false),
                    OfferId = table.Column<int>(nullable: true),
                    OrderId = table.Column<int>(nullable: true),
                    PartnerId = table.Column<int>(nullable: true),
                    RequestId = table.Column<int>(nullable: true),
                    SameEmployee = table.Column<bool>(nullable: false),
                    SameManager = table.Column<bool>(nullable: false),
                    Skip = table.Column<bool>(nullable: false),
                    SkipDstEmployee = table.Column<bool>(nullable: false),
                    SkipDstManager = table.Column<bool>(nullable: false),
                    SkipSrcEmployee = table.Column<bool>(nullable: false),
                    SkipSrcManager = table.Column<bool>(nullable: false),
                    SrcEmployeeEmailSend = table.Column<bool>(nullable: false),
                    SrcEmployeeValidateAt = table.Column<DateTime>(nullable: true),
                    SrcEmployeeValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    SrcManagerEmailSend = table.Column<bool>(nullable: false),
                    SrcManagerValidateAt = table.Column<DateTime>(nullable: true),
                    SrcManagerValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    StockId = table.Column<int>(nullable: true),
                    SyncCompletedErrorCount = table.Column<int>(nullable: false),
                    SyncDstEmployeeErrorCount = table.Column<int>(nullable: false),
                    SyncDstManagerErrorCount = table.Column<int>(nullable: false),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    SyncSrcEmployeeErrorCount = table.Column<int>(nullable: false),
                    SyncSrcManagerErrorCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailStatus_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_AssetOp_AssetOpId",
                        column: x => x.AssetOpId,
                        principalTable: "AssetOp",
                        principalColumn: "AssetOpId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_BudgetBase_BudgetBaseId",
                        column: x => x.BudgetBaseId,
                        principalTable: "BudgetBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_CostCenter_CostCenterIdFinal",
                        column: x => x.CostCenterIdFinal,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_CostCenter_CostCenterIdInitial",
                        column: x => x.CostCenterIdInitial,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_AspNetUsers_DstEmployeeValidateBy",
                        column: x => x.DstEmployeeValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_AspNetUsers_DstManagerValidateBy",
                        column: x => x.DstManagerValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_EmailType_EmailTypeId",
                        column: x => x.EmailTypeId,
                        principalTable: "EmailType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Error_ErrorId",
                        column: x => x.ErrorId,
                        principalTable: "Error",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_AspNetUsers_FinalValidateBy",
                        column: x => x.FinalValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_AspNetUsers_SrcEmployeeValidateBy",
                        column: x => x.SrcEmployeeValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_AspNetUsers_SrcManagerValidateBy",
                        column: x => x.SrcManagerValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailStatus_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_AppStateId",
                table: "EmailStatus",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_AssetId",
                table: "EmailStatus",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_AssetOpId",
                table: "EmailStatus",
                column: "AssetOpId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_BudgetBaseId",
                table: "EmailStatus",
                column: "BudgetBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_CompanyId",
                table: "EmailStatus",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_CostCenterIdFinal",
                table: "EmailStatus",
                column: "CostCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_CostCenterIdInitial",
                table: "EmailStatus",
                column: "CostCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_DstEmployeeValidateBy",
                table: "EmailStatus",
                column: "DstEmployeeValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_DstManagerValidateBy",
                table: "EmailStatus",
                column: "DstManagerValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_EmailTypeId",
                table: "EmailStatus",
                column: "EmailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_EmployeeIdFinal",
                table: "EmailStatus",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_EmployeeIdInitial",
                table: "EmailStatus",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_ErrorId",
                table: "EmailStatus",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_FinalValidateBy",
                table: "EmailStatus",
                column: "FinalValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_OfferId",
                table: "EmailStatus",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_OrderId",
                table: "EmailStatus",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_PartnerId",
                table: "EmailStatus",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_RequestId",
                table: "EmailStatus",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_SrcEmployeeValidateBy",
                table: "EmailStatus",
                column: "SrcEmployeeValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_SrcManagerValidateBy",
                table: "EmailStatus",
                column: "SrcManagerValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailStatus_StockId",
                table: "EmailStatus",
                column: "StockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailStatus");
        }
    }
}
