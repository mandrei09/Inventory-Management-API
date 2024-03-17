using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "OfferMaterial",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AppStateId = table.Column<int>(nullable: true),
                    BudgetId = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 120, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Info = table.Column<string>(maxLength: 450, nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    Validated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Request_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestOp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccSystemId = table.Column<int>(nullable: true),
                    BudgetIdFinal = table.Column<int>(nullable: true),
                    BudgetIdInitial = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
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
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ProjectIdFinal = table.Column<int>(nullable: true),
                    ProjectIdInitial = table.Column<int>(nullable: true),
                    RegisterConfAt = table.Column<DateTime>(nullable: true),
                    RegisterConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    ReleaseConfAt = table.Column<DateTime>(nullable: true),
                    ReleaseConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    RequestId = table.Column<int>(nullable: false),
                    RequestStateId = table.Column<int>(nullable: true),
                    SrcConfAt = table.Column<DateTime>(nullable: true),
                    SrcConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    Validated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestOp_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Budget_BudgetIdFinal",
                        column: x => x.BudgetIdFinal,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Budget_BudgetIdInitial",
                        column: x => x.BudgetIdInitial,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_CostCenter_CostCenterIdFinal",
                        column: x => x.CostCenterIdFinal,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_CostCenter_CostCenterIdInitial",
                        column: x => x.CostCenterIdInitial,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestOp_AspNetUsers_DstConfBy",
                        column: x => x.DstConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Project_ProjectIdFinal",
                        column: x => x.ProjectIdFinal,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Project_ProjectIdInitial",
                        column: x => x.ProjectIdInitial,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_AspNetUsers_RegisterConfBy",
                        column: x => x.RegisterConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_AspNetUsers_ReleaseConfBy",
                        column: x => x.ReleaseConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestOp_AppState_RequestStateId",
                        column: x => x.RequestStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestOp_AspNetUsers_SrcConfBy",
                        column: x => x.SrcConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_RequestId",
                table: "OfferOp",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferMaterial_RequestId",
                table: "OfferMaterial",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_RequestId",
                table: "Offer",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_AccMonthId",
                table: "Request",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_AppStateId",
                table: "Request",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_BudgetId",
                table: "Request",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_BudgetManagerId",
                table: "Request",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_CompanyId",
                table: "Request",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_CostCenterId",
                table: "Request",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_EmployeeId",
                table: "Request",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProjectId",
                table: "Request",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_UserId",
                table: "Request",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_AccMonthId",
                table: "RequestOp",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_AccSystemId",
                table: "RequestOp",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_BudgetIdFinal",
                table: "RequestOp",
                column: "BudgetIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_BudgetIdInitial",
                table: "RequestOp",
                column: "BudgetIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_BudgetManagerId",
                table: "RequestOp",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_CompanyId",
                table: "RequestOp",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_CostCenterIdFinal",
                table: "RequestOp",
                column: "CostCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_CostCenterIdInitial",
                table: "RequestOp",
                column: "CostCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_DocumentId",
                table: "RequestOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_DstConfBy",
                table: "RequestOp",
                column: "DstConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_EmployeeIdFinal",
                table: "RequestOp",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_EmployeeIdInitial",
                table: "RequestOp",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_ProjectIdFinal",
                table: "RequestOp",
                column: "ProjectIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_ProjectIdInitial",
                table: "RequestOp",
                column: "ProjectIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_RegisterConfBy",
                table: "RequestOp",
                column: "RegisterConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_ReleaseConfBy",
                table: "RequestOp",
                column: "ReleaseConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_RequestId",
                table: "RequestOp",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_RequestStateId",
                table: "RequestOp",
                column: "RequestStateId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestOp_SrcConfBy",
                table: "RequestOp",
                column: "SrcConfBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Request_RequestId",
                table: "Offer",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferMaterial_Request_RequestId",
                table: "OfferMaterial",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_Request_RequestId",
                table: "OfferOp",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Request_RequestId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferMaterial_Request_RequestId",
                table: "OfferMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_Request_RequestId",
                table: "OfferOp");

            migrationBuilder.DropTable(
                name: "RequestOp");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_RequestId",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferMaterial_RequestId",
                table: "OfferMaterial");

            migrationBuilder.DropIndex(
                name: "IX_Offer_RequestId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "OfferMaterial");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Offer");
        }
    }
}
