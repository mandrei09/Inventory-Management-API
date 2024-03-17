using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMultipleRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "TableDefinition",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "RouteChildren",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Route",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "PermissionType",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "PermissionRole",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Permission",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "ColumnDefinition",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ColumnDefinitionRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColumnDefinitionId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    RoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnDefinitionRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColumnDefinitionRole_ColumnDefinition_ColumnDefinitionId",
                        column: x => x.ColumnDefinitionId,
                        principalTable: "ColumnDefinition",
                        principalColumn: "ColumnDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColumnDefinitionRole_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouteChildrenRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    RouteChildrenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteChildrenRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteChildrenRole_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RouteChildrenRole_RouteChildren_RouteChildrenId",
                        column: x => x.RouteChildrenId,
                        principalTable: "RouteChildren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    RoleId = table.Column<string>(nullable: true),
                    RouteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteRole_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RouteRole_Route_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnDefinitionRole_ColumnDefinitionId",
                table: "ColumnDefinitionRole",
                column: "ColumnDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnDefinitionRole_RoleId",
                table: "ColumnDefinitionRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteChildrenRole_RoleId",
                table: "RouteChildrenRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteChildrenRole_RouteChildrenId",
                table: "RouteChildrenRole",
                column: "RouteChildrenId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteRole_RoleId",
                table: "RouteRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteRole_RouteId",
                table: "RouteRole",
                column: "RouteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnDefinitionRole");

            migrationBuilder.DropTable(
                name: "RouteChildrenRole");

            migrationBuilder.DropTable(
                name: "RouteRole");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "TableDefinition");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "RouteChildren");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "PermissionType");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "PermissionRole");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "ColumnDefinition");
        }
    }
}
