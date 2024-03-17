using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressDetailId",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressDetailId",
                table: "Partner",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Contract",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressDetail = table.Column<string>(nullable: true),
                    CityId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Fax = table.Column<string>(maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(nullable: true),
                    UniqueName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Address_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    OwnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    OrgANId = table.Column<string>(nullable: true),
                    OrgName = table.Column<string>(nullable: true),
                    Organization = table.Column<string>(nullable: true),
                    UniqueName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.OwnerId);
                    table.ForeignKey(
                        name: "FK_Owner_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Owner_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocation_AddressDetailId",
                table: "PartnerLocation",
                column: "AddressDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_AddressDetailId",
                table: "Partner",
                column: "AddressDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_OwnerId",
                table: "Contract",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CityId",
                table: "Address",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CompanyId",
                table: "Address",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_CompanyId",
                table: "Owner",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_EmployeeId",
                table: "Owner",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Owner_OwnerId",
                table: "Contract",
                column: "OwnerId",
                principalTable: "Owner",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partner_Address_AddressDetailId",
                table: "Partner",
                column: "AddressDetailId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocation_Address_AddressDetailId",
                table: "PartnerLocation",
                column: "AddressDetailId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Owner_OwnerId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Partner_Address_AddressDetailId",
                table: "Partner");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocation_Address_AddressDetailId",
                table: "PartnerLocation");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocation_AddressDetailId",
                table: "PartnerLocation");

            migrationBuilder.DropIndex(
                name: "IX_Partner_AddressDetailId",
                table: "Partner");

            migrationBuilder.DropIndex(
                name: "IX_Contract_OwnerId",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "AddressDetailId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "AddressDetailId",
                table: "Partner");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Contract");
        }
    }
}
