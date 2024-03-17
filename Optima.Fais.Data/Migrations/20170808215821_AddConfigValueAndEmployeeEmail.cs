using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddConfigValueAndEmployeeEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employee",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfigValue",
                columns: table => new
                {
                    ConfigValueId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BoolValue = table.Column<bool>(nullable: true),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DateValue = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Group = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NumericValue = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    TextValue = table.Column<string>(maxLength: 200, nullable: true),
                    ValueType = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigValue", x => x.ConfigValueId);
                    table.ForeignKey(
                        name: "FK_ConfigValue_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigValue_CompanyId",
                table: "ConfigValue",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigValue");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employee");
        }
    }
}
