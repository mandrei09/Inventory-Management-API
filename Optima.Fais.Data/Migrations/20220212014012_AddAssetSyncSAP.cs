using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddAssetSyncSAP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DictionaryItemId",
                table: "Brand",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostCenterEmpId",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotSync",
                table: "Asset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AcquisitionAssetSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ASSET = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    COMPANYCODE = table.Column<string>(nullable: true),
                    CURRENCY = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DOC_DATE = table.Column<string>(nullable: true),
                    EXCH_RATE = table.Column<decimal>(nullable: false),
                    GL_ACCOUNT = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    HEADER_TXT = table.Column<string>(nullable: true),
                    ITEM_TEXT = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NET_AMOUNT = table.Column<decimal>(nullable: false),
                    NotSync = table.Column<bool>(nullable: false),
                    PSTNG_DATE = table.Column<string>(nullable: true),
                    REF_DOC_NO = table.Column<string>(nullable: true),
                    STORNO = table.Column<string>(nullable: true),
                    SUBNUMBER = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    TAX_AMOUNT = table.Column<decimal>(nullable: false),
                    TAX_CODE = table.Column<string>(nullable: true),
                    TOTAL_AMOUNT = table.Column<decimal>(nullable: false),
                    VENDOR_NO = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcquisitionAssetSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcquisitionAssetSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcquisitionAssetSAP_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcquisitionAssetSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetChangeSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ASSET = table.Column<string>(nullable: true),
                    ASSETCLASS = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    BASE_UOM = table.Column<string>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    CAP_DATE = table.Column<string>(nullable: true),
                    COMPANYCODE = table.Column<string>(nullable: true),
                    COSTCENTER = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DESCRIPT = table.Column<string>(nullable: true),
                    DESCRIPT2 = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    INTERN_ORD = table.Column<string>(nullable: true),
                    INVENT_NO = table.Column<string>(nullable: true),
                    IN_CONSERVATION = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LAST_INVENTORY_DATE = table.Column<string>(nullable: true),
                    LAST_INVENTORY_DOCNO = table.Column<string>(nullable: true),
                    LOCATION = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    OPTIMA_ASSET_NO = table.Column<string>(nullable: true),
                    OPTIMA_ASSET_PARENT_NO = table.Column<string>(nullable: true),
                    PERSON_NO = table.Column<string>(nullable: true),
                    PLANT = table.Column<string>(nullable: true),
                    PLATE_NO = table.Column<string>(nullable: true),
                    POSTCAP = table.Column<string>(nullable: true),
                    PROP_IND = table.Column<string>(nullable: true),
                    QUANTITY = table.Column<int>(nullable: false),
                    ROOM = table.Column<string>(nullable: true),
                    SERIAL_NO = table.Column<string>(nullable: true),
                    SUBNUMBER = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    VENDOR_NO = table.Column<string>(nullable: true),
                    ZZCLAS = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetChangeSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetChangeSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetChangeSAP_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetChangeSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetInvMinusSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AMOUNT = table.Column<decimal>(nullable: false),
                    ASSET = table.Column<string>(nullable: true),
                    ASVAL_DATE = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    COMPANYCODE = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DOC_DATE = table.Column<string>(nullable: true),
                    DOC_TYPE = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    INVENTORY_DIFF = table.Column<string>(nullable: true),
                    ITEM_TEXT = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    PSTNG_DATE = table.Column<string>(nullable: true),
                    REF_DOC_NO = table.Column<string>(nullable: true),
                    SUBNUMBER = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    TRANSTYPE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetInvMinusSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetInvMinusSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetInvMinusSAP_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetInvMinusSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetInvPlusSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AMOUNT = table.Column<decimal>(nullable: false),
                    ASSET = table.Column<string>(nullable: true),
                    ASVAL_DATE = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    COMPANYCODE = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DOC_DATE = table.Column<string>(nullable: true),
                    DOC_TYPE = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    ITEM_TEXT = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    PSTNG_DATE = table.Column<string>(nullable: true),
                    REF_DOC_NO = table.Column<string>(nullable: true),
                    SUBNUMBER = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    TRANSTYPE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetInvPlusSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetInvPlusSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetInvPlusSAP_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetInvPlusSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreateAssetSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ASSET = table.Column<string>(nullable: true),
                    ASSETCLASS = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    BASE_UOM = table.Column<string>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    CAP_DATE = table.Column<string>(nullable: true),
                    COMPANYCODE = table.Column<string>(nullable: true),
                    COSTCENTER = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DESCRIPT = table.Column<string>(nullable: true),
                    DESCRIPT2 = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    INTERN_ORD = table.Column<string>(nullable: true),
                    INVENT_NO = table.Column<string>(nullable: true),
                    IN_CONSERVATION = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LAST_INVENTORY_DATE = table.Column<string>(nullable: true),
                    LAST_INVENTORY_DOCNO = table.Column<string>(nullable: true),
                    LOCATION = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    OPTIMA_ASSET_NO = table.Column<string>(nullable: true),
                    OPTIMA_ASSET_PARENT_NO = table.Column<string>(nullable: true),
                    PERSON_NO = table.Column<string>(nullable: true),
                    PLANT = table.Column<string>(nullable: true),
                    PLATE_NO = table.Column<string>(nullable: true),
                    POSTCAP = table.Column<string>(nullable: true),
                    PROP_IND = table.Column<string>(nullable: true),
                    QUANTITY = table.Column<int>(nullable: false),
                    RESP_CCTR = table.Column<string>(nullable: true),
                    ROOM = table.Column<string>(nullable: true),
                    SERIAL_NO = table.Column<string>(nullable: true),
                    SUBNUMBER = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    TESTRUN = table.Column<string>(nullable: true),
                    VENDOR_NO = table.Column<string>(nullable: true),
                    XSUBNO = table.Column<string>(nullable: true),
                    ZZCLAS = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateAssetSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreateAssetSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreateAssetSAP_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreateAssetSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RetireAssetSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AMOUNT = table.Column<decimal>(nullable: false),
                    ASSET = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    BASE_UOM = table.Column<string>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    COMPANYCODE = table.Column<string>(nullable: true),
                    COMPL_RET = table.Column<string>(nullable: true),
                    CURRENCY = table.Column<string>(nullable: true),
                    CURRENT_YEAR_ACQUISITIONS = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DOC_DATE = table.Column<string>(nullable: true),
                    DOC_TYPE = table.Column<string>(nullable: true),
                    FIS_PERIOD = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    ITEM_TEXT = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    PERCENT = table.Column<decimal>(nullable: false),
                    PRIOR_YEAR_ACQUISITIONS = table.Column<string>(nullable: true),
                    PSTNG_DATE = table.Column<string>(nullable: true),
                    QUANTITY = table.Column<decimal>(nullable: false),
                    REF_DOC_NO = table.Column<string>(nullable: true),
                    SUBNUMBER = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    VALUEDATE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetireAssetSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetireAssetSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RetireAssetSAP_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RetireAssetSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferAssetSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AMOUNT = table.Column<decimal>(nullable: false),
                    ASVAL_DATE = table.Column<string>(nullable: true),
                    AccMonthId = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    BASE_UOM = table.Column<string>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    COMPANYCODE = table.Column<string>(nullable: true),
                    COMPL_TRANSFER = table.Column<string>(nullable: true),
                    CURRENCY = table.Column<string>(nullable: true),
                    CURRENT_YEAR_ACQUISITIONS = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DOC_DATE = table.Column<string>(nullable: true),
                    DOC_TYPE = table.Column<string>(nullable: true),
                    FIS_PERIOD = table.Column<string>(nullable: true),
                    FROM_ASSET = table.Column<string>(nullable: true),
                    FROM_SUBNUMBER = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    ITEM_TEXT = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    PERCENT = table.Column<decimal>(nullable: false),
                    PRIOR_YEAR_ACQUISITIONS = table.Column<string>(nullable: true),
                    PSTNG_DATE = table.Column<string>(nullable: true),
                    QUANTITY = table.Column<decimal>(nullable: false),
                    REF_DOC_NO = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    TO_ASSET = table.Column<string>(nullable: true),
                    TO_SUBNUMBER = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferAssetSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferAssetSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferAssetSAP_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferAssetSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferInStockSAP",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccMonthId = table.Column<int>(nullable: false),
                    Asset = table.Column<string>(nullable: true),
                    AssetStockId = table.Column<int>(nullable: false),
                    Batch = table.Column<string>(nullable: true),
                    BudgetManagerId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Doc_Date = table.Column<string>(nullable: true),
                    Gl_Account = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Item_Text = table.Column<string>(nullable: true),
                    Material = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    NotSync = table.Column<bool>(nullable: false),
                    Plant = table.Column<string>(nullable: true),
                    Pstng_Date = table.Column<string>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: false),
                    Storage_Location = table.Column<string>(nullable: true),
                    SubNumber = table.Column<string>(nullable: true),
                    SyncErrorCount = table.Column<int>(nullable: false),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferInStockSAP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferInStockSAP_AccMonth_AccMonthId",
                        column: x => x.AccMonthId,
                        principalTable: "AccMonth",
                        principalColumn: "AccMonthId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferInStockSAP_Asset_AssetStockId",
                        column: x => x.AssetStockId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferInStockSAP_BudgetManager_BudgetManagerId",
                        column: x => x.BudgetManagerId,
                        principalTable: "BudgetManager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brand_DictionaryItemId",
                table: "Brand",
                column: "DictionaryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_CostCenterEmpId",
                table: "Asset",
                column: "CostCenterEmpId");

            migrationBuilder.CreateIndex(
                name: "IX_AcquisitionAssetSAP_AccMonthId",
                table: "AcquisitionAssetSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_AcquisitionAssetSAP_AssetId",
                table: "AcquisitionAssetSAP",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AcquisitionAssetSAP_BudgetManagerId",
                table: "AcquisitionAssetSAP",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetChangeSAP_AccMonthId",
                table: "AssetChangeSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetChangeSAP_AssetId",
                table: "AssetChangeSAP",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetChangeSAP_BudgetManagerId",
                table: "AssetChangeSAP",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvMinusSAP_AccMonthId",
                table: "AssetInvMinusSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvMinusSAP_AssetId",
                table: "AssetInvMinusSAP",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvMinusSAP_BudgetManagerId",
                table: "AssetInvMinusSAP",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvPlusSAP_AccMonthId",
                table: "AssetInvPlusSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvPlusSAP_AssetId",
                table: "AssetInvPlusSAP",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetInvPlusSAP_BudgetManagerId",
                table: "AssetInvPlusSAP",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateAssetSAP_AccMonthId",
                table: "CreateAssetSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateAssetSAP_AssetId",
                table: "CreateAssetSAP",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CreateAssetSAP_BudgetManagerId",
                table: "CreateAssetSAP",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_RetireAssetSAP_AccMonthId",
                table: "RetireAssetSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_RetireAssetSAP_AssetId",
                table: "RetireAssetSAP",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_RetireAssetSAP_BudgetManagerId",
                table: "RetireAssetSAP",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferAssetSAP_AccMonthId",
                table: "TransferAssetSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferAssetSAP_AssetId",
                table: "TransferAssetSAP",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferAssetSAP_BudgetManagerId",
                table: "TransferAssetSAP",
                column: "BudgetManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferInStockSAP_AccMonthId",
                table: "TransferInStockSAP",
                column: "AccMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferInStockSAP_AssetStockId",
                table: "TransferInStockSAP",
                column: "AssetStockId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferInStockSAP_BudgetManagerId",
                table: "TransferInStockSAP",
                column: "BudgetManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_CostCenter_CostCenterEmpId",
                table: "Asset",
                column: "CostCenterEmpId",
                principalTable: "CostCenter",
                principalColumn: "CostCenterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_DictionaryItem_DictionaryItemId",
                table: "Brand",
                column: "DictionaryItemId",
                principalTable: "DictionaryItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_CostCenter_CostCenterEmpId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Brand_DictionaryItem_DictionaryItemId",
                table: "Brand");

            migrationBuilder.DropTable(
                name: "AcquisitionAssetSAP");

            migrationBuilder.DropTable(
                name: "AssetChangeSAP");

            migrationBuilder.DropTable(
                name: "AssetInvMinusSAP");

            migrationBuilder.DropTable(
                name: "AssetInvPlusSAP");

            migrationBuilder.DropTable(
                name: "CreateAssetSAP");

            migrationBuilder.DropTable(
                name: "RetireAssetSAP");

            migrationBuilder.DropTable(
                name: "TransferAssetSAP");

            migrationBuilder.DropTable(
                name: "TransferInStockSAP");

            migrationBuilder.DropIndex(
                name: "IX_Brand_DictionaryItemId",
                table: "Brand");

            migrationBuilder.DropIndex(
                name: "IX_Asset_CostCenterEmpId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "DictionaryItemId",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "CostCenterEmpId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "NotSync",
                table: "Asset");
        }
    }
}
