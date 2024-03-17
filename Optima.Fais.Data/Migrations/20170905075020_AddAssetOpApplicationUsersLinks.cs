using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetOpApplicationUsersLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_DstConfBy",
                table: "AssetOp",
                column: "DstConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_RegisterConfBy",
                table: "AssetOp",
                column: "RegisterConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_ReleaseConfBy",
                table: "AssetOp",
                column: "ReleaseConfBy");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_SrcConfBy",
                table: "AssetOp",
                column: "SrcConfBy");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AspNetUsers_DstConfBy",
                table: "AssetOp",
                column: "DstConfBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AspNetUsers_RegisterConfBy",
                table: "AssetOp",
                column: "RegisterConfBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AspNetUsers_ReleaseConfBy",
                table: "AssetOp",
                column: "ReleaseConfBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_AspNetUsers_SrcConfBy",
                table: "AssetOp",
                column: "SrcConfBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AspNetUsers_DstConfBy",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AspNetUsers_RegisterConfBy",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AspNetUsers_ReleaseConfBy",
                table: "AssetOp");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetOp_AspNetUsers_SrcConfBy",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_DstConfBy",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_RegisterConfBy",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_ReleaseConfBy",
                table: "AssetOp");

            migrationBuilder.DropIndex(
                name: "IX_AssetOp_SrcConfBy",
                table: "AssetOp");
        }
    }
}
