using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmailOfferStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailOfferStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppStateId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DocumentNumber = table.Column<int>(nullable: false),
                    EmailSend = table.Column<bool>(nullable: false),
                    EmailSkip = table.Column<bool>(nullable: false),
                    EmailTypeId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: true),
                    ErrorId = table.Column<int>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    GuidAll = table.Column<Guid>(nullable: false),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    OfferId = table.Column<int>(nullable: true),
                    OwnerId = table.Column<int>(nullable: true),
                    PartnerId = table.Column<int>(nullable: true),
                    RequestId = table.Column<int>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOfferStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_EmailType_EmailTypeId",
                        column: x => x.EmailTypeId,
                        principalTable: "EmailType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_Error_ErrorId",
                        column: x => x.ErrorId,
                        principalTable: "Error",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_Owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailOfferStatus_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_AppStateId",
                table: "EmailOfferStatus",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_CompanyId",
                table: "EmailOfferStatus",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_EmailTypeId",
                table: "EmailOfferStatus",
                column: "EmailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_EmployeeId",
                table: "EmailOfferStatus",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_ErrorId",
                table: "EmailOfferStatus",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_OfferId",
                table: "EmailOfferStatus",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_OwnerId",
                table: "EmailOfferStatus",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_PartnerId",
                table: "EmailOfferStatus",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOfferStatus_RequestId",
                table: "EmailOfferStatus",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailOfferStatus");
        }
    }
}
