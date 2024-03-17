using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMasterType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "isInTransfer",
            //    table: "Asset",
            //    newName: "IsInTransfer");

            migrationBuilder.AddColumn<int>(
                name: "MasterTypeId",
                table: "AssetAdmMD",
                nullable: true);

            //migrationBuilder.AlterColumn<bool>(
            //    name: "IsInTransfer",
            //    table: "Asset",
            //    nullable: false,
            //    oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "MasterTypeId",
                table: "Asset",
                nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "Account",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Code = table.Column<string>(maxLength: 30, nullable: false),
            //        CompanyId = table.Column<int>(nullable: true),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        Name = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Account", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Account_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Company",
            //            principalColumn: "CompanyId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Article",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Code = table.Column<string>(maxLength: 30, nullable: false),
            //        CompanyId = table.Column<int>(nullable: true),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        Name = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Article", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Article_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Company",
            //            principalColumn: "CompanyId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AssetNature",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Code = table.Column<string>(maxLength: 30, nullable: false),
            //        CompanyId = table.Column<int>(nullable: true),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        Name = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AssetNature", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AssetNature_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Company",
            //            principalColumn: "CompanyId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "BudgetManager",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Code = table.Column<string>(maxLength: 30, nullable: false),
            //        CompanyId = table.Column<int>(nullable: true),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        Name = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_BudgetManager", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_BudgetManager_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Company",
            //            principalColumn: "CompanyId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ExpAccount",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Code = table.Column<string>(maxLength: 30, nullable: false),
            //        CompanyId = table.Column<int>(nullable: true),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        Name = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ExpAccount", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ExpAccount_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Company",
            //            principalColumn: "CompanyId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateTable(
                name: "MasterType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasterType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateTable(
            //    name: "PartnerLocation",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Address = table.Column<string>(maxLength: 200, nullable: true),
            //        Bank = table.Column<string>(maxLength: 100, nullable: true),
            //        BankAccount = table.Column<string>(maxLength: 30, nullable: true),
            //        CompanyId = table.Column<int>(nullable: true),
            //        ContactInfo = table.Column<string>(maxLength: 200, nullable: true),
            //        CreatedAt = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        ErpCode = table.Column<string>(maxLength: 30, nullable: true),
            //        FiscalCode = table.Column<string>(maxLength: 30, nullable: false),
            //        IsDeleted = table.Column<bool>(nullable: false),
            //        ModifiedAt = table.Column<DateTime>(nullable: true),
            //        ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
            //        Name = table.Column<string>(maxLength: 100, nullable: false),
            //        PayingAccount = table.Column<string>(maxLength: 30, nullable: true),
            //        RegistryNumber = table.Column<string>(maxLength: 30, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PartnerLocation", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PartnerLocation_Company_CompanyId",
            //            column: x => x.CompanyId,
            //            principalTable: "Company",
            //            principalColumn: "CompanyId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_MasterTypeId",
                table: "AssetAdmMD",
                column: "MasterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_MasterTypeId",
                table: "Asset",
                column: "MasterTypeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Account_CompanyId",
            //    table: "Account",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Article_CompanyId",
            //    table: "Article",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AssetNature_CompanyId",
            //    table: "AssetNature",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_BudgetManager_CompanyId",
            //    table: "BudgetManager",
            //    column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExpAccount_CompanyId",
            //    table: "ExpAccount",
            //    column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterType_CompanyId",
                table: "MasterType",
                column: "CompanyId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_PartnerLocation_CompanyId",
            //    table: "PartnerLocation",
            //    column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_MasterType_MasterTypeId",
                table: "Asset",
                column: "MasterTypeId",
                principalTable: "MasterType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_MasterType_MasterTypeId",
                table: "AssetAdmMD",
                column: "MasterTypeId",
                principalTable: "MasterType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_MasterType_MasterTypeId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdmMD_MasterType_MasterTypeId",
                table: "AssetAdmMD");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "AssetNature");

            migrationBuilder.DropTable(
                name: "BudgetManager");

            migrationBuilder.DropTable(
                name: "ExpAccount");

            migrationBuilder.DropTable(
                name: "MasterType");

            migrationBuilder.DropTable(
                name: "PartnerLocation");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdmMD_MasterTypeId",
                table: "AssetAdmMD");

            migrationBuilder.DropIndex(
                name: "IX_Asset_MasterTypeId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "MasterTypeId",
                table: "AssetAdmMD");

            migrationBuilder.DropColumn(
                name: "MasterTypeId",
                table: "Asset");

            migrationBuilder.RenameColumn(
                name: "IsInTransfer",
                table: "Asset",
                newName: "isInTransfer");

            migrationBuilder.AlterColumn<int>(
                name: "isInTransfer",
                table: "Asset",
                nullable: false,
                oldClrType: typeof(bool));
        }
    }
}
