using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddColumnFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColumnFilterId",
                table: "ColumnDefinition",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ColumnFilter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Placeholder = table.Column<string>(nullable: true),
                    Property = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnFilter", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnDefinition_ColumnFilterId",
                table: "ColumnDefinition",
                column: "ColumnFilterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColumnDefinition_ColumnFilter_ColumnFilterId",
                table: "ColumnDefinition",
                column: "ColumnFilterId",
                principalTable: "ColumnFilter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColumnDefinition_ColumnFilter_ColumnFilterId",
                table: "ColumnDefinition");

            migrationBuilder.DropTable(
                name: "ColumnFilter");

            migrationBuilder.DropIndex(
                name: "IX_ColumnDefinition_ColumnFilterId",
                table: "ColumnDefinition");

            migrationBuilder.DropColumn(
                name: "ColumnFilterId",
                table: "ColumnDefinition");
        }
    }
}
