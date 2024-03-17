using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddTableDefinition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableDefinition",
                columns: table => new
                {
                    TableDefinitionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDefinition", x => x.TableDefinitionId);
                    table.ForeignKey(
                        name: "FK_TableDefinition_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ColumnDefinition",
                columns: table => new
                {
                    ColumnDefinitionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Format = table.Column<string>(maxLength: 50, nullable: true),
                    HeaderCode = table.Column<string>(maxLength: 50, nullable: false),
                    Include = table.Column<string>(maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Pipe = table.Column<string>(maxLength: 50, nullable: true),
                    Position = table.Column<int>(nullable: false),
                    Property = table.Column<string>(maxLength: 50, nullable: false),
                    SortBy = table.Column<string>(maxLength: 50, nullable: true),
                    TableDefinitionId = table.Column<int>(nullable: false),
                    TextAlign = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnDefinition", x => x.ColumnDefinitionId);
                    table.ForeignKey(
                        name: "FK_ColumnDefinition_TableDefinition_TableDefinitionId",
                        column: x => x.TableDefinitionId,
                        principalTable: "TableDefinition",
                        principalColumn: "TableDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnDefinition_TableDefinitionId",
                table: "ColumnDefinition",
                column: "TableDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDefinition_CompanyId",
                table: "TableDefinition",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnDefinition");

            migrationBuilder.DropTable(
                name: "TableDefinition");
        }
    }
}
