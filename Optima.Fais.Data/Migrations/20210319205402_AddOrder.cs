using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubTypeId",
                table: "Partner",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UomId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Asset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccountId = table.Column<int>(nullable: true),
                    AdministrationId = table.Column<int>(nullable: true),
                    AppStateId = table.Column<int>(nullable: true),
                    BudgetId = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    Info = table.Column<string>(nullable: true),
                    InterCompanyId = table.Column<int>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PartnerId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Quantity = table.Column<float>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    SubTypeId = table.Column<int>(nullable: true),
                    UomId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    ValueFin = table.Column<decimal>(nullable: false),
                    ValueIni = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offer_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_InterCompany_InterCompanyId",
                        column: x => x.InterCompanyId,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_SubType_SubTypeId",
                        column: x => x.SubTypeId,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccountId = table.Column<int>(nullable: true),
                    AdministrationId = table.Column<int>(nullable: true),
                    AppStateId = table.Column<int>(nullable: true),
                    BudgetId = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    Info = table.Column<string>(nullable: true),
                    InterCompanyId = table.Column<int>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OfferId = table.Column<int>(nullable: true),
                    PartnerId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Quantity = table.Column<float>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    SubTypeId = table.Column<int>(nullable: true),
                    UomId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    ValueFin = table.Column<decimal>(nullable: false),
                    ValueIni = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_InterCompany_InterCompanyId",
                        column: x => x.InterCompanyId,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_SubType_SubTypeId",
                        column: x => x.SubTypeId,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partner_SubTypeId",
                table: "Partner",
                column: "SubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_UomId",
                table: "Budget",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_BudgetId",
                table: "Asset",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OrderId",
                table: "Asset",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_AccMonthId",
                table: "Offer",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_AccountId",
                table: "Offer",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_AdministrationId",
                table: "Offer",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_AppStateId",
                table: "Offer",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_BudgetId",
                table: "Offer",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_BudgetManagerId",
                table: "Offer",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CompanyId",
                table: "Offer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CostCenterId",
                table: "Offer",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_EmployeeId",
                table: "Offer",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_InterCompanyId",
                table: "Offer",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_PartnerId",
                table: "Offer",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_ProjectId",
                table: "Offer",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_SubTypeId",
                table: "Offer",
                column: "SubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_UomId",
                table: "Offer",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_UserId",
                table: "Offer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AccMonthId",
                table: "Order",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AccountId",
                table: "Order",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AdministrationId",
                table: "Order",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AppStateId",
                table: "Order",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BudgetId",
                table: "Order",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BudgetManagerId",
                table: "Order",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CompanyId",
                table: "Order",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CostCenterId",
                table: "Order",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeId",
                table: "Order",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_InterCompanyId",
                table: "Order",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OfferId",
                table: "Order",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PartnerId",
                table: "Order",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_ProjectId",
                table: "Order",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_SubTypeId",
                table: "Order",
                column: "SubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UomId",
                table: "Order",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Budget_BudgetId",
                table: "Asset",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Order_OrderId",
                table: "Asset",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Uom_UomId",
                table: "Budget",
                column: "UomId",
                principalTable: "Uom",
                principalColumn: "UomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_SubType_SubTypeId",
                table: "Partner",
                column: "SubTypeId",
                principalTable: "SubType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Budget_BudgetId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Order_OrderId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Uom_UomId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Partner_SubType_SubTypeId",
                table: "Partner");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Partner_SubTypeId",
                table: "Partner");

            migrationBuilder.DropIndex(
                name: "IX_Budget_UomId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Asset_BudgetId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_OrderId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "SubTypeId",
                table: "Partner");

            migrationBuilder.DropColumn(
                name: "UomId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Asset");
        }
    }
}
