using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class NewTableAssetSyncError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "AssetSyncErrors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ErrorIdId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    SyncDate = table.Column<DateTime>(nullable: false),
                    SyncStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSyncErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetSyncErrors_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetSyncErrors_Error_ErrorIdId",
                        column: x => x.ErrorIdId,
                        principalTable: "Error",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetStateName = table.Column<string>(nullable: true),
                    CompanyCode = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    InvNo = table.Column<string>(nullable: true),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    L1 = table.Column<string>(nullable: true),
                    L2 = table.Column<string>(nullable: true),
                    L3 = table.Column<string>(nullable: true),
                    L4 = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    OfferCode = table.Column<string>(nullable: true),
                    OrderCode = table.Column<string>(nullable: true),
                    OwnerEmail = table.Column<string>(nullable: true),
                    POTypeName = table.Column<string>(nullable: true),
                    S1 = table.Column<string>(nullable: true),
                    S2 = table.Column<string>(nullable: true),
                    S3 = table.Column<string>(nullable: true),
                    Supplier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetSyncErrors_AssetId",
                table: "AssetSyncErrors",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSyncErrors_ErrorIdId",
                table: "AssetSyncErrors",
                column: "ErrorIdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetSyncErrors");

            migrationBuilder.DropTable(
                name: "OrderReports");

            
        }
    }
}
