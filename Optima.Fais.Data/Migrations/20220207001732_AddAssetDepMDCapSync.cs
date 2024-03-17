using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetDepMDCapSync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetDepMDCapSync",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AKTIV = table.Column<string>(nullable: true),
                    ANBTR = table.Column<decimal>(nullable: false),
                    ANLKL = table.Column<string>(nullable: true),
                    ANLN1 = table.Column<string>(nullable: true),
                    ANLN2 = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    BLDAT = table.Column<string>(maxLength: 200, nullable: false),
                    BUDAT = table.Column<string>(nullable: true),
                    BUKRSH = table.Column<string>(maxLength: 100, nullable: false),
                    BUKRST = table.Column<string>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    CAUFN = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsImported = table.Column<bool>(nullable: false),
                    KFZKZ = table.Column<string>(nullable: true),
                    KOSTL = table.Column<string>(nullable: true),
                    KOSTLV = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    SERNR = table.Column<string>(nullable: true),
                    SGTXT = table.Column<string>(nullable: true),
                    STORT = table.Column<string>(nullable: true),
                    TCODE = table.Column<string>(nullable: true),
                    TXA50 = table.Column<string>(nullable: true),
                    TXT50 = table.Column<string>(nullable: true),
                    WERKS = table.Column<string>(nullable: true),
                    XBLNR = table.Column<string>(nullable: true),
                    XSTIL = table.Column<string>(nullable: true),
                    ZZCLAS = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDepMDCapSync", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDepMDCapSync_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDepMDCapSync_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMDCapSync_AccMonthId",
                table: "AssetDepMDCapSync",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMDCapSync_BudgetManagerId",
                table: "AssetDepMDCapSync",
                column: "BudgetManagerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDepMDCapSync");
        }
    }
}
