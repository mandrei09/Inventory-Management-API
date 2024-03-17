using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
          
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessSystem",
                columns: table => new
                {
                    BusinessSystemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_BusinessSystem", x => x.BusinessSystemId);
                    table.ForeignKey(
                        name: "FK_BusinessSystem_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContractAmount",
                columns: table => new
                {
                    ContractAmountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    UomId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractAmount", x => x.ContractAmountId);
                    table.ForeignKey(
                        name: "FK_ContractAmount_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractAmount_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ERPImportResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ERPImportResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AgreementDate = table.Column<DateTime>(type: "date", nullable: true),
                    AmendmentReason = table.Column<string>(nullable: true),
                    AmendmentType = table.Column<string>(nullable: true),
                    AppStateId = table.Column<int>(nullable: true),
                    AutoRenewalInterval = table.Column<int>(nullable: false),
                    BusinessSystemId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 120, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContractAmountId = table.Column<int>(nullable: true),
                    ContractId = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "date", nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExpirationTermType = table.Column<string>(nullable: true),
                    HierarchicalType = table.Column<string>(nullable: true),
                    Info = table.Column<string>(maxLength: 120, nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    IsTestProject = table.Column<bool>(nullable: false),
                    MaximumNumberOfRenewals = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 120, nullable: true),
                    Origin = table.Column<int>(nullable: false),
                    PartnerId = table.Column<int>(nullable: true),
                    RelatedId = table.Column<string>(nullable: true),
                    TemplateId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contract_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contract_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contract_BusinessSystem_BusinessSystemId",
                        column: x => x.BusinessSystemId,
                        principalTable: "BusinessSystem",
                        principalColumn: "BusinessSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contract_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contract_ContractAmount_ContractAmountId",
                        column: x => x.ContractAmountId,
                        principalTable: "ContractAmount",
                        principalColumn: "ContractAmountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contract_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contract_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commodity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContractId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Domain = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UniqueName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commodity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commodity_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commodity_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractDivision",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContractId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UniqueName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDivision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractDivision_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractDivision_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractRegion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContractId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UniqueName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractRegion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractRegion_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractRegion_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractOp",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: true),
                    AccSystemId = table.Column<int>(nullable: true),
                    BusinessSystemId = table.Column<int>(nullable: true),
                    CommodityId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContractAmountId = table.Column<int>(nullable: true),
                    ContractDivisionId = table.Column<int>(nullable: true),
                    ContractId = table.Column<int>(nullable: false),
                    ContractRegionId = table.Column<int>(nullable: true),
                    ContractStateId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DocumentId = table.Column<int>(nullable: false),
                    DstConfAt = table.Column<DateTime>(nullable: true),
                    DstConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    Info = table.Column<string>(maxLength: 450, nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    PartnerId = table.Column<int>(nullable: true),
                    RegisterConfAt = table.Column<DateTime>(nullable: true),
                    RegisterConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    ReleaseConfAt = table.Column<DateTime>(nullable: true),
                    ReleaseConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    SrcConfAt = table.Column<DateTime>(nullable: true),
                    SrcConfBy = table.Column<string>(maxLength: 450, nullable: true),
                    Validated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractOp_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_BusinessSystem_BusinessSystemId",
                        column: x => x.BusinessSystemId,
                        principalTable: "BusinessSystem",
                        principalColumn: "BusinessSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_Commodity_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_ContractAmount_ContractAmountId",
                        column: x => x.ContractAmountId,
                        principalTable: "ContractAmount",
                        principalColumn: "ContractAmountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_ContractDivision_ContractDivisionId",
                        column: x => x.ContractDivisionId,
                        principalTable: "ContractDivision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractOp_ContractRegion_ContractRegionId",
                        column: x => x.ContractRegionId,
                        principalTable: "ContractRegion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_AppState_ContractStateId",
                        column: x => x.ContractStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractOp_AspNetUsers_DstConfBy",
                        column: x => x.DstConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_AspNetUsers_RegisterConfBy",
                        column: x => x.RegisterConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_AspNetUsers_ReleaseConfBy",
                        column: x => x.ReleaseConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractOp_AspNetUsers_SrcConfBy",
                        column: x => x.SrcConfBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_ContractId",
                table: "Order",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessSystem_CompanyId",
                table: "BusinessSystem",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Commodity_CompanyId",
                table: "Commodity",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Commodity_ContractId",
                table: "Commodity",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_AccMonthId",
                table: "Contract",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_AppStateId",
                table: "Contract",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_BusinessSystemId",
                table: "Contract",
                column: "BusinessSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_CompanyId",
                table: "Contract",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_ContractAmountId",
                table: "Contract",
                column: "ContractAmountId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_EmployeeId",
                table: "Contract",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_PartnerId",
                table: "Contract",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAmount_CompanyId",
                table: "ContractAmount",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractAmount_UomId",
                table: "ContractAmount",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDivision_CompanyId",
                table: "ContractDivision",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDivision_ContractId",
                table: "ContractDivision",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_AccMonthId",
                table: "ContractOp",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_AccSystemId",
                table: "ContractOp",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_BusinessSystemId",
                table: "ContractOp",
                column: "BusinessSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_CommodityId",
                table: "ContractOp",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_CompanyId",
                table: "ContractOp",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_ContractAmountId",
                table: "ContractOp",
                column: "ContractAmountId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_ContractDivisionId",
                table: "ContractOp",
                column: "ContractDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_ContractId",
                table: "ContractOp",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_ContractRegionId",
                table: "ContractOp",
                column: "ContractRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_ContractStateId",
                table: "ContractOp",
                column: "ContractStateId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_DocumentId",
                table: "ContractOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_DstConfBy",
                table: "ContractOp",
                column: "DstConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_EmployeeId",
                table: "ContractOp",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_PartnerId",
                table: "ContractOp",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_RegisterConfBy",
                table: "ContractOp",
                column: "RegisterConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_ReleaseConfBy",
                table: "ContractOp",
                column: "ReleaseConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOp_SrcConfBy",
                table: "ContractOp",
                column: "SrcConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractRegion_CompanyId",
                table: "ContractRegion",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractRegion_ContractId",
                table: "ContractRegion",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Contract_ContractId",
                table: "Order",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Contract_ContractId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "ContractOp");

            migrationBuilder.DropTable(
                name: "Commodity");

            migrationBuilder.DropTable(
                name: "ContractDivision");

            migrationBuilder.DropTable(
                name: "ContractRegion");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "BusinessSystem");

            migrationBuilder.DropTable(
                name: "ContractAmount");

            migrationBuilder.DropIndex(
                name: "IX_Order_ContractId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Order");
        }
    }
}
