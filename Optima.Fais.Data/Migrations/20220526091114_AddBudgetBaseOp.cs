using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBudgetBaseOp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetBaseOp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccSystemId = table.Column<int>(nullable: true),
                    AprilFin = table.Column<decimal>(nullable: false),
                    AprilIni = table.Column<decimal>(nullable: false),
                    AugustFin = table.Column<decimal>(nullable: false),
                    AugustIni = table.Column<decimal>(nullable: false),
                    BudgetBaseId = table.Column<int>(nullable: true),
                    BudgetForecastId = table.Column<int>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    BudgetMonthBaseId = table.Column<int>(nullable: true),
                    BudgetStateIdFinal = table.Column<int>(nullable: true),
                    BudgetStateInitialId = table.Column<int>(nullable: true),
                    BudgetTypeId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DecemberFin = table.Column<decimal>(nullable: false),
                    DecemberIni = table.Column<decimal>(nullable: false),
                    DepPeriodFinal = table.Column<int>(nullable: false),
                    DepPeriodInitial = table.Column<int>(nullable: false),
                    DepPeriodRemFinal = table.Column<int>(nullable: false),
                    DepPeriodRemInitial = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false),
                    DstConfAt = table.Column<DateTime>(nullable: true),
                    DstConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    FebruaryFin = table.Column<decimal>(nullable: false),
                    FebruaryIni = table.Column<decimal>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    InfoFin = table.Column<string>(maxLength: 450, nullable: true),
                    InfoIni = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    JanuaryFin = table.Column<decimal>(nullable: false),
                    JanuaryIni = table.Column<decimal>(nullable: false),
                    JulyFin = table.Column<decimal>(nullable: false),
                    JulyIni = table.Column<decimal>(nullable: false),
                    JuneFin = table.Column<decimal>(nullable: false),
                    JuneIni = table.Column<decimal>(nullable: false),
                    MarchFin = table.Column<decimal>(nullable: false),
                    MarchIni = table.Column<decimal>(nullable: false),
                    MayFin = table.Column<decimal>(nullable: false),
                    MayIni = table.Column<decimal>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NovemberFin = table.Column<decimal>(nullable: false),
                    NovemberIni = table.Column<decimal>(nullable: false),
                    OctomberFin = table.Column<decimal>(nullable: false),
                    OctomberIni = table.Column<decimal>(nullable: false),
                    RegisterConfAt = table.Column<DateTime>(nullable: true),
                    RegisterConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    ReleaseConfAt = table.Column<DateTime>(nullable: true),
                    ReleaseConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    SeptemberFin = table.Column<decimal>(nullable: false),
                    SeptemberIni = table.Column<decimal>(nullable: false),
                    SrcConfAt = table.Column<DateTime>(nullable: true),
                    SrcConfBy = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetBaseOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_BudgetBase_BudgetBaseId",
                        column: x => x.BudgetBaseId,
                        principalTable: "BudgetBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_BudgetForecast_BudgetForecastId",
                        column: x => x.BudgetForecastId,
                        principalTable: "BudgetForecast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_BudgetMonthBase_BudgetMonthBaseId",
                        column: x => x.BudgetMonthBaseId,
                        principalTable: "BudgetMonthBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AppState_BudgetStateIdFinal",
                        column: x => x.BudgetStateIdFinal,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AppState_BudgetStateInitialId",
                        column: x => x.BudgetStateInitialId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AspNetUsers_DstConfBy",
                        column: x => x.DstConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AspNetUsers_RegisterConfBy",
                        column: x => x.RegisterConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AspNetUsers_ReleaseConfBy",
                        column: x => x.ReleaseConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetBaseOp_AspNetUsers_SrcConfBy",
                        column: x => x.SrcConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_AccMonthId",
                table: "BudgetBaseOp",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_AccSystemId",
                table: "BudgetBaseOp",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetBaseId",
                table: "BudgetBaseOp",
                column: "BudgetBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetForecastId",
                table: "BudgetBaseOp",
                column: "BudgetForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetManagerId",
                table: "BudgetBaseOp",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetMonthBaseId",
                table: "BudgetBaseOp",
                column: "BudgetMonthBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetStateIdFinal",
                table: "BudgetBaseOp",
                column: "BudgetStateIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetStateInitialId",
                table: "BudgetBaseOp",
                column: "BudgetStateInitialId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_BudgetTypeId",
                table: "BudgetBaseOp",
                column: "BudgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_DocumentId",
                table: "BudgetBaseOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_DstConfBy",
                table: "BudgetBaseOp",
                column: "DstConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_RegisterConfBy",
                table: "BudgetBaseOp",
                column: "RegisterConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_ReleaseConfBy",
                table: "BudgetBaseOp",
                column: "ReleaseConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBaseOp_SrcConfBy",
                table: "BudgetBaseOp",
                column: "SrcConfBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetBaseOp");
        }
    }
}
