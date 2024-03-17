using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmailRequestStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailRequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppStateId = table.Column<int>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DocumentNumber = table.Column<int>(nullable: false),
                    EmailSend = table.Column<bool>(nullable: false),
                    EmailTypeId = table.Column<int>(nullable: false),
                    EmployeeL1EmailSend = table.Column<bool>(nullable: false),
                    EmployeeL1EmailSkip = table.Column<bool>(nullable: false),
                    EmployeeL1ValidateAt = table.Column<DateTime>(nullable: true),
                    EmployeeL1ValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeL2EmailSend = table.Column<bool>(nullable: false),
                    EmployeeL2EmailSkip = table.Column<bool>(nullable: false),
                    EmployeeL2ValidateAt = table.Column<DateTime>(nullable: true),
                    EmployeeL2ValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeL3EmailSend = table.Column<bool>(nullable: false),
                    EmployeeL3EmailSkip = table.Column<bool>(nullable: false),
                    EmployeeL3ValidateAt = table.Column<DateTime>(nullable: true),
                    EmployeeL3ValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeL4EmailSend = table.Column<bool>(nullable: false),
                    EmployeeL4EmailSkip = table.Column<bool>(nullable: false),
                    EmployeeL4ValidateAt = table.Column<DateTime>(nullable: true),
                    EmployeeL4ValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeS1EmailSend = table.Column<bool>(nullable: false),
                    EmployeeS1EmailSkip = table.Column<bool>(nullable: false),
                    EmployeeS1ValidateAt = table.Column<DateTime>(nullable: true),
                    EmployeeS1ValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeS2EmailSend = table.Column<bool>(nullable: false),
                    EmployeeS2EmailSkip = table.Column<bool>(nullable: false),
                    EmployeeS2ValidateAt = table.Column<DateTime>(nullable: true),
                    EmployeeS2ValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeS3EmailSend = table.Column<bool>(nullable: false),
                    EmployeeS3EmailSkip = table.Column<bool>(nullable: false),
                    EmployeeS3ValidateAt = table.Column<DateTime>(nullable: true),
                    EmployeeS3ValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    ErrorId = table.Column<int>(nullable: true),
                    Exported = table.Column<bool>(nullable: false),
                    FinalValidateAt = table.Column<DateTime>(nullable: true),
                    FinalValidateBy = table.Column<string>(maxLength: 450, nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    GuidAll = table.Column<Guid>(nullable: false),
                    Info = table.Column<string>(nullable: true),
                    IsAccepted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NeedBudgetEmailSend = table.Column<bool>(nullable: false),
                    NotCompletedSync = table.Column<bool>(nullable: false),
                    NotEmployeeL1Sync = table.Column<bool>(nullable: false),
                    NotEmployeeL2Sync = table.Column<bool>(nullable: false),
                    NotEmployeeL3Sync = table.Column<bool>(nullable: false),
                    NotEmployeeL4Sync = table.Column<bool>(nullable: false),
                    NotEmployeeS1Sync = table.Column<bool>(nullable: false),
                    NotEmployeeS2Sync = table.Column<bool>(nullable: false),
                    NotEmployeeS3Sync = table.Column<bool>(nullable: false),
                    NotNeedBudgetSync = table.Column<bool>(nullable: false),
                    NotSync = table.Column<bool>(nullable: false),
                    RequestBudgetForecastId = table.Column<int>(nullable: true),
                    RequestId = table.Column<int>(nullable: false),
                    SyncCompletedErrorCount = table.Column<int>(nullable: false),
                    SyncEmployeeL1ErrorCount = table.Column<int>(nullable: false),
                    SyncEmployeeL2ErrorCount = table.Column<int>(nullable: false),
                    SyncEmployeeL3ErrorCount = table.Column<int>(nullable: false),
                    SyncEmployeeL4ErrorCount = table.Column<int>(nullable: false),
                    SyncEmployeeS1ErrorCount = table.Column<int>(nullable: false),
                    SyncEmployeeS2ErrorCount = table.Column<int>(nullable: false),
                    SyncEmployeeS3ErrorCount = table.Column<int>(nullable: false),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    SyncNeedBudgetErrorCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailRequestStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AppState_AppStateId",
                        column: x => x.AppStateId,
                        principalTable: "AppState",
                        principalColumn: "AppStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_EmailType_EmailTypeId",
                        column: x => x.EmailTypeId,
                        principalTable: "EmailType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_EmployeeL1ValidateBy",
                        column: x => x.EmployeeL1ValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_EmployeeL2ValidateBy",
                        column: x => x.EmployeeL2ValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_EmployeeL3ValidateBy",
                        column: x => x.EmployeeL3ValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_EmployeeL4ValidateBy",
                        column: x => x.EmployeeL4ValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_EmployeeS1ValidateBy",
                        column: x => x.EmployeeS1ValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_EmployeeS2ValidateBy",
                        column: x => x.EmployeeS2ValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_EmployeeS3ValidateBy",
                        column: x => x.EmployeeS3ValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_Error_ErrorId",
                        column: x => x.ErrorId,
                        principalTable: "Error",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_AspNetUsers_FinalValidateBy",
                        column: x => x.FinalValidateBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_RequestBudgetForecast_RequestBudgetForecastId",
                        column: x => x.RequestBudgetForecastId,
                        principalTable: "RequestBudgetForecast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailRequestStatus_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_AppStateId",
                table: "EmailRequestStatus",
                column: "AppStateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmailTypeId",
                table: "EmailRequestStatus",
                column: "EmailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmployeeL1ValidateBy",
                table: "EmailRequestStatus",
                column: "EmployeeL1ValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmployeeL2ValidateBy",
                table: "EmailRequestStatus",
                column: "EmployeeL2ValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmployeeL3ValidateBy",
                table: "EmailRequestStatus",
                column: "EmployeeL3ValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmployeeL4ValidateBy",
                table: "EmailRequestStatus",
                column: "EmployeeL4ValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmployeeS1ValidateBy",
                table: "EmailRequestStatus",
                column: "EmployeeS1ValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmployeeS2ValidateBy",
                table: "EmailRequestStatus",
                column: "EmployeeS2ValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_EmployeeS3ValidateBy",
                table: "EmailRequestStatus",
                column: "EmployeeS3ValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_ErrorId",
                table: "EmailRequestStatus",
                column: "ErrorId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_FinalValidateBy",
                table: "EmailRequestStatus",
                column: "FinalValidateBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_RequestBudgetForecastId",
                table: "EmailRequestStatus",
                column: "RequestBudgetForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRequestStatus_RequestId",
                table: "EmailRequestStatus",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailRequestStatus");
        }
    }
}
