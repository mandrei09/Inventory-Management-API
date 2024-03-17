using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddWFHCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WFHCheck",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrandId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DictionaryItemId = table.Column<int>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    Imei = table.Column<string>(nullable: true),
                    InventoryNumber = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModelId = table.Column<int>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    SerialNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WFHCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WFHCheck_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WFHCheck_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WFHCheck_DictionaryItem_DictionaryItemId",
                        column: x => x.DictionaryItemId,
                        principalTable: "DictionaryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WFHCheck_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WFHCheck_Model_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Model",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WFHCheck_BrandId",
                table: "WFHCheck",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_WFHCheck_BudgetManagerId",
                table: "WFHCheck",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_WFHCheck_DictionaryItemId",
                table: "WFHCheck",
                column: "DictionaryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WFHCheck_EmployeeId",
                table: "WFHCheck",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WFHCheck_ModelId",
                table: "WFHCheck",
                column: "ModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WFHCheck");
        }
    }
}
