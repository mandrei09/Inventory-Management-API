using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetImportTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Project",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdmCenterIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdmCenterIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BudgetStateIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepPeriodFinal",
                table: "BudgetOp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepPeriodInitial",
                table: "BudgetOp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepPeriodRemFinal",
                table: "BudgetOp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepPeriodRemInitial",
                table: "BudgetOp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeFinalId",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeInitialId",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionIdFinal",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionIdInitial",
                table: "BudgetOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdmCenterId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepPeriod",
                table: "Budget",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepPeriodRem",
                table: "Budget",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Budget",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Budget",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    ActivityId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_Activity_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectType",
                columns: table => new
                {
                    ProjectTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectType", x => x.ProjectTypeId);
                    table.ForeignKey(
                        name: "FK_ProjectType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectTypeId",
                table: "Project",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_ActivityIdFinal",
                table: "BudgetOp",
                column: "ActivityIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_ActivityIdInitial",
                table: "BudgetOp",
                column: "ActivityIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AssetTypeIdFinal",
                table: "BudgetOp",
                column: "AssetTypeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_AssetTypeIdInitial",
                table: "BudgetOp",
                column: "AssetTypeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_BudgetStateIdFinal",
                table: "BudgetOp",
                column: "BudgetStateIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_CountryIdFinal",
                table: "BudgetOp",
                column: "CountryIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_CountryIdInitial",
                table: "BudgetOp",
                column: "CountryIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_ProjectTypeFinalId",
                table: "BudgetOp",
                column: "ProjectTypeFinalId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_ProjectTypeInitialId",
                table: "BudgetOp",
                column: "ProjectTypeInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_RegionIdFinal",
                table: "BudgetOp",
                column: "RegionIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetOp_RegionIdInitial",
                table: "BudgetOp",
                column: "RegionIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_ActivityId",
                table: "Budget",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AdmCenterId",
                table: "Budget",
                column: "AdmCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_AssetTypeId",
                table: "Budget",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_CountryId",
                table: "Budget",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_ProjectTypeId",
                table: "Budget",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_RegionId",
                table: "Budget",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_CompanyId",
                table: "Activity",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectType_CompanyId",
                table: "ProjectType",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Activity_ActivityId",
                table: "Budget",
                column: "ActivityId",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_AdmCenter_AdmCenterId",
                table: "Budget",
                column: "AdmCenterId",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_AssetType_AssetTypeId",
                table: "Budget",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Country_CountryId",
                table: "Budget",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_ProjectType_ProjectTypeId",
                table: "Budget",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Region_RegionId",
                table: "Budget",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Activity_ActivityIdFinal",
                table: "BudgetOp",
                column: "ActivityIdFinal",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Activity_ActivityIdInitial",
                table: "BudgetOp",
                column: "ActivityIdInitial",
                principalTable: "Activity",
                principalColumn: "ActivityId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_AdmCenter_AdministrationIdFinal",
                table: "BudgetOp",
                column: "AdministrationIdFinal",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_AdmCenter_AdministrationIdInitial",
                table: "BudgetOp",
                column: "AdministrationIdInitial",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_AssetType_AssetTypeIdFinal",
                table: "BudgetOp",
                column: "AssetTypeIdFinal",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_AssetType_AssetTypeIdInitial",
                table: "BudgetOp",
                column: "AssetTypeIdInitial",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_AppState_BudgetStateIdFinal",
                table: "BudgetOp",
                column: "BudgetStateIdFinal",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Country_CountryIdFinal",
                table: "BudgetOp",
                column: "CountryIdFinal",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Country_CountryIdInitial",
                table: "BudgetOp",
                column: "CountryIdInitial",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_ProjectType_ProjectTypeFinalId",
                table: "BudgetOp",
                column: "ProjectTypeFinalId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_ProjectType_ProjectTypeInitialId",
                table: "BudgetOp",
                column: "ProjectTypeInitialId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Region_RegionIdFinal",
                table: "BudgetOp",
                column: "RegionIdFinal",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetOp_Region_RegionIdInitial",
                table: "BudgetOp",
                column: "RegionIdInitial",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectType_ProjectTypeId",
                table: "Project",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Activity_ActivityId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_AdmCenter_AdmCenterId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_AssetType_AssetTypeId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Country_CountryId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_ProjectType_ProjectTypeId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Region_RegionId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Activity_ActivityIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Activity_ActivityIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_AdmCenter_AdministrationIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_AdmCenter_AdministrationIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_AssetType_AssetTypeIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_AssetType_AssetTypeIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_AppState_BudgetStateIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Country_CountryIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Country_CountryIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_ProjectType_ProjectTypeFinalId",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_ProjectType_ProjectTypeInitialId",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Region_RegionIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetOp_Region_RegionIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectType_ProjectTypeId",
                table: "Project");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "ProjectType");

            migrationBuilder.DropIndex(
                name: "IX_Project_ProjectTypeId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_ActivityIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_ActivityIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_AssetTypeIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_AssetTypeIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_BudgetStateIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_CountryIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_CountryIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_ProjectTypeFinalId",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_ProjectTypeInitialId",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_RegionIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_BudgetOp_RegionIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropIndex(
                name: "IX_Budget_ActivityId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_AdmCenterId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_AssetTypeId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_CountryId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_ProjectTypeId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_RegionId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ActivityIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "ActivityIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "AdmCenterIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "AdmCenterIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "AssetTypeIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "AssetTypeIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "BudgetStateIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "CountryIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "CountryIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "DepPeriodFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "DepPeriodInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "DepPeriodRemFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "DepPeriodRemInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "ProjectTypeFinalId",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "ProjectTypeIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "ProjectTypeIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "ProjectTypeInitialId",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "RegionIdFinal",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "RegionIdInitial",
                table: "BudgetOp");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "AdmCenterId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "DepPeriod",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "DepPeriodRem",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Budget");
        }
    }
}
