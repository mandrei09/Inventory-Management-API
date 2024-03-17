using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAspNetUserSubstitute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "TransferInStockSAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "TransferAssetSAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "RetireAssetSAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "CreateAssetSAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "AssetInvPlusSAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "AssetInvMinusSAP",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "AssetChangeSAP",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubstituteId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ToDate",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorId",
                table: "AcquisitionAssetSAP",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferInStockSAP_ErrorId",
                table: "TransferInStockSAP",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferAssetSAP_ErrorId",
                table: "TransferAssetSAP",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_RetireAssetSAP_ErrorId",
                table: "RetireAssetSAP",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateAssetSAP_ErrorId",
                table: "CreateAssetSAP",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvPlusSAP_ErrorId",
                table: "AssetInvPlusSAP",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvMinusSAP_ErrorId",
                table: "AssetInvMinusSAP",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetChangeSAP_ErrorId",
                table: "AssetChangeSAP",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubstituteId",
                table: "AspNetUsers",
                column: "SubstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_AcquisitionAssetSAP_ErrorId",
                table: "AcquisitionAssetSAP",
                column: "ErrorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcquisitionAssetSAP_Error_ErrorId",
                table: "AcquisitionAssetSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Employee_SubstituteId",
                table: "AspNetUsers",
                column: "SubstituteId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetChangeSAP_Error_ErrorId",
                table: "AssetChangeSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetInvMinusSAP_Error_ErrorId",
                table: "AssetInvMinusSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetInvPlusSAP_Error_ErrorId",
                table: "AssetInvPlusSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreateAssetSAP_Error_ErrorId",
                table: "CreateAssetSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RetireAssetSAP_Error_ErrorId",
                table: "RetireAssetSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferAssetSAP_Error_ErrorId",
                table: "TransferAssetSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferInStockSAP_Error_ErrorId",
                table: "TransferInStockSAP",
                column: "ErrorId",
                principalTable: "Error",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcquisitionAssetSAP_Error_ErrorId",
                table: "AcquisitionAssetSAP");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Employee_SubstituteId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetChangeSAP_Error_ErrorId",
                table: "AssetChangeSAP");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetInvMinusSAP_Error_ErrorId",
                table: "AssetInvMinusSAP");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetInvPlusSAP_Error_ErrorId",
                table: "AssetInvPlusSAP");

            migrationBuilder.DropForeignKey(
                name: "FK_CreateAssetSAP_Error_ErrorId",
                table: "CreateAssetSAP");

            migrationBuilder.DropForeignKey(
                name: "FK_RetireAssetSAP_Error_ErrorId",
                table: "RetireAssetSAP");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferAssetSAP_Error_ErrorId",
                table: "TransferAssetSAP");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferInStockSAP_Error_ErrorId",
                table: "TransferInStockSAP");

            migrationBuilder.DropIndex(
                name: "IX_TransferInStockSAP_ErrorId",
                table: "TransferInStockSAP");

            migrationBuilder.DropIndex(
                name: "IX_TransferAssetSAP_ErrorId",
                table: "TransferAssetSAP");

            migrationBuilder.DropIndex(
                name: "IX_RetireAssetSAP_ErrorId",
                table: "RetireAssetSAP");

            migrationBuilder.DropIndex(
                name: "IX_CreateAssetSAP_ErrorId",
                table: "CreateAssetSAP");

            migrationBuilder.DropIndex(
                name: "IX_AssetInvPlusSAP_ErrorId",
                table: "AssetInvPlusSAP");

            migrationBuilder.DropIndex(
                name: "IX_AssetInvMinusSAP_ErrorId",
                table: "AssetInvMinusSAP");

            migrationBuilder.DropIndex(
                name: "IX_AssetChangeSAP_ErrorId",
                table: "AssetChangeSAP");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SubstituteId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AcquisitionAssetSAP_ErrorId",
                table: "AcquisitionAssetSAP");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "TransferInStockSAP");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "TransferAssetSAP");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "RetireAssetSAP");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "CreateAssetSAP");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "AssetInvPlusSAP");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "AssetInvMinusSAP");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "AssetChangeSAP");

            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubstituteId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ErrorId",
                table: "AcquisitionAssetSAP");
        }
    }
}
