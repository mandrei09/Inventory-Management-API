using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Optima.Fais.Data;

namespace Optima.Fais.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190113083524_AddLocationAdmCenterColumn")]
    partial class AddLocationAdmCenterColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Optima.Fais.Model.AccMonth", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AccMonthId");

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<int>("Month");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("AccMonth");
                });

            modelBuilder.Entity("Optima.Fais.Model.AccSystem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AccSystemId");

                    b.Property<int>("AssetClassTypeId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("AssetClassTypeId");

                    b.HasIndex("CompanyId");

                    b.ToTable("AccSystem");
                });

            modelBuilder.Entity("Optima.Fais.Model.AdmCenter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AdmCenterId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("AdmCenter");
                });

            modelBuilder.Entity("Optima.Fais.Model.Administration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AdministrationId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<int?>("CostCenterId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<int?>("DivisionId");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("DivisionId");

                    b.ToTable("Administration");
                });

            modelBuilder.Entity("Optima.Fais.Model.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int?>("AdmCenterId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int?>("EmployeeId");

                    b.Property<string>("FamilyName");

                    b.Property<string>("GivenName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("AdmCenterId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Optima.Fais.Model.AppState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AppStateId");

                    b.Property<string>("Code")
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Mask")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("ParentCode")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("AppState");
                });

            modelBuilder.Entity("Optima.Fais.Model.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetId");

                    b.Property<int?>("AdministrationId");

                    b.Property<int?>("AssetCategoryId");

                    b.Property<int?>("AssetStateId");

                    b.Property<int?>("AssetTypeId");

                    b.Property<int?>("CompanyId");

                    b.Property<int?>("CostCenterId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool?>("Custody");

                    b.Property<int?>("DepartmentId");

                    b.Property<int?>("DocumentId");

                    b.Property<string>("ERPCode")
                        .HasMaxLength(50);

                    b.Property<int?>("EmployeeId");

                    b.Property<string>("InvNo")
                        .HasMaxLength(30);

                    b.Property<int?>("InvStateId");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<int?>("ParentAssetId");

                    b.Property<DateTime?>("PurchaseDate")
                        .HasColumnType("date");

                    b.Property<float>("Quantity");

                    b.Property<int?>("RoomId");

                    b.Property<string>("SAPCode");

                    b.Property<string>("SerialNumber")
                        .HasMaxLength(50);

                    b.Property<int?>("UomId");

                    b.Property<bool>("Validated");

                    b.Property<decimal>("ValueInv")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueRem")
                        .HasColumnType("decimal(18, 4)");

                    b.HasKey("Id");

                    b.HasIndex("AdministrationId");

                    b.HasIndex("AssetCategoryId");

                    b.HasIndex("AssetStateId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("DocumentId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("InvStateId");

                    b.HasIndex("ParentAssetId");

                    b.HasIndex("RoomId");

                    b.HasIndex("UomId");

                    b.ToTable("Asset");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetAC", b =>
                {
                    b.Property<int>("AssetClassTypeId");

                    b.Property<int>("AssetId");

                    b.Property<int>("AssetClassId");

                    b.Property<int>("AssetClassIdIn");

                    b.HasKey("AssetClassTypeId", "AssetId");

                    b.HasIndex("AssetClassId");

                    b.HasIndex("AssetClassIdIn");

                    b.HasIndex("AssetId");

                    b.ToTable("AssetAC");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetAdmIn", b =>
                {
                    b.Property<int>("AssetId")
                        .HasColumnName("AssetId");

                    b.Property<int?>("AdministrationId");

                    b.Property<int?>("AssetCategoryId");

                    b.Property<int?>("AssetStateId");

                    b.Property<int?>("AssetTypeId");

                    b.Property<int?>("CostCenterId");

                    b.Property<int?>("DepartmentId");

                    b.Property<int?>("EmployeeId");

                    b.Property<int?>("InvStateId");

                    b.Property<int?>("RoomId");

                    b.HasKey("AssetId");

                    b.HasIndex("AdministrationId");

                    b.HasIndex("AssetCategoryId");

                    b.HasIndex("AssetStateId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("InvStateId");

                    b.HasIndex("RoomId");

                    b.ToTable("AssetAdmIn");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetAdmMD", b =>
                {
                    b.Property<int>("AccMonthId");

                    b.Property<int>("AssetId");

                    b.Property<int?>("AdministrationId");

                    b.Property<int?>("AssetCategoryId");

                    b.Property<int?>("AssetStateId");

                    b.Property<int?>("AssetTypeId");

                    b.Property<int?>("CostCenterId");

                    b.Property<int?>("DepartmentId");

                    b.Property<int?>("EmployeeId");

                    b.Property<int?>("RoomId");

                    b.HasKey("AccMonthId", "AssetId");

                    b.HasIndex("AdministrationId");

                    b.HasIndex("AssetCategoryId");

                    b.HasIndex("AssetId");

                    b.HasIndex("AssetStateId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("RoomId");

                    b.ToTable("AssetAdmMD");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetCategoryId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Prefix")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("AssetCategory");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetClassId");

                    b.Property<int>("AssetClassTypeId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<int>("DepPeriodDefault");

                    b.Property<int>("DepPeriodMax");

                    b.Property<int>("DepPeriodMin");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("ParentAssetClassId");

                    b.HasKey("Id");

                    b.HasIndex("AssetClassTypeId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("ParentAssetClassId");

                    b.ToTable("AssetClass");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetClassType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetClassTypeId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("AssetClassType");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetDep", b =>
                {
                    b.Property<int>("AccSystemId");

                    b.Property<int>("AssetId");

                    b.Property<int>("DepPeriod");

                    b.Property<int>("DepPeriodIn");

                    b.Property<int>("DepPeriodMonth");

                    b.Property<int>("DepPeriodMonthIn");

                    b.Property<int>("DepPeriodRem");

                    b.Property<int>("DepPeriodRemIn");

                    b.Property<bool?>("DirectExpense");

                    b.Property<DateTime?>("UsageEndDate")
                        .HasColumnType("date");

                    b.Property<DateTime?>("UsageStartDate")
                        .HasColumnType("date");

                    b.Property<decimal>("ValueDep")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueDepIn")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueDepPU")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueDepPUIn")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueDepYTD")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueDepYTDIn")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueInv")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueInvIn")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueRem")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueRemIn")
                        .HasColumnType("decimal(18, 4)");

                    b.HasKey("AccSystemId", "AssetId");

                    b.HasIndex("AssetId");

                    b.ToTable("AssetDep");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetDepMD", b =>
                {
                    b.Property<int>("AccMonthId");

                    b.Property<int>("AccSystemId");

                    b.Property<int>("AssetId");

                    b.Property<int>("DepPeriod");

                    b.Property<int>("DepPeriodMonth");

                    b.Property<int>("DepPeriodRem");

                    b.Property<decimal>("ValueDep")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueDepPU")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueDepYTD")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueInv")
                        .HasColumnType("decimal(18, 4)");

                    b.Property<decimal>("ValueRem")
                        .HasColumnType("decimal(18, 4)");

                    b.HasKey("AccMonthId", "AccSystemId", "AssetId");

                    b.HasIndex("AccSystemId");

                    b.HasIndex("AssetId");

                    b.ToTable("AssetDepMD");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetInv", b =>
                {
                    b.Property<int>("AssetId");

                    b.Property<bool?>("AllowLabel");

                    b.Property<string>("Barcode")
                        .HasMaxLength(30);

                    b.Property<string>("Info")
                        .HasMaxLength(200);

                    b.Property<string>("InvName")
                        .HasMaxLength(120);

                    b.Property<string>("InvNoOld")
                        .HasMaxLength(30);

                    b.Property<int>("InvStateId");

                    b.Property<string>("Model")
                        .HasMaxLength(30);

                    b.Property<string>("Producer")
                        .HasMaxLength(30);

                    b.HasKey("AssetId");

                    b.HasIndex("InvStateId");

                    b.ToTable("AssetInv");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetNi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetNiId");

                    b.Property<bool>("AllowLabel");

                    b.Property<int?>("AssetId");

                    b.Property<string>("Code1")
                        .HasMaxLength(30);

                    b.Property<string>("Code2")
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<int?>("EmployeeId");

                    b.Property<string>("Info")
                        .HasMaxLength(200);

                    b.Property<int?>("InvStateId");

                    b.Property<int>("InventoryId");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Model")
                        .HasMaxLength(30);

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name1")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<string>("Name2")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<string>("Producer")
                        .HasMaxLength(30);

                    b.Property<float>("Quantity");

                    b.Property<int?>("RoomId");

                    b.Property<string>("SerialNumber")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("InvStateId");

                    b.HasIndex("InventoryId");

                    b.HasIndex("RoomId");

                    b.ToTable("AssetNi");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetOp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetOpId");

                    b.Property<int?>("AccSystemId");

                    b.Property<int?>("AdministrationIdFinal");

                    b.Property<int?>("AdministrationIdInitial");

                    b.Property<int?>("AssetCategoryIdFinal");

                    b.Property<int?>("AssetCategoryIdInitial");

                    b.Property<int>("AssetId");

                    b.Property<int?>("AssetOpStateId");

                    b.Property<int?>("AssetStateIdFinal");

                    b.Property<int?>("AssetStateIdInitial");

                    b.Property<int?>("CostCenterIdFinal");

                    b.Property<int?>("CostCenterIdInitial");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool?>("DepUpdate");

                    b.Property<int?>("DepartmentIdFinal");

                    b.Property<int?>("DepartmentIdInitial");

                    b.Property<int>("DocumentId");

                    b.Property<DateTime?>("DstConfAt");

                    b.Property<string>("DstConfBy")
                        .HasMaxLength(450);

                    b.Property<int?>("EmployeeIdFinal");

                    b.Property<int?>("EmployeeIdInitial");

                    b.Property<string>("Info")
                        .HasMaxLength(200);

                    b.Property<string>("InvName");

                    b.Property<int?>("InvStateIdFinal");

                    b.Property<int?>("InvStateIdInitial");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<DateTime?>("RegisterConfAt");

                    b.Property<string>("RegisterConfBy")
                        .HasMaxLength(450);

                    b.Property<DateTime?>("ReleaseConfAt");

                    b.Property<string>("ReleaseConfBy")
                        .HasMaxLength(450);

                    b.Property<int?>("RoomIdFinal");

                    b.Property<int?>("RoomIdInitial");

                    b.Property<DateTime?>("SrcConfAt");

                    b.Property<string>("SrcConfBy")
                        .HasMaxLength(450);

                    b.Property<decimal?>("ValueAdd");

                    b.HasKey("Id");

                    b.HasIndex("AccSystemId");

                    b.HasIndex("AdministrationIdFinal");

                    b.HasIndex("AdministrationIdInitial");

                    b.HasIndex("AssetCategoryIdFinal");

                    b.HasIndex("AssetCategoryIdInitial");

                    b.HasIndex("AssetId");

                    b.HasIndex("AssetOpStateId");

                    b.HasIndex("AssetStateIdFinal");

                    b.HasIndex("AssetStateIdInitial");

                    b.HasIndex("CostCenterIdFinal");

                    b.HasIndex("CostCenterIdInitial");

                    b.HasIndex("DepartmentIdFinal");

                    b.HasIndex("DepartmentIdInitial");

                    b.HasIndex("DocumentId");

                    b.HasIndex("DstConfBy");

                    b.HasIndex("EmployeeIdFinal");

                    b.HasIndex("EmployeeIdInitial");

                    b.HasIndex("InvStateIdFinal");

                    b.HasIndex("InvStateIdInitial");

                    b.HasIndex("RegisterConfBy");

                    b.HasIndex("ReleaseConfBy");

                    b.HasIndex("RoomIdFinal");

                    b.HasIndex("RoomIdInitial");

                    b.HasIndex("SrcConfBy");

                    b.ToTable("AssetOp");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetStateId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("HasDep");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsFinal");

                    b.Property<bool>("IsInitial");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("AssetState");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("AssetTypeId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("AssetType");
                });

            modelBuilder.Entity("Optima.Fais.Model.ColumnDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ColumnDefinitionId");

                    b.Property<bool>("Active");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Format")
                        .HasMaxLength(50);

                    b.Property<string>("HeaderCode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Include")
                        .HasMaxLength(100);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Pipe")
                        .HasMaxLength(50);

                    b.Property<int>("Position");

                    b.Property<string>("Property")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("SortBy")
                        .HasMaxLength(50);

                    b.Property<int>("TableDefinitionId");

                    b.Property<string>("TextAlign")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("TableDefinitionId");

                    b.ToTable("ColumnDefinition");
                });

            modelBuilder.Entity("Optima.Fais.Model.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CompanyId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("Optima.Fais.Model.ConfigValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ConfigValueId");

                    b.Property<bool?>("BoolValue");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<DateTime?>("DateValue")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Group")
                        .HasMaxLength(50);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<decimal?>("NumericValue")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("TextValue")
                        .HasMaxLength(200);

                    b.Property<string>("ValueType")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("ConfigValue");
                });

            modelBuilder.Entity("Optima.Fais.Model.CostCenter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CostCenterId");

                    b.Property<int?>("AdmCenterId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("ERPCode")
                        .HasMaxLength(50);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("AdmCenterId");

                    b.HasIndex("CompanyId");

                    b.ToTable("CostCenter");
                });

            modelBuilder.Entity("Optima.Fais.Model.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DepartmentId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("ERPCode")
                        .HasMaxLength(50);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int?>("TeamLeaderId");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("TeamLeaderId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("Optima.Fais.Model.DictionaryItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<int?>("AssetCategoryId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<int?>("DictionaryTypeId");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("AssetCategoryId");

                    b.HasIndex("DictionaryTypeId");

                    b.ToTable("DictionaryItem");
                });

            modelBuilder.Entity("Optima.Fais.Model.DictionaryType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("DictionaryType");
                });

            modelBuilder.Entity("Optima.Fais.Model.Division", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DivisionId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Division");
                });

            modelBuilder.Entity("Optima.Fais.Model.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DocumentId");

                    b.Property<bool?>("Approved");

                    b.Property<int?>("CompanyId");

                    b.Property<int?>("CostCenterId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("date");

                    b.Property<string>("Details")
                        .HasMaxLength(255);

                    b.Property<string>("DocNo1")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("DocNo2")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("DocumentDate")
                        .HasColumnType("date");

                    b.Property<int>("DocumentTypeId");

                    b.Property<bool?>("Exported");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<int?>("ParentDocumentId");

                    b.Property<int?>("PartnerId");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("date");

                    b.Property<DateTime?>("ValidationDate");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("DocumentTypeId");

                    b.HasIndex("ParentDocumentId");

                    b.HasIndex("PartnerId");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("Optima.Fais.Model.DocumentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DocumentTypeId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Mask")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ParentCode")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Prefix")
                        .HasMaxLength(10);

                    b.Property<string>("Suffix")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("DocumentType");
                });

            modelBuilder.Entity("Optima.Fais.Model.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("EmployeeId");

                    b.Property<int?>("AdmCenterId");

                    b.Property<int?>("CompanyId");

                    b.Property<int?>("CostCenterId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<int?>("DepartmentId");

                    b.Property<int?>("DivisionId");

                    b.Property<string>("ERPCode")
                        .HasMaxLength(50);

                    b.Property<string>("Email")
                        .HasMaxLength(100);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("InternalCode")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("AdmCenterId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("DivisionId");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("Optima.Fais.Model.EntityFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("EntityFileId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<int>("EntityId");

                    b.Property<int>("EntityTypeId");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Info")
                        .HasMaxLength(100);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<double>("Size");

                    b.Property<string>("StoredAs")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("EntityTypeId");

                    b.ToTable("EntityFile");
                });

            modelBuilder.Entity("Optima.Fais.Model.EntityType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("EntityTypeId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("UploadFolder")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("EntityType");
                });

            modelBuilder.Entity("Optima.Fais.Model.Inventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("InventoryId");

                    b.Property<bool>("Active");

                    b.Property<int?>("AdministrationId");

                    b.Property<int?>("CompanyId");

                    b.Property<int?>("CostCenterId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("DocumentId");

                    b.Property<int?>("EmployeeId");

                    b.Property<DateTime?>("End")
                        .HasColumnType("date");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<int?>("RoomId");

                    b.Property<DateTime?>("Start")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("AdministrationId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("DocumentId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("RoomId");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("Optima.Fais.Model.InventoryAsset", b =>
                {
                    b.Property<int>("InventoryId");

                    b.Property<int>("AssetId");

                    b.Property<int?>("AdministrationIdFinal");

                    b.Property<int?>("AdministrationIdInitial");

                    b.Property<int?>("CostCenterIdFinal");

                    b.Property<int?>("CostCenterIdInitial");

                    b.Property<int?>("DetailStateId");

                    b.Property<int?>("EmployeeIdFinal");

                    b.Property<int?>("EmployeeIdInitial");

                    b.Property<string>("Info")
                        .HasMaxLength(200);

                    b.Property<string>("Model")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Producer")
                        .HasMaxLength(100);

                    b.Property<float>("QFinal");

                    b.Property<float>("QInitial");

                    b.Property<int?>("RoomIdFinal");

                    b.Property<int?>("RoomIdInitial");

                    b.Property<string>("SerialNumber")
                        .HasMaxLength(50);

                    b.Property<int?>("StateIdFinal");

                    b.Property<int?>("StateIdInitial");

                    b.HasKey("InventoryId", "AssetId");

                    b.HasIndex("AdministrationIdFinal");

                    b.HasIndex("AdministrationIdInitial");

                    b.HasIndex("AssetId")
                        .IsUnique();

                    b.HasIndex("CostCenterIdFinal");

                    b.HasIndex("CostCenterIdInitial");

                    b.HasIndex("DetailStateId");

                    b.HasIndex("EmployeeIdFinal");

                    b.HasIndex("EmployeeIdInitial");

                    b.HasIndex("ModifiedBy");

                    b.HasIndex("RoomIdFinal");

                    b.HasIndex("RoomIdInitial");

                    b.HasIndex("StateIdFinal");

                    b.HasIndex("StateIdInitial");

                    b.ToTable("InventoryAsset");
                });

            modelBuilder.Entity("Optima.Fais.Model.InventoryListApn", b =>
                {
                    b.Property<int>("AssetId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AssetCategory");

                    b.Property<string>("AssetStateFinal");

                    b.Property<string>("AssetStateInitial");

                    b.Property<string>("CostCenterNameFinal");

                    b.Property<string>("CostCenterNameInitial");

                    b.Property<string>("Description");

                    b.Property<string>("GpsCoordinates");

                    b.Property<string>("Info");

                    b.Property<string>("InvNo");

                    b.Property<string>("InvNoParent");

                    b.Property<DateTime?>("InventoryDate");

                    b.Property<string>("LocationNameFinal");

                    b.Property<string>("LocationNameInitial");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<float>("QFinal");

                    b.Property<float>("Qinitial");

                    b.Property<string>("RoomCodeFinal");

                    b.Property<string>("RoomCodeInitial");

                    b.Property<string>("RoomNameFinal");

                    b.Property<string>("RoomNameInitial");

                    b.Property<string>("SerialNumber");

                    b.Property<string>("StoredAs");

                    b.Property<string>("StreetCodeFinal");

                    b.Property<string>("StreetCodeInitial");

                    b.Property<string>("StreetNameFinal");

                    b.Property<string>("StreetNameInitial");

                    b.Property<string>("Um");

                    b.Property<string>("UserEmployeeFullNameFinal");

                    b.Property<string>("UserEmployeeFullNameInitial");

                    b.Property<string>("UserEmployeeInternalCodeFinal");

                    b.Property<string>("UserEmployeeInternalCodeInitial");

                    b.HasKey("AssetId");

                    b.ToTable("InventoryListApns");
                });

            modelBuilder.Entity("Optima.Fais.Model.InvState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("InvStateId");

                    b.Property<string>("Code")
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Mask")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("ParentCode")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("InvState");
                });

            modelBuilder.Entity("Optima.Fais.Model.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("LocationId");

                    b.Property<string>("Address")
                        .HasMaxLength(200);

                    b.Property<int?>("AdmCenterId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<int?>("CostCenterId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("ERPCode")
                        .HasMaxLength(50);

                    b.Property<bool>("IsDeleted");

                    b.Property<decimal>("Latitude");

                    b.Property<decimal>("Longitude");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Prefix")
                        .HasMaxLength(30);

                    b.Property<int?>("RegionId");

                    b.HasKey("Id");

                    b.HasIndex("AdmCenterId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("RegionId");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("Optima.Fais.Model.Partner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PartnerId");

                    b.Property<string>("Address")
                        .HasMaxLength(200);

                    b.Property<string>("Bank")
                        .HasMaxLength(100);

                    b.Property<string>("BankAccount")
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<string>("ContactInfo")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("ErpCode")
                        .HasMaxLength(30);

                    b.Property<string>("FiscalCode")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PayingAccount")
                        .HasMaxLength(30);

                    b.Property<string>("RegistryNumber")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Partner");
                });

            modelBuilder.Entity("Optima.Fais.Model.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RegionId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Region");
                });

            modelBuilder.Entity("Optima.Fais.Model.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RoomId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CostCenterId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("ERPCode")
                        .HasMaxLength(50);

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("LocationId");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int?>("ParentRoomId");

                    b.HasKey("Id");

                    b.HasIndex("CostCenterId");

                    b.HasIndex("LocationId");

                    b.HasIndex("ParentRoomId");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("Optima.Fais.Model.TableDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TableDefinitionId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("TableDefinition");
                });

            modelBuilder.Entity("Optima.Fais.Model.Uom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UomId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(450);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedAt");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(450);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Uom");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Optima.Fais.Model.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Optima.Fais.Model.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Optima.Fais.Model.AccMonth", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AccSystem", b =>
                {
                    b.HasOne("Optima.Fais.Model.AssetClassType", "AssetClassType")
                        .WithMany()
                        .HasForeignKey("AssetClassTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AdmCenter", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Administration", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Division", "Division")
                        .WithMany("Administrations")
                        .HasForeignKey("DivisionId");
                });

            modelBuilder.Entity("Optima.Fais.Model.ApplicationUser", b =>
                {
                    b.HasOne("Optima.Fais.Model.AdmCenter", "AdmCenter")
                        .WithMany()
                        .HasForeignKey("AdmCenterId");

                    b.HasOne("Optima.Fais.Model.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AppState", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Asset", b =>
                {
                    b.HasOne("Optima.Fais.Model.Administration", "Administration")
                        .WithMany()
                        .HasForeignKey("AdministrationId");

                    b.HasOne("Optima.Fais.Model.AssetCategory", "AssetCategory")
                        .WithMany()
                        .HasForeignKey("AssetCategoryId");

                    b.HasOne("Optima.Fais.Model.AssetState", "AssetState")
                        .WithMany()
                        .HasForeignKey("AssetStateId");

                    b.HasOne("Optima.Fais.Model.AssetType", "AssetType")
                        .WithMany()
                        .HasForeignKey("AssetTypeId");

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("Optima.Fais.Model.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId");

                    b.HasOne("Optima.Fais.Model.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.HasOne("Optima.Fais.Model.InvState", "InvState")
                        .WithMany()
                        .HasForeignKey("InvStateId");

                    b.HasOne("Optima.Fais.Model.Asset", "ParentAsset")
                        .WithMany("ChildAssets")
                        .HasForeignKey("ParentAssetId");

                    b.HasOne("Optima.Fais.Model.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");

                    b.HasOne("Optima.Fais.Model.Uom", "Uom")
                        .WithMany()
                        .HasForeignKey("UomId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetAC", b =>
                {
                    b.HasOne("Optima.Fais.Model.AssetClass", "AssetClass")
                        .WithMany()
                        .HasForeignKey("AssetClassId");

                    b.HasOne("Optima.Fais.Model.AssetClass", "AssetClassIn")
                        .WithMany()
                        .HasForeignKey("AssetClassIdIn");

                    b.HasOne("Optima.Fais.Model.AssetClassType", "AssetClassType")
                        .WithMany()
                        .HasForeignKey("AssetClassTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetAdmIn", b =>
                {
                    b.HasOne("Optima.Fais.Model.Administration", "Administration")
                        .WithMany()
                        .HasForeignKey("AdministrationId");

                    b.HasOne("Optima.Fais.Model.AssetCategory", "AssetCategory")
                        .WithMany()
                        .HasForeignKey("AssetCategoryId");

                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.AssetState", "AssetState")
                        .WithMany()
                        .HasForeignKey("AssetStateId");

                    b.HasOne("Optima.Fais.Model.AssetType", "AssetType")
                        .WithMany()
                        .HasForeignKey("AssetTypeId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("Optima.Fais.Model.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.HasOne("Optima.Fais.Model.InvState", "InvState")
                        .WithMany()
                        .HasForeignKey("InvStateId");

                    b.HasOne("Optima.Fais.Model.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetAdmMD", b =>
                {
                    b.HasOne("Optima.Fais.Model.AccMonth", "AccMonth")
                        .WithMany()
                        .HasForeignKey("AccMonthId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Administration", "Administration")
                        .WithMany()
                        .HasForeignKey("AdministrationId");

                    b.HasOne("Optima.Fais.Model.AssetCategory", "AssetCategory")
                        .WithMany()
                        .HasForeignKey("AssetCategoryId");

                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithMany("AssetAdmMDs")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.AssetState", "AssetState")
                        .WithMany()
                        .HasForeignKey("AssetStateId");

                    b.HasOne("Optima.Fais.Model.AssetType", "AssetType")
                        .WithMany()
                        .HasForeignKey("AssetTypeId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("Optima.Fais.Model.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.HasOne("Optima.Fais.Model.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetCategory", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetClass", b =>
                {
                    b.HasOne("Optima.Fais.Model.AssetClassType", "AssetClassType")
                        .WithMany("AssetClasses")
                        .HasForeignKey("AssetClassTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.AssetClass", "ParentAssetClass")
                        .WithMany("ChildAssetClasses")
                        .HasForeignKey("ParentAssetClassId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetClassType", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetDep", b =>
                {
                    b.HasOne("Optima.Fais.Model.AccSystem", "AccSystem")
                        .WithMany()
                        .HasForeignKey("AccSystemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithMany("AssetDeps")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetDepMD", b =>
                {
                    b.HasOne("Optima.Fais.Model.AccMonth", "AccMonth")
                        .WithMany()
                        .HasForeignKey("AccMonthId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.AccSystem", "AccSystem")
                        .WithMany()
                        .HasForeignKey("AccSystemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithMany("AssetDepMDs")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetInv", b =>
                {
                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithOne("AssetInv")
                        .HasForeignKey("Optima.Fais.Model.AssetInv", "AssetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.InvState", "InvState")
                        .WithMany()
                        .HasForeignKey("InvStateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetNi", b =>
                {
                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId");

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.HasOne("Optima.Fais.Model.InvState", "InvState")
                        .WithMany()
                        .HasForeignKey("InvStateId");

                    b.HasOne("Optima.Fais.Model.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetOp", b =>
                {
                    b.HasOne("Optima.Fais.Model.AccSystem", "AccSystem")
                        .WithMany()
                        .HasForeignKey("AccSystemId");

                    b.HasOne("Optima.Fais.Model.Administration", "AdministrationFinal")
                        .WithMany()
                        .HasForeignKey("AdministrationIdFinal");

                    b.HasOne("Optima.Fais.Model.Administration", "AdministrationInitial")
                        .WithMany()
                        .HasForeignKey("AdministrationIdInitial");

                    b.HasOne("Optima.Fais.Model.AssetCategory", "AssetCategoryFinal")
                        .WithMany()
                        .HasForeignKey("AssetCategoryIdFinal");

                    b.HasOne("Optima.Fais.Model.AssetCategory", "AssetCategoryInitial")
                        .WithMany()
                        .HasForeignKey("AssetCategoryIdInitial");

                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.AppState", "AssetOpState")
                        .WithMany()
                        .HasForeignKey("AssetOpStateId");

                    b.HasOne("Optima.Fais.Model.AssetState", "AssetStateFinal")
                        .WithMany()
                        .HasForeignKey("AssetStateIdFinal");

                    b.HasOne("Optima.Fais.Model.AssetState", "AssetStateInitial")
                        .WithMany()
                        .HasForeignKey("AssetStateIdInitial");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenterFinal")
                        .WithMany()
                        .HasForeignKey("CostCenterIdFinal");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenterInitial")
                        .WithMany()
                        .HasForeignKey("CostCenterIdInitial");

                    b.HasOne("Optima.Fais.Model.Department", "DepartmentFinal")
                        .WithMany()
                        .HasForeignKey("DepartmentIdFinal");

                    b.HasOne("Optima.Fais.Model.Department", "DepartmentInitial")
                        .WithMany()
                        .HasForeignKey("DepartmentIdInitial");

                    b.HasOne("Optima.Fais.Model.Document", "Document")
                        .WithMany("Operations")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.ApplicationUser", "DstConfUser")
                        .WithMany()
                        .HasForeignKey("DstConfBy");

                    b.HasOne("Optima.Fais.Model.Employee", "EmployeeFinal")
                        .WithMany()
                        .HasForeignKey("EmployeeIdFinal");

                    b.HasOne("Optima.Fais.Model.Employee", "EmployeeInitial")
                        .WithMany()
                        .HasForeignKey("EmployeeIdInitial");

                    b.HasOne("Optima.Fais.Model.InvState", "InvStateFinal")
                        .WithMany()
                        .HasForeignKey("InvStateIdFinal");

                    b.HasOne("Optima.Fais.Model.InvState", "InvStateInitial")
                        .WithMany()
                        .HasForeignKey("InvStateIdInitial");

                    b.HasOne("Optima.Fais.Model.ApplicationUser", "RegisterConfUser")
                        .WithMany()
                        .HasForeignKey("RegisterConfBy");

                    b.HasOne("Optima.Fais.Model.ApplicationUser", "ReleaseConfUser")
                        .WithMany()
                        .HasForeignKey("ReleaseConfBy");

                    b.HasOne("Optima.Fais.Model.Room", "RoomFinal")
                        .WithMany()
                        .HasForeignKey("RoomIdFinal");

                    b.HasOne("Optima.Fais.Model.Room", "RoomInitial")
                        .WithMany()
                        .HasForeignKey("RoomIdInitial");

                    b.HasOne("Optima.Fais.Model.ApplicationUser", "SrcConfUser")
                        .WithMany()
                        .HasForeignKey("SrcConfBy");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetState", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.AssetType", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.ColumnDefinition", b =>
                {
                    b.HasOne("Optima.Fais.Model.TableDefinition", "TableDefinition")
                        .WithMany("ColumnDefinitions")
                        .HasForeignKey("TableDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Optima.Fais.Model.ConfigValue", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.CostCenter", b =>
                {
                    b.HasOne("Optima.Fais.Model.AdmCenter", "AdmCenter")
                        .WithMany("CostCenters")
                        .HasForeignKey("AdmCenterId");

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Department", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.Employee", "TeamLeader")
                        .WithMany()
                        .HasForeignKey("TeamLeaderId");
                });

            modelBuilder.Entity("Optima.Fais.Model.DictionaryItem", b =>
                {
                    b.HasOne("Optima.Fais.Model.AssetCategory", "AssetCategory")
                        .WithMany()
                        .HasForeignKey("AssetCategoryId");

                    b.HasOne("Optima.Fais.Model.DictionaryType", "DictionaryType")
                        .WithMany("DictionaryItems")
                        .HasForeignKey("DictionaryTypeId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Document", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.DocumentType", "DocumentType")
                        .WithMany()
                        .HasForeignKey("DocumentTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Document", "ParentDocument")
                        .WithMany("ChildDocuments")
                        .HasForeignKey("ParentDocumentId");

                    b.HasOne("Optima.Fais.Model.Partner", "Partner")
                        .WithMany()
                        .HasForeignKey("PartnerId");
                });

            modelBuilder.Entity("Optima.Fais.Model.DocumentType", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Employee", b =>
                {
                    b.HasOne("Optima.Fais.Model.AdmCenter", "AdmCenter")
                        .WithMany()
                        .HasForeignKey("AdmCenterId");

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.HasOne("Optima.Fais.Model.Division", "Division")
                        .WithMany()
                        .HasForeignKey("DivisionId");
                });

            modelBuilder.Entity("Optima.Fais.Model.EntityFile", b =>
                {
                    b.HasOne("Optima.Fais.Model.EntityType", "EntityType")
                        .WithMany()
                        .HasForeignKey("EntityTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Optima.Fais.Model.Inventory", b =>
                {
                    b.HasOne("Optima.Fais.Model.Administration", "Administration")
                        .WithMany()
                        .HasForeignKey("AdministrationId");

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Document", "Document")
                        .WithMany()
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId");

                    b.HasOne("Optima.Fais.Model.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("Optima.Fais.Model.InventoryAsset", b =>
                {
                    b.HasOne("Optima.Fais.Model.Administration", "AdministrationFinal")
                        .WithMany()
                        .HasForeignKey("AdministrationIdFinal");

                    b.HasOne("Optima.Fais.Model.Administration", "AdministrationInitial")
                        .WithMany()
                        .HasForeignKey("AdministrationIdInitial");

                    b.HasOne("Optima.Fais.Model.Asset", "Asset")
                        .WithOne("InventoryAsset")
                        .HasForeignKey("Optima.Fais.Model.InventoryAsset", "AssetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenterFinal")
                        .WithMany()
                        .HasForeignKey("CostCenterIdFinal");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenterInitial")
                        .WithMany()
                        .HasForeignKey("CostCenterIdInitial");

                    b.HasOne("Optima.Fais.Model.InvState", "DetailState")
                        .WithMany()
                        .HasForeignKey("DetailStateId");

                    b.HasOne("Optima.Fais.Model.Employee", "EmployeeFinal")
                        .WithMany()
                        .HasForeignKey("EmployeeIdFinal");

                    b.HasOne("Optima.Fais.Model.Employee", "EmployeeInitial")
                        .WithMany()
                        .HasForeignKey("EmployeeIdInitial");

                    b.HasOne("Optima.Fais.Model.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.ApplicationUser", "ModifiedByUser")
                        .WithMany()
                        .HasForeignKey("ModifiedBy");

                    b.HasOne("Optima.Fais.Model.Room", "RoomFinal")
                        .WithMany()
                        .HasForeignKey("RoomIdFinal");

                    b.HasOne("Optima.Fais.Model.Room", "RoomInitial")
                        .WithMany()
                        .HasForeignKey("RoomIdInitial");

                    b.HasOne("Optima.Fais.Model.InvState", "StateFinal")
                        .WithMany()
                        .HasForeignKey("StateIdFinal");

                    b.HasOne("Optima.Fais.Model.InvState", "StateInitial")
                        .WithMany()
                        .HasForeignKey("StateIdInitial");
                });

            modelBuilder.Entity("Optima.Fais.Model.InvState", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Location", b =>
                {
                    b.HasOne("Optima.Fais.Model.AdmCenter", "AdmCenter")
                        .WithMany()
                        .HasForeignKey("AdmCenterId");

                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Region", "Region")
                        .WithMany("Locations")
                        .HasForeignKey("RegionId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Partner", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Region", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Room", b =>
                {
                    b.HasOne("Optima.Fais.Model.CostCenter", "CostCenter")
                        .WithMany()
                        .HasForeignKey("CostCenterId");

                    b.HasOne("Optima.Fais.Model.Location", "Location")
                        .WithMany("Rooms")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Optima.Fais.Model.Room", "ParentRoom")
                        .WithMany("ChildRooms")
                        .HasForeignKey("ParentRoomId");
                });

            modelBuilder.Entity("Optima.Fais.Model.TableDefinition", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("Optima.Fais.Model.Uom", b =>
                {
                    b.HasOne("Optima.Fais.Model.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");
                });
        }
    }
}
