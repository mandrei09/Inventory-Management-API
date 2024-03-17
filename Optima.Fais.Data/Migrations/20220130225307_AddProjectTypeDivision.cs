using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddProjectTypeDivision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectType_Department_DepartmentId",
                table: "ProjectType");

            migrationBuilder.DropIndex(
                name: "IX_ProjectType_DepartmentId",
                table: "ProjectType");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ProjectType");

            migrationBuilder.CreateTable(
                name: "ProjectTypeDivision",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DivisionId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ProjectTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTypeDivision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTypeDivision_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "DivisionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTypeDivision_ProjectType_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalTable: "ProjectType",
                        principalColumn: "ProjectTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTypeDivision_DivisionId",
                table: "ProjectTypeDivision",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTypeDivision_ProjectTypeId",
                table: "ProjectTypeDivision",
                column: "ProjectTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTypeDivision");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "ProjectType",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectType_DepartmentId",
                table: "ProjectType",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectType_Department_DepartmentId",
                table: "ProjectType",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
