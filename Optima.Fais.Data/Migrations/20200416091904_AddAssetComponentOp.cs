using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetComponentOp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetComponentOp",
                columns: table => new
                {
                    AssetComponentOpId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetComponentId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DocumentId = table.Column<int>(nullable: false),
                    EmployeeIdFinal = table.Column<int>(nullable: true),
                    EmployeeIdInitial = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Quantity = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetComponentOp", x => x.AssetComponentOpId);
                    table.ForeignKey(
                        name: "FK_AssetComponentOp_AssetComponent_AssetComponentId",
                        column: x => x.AssetComponentId,
                        principalTable: "AssetComponent",
                        principalColumn: "AssetComponentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetComponentOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetComponentOp_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetComponentOp_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponentOp_AssetComponentId",
                table: "AssetComponentOp",
                column: "AssetComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponentOp_DocumentId",
                table: "AssetComponentOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponentOp_EmployeeIdFinal",
                table: "AssetComponentOp",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponentOp_EmployeeIdInitial",
                table: "AssetComponentOp",
                column: "EmployeeIdInitial");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetComponentOp");
        }
    }
}
