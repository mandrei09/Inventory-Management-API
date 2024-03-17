using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmailManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    NotifyEnabled = table.Column<bool>(nullable: false),
                    NotifyEnd = table.Column<DateTime>(nullable: true),
                    NotifyInterval = table.Column<int>(nullable: false),
                    NotifyLast = table.Column<DateTime>(nullable: true),
                    NotifyStart = table.Column<DateTime>(nullable: true),
                    UploadFolder = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailManager",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetComponentId = table.Column<int>(nullable: true),
                    AssetId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmailTypeId = table.Column<int>(nullable: false),
                    EmployeeIdFinal = table.Column<int>(nullable: true),
                    EmployeeIdInitial = table.Column<int>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    RoomIdFinal = table.Column<int>(nullable: true),
                    RoomIdInitial = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailManager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailManager_AssetComponent_AssetComponentId",
                        column: x => x.AssetComponentId,
                        principalTable: "AssetComponent",
                        principalColumn: "AssetComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailManager_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailManager_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailManager_EmailType_EmailTypeId",
                        column: x => x.EmailTypeId,
                        principalTable: "EmailType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailManager_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailManager_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailManager_Room_RoomIdFinal",
                        column: x => x.RoomIdFinal,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailManager_Room_RoomIdInitial",
                        column: x => x.RoomIdInitial,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_AssetComponentId",
                table: "EmailManager",
                column: "AssetComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_AssetId",
                table: "EmailManager",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_CompanyId",
                table: "EmailManager",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_EmailTypeId",
                table: "EmailManager",
                column: "EmailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_EmployeeIdFinal",
                table: "EmailManager",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_EmployeeIdInitial",
                table: "EmailManager",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_RoomIdFinal",
                table: "EmailManager",
                column: "RoomIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_EmailManager_RoomIdInitial",
                table: "EmailManager",
                column: "RoomIdInitial");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailManager");

            migrationBuilder.DropTable(
                name: "EmailType");
        }
    }
}
