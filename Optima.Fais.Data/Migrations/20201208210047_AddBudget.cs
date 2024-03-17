using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccountId = table.Column<int>(nullable: true),
                    AdministrationId = table.Column<int>(nullable: true),
                    AppStateId = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 120, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Info = table.Column<string>(maxLength: 120, nullable: true),
                    InterCompanyId = table.Column<int>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 120, nullable: true),
                    PartnerId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Quantity = table.Column<float>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    SubTypeId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    ValueFin = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueIni = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budget_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_InterCompany_InterCompanyId",
                        column: x => x.InterCompanyId,
                        principalTable: "InterCompany",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_SubType_SubTypeId",
                        column: x => x.SubTypeId,
                        principalTable: "SubType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budget_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AccMonthId",
                table: "Budget",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AccountId",
                table: "Budget",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AdministrationId",
                table: "Budget",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AppStateId",
                table: "Budget",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_BudgetManagerId",
                table: "Budget",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_CompanyId",
                table: "Budget",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_CostCenterId",
                table: "Budget",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_EmployeeId",
                table: "Budget",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_InterCompanyId",
                table: "Budget",
                column: "InterCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_PartnerId",
                table: "Budget",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_ProjectId",
                table: "Budget",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_SubTypeId",
                table: "Budget",
                column: "SubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_UserId",
                table: "Budget",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budget");
        }
    }
}
