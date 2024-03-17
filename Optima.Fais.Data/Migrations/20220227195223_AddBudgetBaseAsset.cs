using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetBaseAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepPeriod",
                table: "Asset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BudgetBaseAsset",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: false),
                    AppStateId = table.Column<int>(nullable: false),
                    April = table.Column<decimal>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    August = table.Column<decimal>(nullable: false),
                    BudgetBaseId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    BudgetTypeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    December = table.Column<decimal>(nullable: false),
                    DepPeriod = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_BudgetBaseAsset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetBaseAsset_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseAsset_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseAsset_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseAsset_BudgetBase_BudgetBaseId",
                        column: x => x.BudgetBaseId,
                        principalTable: "BudgetBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetBaseAsset_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetBaseAsset_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseAsset_AccMonthId",
                table: "BudgetBaseAsset",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseAsset_AppStateId",
                table: "BudgetBaseAsset",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseAsset_AssetId",
                table: "BudgetBaseAsset",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseAsset_BudgetBaseId",
                table: "BudgetBaseAsset",
                column: "BudgetBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseAsset_BudgetManagerId",
                table: "BudgetBaseAsset",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseAsset_BudgetTypeId",
                table: "BudgetBaseAsset",
                column: "BudgetTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetBaseAsset");

            migrationBuilder.DropColumn(
                name: "DepPeriod",
                table: "Asset");
        }
    }
}
