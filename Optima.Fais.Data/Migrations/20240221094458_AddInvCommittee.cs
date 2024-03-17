using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInvCommittee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "InvCommittee",
                columns: table => new
                {
                    InvCommitteeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCommittee", x => x.InvCommitteeId);
                    table.ForeignKey(
                        name: "FK_InvCommittee_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvCommitteePosition",
                columns: table => new
                {
                    InvCommitteePositionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCommitteePosition", x => x.InvCommitteePositionId);
                });

            migrationBuilder.CreateTable(
                name: "InvCommitteeMember",
                columns: table => new
                {
                    InvCommitteeMemberId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    InvCommitteeId = table.Column<int>(nullable: true),
                    InvCommitteePositionId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCommitteeMember", x => x.InvCommitteeMemberId);
                    table.ForeignKey(
                        name: "FK_InvCommitteeMember_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvCommitteeMember_InvCommittee_InvCommitteeId",
                        column: x => x.InvCommitteeId,
                        principalTable: "InvCommittee",
                        principalColumn: "InvCommitteeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvCommitteeMember_InvCommitteePosition_InvCommitteePositionId",
                        column: x => x.InvCommitteePositionId,
                        principalTable: "InvCommitteePosition",
                        principalColumn: "InvCommitteePositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvCommittee_CompanyId",
                table: "InvCommittee",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvCommitteeMember_EmployeeId",
                table: "InvCommitteeMember",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvCommitteeMember_InvCommitteeId",
                table: "InvCommitteeMember",
                column: "InvCommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvCommitteeMember_InvCommitteePositionId",
                table: "InvCommitteeMember",
                column: "InvCommitteePositionId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "InvCommitteeMember");

            migrationBuilder.DropTable(
                name: "InvCommittee");

            migrationBuilder.DropTable(
                name: "InvCommitteePosition");

        }
    }
}
