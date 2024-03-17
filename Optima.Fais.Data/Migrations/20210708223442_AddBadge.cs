using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddBadge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "RouteChildren",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BadgeId",
                table: "RouteChildren",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IconRouteId",
                table: "RouteChildren",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Variant",
                table: "RouteChildren",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Route",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BadgeId",
                table: "Route",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Variant",
                table: "Route",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "PermissionType",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "PermissionRole",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Permission",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "IconRoute",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Badge",
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
                    Text = table.Column<string>(maxLength: 100, nullable: true),
                    Variant = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badge", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteChildren_BadgeId",
                table: "RouteChildren",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteChildren_IconRouteId",
                table: "RouteChildren",
                column: "IconRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_BadgeId",
                table: "Route",
                column: "BadgeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Route_Badge_BadgeId",
                table: "Route",
                column: "BadgeId",
                principalTable: "Badge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteChildren_Badge_BadgeId",
                table: "RouteChildren",
                column: "BadgeId",
                principalTable: "Badge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteChildren_IconRoute_IconRouteId",
                table: "RouteChildren",
                column: "IconRouteId",
                principalTable: "IconRoute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Route_Badge_BadgeId",
                table: "Route");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteChildren_Badge_BadgeId",
                table: "RouteChildren");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteChildren_IconRoute_IconRouteId",
                table: "RouteChildren");

            migrationBuilder.DropTable(
                name: "Badge");

            migrationBuilder.DropIndex(
                name: "IX_RouteChildren_BadgeId",
                table: "RouteChildren");

            migrationBuilder.DropIndex(
                name: "IX_RouteChildren_IconRouteId",
                table: "RouteChildren");

            migrationBuilder.DropIndex(
                name: "IX_Route_BadgeId",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "RouteChildren");

            migrationBuilder.DropColumn(
                name: "BadgeId",
                table: "RouteChildren");

            migrationBuilder.DropColumn(
                name: "IconRouteId",
                table: "RouteChildren");

            migrationBuilder.DropColumn(
                name: "Variant",
                table: "RouteChildren");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "BadgeId",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "Variant",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "PermissionType");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "PermissionRole");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "IconRoute");
        }
    }
}
