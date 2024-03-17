using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetBase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    ActivityId = table.Column<int>(nullable: true),
                    AdmCenterId = table.Column<int>(nullable: true),
                    AppStateId = table.Column<int>(nullable: true),
                    AssetTypeId = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: true),
                    BudgetTypeId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 120, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CountryId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DepPeriod = table.Column<decimal>(nullable: false),
                    DepPeriodRem = table.Column<decimal>(nullable: false),
                    DepartmentId = table.Column<int>(nullable: true),
                    DivisionId = table.Column<int>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    Info = table.Column<string>(maxLength: 120, nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsFirst = table.Column<bool>(nullable: false),
                    IsLast = table.Column<bool>(nullable: false),
                    LastYearValue = table.Column<decimal>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 120, nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    ProjectTypeId = table.Column<int>(nullable: true),
                    RegionId = table.Column<int>(nullable: true),
                    StartMonthId = table.Column<int>(nullable: true),
                    Total = table.Column<decimal>(nullable: false),
                    UomId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    ValueAsset = table.Column<decimal>(nullable: false),
                    ValueFin = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueIni = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueOrder = table.Column<decimal>(nullable: false),
                    ValueRem = table.Column<decimal>(nullable: false),
                    ValueUsed = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetBase_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Activity_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activity",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_AdmCenter_AdmCenterId",
                        column: x => x.AdmCenterId,
                        principalTable: "AdmCenter",
                        principalColumn: "AdmCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_AssetType_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetType",
                        principalColumn: "AssetTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_ProjectType_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalTable: "ProjectType",
                        principalColumn: "ProjectTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_AccMonth_StartMonthId",
                        column: x => x.StartMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBase_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetForecast",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    April = table.Column<decimal>(nullable: false),
                    August = table.Column<decimal>(nullable: false),
                    BudgetBaseId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    BudgetTypeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    December = table.Column<decimal>(nullable: false),
                    February = table.Column<decimal>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsFirst = table.Column<bool>(nullable: false),
                    IsLast = table.Column<bool>(nullable: false),
                    January = table.Column<decimal>(nullable: false),
                    July = table.Column<decimal>(nullable: false),
                    June = table.Column<decimal>(nullable: false),
                    March = table.Column<decimal>(nullable: false),
                    May = table.Column<decimal>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    November = table.Column<decimal>(nullable: false),
                    Octomber = table.Column<decimal>(nullable: false),
                    September = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetForecast", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetForecast_BudgetBase_BudgetBaseId",
                        column: x => x.BudgetBaseId,
                        principalTable: "BudgetBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetForecast_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetForecast_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetMonthBase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    April = table.Column<decimal>(nullable: false),
                    August = table.Column<decimal>(nullable: false),
                    BudgetBaseId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    BudgetTypeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    December = table.Column<decimal>(nullable: false),
                    February = table.Column<decimal>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsFirst = table.Column<bool>(nullable: false),
                    IsLast = table.Column<bool>(nullable: false),
                    January = table.Column<decimal>(nullable: false),
                    July = table.Column<decimal>(nullable: false),
                    June = table.Column<decimal>(nullable: false),
                    March = table.Column<decimal>(nullable: false),
                    May = table.Column<decimal>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    November = table.Column<decimal>(nullable: false),
                    Octomber = table.Column<decimal>(nullable: false),
                    September = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetMonthBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetMonthBase_BudgetBase_BudgetBaseId",
                        column: x => x.BudgetBaseId,
                        principalTable: "BudgetBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetMonthBase_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetMonthBase_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_AccMonthId",
                table: "BudgetBase",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_ActivityId",
                table: "BudgetBase",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_AdmCenterId",
                table: "BudgetBase",
                column: "AdmCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_AppStateId",
                table: "BudgetBase",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_AssetTypeId",
                table: "BudgetBase",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_BudgetManagerId",
                table: "BudgetBase",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_BudgetTypeId",
                table: "BudgetBase",
                column: "BudgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_CompanyId",
                table: "BudgetBase",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_CostCenterId",
                table: "BudgetBase",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_CountryId",
                table: "BudgetBase",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_DepartmentId",
                table: "BudgetBase",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_DivisionId",
                table: "BudgetBase",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_EmployeeId",
                table: "BudgetBase",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_ProjectId",
                table: "BudgetBase",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_ProjectTypeId",
                table: "BudgetBase",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_RegionId",
                table: "BudgetBase",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_StartMonthId",
                table: "BudgetBase",
                column: "StartMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_UomId",
                table: "BudgetBase",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBase_UserId",
                table: "BudgetBase",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetForecast_BudgetBaseId",
                table: "BudgetForecast",
                column: "BudgetBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetForecast_BudgetManagerId",
                table: "BudgetForecast",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetForecast_BudgetTypeId",
                table: "BudgetForecast",
                column: "BudgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonthBase_BudgetBaseId",
                table: "BudgetMonthBase",
                column: "BudgetBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonthBase_BudgetManagerId",
                table: "BudgetMonthBase",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonthBase_BudgetTypeId",
                table: "BudgetMonthBase",
                column: "BudgetTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetForecast");

            migrationBuilder.DropTable(
                name: "BudgetMonthBase");

            migrationBuilder.DropTable(
                name: "BudgetBase");
        }
    }
}
