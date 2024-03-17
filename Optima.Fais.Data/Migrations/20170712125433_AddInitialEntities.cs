using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddInitialEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdmCenterId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "EntityType",
                columns: table => new
                {
                    EntityTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    UploadFolder = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityType", x => x.EntityTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AccMonth",
                columns: table => new
                {
                    AccMonthId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccMonth", x => x.AccMonthId);
                    table.ForeignKey(
                        name: "FK_AccMonth_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdmCenter",
                columns: table => new
                {
                    AdmCenterId = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_AdmCenter", x => x.AdmCenterId);
                    table.ForeignKey(
                        name: "FK_AdmCenter_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetCategory",
                columns: table => new
                {
                    AssetCategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Prefix = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategory", x => x.AssetCategoryId);
                    table.ForeignKey(
                        name: "FK_AssetCategory_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetClassType",
                columns: table => new
                {
                    AssetClassTypeId = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_AssetClassType", x => x.AssetClassTypeId);
                    table.ForeignKey(
                        name: "FK_AssetClassType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetState",
                columns: table => new
                {
                    AssetStateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    HasDep = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsFinal = table.Column<bool>(nullable: false),
                    IsInitial = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetState", x => x.AssetStateId);
                    table.ForeignKey(
                        name: "FK_AssetState_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetType",
                columns: table => new
                {
                    AssetTypeId = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_AssetType", x => x.AssetTypeId);
                    table.ForeignKey(
                        name: "FK_AssetType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentType",
                columns: table => new
                {
                    DocumentTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Mask = table.Column<string>(maxLength: 200, nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ParentCode = table.Column<string>(maxLength: 30, nullable: false),
                    Prefix = table.Column<string>(maxLength: 10, nullable: true),
                    Suffix = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.DocumentTypeId);
                    table.ForeignKey(
                        name: "FK_DocumentType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvState",
                columns: table => new
                {
                    InvStateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Mask = table.Column<string>(maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    ParentCode = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvState", x => x.InvStateId);
                    table.ForeignKey(
                        name: "FK_InvState_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Partner",
                columns: table => new
                {
                    PartnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    Bank = table.Column<string>(maxLength: 100, nullable: true),
                    BankAccount = table.Column<string>(maxLength: 30, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContactInfo = table.Column<string>(maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ErpCode = table.Column<string>(maxLength: 30, nullable: true),
                    FiscalCode = table.Column<string>(maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    PayingAccount = table.Column<string>(maxLength: 30, nullable: true),
                    RegistryNumber = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partner", x => x.PartnerId);
                    table.ForeignKey(
                        name: "FK_Partner_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    RegionId = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_Region", x => x.RegionId);
                    table.ForeignKey(
                        name: "FK_Region_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Uom",
                columns: table => new
                {
                    UomId = table.Column<int>(nullable: false)
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
                    table.PrimaryKey("PK_Uom", x => x.UomId);
                    table.ForeignKey(
                        name: "FK_Uom_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityFile",
                columns: table => new
                {
                    EntityFileId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EntityId = table.Column<int>(nullable: false),
                    EntityTypeId = table.Column<int>(nullable: false),
                    FileType = table.Column<string>(maxLength: 100, nullable: false),
                    Info = table.Column<string>(maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Size = table.Column<double>(nullable: false),
                    StoredAs = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityFile", x => x.EntityFileId);
                    table.ForeignKey(
                        name: "FK_EntityFile_EntityType_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityType",
                        principalColumn: "EntityTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostCenter",
                columns: table => new
                {
                    CostCenterId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdmCenterId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ERPCode = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenter", x => x.CostCenterId);
                    table.ForeignKey(
                        name: "FK_CostCenter_AdmCenter_AdmCenterId",
                        column: x => x.AdmCenterId,
                        principalTable: "AdmCenter",
                        principalColumn: "AdmCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenter_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccSystem",
                columns: table => new
                {
                    AccSystemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetClassTypeId = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_AccSystem", x => x.AccSystemId);
                    table.ForeignKey(
                        name: "FK_AccSystem_AssetClassType_AssetClassTypeId",
                        column: x => x.AssetClassTypeId,
                        principalTable: "AssetClassType",
                        principalColumn: "AssetClassTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccSystem_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetClass",
                columns: table => new
                {
                    AssetClassId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetClassTypeId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DepPeriodDefault = table.Column<int>(nullable: false),
                    DepPeriodMax = table.Column<int>(nullable: false),
                    DepPeriodMin = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    ParentAssetClassId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetClass", x => x.AssetClassId);
                    table.ForeignKey(
                        name: "FK_AssetClass_AssetClassType_AssetClassTypeId",
                        column: x => x.AssetClassTypeId,
                        principalTable: "AssetClassType",
                        principalColumn: "AssetClassTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetClass_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetClass_AssetClass_ParentAssetClassId",
                        column: x => x.ParentAssetClassId,
                        principalTable: "AssetClass",
                        principalColumn: "AssetClassId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    DocumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Approved = table.Column<bool>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: true),
                    Details = table.Column<string>(maxLength: 255, nullable: true),
                    DocNo1 = table.Column<string>(maxLength: 50, nullable: false),
                    DocNo2 = table.Column<string>(maxLength: 50, nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "date", nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    Exported = table.Column<bool>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ParentDocumentId = table.Column<int>(nullable: true),
                    PartnerId = table.Column<int>(nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "date", nullable: false),
                    ValidationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Document_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "DocumentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Document_Document_ParentDocumentId",
                        column: x => x.ParentDocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ERPCode = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Prefix = table.Column<string>(maxLength: 30, nullable: true),
                    RegionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Location_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ERPCode = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ParentRoomId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_Room_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_Room_ParentRoomId",
                        column: x => x.ParentRoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetAdmMD",
                columns: table => new
                {
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    AdministrationId = table.Column<int>(nullable: true),
                    AssetCategoryId = table.Column<int>(nullable: true),
                    AssetStateId = table.Column<int>(nullable: true),
                    AssetTypeId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    RoomId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAdmMD", x => new { x.AccMonthId, x.AssetId });
                    table.ForeignKey(
                        name: "FK_AssetAdmMD_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetAdmMD_AssetCategory_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategory",
                        principalColumn: "AssetCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmMD_AssetState_AssetStateId",
                        column: x => x.AssetStateId,
                        principalTable: "AssetState",
                        principalColumn: "AssetStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmMD_AssetType_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetType",
                        principalColumn: "AssetTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmMD_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmMD_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetDepMD",
                columns: table => new
                {
                    AccMonthId = table.Column<int>(nullable: false),
                    AccSystemId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    DepPeriod = table.Column<int>(nullable: false),
                    DepPeriodMonth = table.Column<int>(nullable: false),
                    DepPeriodRem = table.Column<int>(nullable: false),
                    ValueDep = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueDepPU = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueDepYTD = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueInv = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueRem = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDepMD", x => new { x.AccMonthId, x.AccSystemId, x.AssetId });
                    table.ForeignKey(
                        name: "FK_AssetDepMD_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDepMD_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetDep",
                columns: table => new
                {
                    AccSystemId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    DepPeriod = table.Column<int>(nullable: false),
                    DepPeriodIn = table.Column<int>(nullable: false),
                    DepPeriodMonth = table.Column<int>(nullable: false),
                    DepPeriodMonthIn = table.Column<int>(nullable: false),
                    DepPeriodRem = table.Column<int>(nullable: false),
                    DepPeriodRemIn = table.Column<int>(nullable: false),
                    DirectExpense = table.Column<bool>(nullable: true),
                    UsageEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    UsageStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    ValueDep = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueDepIn = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueDepPU = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueDepPUIn = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueDepYTD = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueDepYTDIn = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueInv = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueInvIn = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueRem = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueRemIn = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDep", x => new { x.AccSystemId, x.AssetId });
                    table.ForeignKey(
                        name: "FK_AssetDep_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetOp",
                columns: table => new
                {
                    AssetOpId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccSystemId = table.Column<int>(nullable: true),
                    AdministrationIdFinal = table.Column<int>(nullable: true),
                    AdministrationIdInitial = table.Column<int>(nullable: true),
                    AssetCategoryIdFinal = table.Column<int>(nullable: true),
                    AssetCategoryIdInitial = table.Column<int>(nullable: true),
                    AssetId = table.Column<int>(nullable: false),
                    AssetStateIdFinal = table.Column<int>(nullable: true),
                    AssetStateIdInitial = table.Column<int>(nullable: true),
                    CostCenterIdFinal = table.Column<int>(nullable: true),
                    CostCenterIdInitial = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DepUpdate = table.Column<bool>(nullable: true),
                    DepartmentIdFinal = table.Column<int>(nullable: true),
                    DepartmentIdInitial = table.Column<int>(nullable: true),
                    DocumentId = table.Column<int>(nullable: false),
                    EmployeeIdFinal = table.Column<int>(nullable: true),
                    EmployeeIdInitial = table.Column<int>(nullable: true),
                    InvStateIdFinal = table.Column<int>(nullable: true),
                    InvStateIdInitial = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    RoomIdFinal = table.Column<int>(nullable: true),
                    RoomIdInitial = table.Column<int>(nullable: true),
                    ValueAdd = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetOp", x => x.AssetOpId);
                    table.ForeignKey(
                        name: "FK_AssetOp_AccSystem_AccSystemId",
                        column: x => x.AccSystemId,
                        principalTable: "AccSystem",
                        principalColumn: "AccSystemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_AssetCategory_AssetCategoryIdFinal",
                        column: x => x.AssetCategoryIdFinal,
                        principalTable: "AssetCategory",
                        principalColumn: "AssetCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_AssetCategory_AssetCategoryIdInitial",
                        column: x => x.AssetCategoryIdInitial,
                        principalTable: "AssetCategory",
                        principalColumn: "AssetCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_AssetState_AssetStateIdFinal",
                        column: x => x.AssetStateIdFinal,
                        principalTable: "AssetState",
                        principalColumn: "AssetStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_AssetState_AssetStateIdInitial",
                        column: x => x.AssetStateIdInitial,
                        principalTable: "AssetState",
                        principalColumn: "AssetStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_CostCenter_CostCenterIdFinal",
                        column: x => x.CostCenterIdFinal,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_CostCenter_CostCenterIdInitial",
                        column: x => x.CostCenterIdInitial,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetOp_InvState_InvStateIdFinal",
                        column: x => x.InvStateIdFinal,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_InvState_InvStateIdInitial",
                        column: x => x.InvStateIdInitial,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_Room_RoomIdFinal",
                        column: x => x.RoomIdFinal,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetOp_Room_RoomIdInitial",
                        column: x => x.RoomIdInitial,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdmCenterId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DepartmentId = table.Column<int>(nullable: true),
                    ERPCode = table.Column<string>(maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    InternalCode = table.Column<string>(maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employee_AdmCenter_AdmCenterId",
                        column: x => x.AdmCenterId,
                        principalTable: "AdmCenter",
                        principalColumn: "AdmCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Administration",
                columns: table => new
                {
                    AdministrationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administration", x => x.AdministrationId);
                    table.ForeignKey(
                        name: "FK_Administration_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Administration_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    ERPCode = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    TeamLeaderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentId);
                    table.ForeignKey(
                        name: "FK_Department_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Department_Employee_TeamLeaderId",
                        column: x => x.TeamLeaderId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    AdministrationId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    DocumentId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: true),
                    End = table.Column<DateTime>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    RoomId = table.Column<int>(nullable: true),
                    Start = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventory_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    AssetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministrationId = table.Column<int>(nullable: true),
                    AssetCategoryId = table.Column<int>(nullable: true),
                    AssetStateId = table.Column<int>(nullable: true),
                    AssetTypeId = table.Column<int>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Custody = table.Column<bool>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: true),
                    DocumentId = table.Column<int>(nullable: true),
                    ERPCode = table.Column<string>(maxLength: 50, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    InvNo = table.Column<string>(maxLength: 30, nullable: true),
                    InvStateId = table.Column<int>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name = table.Column<string>(maxLength: 120, nullable: false),
                    ParentAssetId = table.Column<int>(nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    Quantity = table.Column<float>(nullable: false),
                    RoomId = table.Column<int>(nullable: true),
                    SerialNumber = table.Column<string>(maxLength: 50, nullable: true),
                    UomId = table.Column<int>(nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    ValueInv = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    ValueRem = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_Asset_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_AssetCategory_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategory",
                        principalColumn: "AssetCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_AssetState_AssetStateId",
                        column: x => x.AssetStateId,
                        principalTable: "AssetState",
                        principalColumn: "AssetStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_AssetType_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetType",
                        principalColumn: "AssetTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_InvState_InvStateId",
                        column: x => x.InvStateId,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Asset_ParentAssetId",
                        column: x => x.ParentAssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asset_Uom_UomId",
                        column: x => x.UomId,
                        principalTable: "Uom",
                        principalColumn: "UomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetAC",
                columns: table => new
                {
                    AssetClassTypeId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    AssetClassId = table.Column<int>(nullable: false),
                    AssetClassIdIn = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAC", x => new { x.AssetClassTypeId, x.AssetId });
                    table.ForeignKey(
                        name: "FK_AssetAC_AssetClass_AssetClassId",
                        column: x => x.AssetClassId,
                        principalTable: "AssetClass",
                        principalColumn: "AssetClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAC_AssetClass_AssetClassIdIn",
                        column: x => x.AssetClassIdIn,
                        principalTable: "AssetClass",
                        principalColumn: "AssetClassId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAC_AssetClassType_AssetClassTypeId",
                        column: x => x.AssetClassTypeId,
                        principalTable: "AssetClassType",
                        principalColumn: "AssetClassTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetAC_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetAdmIn",
                columns: table => new
                {
                    AssetId = table.Column<int>(nullable: false),
                    AdministrationId = table.Column<int>(nullable: true),
                    AssetCategoryId = table.Column<int>(nullable: true),
                    AssetStateId = table.Column<int>(nullable: true),
                    AssetTypeId = table.Column<int>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    InvStateId = table.Column<int>(nullable: true),
                    RoomId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAdmIn", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_Administration_AdministrationId",
                        column: x => x.AdministrationId,
                        principalTable: "Administration",
                        principalColumn: "AdministrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_AssetCategory_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategory",
                        principalColumn: "AssetCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_AssetState_AssetStateId",
                        column: x => x.AssetStateId,
                        principalTable: "AssetState",
                        principalColumn: "AssetStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_AssetType_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetType",
                        principalColumn: "AssetTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_CostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_InvState_InvStateId",
                        column: x => x.InvStateId,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdmIn_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetInv",
                columns: table => new
                {
                    AssetId = table.Column<int>(nullable: false),
                    AllowLabel = table.Column<bool>(nullable: true),
                    Barcode = table.Column<string>(maxLength: 30, nullable: true),
                    Info = table.Column<string>(maxLength: 200, nullable: true),
                    InvName = table.Column<string>(maxLength: 120, nullable: true),
                    InvNoOld = table.Column<string>(maxLength: 30, nullable: true),
                    Model = table.Column<string>(maxLength: 30, nullable: true),
                    Producer = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetInv", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_AssetInv_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetNi",
                columns: table => new
                {
                    AssetNiId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowLabel = table.Column<bool>(nullable: false),
                    AssetId = table.Column<int>(nullable: true),
                    Code1 = table.Column<string>(maxLength: 30, nullable: true),
                    Code2 = table.Column<string>(maxLength: 30, nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    EmployeeId = table.Column<int>(nullable: true),
                    Info = table.Column<string>(maxLength: 200, nullable: true),
                    InvStateId = table.Column<int>(nullable: true),
                    InventoryId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Model = table.Column<string>(maxLength: 30, nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Name1 = table.Column<string>(maxLength: 120, nullable: false),
                    Name2 = table.Column<string>(maxLength: 120, nullable: false),
                    Producer = table.Column<string>(maxLength: 30, nullable: true),
                    Quantity = table.Column<float>(nullable: false),
                    RoomId = table.Column<int>(nullable: true),
                    SerialNumber = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetNi", x => x.AssetNiId);
                    table.ForeignKey(
                        name: "FK_AssetNi_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetNi_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetNi_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetNi_InvState_InvStateId",
                        column: x => x.InvStateId,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetNi_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetNi_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAsset",
                columns: table => new
                {
                    InventoryId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    CostCenterIdFinal = table.Column<int>(nullable: true),
                    CostCenterIdInitial = table.Column<int>(nullable: true),
                    DetailStateId = table.Column<int>(nullable: true),
                    EmployeeIdFinal = table.Column<int>(nullable: true),
                    EmployeeIdInitial = table.Column<int>(nullable: true),
                    Model = table.Column<string>(maxLength: 100, nullable: true),
                    Producer = table.Column<string>(maxLength: 100, nullable: true),
                    QFinal = table.Column<float>(nullable: false),
                    QInitial = table.Column<float>(nullable: false),
                    RoomIdFinal = table.Column<int>(nullable: true),
                    RoomIdInitial = table.Column<int>(nullable: true),
                    SerialNumber = table.Column<string>(maxLength: 50, nullable: true),
                    StateIdFinal = table.Column<int>(nullable: true),
                    StateIdInitial = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAsset", x => new { x.InventoryId, x.AssetId });
                    table.ForeignKey(
                        name: "FK_InventoryAsset_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_CostCenter_CostCenterIdFinal",
                        column: x => x.CostCenterIdFinal,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_CostCenter_CostCenterIdInitial",
                        column: x => x.CostCenterIdInitial,
                        principalTable: "CostCenter",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_InvState_DetailStateId",
                        column: x => x.DetailStateId,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_Employee_EmployeeIdFinal",
                        column: x => x.EmployeeIdFinal,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_Employee_EmployeeIdInitial",
                        column: x => x.EmployeeIdInitial,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_Room_RoomIdFinal",
                        column: x => x.RoomIdFinal,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_Room_RoomIdInitial",
                        column: x => x.RoomIdInitial,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_InvState_StateIdFinal",
                        column: x => x.StateIdFinal,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAsset_InvState_StateIdInitial",
                        column: x => x.StateIdInitial,
                        principalTable: "InvState",
                        principalColumn: "InvStateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AdmCenterId",
                table: "AspNetUsers",
                column: "AdmCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AccMonth_CompanyId",
                table: "AccMonth",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccSystem_AssetClassTypeId",
                table: "AccSystem",
                column: "AssetClassTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccSystem_CompanyId",
                table: "AccSystem",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmCenter_CompanyId",
                table: "AdmCenter",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Administration_CompanyId",
                table: "Administration",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Administration_EmployeeId",
                table: "Administration",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AdministrationId",
                table: "Asset",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AssetCategoryId",
                table: "Asset",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AssetStateId",
                table: "Asset",
                column: "AssetStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AssetTypeId",
                table: "Asset",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_CompanyId",
                table: "Asset",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_CostCenterId",
                table: "Asset",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_DepartmentId",
                table: "Asset",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_DocumentId",
                table: "Asset",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_EmployeeId",
                table: "Asset",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InvStateId",
                table: "Asset",
                column: "InvStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_ParentAssetId",
                table: "Asset",
                column: "ParentAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_RoomId",
                table: "Asset",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_UomId",
                table: "Asset",
                column: "UomId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAC_AssetClassId",
                table: "AssetAC",
                column: "AssetClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAC_AssetClassIdIn",
                table: "AssetAC",
                column: "AssetClassIdIn");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAC_AssetId",
                table: "AssetAC",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_AdministrationId",
                table: "AssetAdmIn",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_AssetCategoryId",
                table: "AssetAdmIn",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_AssetStateId",
                table: "AssetAdmIn",
                column: "AssetStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_AssetTypeId",
                table: "AssetAdmIn",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_CostCenterId",
                table: "AssetAdmIn",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_DepartmentId",
                table: "AssetAdmIn",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_EmployeeId",
                table: "AssetAdmIn",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_InvStateId",
                table: "AssetAdmIn",
                column: "InvStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmIn_RoomId",
                table: "AssetAdmIn",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_AdministrationId",
                table: "AssetAdmMD",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_AssetCategoryId",
                table: "AssetAdmMD",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_AssetId",
                table: "AssetAdmMD",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_AssetStateId",
                table: "AssetAdmMD",
                column: "AssetStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_AssetTypeId",
                table: "AssetAdmMD",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_CostCenterId",
                table: "AssetAdmMD",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_DepartmentId",
                table: "AssetAdmMD",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_EmployeeId",
                table: "AssetAdmMD",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdmMD_RoomId",
                table: "AssetAdmMD",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategory_CompanyId",
                table: "AssetCategory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetClass_AssetClassTypeId",
                table: "AssetClass",
                column: "AssetClassTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetClass_CompanyId",
                table: "AssetClass",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetClass_ParentAssetClassId",
                table: "AssetClass",
                column: "ParentAssetClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetClassType_CompanyId",
                table: "AssetClassType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDep_AssetId",
                table: "AssetDep",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMD_AccSystemId",
                table: "AssetDepMD",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDepMD_AssetId",
                table: "AssetDepMD",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNi_AssetId",
                table: "AssetNi",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNi_CompanyId",
                table: "AssetNi",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNi_EmployeeId",
                table: "AssetNi",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNi_InvStateId",
                table: "AssetNi",
                column: "InvStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNi_InventoryId",
                table: "AssetNi",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNi_RoomId",
                table: "AssetNi",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AccSystemId",
                table: "AssetOp",
                column: "AccSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AdministrationIdFinal",
                table: "AssetOp",
                column: "AdministrationIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AdministrationIdInitial",
                table: "AssetOp",
                column: "AdministrationIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetCategoryIdFinal",
                table: "AssetOp",
                column: "AssetCategoryIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetCategoryIdInitial",
                table: "AssetOp",
                column: "AssetCategoryIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetId",
                table: "AssetOp",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetStateIdFinal",
                table: "AssetOp",
                column: "AssetStateIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_AssetStateIdInitial",
                table: "AssetOp",
                column: "AssetStateIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_CostCenterIdFinal",
                table: "AssetOp",
                column: "CostCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_CostCenterIdInitial",
                table: "AssetOp",
                column: "CostCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_DepartmentIdFinal",
                table: "AssetOp",
                column: "DepartmentIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_DepartmentIdInitial",
                table: "AssetOp",
                column: "DepartmentIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_DocumentId",
                table: "AssetOp",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_EmployeeIdFinal",
                table: "AssetOp",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_EmployeeIdInitial",
                table: "AssetOp",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_InvStateIdFinal",
                table: "AssetOp",
                column: "InvStateIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_InvStateIdInitial",
                table: "AssetOp",
                column: "InvStateIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_RoomIdFinal",
                table: "AssetOp",
                column: "RoomIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_AssetOp_RoomIdInitial",
                table: "AssetOp",
                column: "RoomIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_AssetState_CompanyId",
                table: "AssetState",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetType_CompanyId",
                table: "AssetType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_AdmCenterId",
                table: "CostCenter",
                column: "AdmCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenter_CompanyId",
                table: "CostCenter",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CompanyId",
                table: "Department",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_TeamLeaderId",
                table: "Department",
                column: "TeamLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_CompanyId",
                table: "Document",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_CostCenterId",
                table: "Document",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_DocumentTypeId",
                table: "Document",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ParentDocumentId",
                table: "Document",
                column: "ParentDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_PartnerId",
                table: "Document",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentType_CompanyId",
                table: "DocumentType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_AdmCenterId",
                table: "Employee",
                column: "AdmCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CompanyId",
                table: "Employee",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CostCenterId",
                table: "Employee",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_EntityTypeId",
                table: "EntityFile",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_AdministrationId",
                table: "Inventory",
                column: "AdministrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_CompanyId",
                table: "Inventory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_CostCenterId",
                table: "Inventory",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_DocumentId",
                table: "Inventory",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_EmployeeId",
                table: "Inventory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_RoomId",
                table: "Inventory",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_AssetId",
                table: "InventoryAsset",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_CostCenterIdFinal",
                table: "InventoryAsset",
                column: "CostCenterIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_CostCenterIdInitial",
                table: "InventoryAsset",
                column: "CostCenterIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_DetailStateId",
                table: "InventoryAsset",
                column: "DetailStateId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_EmployeeIdFinal",
                table: "InventoryAsset",
                column: "EmployeeIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_EmployeeIdInitial",
                table: "InventoryAsset",
                column: "EmployeeIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_RoomIdFinal",
                table: "InventoryAsset",
                column: "RoomIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_RoomIdInitial",
                table: "InventoryAsset",
                column: "RoomIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_StateIdFinal",
                table: "InventoryAsset",
                column: "StateIdFinal");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAsset_StateIdInitial",
                table: "InventoryAsset",
                column: "StateIdInitial");

            migrationBuilder.CreateIndex(
                name: "IX_InvState_CompanyId",
                table: "InvState",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CompanyId",
                table: "Location",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_CostCenterId",
                table: "Location",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_RegionId",
                table: "Location",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_CompanyId",
                table: "Partner",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Region_CompanyId",
                table: "Region",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_CostCenterId",
                table: "Room",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_LocationId",
                table: "Room",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_ParentRoomId",
                table: "Room",
                column: "ParentRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Uom_CompanyId",
                table: "Uom",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AdmCenter_AdmCenterId",
                table: "AspNetUsers",
                column: "AdmCenterId",
                principalTable: "AdmCenter",
                principalColumn: "AdmCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Employee_EmployeeId",
                table: "AssetAdmMD",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Administration_AdministrationId",
                table: "AssetAdmMD",
                column: "AdministrationId",
                principalTable: "Administration",
                principalColumn: "AdministrationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Department_DepartmentId",
                table: "AssetAdmMD",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdmMD_Asset_AssetId",
                table: "AssetAdmMD",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDepMD_Asset_AssetId",
                table: "AssetDepMD",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetDep_Asset_AssetId",
                table: "AssetDep",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Employee_EmployeeIdFinal",
                table: "AssetOp",
                column: "EmployeeIdFinal",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Employee_EmployeeIdInitial",
                table: "AssetOp",
                column: "EmployeeIdInitial",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Administration_AdministrationIdFinal",
                table: "AssetOp",
                column: "AdministrationIdFinal",
                principalTable: "Administration",
                principalColumn: "AdministrationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Administration_AdministrationIdInitial",
                table: "AssetOp",
                column: "AdministrationIdInitial",
                principalTable: "Administration",
                principalColumn: "AdministrationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Department_DepartmentIdFinal",
                table: "AssetOp",
                column: "DepartmentIdFinal",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Department_DepartmentIdInitial",
                table: "AssetOp",
                column: "DepartmentIdInitial",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetOp_Asset_AssetId",
                table: "AssetOp",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Department_DepartmentId",
                table: "Employee",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AdmCenter_AdmCenterId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AdmCenter_Company_CompanyId",
                table: "AdmCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenter_Company_CompanyId",
                table: "CostCenter");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Company_CompanyId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Company_CompanyId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Employee_TeamLeaderId",
                table: "Department");

            migrationBuilder.DropTable(
                name: "AssetAC");

            migrationBuilder.DropTable(
                name: "AssetAdmIn");

            migrationBuilder.DropTable(
                name: "AssetAdmMD");

            migrationBuilder.DropTable(
                name: "AssetDep");

            migrationBuilder.DropTable(
                name: "AssetDepMD");

            migrationBuilder.DropTable(
                name: "AssetInv");

            migrationBuilder.DropTable(
                name: "AssetNi");

            migrationBuilder.DropTable(
                name: "AssetOp");

            migrationBuilder.DropTable(
                name: "EntityFile");

            migrationBuilder.DropTable(
                name: "InventoryAsset");

            migrationBuilder.DropTable(
                name: "AssetClass");

            migrationBuilder.DropTable(
                name: "AccMonth");

            migrationBuilder.DropTable(
                name: "AccSystem");

            migrationBuilder.DropTable(
                name: "EntityType");

            migrationBuilder.DropTable(
                name: "Asset");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "AssetClassType");

            migrationBuilder.DropTable(
                name: "AssetCategory");

            migrationBuilder.DropTable(
                name: "AssetState");

            migrationBuilder.DropTable(
                name: "AssetType");

            migrationBuilder.DropTable(
                name: "InvState");

            migrationBuilder.DropTable(
                name: "Uom");

            migrationBuilder.DropTable(
                name: "Administration");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "DocumentType");

            migrationBuilder.DropTable(
                name: "Partner");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "CostCenter");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "AdmCenter");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AdmCenterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdmCenterId",
                table: "AspNetUsers");
        }
    }
}
