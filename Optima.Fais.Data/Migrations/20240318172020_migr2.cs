using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class migr2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Area_AreaId",
                table: "Matrix");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_AreaId",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Matrix");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Matrix",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    AreaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaId);
                    table.ForeignKey(
                        name: "FK_Area_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_AreaId",
                table: "Matrix",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_CompanyId",
                table: "Area",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Area_AreaId",
                table: "Matrix",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "AreaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
