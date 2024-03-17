using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetOp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetOp",
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
                    BudgetId = table.Column<int>(nullable: false),
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
                    InfoFin = table.Column<string>(maxLength: 450, nullable: true),
                    InfoIni = table.Column<string>(maxLength: 450, nullable: true),
                    InterCompanyIdFinal = table.Column<int>(nullable: true),
                    InterCompanyIdInitial = table.Column<int>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
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
                    Validated = table.Column<bool>(nullable: false),
                    ValueFin1 = table.Column<decimal>(nullable: false),
                    ValueFin2 = table.Column<decimal>(nullable: false),
                    ValueIni1 = table.Column<decimal>(nullable: false),
                    ValueIni2 = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetOp_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Account_AccountIdFinal",
                        column: x => x.AccountIdFinal,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Account_AccountIdInitial",
                        column: x => x.AccountIdInitial,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Administration_AdministrationIdFinal",
                        column: x => x.AdministrationIdFinal,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Administration_AdministrationIdInitial",
                        column: x => x.AdministrationIdInitial,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetOp_BudgetManager_BudgetManagerIdFinal",
                        column: x => x.BudgetManagerIdFinal,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_BudgetManager_BudgetManagerIdInitial",
                        column: x => x.BudgetManagerIdInitial,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_AppState_BudgetStateId",
                        column: x => x.BudgetStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Company_CompanyIdFinal",
                        column: x => x.CompanyIdFinal,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Company_CompanyIdInitial",
                        column: x => x.CompanyIdInitial,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_CostCenter_CostCenterIdFinal",
                        column: x => x.CostCenterIdFinal,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_CostCenter_CostCenterIdInitial",
                        column: x => x.CostCenterIdInitial,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetOp_AspNetUsers_DstConfBy",
                        column: x => x.DstConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_InterCompany_InterCompanyIdFinal",
                        column: x => x.InterCompanyIdFinal,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_InterCompany_InterCompanyIdInitial",
                        column: x => x.InterCompanyIdInitial,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Partner_PartnerIdFinal",
                        column: x => x.PartnerIdFinal,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Partner_PartnerIdInitial",
                        column: x => x.PartnerIdInitial,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Project_ProjectIdFinal",
                        column: x => x.ProjectIdFinal,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_Project_ProjectIdInitial",
                        column: x => x.ProjectIdInitial,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_AspNetUsers_RegisterConfBy",
                        column: x => x.RegisterConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_AspNetUsers_ReleaseConfBy",
                        column: x => x.ReleaseConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_AspNetUsers_SrcConfBy",
                        column: x => x.SrcConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_SubType_SubTypeIdFinal",
                        column: x => x.SubTypeIdFinal,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetOp_SubType_SubTypeIdInitial",
                        column: x => x.SubTypeIdInitial,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AccMonthId",
                table: "BudgetOp",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AccSystemId",
                table: "BudgetOp",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AccountIdFinal",
                table: "BudgetOp",
                column: "AccountIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AccountIdInitial",
                table: "BudgetOp",
                column: "AccountIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AdministrationIdFinal",
                table: "BudgetOp",
                column: "AdministrationIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AdministrationIdInitial",
                table: "BudgetOp",
                column: "AdministrationIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_BudgetId",
                table: "BudgetOp",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_BudgetManagerIdFinal",
                table: "BudgetOp",
                column: "BudgetManagerIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_BudgetManagerIdInitial",
                table: "BudgetOp",
                column: "BudgetManagerIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_BudgetStateId",
                table: "BudgetOp",
                column: "BudgetStateId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_CompanyIdFinal",
                table: "BudgetOp",
                column: "CompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_CompanyIdInitial",
                table: "BudgetOp",
                column: "CompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_CostCenterIdFinal",
                table: "BudgetOp",
                column: "CostCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_CostCenterIdInitial",
                table: "BudgetOp",
                column: "CostCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_DocumentId",
                table: "BudgetOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_DstConfBy",
                table: "BudgetOp",
                column: "DstConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_EmployeeIdFinal",
                table: "BudgetOp",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_EmployeeIdInitial",
                table: "BudgetOp",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_InterCompanyIdFinal",
                table: "BudgetOp",
                column: "InterCompanyIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_InterCompanyIdInitial",
                table: "BudgetOp",
                column: "InterCompanyIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_PartnerIdFinal",
                table: "BudgetOp",
                column: "PartnerIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_PartnerIdInitial",
                table: "BudgetOp",
                column: "PartnerIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_ProjectIdFinal",
                table: "BudgetOp",
                column: "ProjectIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_ProjectIdInitial",
                table: "BudgetOp",
                column: "ProjectIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_RegisterConfBy",
                table: "BudgetOp",
                column: "RegisterConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_ReleaseConfBy",
                table: "BudgetOp",
                column: "ReleaseConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_SrcConfBy",
                table: "BudgetOp",
                column: "SrcConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_SubTypeIdFinal",
                table: "BudgetOp",
                column: "SubTypeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_SubTypeIdInitial",
                table: "BudgetOp",
                column: "SubTypeIdInitial");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetOp");
        }
    }
}
