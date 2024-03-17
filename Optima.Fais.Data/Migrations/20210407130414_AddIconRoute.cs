using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddIconRoute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_PermissionType_PermissionTypeId",
                table: "Permission");

            migrationBuilder.AddColumn<int>(
                name: "IconRouteId",
                table: "Route",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "PermissionRole",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PermissionTypeId",
                table: "Permission",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "IconRoute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IconRoute", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Route_IconRouteId",
                table: "Route",
                column: "IconRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRole_RouteId",
                table: "PermissionRole",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_PermissionType_PermissionTypeId",
                table: "Permission",
                column: "PermissionTypeId",
                principalTable: "PermissionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Route_RouteId",
                table: "PermissionRole",
                column: "RouteId",
                principalTable: "Route",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Route_IconRoute_IconRouteId",
                table: "Route",
                column: "IconRouteId",
                principalTable: "IconRoute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_PermissionType_PermissionTypeId",
                table: "Permission");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Route_RouteId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Route_IconRoute_IconRouteId",
                table: "Route");

            migrationBuilder.DropTable(
                name: "IconRoute");

            migrationBuilder.DropIndex(
                name: "IX_Route_IconRouteId",
                table: "Route");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRole_RouteId",
                table: "PermissionRole");

            migrationBuilder.DropColumn(
                name: "IconRouteId",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "PermissionRole");

            migrationBuilder.AlterColumn<int>(
                name: "PermissionTypeId",
                table: "Permission",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_PermissionType_PermissionTypeId",
                table: "Permission",
                column: "PermissionTypeId",
                principalTable: "PermissionType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
