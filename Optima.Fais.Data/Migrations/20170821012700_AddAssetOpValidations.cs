using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetOpValidations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetOpStateId",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DstConfAt",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DstConfBy",
                table: "AssetOp",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterConfAt",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisterConfBy",
                table: "AssetOp",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseConfAt",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReleaseConfBy",
                table: "AssetOp",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SrcConfAt",
                table: "AssetOp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SrcConfBy",
                table: "AssetOp",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppState",
                columns: table => new
                {
                    AppStateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Mask = table.Column<string>(maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    ParentCode = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppState", x => x.AppStateId);
                    table.ForeignKey(
                        name: "FK_AppState_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetOpStateId",
                table: "AssetOp",
                column: "AssetOpStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AppState_CompanyId",
                table: "AppState",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AppState_AssetOpStateId",
                table: "AssetOp",
                column: "AssetOpStateId",
                principalTable: "AppState",
                principalColumn: "AppStateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AppState_AssetOpStateId",
                table: "AssetOp");

            migrationBuilder.DropTable(
                name: "AppState");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_AssetOpStateId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "AssetOpStateId",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "DstConfAt",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "DstConfBy",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "RegisterConfAt",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "RegisterConfBy",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "ReleaseConfAt",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "ReleaseConfBy",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "SrcConfAt",
                table: "AssetOp");

            migrationBuilder.DropColumn(
                name: "SrcConfBy",
                table: "AssetOp");
        }
    }
}
