using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfferTypeId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OfferType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
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
                    table.PrimaryKey("PK_OfferType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_OfferTypeId",
                table: "Offer",
                column: "OfferTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferType_CompanyId",
                table: "OfferType",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_OfferType_OfferTypeId",
                table: "Offer",
                column: "OfferTypeId",
                principalTable: "OfferType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_OfferType_OfferTypeId",
                table: "Offer");

            migrationBuilder.DropTable(
                name: "OfferType");

            migrationBuilder.DropIndex(
                name: "IX_Offer_OfferTypeId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "OfferTypeId",
                table: "Offer");
        }
    }
}
