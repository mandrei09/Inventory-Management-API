using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddDivision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administration_Employee_EmployeeId",
                table: "Administration");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAsset_AssetId",
                table: "InventoryAsset");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Administration",
                newName: "DivisionId");

            migrationBuilder.RenameIndex(
                name: "IX_Administration_EmployeeId",
                table: "Administration",
                newName: "IX_Administration_DivisionId");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Location",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Location",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AdministrationIdFinal",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdministrationIdInitial",
                table: "InventoryAsset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "Administration",
                nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "Division",
            //    columns: table => new
            //    {
            //        DivisionId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Code = table.Column<string>(maxLength: 30, nullable: false),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        Name = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Division", x => x.DivisionId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "InventoryListApns",
            //    columns: table => new
            //    {
            //        AssetId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        AssetCategory = table.Column<string>(nullable: true),
            //        AssetStateFinal = table.Column<string>(nullable: true),
            //        AssetStateInitial = table.Column<string>(nullable: true),
            //        CostCenterNameFinal = table.Column<string>(nullable: true),
            //        CostCenterNameInitial = table.Column<string>(nullable: true),
            //        Description = table.Column<string>(nullable: true),
            //        GpsCoordinates = table.Column<string>(nullable: true),
            //        Info = table.Column<string>(nullable: true),
            //        InvNo = table.Column<string>(nullable: true),
            //        InvNoParent = table.Column<string>(nullable: true),
            //        InventoryDate = table.Column<DateTime>(nullable: true),
            //        LocationNameFinal = table.Column<string>(nullable: true),
            //        LocationNameInitial = table.Column<string>(nullable: true),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        QFinal = table.Column<float>(nullable: false),
            //        Qinitial = table.Column<float>(nullable: false),
            //        RoomCodeFinal = table.Column<string>(nullable: true),
            //        RoomCodeInitial = table.Column<string>(nullable: true),
            //        RoomNameFinal = table.Column<string>(nullable: true),
            //        RoomNameInitial = table.Column<string>(nullable: true),
            //        SerialNumber = table.Column<string>(nullable: true),
            //        StoredAs = table.Column<string>(nullable: true),
            //        StreetCodeFinal = table.Column<string>(nullable: true),
            //        StreetCodeInitial = table.Column<string>(nullable: true),
            //        StreetNameFinal = table.Column<string>(nullable: true),
            //        StreetNameInitial = table.Column<string>(nullable: true),
            //        Um = table.Column<string>(nullable: true),
            //        UserEmployeeFullNameFinal = table.Column<string>(nullable: true),
            //        UserEmployeeFullNameInitial = table.Column<string>(nullable: true),
            //        UserEmployeeInternalCodeFinal = table.Column<string>(nullable: true),
            //        UserEmployeeInternalCodeInitial = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_InventoryListApns", x => x.AssetId);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_AdministrationIdFinal",
                table: "InventoryAsset",
                column: "AdministrationIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_AdministrationIdInitial",
                table: "InventoryAsset",
                column: "AdministrationIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_AssetId",
                table: "InventoryAsset",
                column: "AssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administration_CostCenterId",
                table: "Administration",
                column: "CostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Administration_CostCenter_CostCenterId",
                table: "Administration",
                column: "CostCenterId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Administration_Division_DivisionId",
                table: "Administration",
                column: "DivisionId",
                principalTable: "Division",
                principalColumn: "DivisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAsset_Administration_AdministrationIdFinal",
                table: "InventoryAsset",
                column: "AdministrationIdFinal",
                principalTable: "Administration",
                principalColumn: "AdministrationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAsset_Administration_AdministrationIdInitial",
                table: "InventoryAsset",
                column: "AdministrationIdInitial",
                principalTable: "Administration",
                principalColumn: "AdministrationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Administration_CostCenter_CostCenterId",
            //    table: "Administration");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Administration_Division_DivisionId",
            //    table: "Administration");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InventoryAsset_Administration_AdministrationIdFinal",
            //    table: "InventoryAsset");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_InventoryAsset_Administration_AdministrationIdInitial",
            //    table: "InventoryAsset");

            //migrationBuilder.DropTable(
            //    name: "Division");

            //migrationBuilder.DropTable(
            //    name: "InventoryListApns");

            //migrationBuilder.DropIndex(
            //    name: "IX_InventoryAsset_AdministrationIdFinal",
            //    table: "InventoryAsset");

            //migrationBuilder.DropIndex(
            //    name: "IX_InventoryAsset_AdministrationIdInitial",
            //    table: "InventoryAsset");

            //migrationBuilder.DropIndex(
            //    name: "IX_InventoryAsset_AssetId",
            //    table: "InventoryAsset");

            //migrationBuilder.DropIndex(
            //    name: "IX_Administration_CostCenterId",
            //    table: "Administration");

            //migrationBuilder.DropColumn(
            //    name: "Latitude",
            //    table: "Location");

            //migrationBuilder.DropColumn(
            //    name: "Longitude",
            //    table: "Location");

            //migrationBuilder.DropColumn(
            //    name: "AdministrationIdFinal",
            //    table: "InventoryAsset");

            //migrationBuilder.DropColumn(
            //    name: "AdministrationIdInitial",
            //    table: "InventoryAsset");

            //migrationBuilder.DropColumn(
            //    name: "CostCenterId",
            //    table: "Administration");

            //migrationBuilder.RenameColumn(
            //    name: "DivisionId",
            //    table: "Administration",
            //    newName: "EmployeeId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Administration_DivisionId",
            //    table: "Administration",
            //    newName: "IX_Administration_EmployeeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_InventoryAsset_AssetId",
            //    table: "InventoryAsset",
            //    column: "AssetId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Administration_Employee_EmployeeId",
            //    table: "Administration",
            //    column: "EmployeeId",
            //    principalTable: "Employee",
            //    principalColumn: "EmployeeId",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
