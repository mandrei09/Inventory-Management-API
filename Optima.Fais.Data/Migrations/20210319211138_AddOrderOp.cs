using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOrderOp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfferOp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccSystemId = table.Column<int>(nullable: true),
                    AccountIdFinal = table.Column<int>(nullable: true),
                    AccountIdInitial = table.Column<int>(nullable: true),
                    AdministrationIdFinal = table.Column<int>(nullable: true),
                    AdministrationIdInitial = table.Column<int>(nullable: true),
                    BudgetFinalId = table.Column<int>(nullable: true),
                    BudgetIdFinal = table.Column<int>(nullable: true),
                    BudgetIdInitial = table.Column<int>(nullable: true),
                    BudgetInitialId = table.Column<int>(nullable: true),
                    BudgetManagerIdFinal = table.Column<int>(nullable: true),
                    BudgetManagerIdInitial = table.Column<int>(nullable: true),
                    BudgetStateId = table.Column<int>(nullable: true),
                    CompanyIdFinal = table.Column<int>(nullable: true),
                    CompanyIdInitial = table.Column<int>(nullable: true),
                    CostCenterIdFinal = table.Column<int>(nullable: true),
                    CostCenterIdInitial = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DocumentId = table.Column<int>(nullable: false),
                    DstConfAt = table.Column<DateTime>(nullable: true),
                    DstConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeIdFinal = table.Column<int>(nullable: true),
                    EmployeeIdInitial = table.Column<int>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    InfoFin = table.Column<string>(maxLength: 450, nullable: true),
                    InfoIni = table.Column<string>(maxLength: 450, nullable: true),
                    InterCompanyIdFinal = table.Column<int>(nullable: true),
                    InterCompanyIdInitial = table.Column<int>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    OfferId = table.Column<int>(nullable: false),
                    PartnerIdFinal = table.Column<int>(nullable: true),
                    PartnerIdInitial = table.Column<int>(nullable: true),
                    ProjectIdFinal = table.Column<int>(nullable: true),
                    ProjectIdInitial = table.Column<int>(nullable: true),
                    QuantityFin = table.Column<float>(nullable: false),
                    QuantityIni = table.Column<float>(nullable: false),
                    RegisterConfAt = table.Column<DateTime>(nullable: true),
                    RegisterConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    ReleaseConfAt = table.Column<DateTime>(nullable: true),
                    ReleaseConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    SrcConfAt = table.Column<DateTime>(nullable: true),
                    SrcConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    SubTypeIdFinal = table.Column<int>(nullable: true),
                    SubTypeIdInitial = table.Column<int>(nullable: true),
                    UomId = table.Column<int>(nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    ValueFin1 = table.Column<decimal>(nullable: false),
                    ValueFin2 = table.Column<decimal>(nullable: false),
                    ValueIni1 = table.Column<decimal>(nullable: false),
                    ValueIni2 = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferOp_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Account_AccountIdFinal",
                        column: x => x.AccountIdFinal,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Account_AccountIdInitial",
                        column: x => x.AccountIdInitial,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Administration_AdministrationIdFinal",
                        column: x => x.AdministrationIdFinal,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Administration_AdministrationIdInitial",
                        column: x => x.AdministrationIdInitial,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Budget_BudgetFinalId",
                        column: x => x.BudgetFinalId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Budget_BudgetInitialId",
                        column: x => x.BudgetInitialId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_BudgetManager_BudgetManagerIdFinal",
                        column: x => x.BudgetManagerIdFinal,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_BudgetManager_BudgetManagerIdInitial",
                        column: x => x.BudgetManagerIdInitial,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_AppState_BudgetStateId",
                        column: x => x.BudgetStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Company_CompanyIdFinal",
                        column: x => x.CompanyIdFinal,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Company_CompanyIdInitial",
                        column: x => x.CompanyIdInitial,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_CostCenter_CostCenterIdFinal",
                        column: x => x.CostCenterIdFinal,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_CostCenter_CostCenterIdInitial",
                        column: x => x.CostCenterIdInitial,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferOp_AspNetUsers_DstConfBy",
                        column: x => x.DstConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_InterCompany_InterCompanyIdFinal",
                        column: x => x.InterCompanyIdFinal,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_InterCompany_InterCompanyIdInitial",
                        column: x => x.InterCompanyIdInitial,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferOp_Partner_PartnerIdFinal",
                        column: x => x.PartnerIdFinal,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Partner_PartnerIdInitial",
                        column: x => x.PartnerIdInitial,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Project_ProjectIdFinal",
                        column: x => x.ProjectIdFinal,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Project_ProjectIdInitial",
                        column: x => x.ProjectIdInitial,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_AspNetUsers_RegisterConfBy",
                        column: x => x.RegisterConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_AspNetUsers_ReleaseConfBy",
                        column: x => x.ReleaseConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_AspNetUsers_SrcConfBy",
                        column: x => x.SrcConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_SubType_SubTypeIdFinal",
                        column: x => x.SubTypeIdFinal,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_SubType_SubTypeIdInitial",
                        column: x => x.SubTypeIdInitial,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferOp_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderOp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccSystemId = table.Column<int>(nullable: true),
                    AccountIdFinal = table.Column<int>(nullable: true),
                    AccountIdInitial = table.Column<int>(nullable: true),
                    AdministrationIdFinal = table.Column<int>(nullable: true),
                    AdministrationIdInitial = table.Column<int>(nullable: true),
                    BudgetFinalId = table.Column<int>(nullable: true),
                    BudgetIdFinal = table.Column<int>(nullable: true),
                    BudgetIdInitial = table.Column<int>(nullable: true),
                    BudgetInitialId = table.Column<int>(nullable: true),
                    BudgetManagerIdFinal = table.Column<int>(nullable: true),
                    BudgetManagerIdInitial = table.Column<int>(nullable: true),
                    BudgetStateId = table.Column<int>(nullable: true),
                    CompanyIdFinal = table.Column<int>(nullable: true),
                    CompanyIdInitial = table.Column<int>(nullable: true),
                    CostCenterIdFinal = table.Column<int>(nullable: true),
                    CostCenterIdInitial = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DocumentId = table.Column<int>(nullable: false),
                    DstConfAt = table.Column<DateTime>(nullable: true),
                    DstConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeIdFinal = table.Column<int>(nullable: true),
                    EmployeeIdInitial = table.Column<int>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    InfoFin = table.Column<string>(maxLength: 450, nullable: true),
                    InfoIni = table.Column<string>(maxLength: 450, nullable: true),
                    InterCompanyIdFinal = table.Column<int>(nullable: true),
                    InterCompanyIdInitial = table.Column<int>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    OfferFinalId = table.Column<int>(nullable: true),
                    OfferIdFinal = table.Column<int>(nullable: true),
                    OfferIdInitial = table.Column<int>(nullable: true),
                    OfferInitialId = table.Column<int>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    PartnerIdFinal = table.Column<int>(nullable: true),
                    PartnerIdInitial = table.Column<int>(nullable: true),
                    ProjectIdFinal = table.Column<int>(nullable: true),
                    ProjectIdInitial = table.Column<int>(nullable: true),
                    QuantityFin = table.Column<float>(nullable: false),
                    QuantityIni = table.Column<float>(nullable: false),
                    RegisterConfAt = table.Column<DateTime>(nullable: true),
                    RegisterConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    ReleaseConfAt = table.Column<DateTime>(nullable: true),
                    ReleaseConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    SrcConfAt = table.Column<DateTime>(nullable: true),
                    SrcConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    SubTypeIdFinal = table.Column<int>(nullable: true),
                    SubTypeIdInitial = table.Column<int>(nullable: true),
                    UomId = table.Column<int>(nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    ValueFin1 = table.Column<decimal>(nullable: false),
                    ValueFin2 = table.Column<decimal>(nullable: false),
                    ValueIni1 = table.Column<decimal>(nullable: false),
                    ValueIni2 = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderOp_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Account_AccountIdFinal",
                        column: x => x.AccountIdFinal,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Account_AccountIdInitial",
                        column: x => x.AccountIdInitial,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Administration_AdministrationIdFinal",
                        column: x => x.AdministrationIdFinal,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Administration_AdministrationIdInitial",
                        column: x => x.AdministrationIdInitial,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Budget_BudgetFinalId",
                        column: x => x.BudgetFinalId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Budget_BudgetInitialId",
                        column: x => x.BudgetInitialId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_BudgetManager_BudgetManagerIdFinal",
                        column: x => x.BudgetManagerIdFinal,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_BudgetManager_BudgetManagerIdInitial",
                        column: x => x.BudgetManagerIdInitial,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_AppState_BudgetStateId",
                        column: x => x.BudgetStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Company_CompanyIdFinal",
                        column: x => x.CompanyIdFinal,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Company_CompanyIdInitial",
                        column: x => x.CompanyIdInitial,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_CostCenter_CostCenterIdFinal",
                        column: x => x.CostCenterIdFinal,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_CostCenter_CostCenterIdInitial",
                        column: x => x.CostCenterIdInitial,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderOp_AspNetUsers_DstConfBy",
                        column: x => x.DstConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_InterCompany_InterCompanyIdFinal",
                        column: x => x.InterCompanyIdFinal,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_InterCompany_InterCompanyIdInitial",
                        column: x => x.InterCompanyIdInitial,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Offer_OfferFinalId",
                        column: x => x.OfferFinalId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Offer_OfferInitialId",
                        column: x => x.OfferInitialId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderOp_Partner_PartnerIdFinal",
                        column: x => x.PartnerIdFinal,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Partner_PartnerIdInitial",
                        column: x => x.PartnerIdInitial,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Project_ProjectIdFinal",
                        column: x => x.ProjectIdFinal,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Project_ProjectIdInitial",
                        column: x => x.ProjectIdInitial,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_AspNetUsers_RegisterConfBy",
                        column: x => x.RegisterConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_AspNetUsers_ReleaseConfBy",
                        column: x => x.ReleaseConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_AspNetUsers_SrcConfBy",
                        column: x => x.SrcConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_SubType_SubTypeIdFinal",
                        column: x => x.SubTypeIdFinal,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_SubType_SubTypeIdInitial",
                        column: x => x.SubTypeIdInitial,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOp_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AccMonthId",
                table: "OfferOp",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AccSystemId",
                table: "OfferOp",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AccountIdFinal",
                table: "OfferOp",
                column: "AccountIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AccountIdInitial",
                table: "OfferOp",
                column: "AccountIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AdministrationIdFinal",
                table: "OfferOp",
                column: "AdministrationIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AdministrationIdInitial",
                table: "OfferOp",
                column: "AdministrationIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_BudgetFinalId",
                table: "OfferOp",
                column: "BudgetFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_BudgetInitialId",
                table: "OfferOp",
                column: "BudgetInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_BudgetManagerIdFinal",
                table: "OfferOp",
                column: "BudgetManagerIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_BudgetManagerIdInitial",
                table: "OfferOp",
                column: "BudgetManagerIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_BudgetStateId",
                table: "OfferOp",
                column: "BudgetStateId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_CompanyIdFinal",
                table: "OfferOp",
                column: "CompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_CompanyIdInitial",
                table: "OfferOp",
                column: "CompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_CostCenterIdFinal",
                table: "OfferOp",
                column: "CostCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_CostCenterIdInitial",
                table: "OfferOp",
                column: "CostCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_DocumentId",
                table: "OfferOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_DstConfBy",
                table: "OfferOp",
                column: "DstConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_EmployeeIdFinal",
                table: "OfferOp",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_EmployeeIdInitial",
                table: "OfferOp",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_InterCompanyIdFinal",
                table: "OfferOp",
                column: "InterCompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_InterCompanyIdInitial",
                table: "OfferOp",
                column: "InterCompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_OfferId",
                table: "OfferOp",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_PartnerIdFinal",
                table: "OfferOp",
                column: "PartnerIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_PartnerIdInitial",
                table: "OfferOp",
                column: "PartnerIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_ProjectIdFinal",
                table: "OfferOp",
                column: "ProjectIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_ProjectIdInitial",
                table: "OfferOp",
                column: "ProjectIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_RegisterConfBy",
                table: "OfferOp",
                column: "RegisterConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_ReleaseConfBy",
                table: "OfferOp",
                column: "ReleaseConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_SrcConfBy",
                table: "OfferOp",
                column: "SrcConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_SubTypeIdFinal",
                table: "OfferOp",
                column: "SubTypeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_SubTypeIdInitial",
                table: "OfferOp",
                column: "SubTypeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_UomId",
                table: "OfferOp",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AccMonthId",
                table: "OrderOp",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AccSystemId",
                table: "OrderOp",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AccountIdFinal",
                table: "OrderOp",
                column: "AccountIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AccountIdInitial",
                table: "OrderOp",
                column: "AccountIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AdministrationIdFinal",
                table: "OrderOp",
                column: "AdministrationIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_AdministrationIdInitial",
                table: "OrderOp",
                column: "AdministrationIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_BudgetFinalId",
                table: "OrderOp",
                column: "BudgetFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_BudgetInitialId",
                table: "OrderOp",
                column: "BudgetInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_BudgetManagerIdFinal",
                table: "OrderOp",
                column: "BudgetManagerIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_BudgetManagerIdInitial",
                table: "OrderOp",
                column: "BudgetManagerIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_BudgetStateId",
                table: "OrderOp",
                column: "BudgetStateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_CompanyIdFinal",
                table: "OrderOp",
                column: "CompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_CompanyIdInitial",
                table: "OrderOp",
                column: "CompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_CostCenterIdFinal",
                table: "OrderOp",
                column: "CostCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_CostCenterIdInitial",
                table: "OrderOp",
                column: "CostCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_DocumentId",
                table: "OrderOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_DstConfBy",
                table: "OrderOp",
                column: "DstConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_EmployeeIdFinal",
                table: "OrderOp",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_EmployeeIdInitial",
                table: "OrderOp",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_InterCompanyIdFinal",
                table: "OrderOp",
                column: "InterCompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_InterCompanyIdInitial",
                table: "OrderOp",
                column: "InterCompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_OfferFinalId",
                table: "OrderOp",
                column: "OfferFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_OfferInitialId",
                table: "OrderOp",
                column: "OfferInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_OrderId",
                table: "OrderOp",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_PartnerIdFinal",
                table: "OrderOp",
                column: "PartnerIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_PartnerIdInitial",
                table: "OrderOp",
                column: "PartnerIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_ProjectIdFinal",
                table: "OrderOp",
                column: "ProjectIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_ProjectIdInitial",
                table: "OrderOp",
                column: "ProjectIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_RegisterConfBy",
                table: "OrderOp",
                column: "RegisterConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_ReleaseConfBy",
                table: "OrderOp",
                column: "ReleaseConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_SrcConfBy",
                table: "OrderOp",
                column: "SrcConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_SubTypeIdFinal",
                table: "OrderOp",
                column: "SubTypeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_SubTypeIdInitial",
                table: "OrderOp",
                column: "SubTypeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOp_UomId",
                table: "OrderOp",
                column: "UomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferOp");

            migrationBuilder.DropTable(
                name: "OrderOp");
        }
    }
}
