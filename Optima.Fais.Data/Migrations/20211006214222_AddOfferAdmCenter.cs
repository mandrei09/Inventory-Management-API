using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOfferAdmCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "AdmCenterIdFinal",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdmCenterIdInitial",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeIdFinal",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeIdInitial",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeIdFinal",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeIdInitial",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionIdFinal",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionIdInitial",
                table: "OfferOp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdmCenterId",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetTypeId",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTypeId",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AdmCenterIdFinal",
                table: "OfferOp",
                column: "AdmCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AdmCenterIdInitial",
                table: "OfferOp",
                column: "AdmCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AssetTypeIdFinal",
                table: "OfferOp",
                column: "AssetTypeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_AssetTypeIdInitial",
                table: "OfferOp",
                column: "AssetTypeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_ProjectTypeIdFinal",
                table: "OfferOp",
                column: "ProjectTypeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_ProjectTypeIdInitial",
                table: "OfferOp",
                column: "ProjectTypeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_RegionIdFinal",
                table: "OfferOp",
                column: "RegionIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_OfferOp_RegionIdInitial",
                table: "OfferOp",
                column: "RegionIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_AdmCenterId",
                table: "Offer",
                column: "AdmCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_AssetTypeId",
                table: "Offer",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_ProjectTypeId",
                table: "Offer",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_RegionId",
                table: "Offer",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_AdmCenter_AdmCenterId",
                table: "Offer",
                column: "AdmCenterId",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_AssetType_AssetTypeId",
                table: "Offer",
                column: "AssetTypeId",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_ProjectType_ProjectTypeId",
                table: "Offer",
                column: "ProjectTypeId",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Region_RegionId",
                table: "Offer",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_AdmCenter_AdmCenterIdFinal",
                table: "OfferOp",
                column: "AdmCenterIdFinal",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_AdmCenter_AdmCenterIdInitial",
                table: "OfferOp",
                column: "AdmCenterIdInitial",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_AssetType_AssetTypeIdFinal",
                table: "OfferOp",
                column: "AssetTypeIdFinal",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_AssetType_AssetTypeIdInitial",
                table: "OfferOp",
                column: "AssetTypeIdInitial",
                principalTable: "AssetType",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_ProjectType_ProjectTypeIdFinal",
                table: "OfferOp",
                column: "ProjectTypeIdFinal",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_ProjectType_ProjectTypeIdInitial",
                table: "OfferOp",
                column: "ProjectTypeIdInitial",
                principalTable: "ProjectType",
                principalColumn: "ProjectTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_Region_RegionIdFinal",
                table: "OfferOp",
                column: "RegionIdFinal",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferOp_Region_RegionIdInitial",
                table: "OfferOp",
                column: "RegionIdInitial",
                principalTable: "Region",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_AdmCenter_AdmCenterId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_AssetType_AssetTypeId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_ProjectType_ProjectTypeId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Region_RegionId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_AdmCenter_AdmCenterIdFinal",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_AdmCenter_AdmCenterIdInitial",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_AssetType_AssetTypeIdFinal",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_AssetType_AssetTypeIdInitial",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_ProjectType_ProjectTypeIdFinal",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_ProjectType_ProjectTypeIdInitial",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_Region_RegionIdFinal",
                table: "OfferOp");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferOp_Region_RegionIdInitial",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_AdmCenterIdFinal",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_AdmCenterIdInitial",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_AssetTypeIdFinal",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_AssetTypeIdInitial",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_ProjectTypeIdFinal",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_ProjectTypeIdInitial",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_RegionIdFinal",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_OfferOp_RegionIdInitial",
                table: "OfferOp");

            migrationBuilder.DropIndex(
                name: "IX_Offer_AdmCenterId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_AssetTypeId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_ProjectTypeId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_RegionId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "AdmCenterIdFinal",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "AdmCenterIdInitial",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "AssetTypeIdFinal",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "AssetTypeIdInitial",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "ProjectTypeIdFinal",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "ProjectTypeIdInitial",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "RegionIdFinal",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "RegionIdInitial",
                table: "OfferOp");

            migrationBuilder.DropColumn(
                name: "AdmCenterId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Offer");
        }
    }
}
