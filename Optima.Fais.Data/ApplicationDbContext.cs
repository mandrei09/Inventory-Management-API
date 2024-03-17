using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Optima.Faia.Model;
using Optima.Fais.Model;
using Optima.Fais.Model.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
namespace Optima.Fais.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

        public DbSet<Model.AccSystem> AccSystems { get; set; }
        public DbSet<Model.Asset> Assets { get; set; }
        public DbSet<Model.AssetAdmMD> AssetAdmMDs { get; set; }
        public DbSet<Model.AssetDep> AssetDeps { get; set; }
        public DbSet<Model.AcquisitionAssetSAP> AcquisitionAssetsSAPs { get; set; }
        public DbSet<Model.AssetChangeSAP> AssetChangeSAPs { get; set; }
        public DbSet<Model.CreateAssetSAP> CreateAssetSAPs { get; set; }
        public DbSet<Model.AssetDepMD> AssetDepMDs { get; set; }
        public DbSet<Model.AssetAC> AssetACs { get; set; }
        public DbSet<Model.AssetOp> AssetOps { get; set; }
        public DbSet<Model.AssetComponentOp> AssetComponentOps { get; set; }
        public DbSet<Model.AssetSyncError> AssetSyncErrors { get; set; }
        public DbSet<Model.InventoryAsset> InventoryAssets { get; set; }

        public DbSet<Model.Department> Departments { get; set; }
        public DbSet<Model.Division> Divisions { get; set; }

        public DbSet<Model.Country> Countries { get; set; }
        public DbSet<Model.County> Counties { get; set; }
        public DbSet<Model.City> Cities { get; set; }
        public DbSet<Model.Dimension> Dimensions { get; set; }
        public DbSet<Model.EntityType> EntityTypes { get; set; }

        public DbSet<Model.Budget> Budgets { get; set; }
        public DbSet<Model.BudgetOp> BudgetOps { get; set; }
        public DbSet<Model.BudgetBaseOp> BudgetBaseOps { get; set; }

        public DbSet<Model.BudgetBase> BudgetBases { get; set; }
        public DbSet<Model.BudgetMonthBase> BudgetMonthBases { get; set; }
        public DbSet<Model.BudgetForecast> BudgetForecasts { get; set; }

        public DbSet<Model.Order> Orders { get; set; }
        public DbSet<Model.OrderOp> OrderOps { get; set; }

        public DbSet<Model.Offer> Offers { get; set; }
        public DbSet<Model.OfferOp> OfferOps { get; set; }

        public DbSet<Model.Contract> Contracts { get; set; }
        public DbSet<Model.ContractOp> ContractOps { get; set; }

        public DbSet<Model.Request> Requests { get; set; }
        public DbSet<Model.RequestOp> RequestOps { get; set; }

        public DbSet<Model.Location> Locations { get; set; }
        public DbSet<Model.Room> Rooms { get; set; }
        public DbSet<Model.CostCenter> CostCenters { get; set; }
        public DbSet<Model.Company> Companies { get; set; }
        public DbSet<Model.Committee> Committees { get; set; }
        public DbSet<Model.InterCompany> InterCompanies { get; set; }
        public DbSet<Model.InsuranceCategory> InsuranceCategories { get; set; }
        public DbSet<Model.EntityFile> EntityFiles { get; set; }
        public DbSet<Model.Employee> Employees { get; set; }
        public DbSet<Model.EmployeeCostCenter> EmployeeCostCenters { get; set; }
		public DbSet<Model.EmployeeCompany> EmployeeCompanies { get; set; }
		public DbSet<Model.EmployeeDivision> EmployeeDivisions { get; set; }
        public DbSet<Model.AssetInv> AssetInvs { get; set; }
        public DbSet<Model.Uom> Uoms { get; set; }
        public DbSet<Model.AssetCategory> AssetCategories { get; set; }
        public DbSet<Model.InvState> InvStates { get; set; }
        public DbSet<Model.Material> Materials { get; set; }
        public DbSet<Model.AdmCenter> AdmCenters { get; set; }
        public DbSet<Model.Region> Regions { get; set; }
        public DbSet<Model.AssetNi> AssetNis { get; set; }
        public DbSet<Model.PartnerLocation> PartnerLocations { get; set; }
        public DbSet<Model.AssetNature> AssetNatures { get; set; }
        public DbSet<Model.BudgetManager> BudgetManagers { get; set; }
        public DbSet<Model.Account> Accounts { get; set; }
        public DbSet<Model.ExpAccount> ExpAccounts { get; set; }
        public DbSet<Model.InventoryListApn> InventoryListApns { get; set; }

        public DbSet<Model.DictionaryType> DictionaryTypes { get; set; }
        public DbSet<Model.DictionaryItem> DictionaryItems { get; set; }

        public DbSet<Model.Inventory> Inventories { get; set; }

        public DbSet<Model.AssetComponent> AssetComponents { get; set; }
        public DbSet<Model.EmployeeEmailResult> EmployeeEmailResults { get; set; }
        public DbSet<Model.EmployeeNotValidatedEmailResult> EmployeeNotValidatedEmailResults { get; set; }

        public DbSet<Model.AuditInventoryV1T1> AuditInventoryV1T1s { get; set; }
        public DbSet<Model.InventoryUserBuildingScanDetail> InventoryUserBuildingScanDetails { get; set; }
        public DbSet<Model.InventoryReportByAdmCenter> InventoryReportByAdmCenters { get; set; }
        public DbSet<Model.InventoryChartProcentage> InventoryChartProcentages { get; set; }
        public DbSet<Model.InventoryTotal> InventoryTotals { get; set; }
        public DbSet<Model.CostCenterTotal> CostCenterTotals { get; set; }
        public DbSet<Model.AssetTypeTotal> AssetTypeTotals { get; set; }
        public DbSet<Model.ProjectTotal> ProjectTotals { get; set; }
        public DbSet<Model.TypeTotal> TypeTotals { get; set; }
        public DbSet<Model.DepartmentTotal> DepartmentTotals { get; set; }
        public DbSet<Model.BudgetManagerTotal> BudgetManagerTotals { get; set; }
        public DbSet<Model.DivisionTotal> DivisionTotals { get; set; }
        public DbSet<Model.InventoryEmployeeProcentage> InventoryEmployeeProcentages { get; set; }
        public DbSet<Model.InventoryRoomProcentage> InventoryRoomProcentages { get; set; }
        public DbSet<Model.InventoryPieChartByDay> InventoryPieChartByDays { get; set; }
        public DbSet<Model.AuditInventoryResult> AuditInventoryResults { get; set; }
        public DbSet<Model.InventoryRegionProcentage> InventoryRegionProcentages { get; set; }
        public DbSet<Model.InventoryLocationProcentage> InventoryLocationProcentage { get; set; }
        public DbSet<Model.Reporting.InventoryListEmail> InventoryListEmails { get; set; }
        public DbSet<Model.Audit> Audits { get; set; }
        public DbSet<Model.SubTypeReport> SubTypeReports { get; set; }
        public DbSet<Model.DimensionERP> DimensionERPS { get; set; }
        public DbSet<Model.RecordCount> RecordCounts { get; set; }

        public DbSet<Model.BudgetTotalProcentage> BudgetTotalProcentages { get; set; }
        public DbSet<Model.AuditBudget> AuditBudgets { get; set; }
        public DbSet<Model.BudgetCompanyProcentage> BudgetCompanyProcentages { get; set; }
        public DbSet<Model.BudgetProjectProcentage> BudgetProjectProcentages { get; set; }
        public DbSet<Model.BudgetCostCenterProcentage> BudgetCostCenterProcentages { get; set; }
        public DbSet<Model.BudgetExpenceTypeProcentage> BudgetExpenceTypeProcentages { get; set; }
        public DbSet<Model.BudgetEmployeeProcentage> BudgetEmployeeProcentages { get; set; }
        public DbSet<Model.BudgetSubTypeProcentage> BudgetSubTypeProcentages { get; set; }

        public DbSet<Model.BudgetStatus> BudgetStatus { get; set; }
        public DbSet<Model.DepartmentReport> DepartmentReports { get; set; }
        public DbSet<Model.DivisionReport> DivisionReports { get; set; }
        public DbSet<Model.CostCenterReport> CostCenterReports { get; set; }
        public DbSet<Model.TypeReport> TypeReports { get; set; }
        public DbSet<Model.AssetTypeReport> AssetTypeReports { get; set; }
        public DbSet<Model.DashboardExport> DashboardExports { get; set; }

        public DbSet<Model.Device> Devices { get; set; }
        public DbSet<Model.DeviceType> DeviceTypes { get; set; }

        public DbSet<Model.BudgetMonth> BudgetMonths { get; set; }

        public DbSet<Model.LocationStatus> LocationStatus { get; set; }
        public DbSet<Model.ProjectGroup> ProjectGroups { get; set; }

        public DbSet<Model.CountryGroup> CountryGroups { get; set; }
        public DbSet<Model.AdmCenterGroup> AdmCenterGroups { get; set; }
        public DbSet<Model.RegionGroup> RegionGroups { get; set; }
        public DbSet<Model.AssetTypeGroup> AssetTypeGroups { get; set; }
        public DbSet<Model.CompanyGroup> CompanyGroups { get; set; }
        public DbSet<Model.CompanyDynamicGroup> CompanyDynamicGroups { get; set; }
        public DbSet<Model.DepartmentDynamicGroup> DepartmentDynamicGroups { get; set; }
        public DbSet<Model.DivisionDynamicGroup> DivisionDynamicGroups { get; set; }
        public DbSet<Model.AdmCenterDynamicGroup> AdmCenterDynamicGroups { get; set; }

        public DbSet<Model.RegionDynamicGroup> RegionDynamicGroups { get; set; }

        public DbSet<Model.AssetTypeDynamicGroup> AssetTypeDynamicGroups { get; set; }
        public DbSet<Model.ProjectDynamicGroup> ProjectDynamicGroups { get; set; }
        public DbSet<Model.ProjectTypeDynamicGroup> ProjectTypeDynamicGroups { get; set; }

        public DbSet<Model.CompanyDynamicGroupMonth> CompanyDynamicGroupMonths { get; set; }

        public DbSet<Model.EmployeesStatus> EmployeesStatus { get; set; }
		public DbSet<Model.UserStatus> UserStatus { get; set; }
		public DbSet<Model.UserRequestPerMonthStatus> UserRequestPerMonthStatus { get; set; }

		public DbSet<AuditHistory> AuditLogs { get; set; }
        public DbSet<ERPImportResult> ERPImportResults { get; set; }

        public DbSet<Model.ExpAccountGroup> ExpAccountGroups { get; set; }
        public DbSet<Model.EntityFilePartner> EntityFilePartners { get; set; }

		public DbSet<Model.PrintLabel> PrintLabels { get; set; }
		public DbSet<Model.AdministrationTotal> AdministrationTotals { get; set; }
        public DbSet<Model.AdministrationFAR> AdministrationFARs { get; set; }
        public DbSet<Model.TeamManagerWHTotal> TeamManagerWHTotals { get; set; }
		public DbSet<Model.ResponsableWHTotal> ResponsableWHTotals { get; set; }
		public DbSet<Model.ResponsableTotal> ResponsableTotals { get; set; }
		public DbSet<Model.ActivityTotal> ActivityTotals { get; set; }
		public DbSet<Model.ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Model.AppState> AppStates { get; set; }
		public DbSet<Model.Activity> Activities { get; set; }
		public DbSet<OrderReport> OrderReports { get; set; }
        public DbSet<InvCommittee> InvCommittees { get; set; }
        public DbSet<Model.InvCommitteePosition> CommitteePositions { get; set; }
        public DbSet<InvCommitteeMember> InvCommitteeMembers { get; set; }
        public DbSet<InventoryPlan> InventoryPlans { get; set; }

        public ApplicationDbContext()
		{
			Database.SetCommandTimeout(150000);
		}

		public string UserName
		{
			get; set;
		}

		public string UserId
		{
			get; set;
		}

        public virtual async Task<int> SaveChangesAsync(string userId = null)
        {
            OnBeforeSaveChanges(userId);

            SaveChanges();

             var result = await base.SaveChangesAsync();
            return result;
        }

        private void OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);

                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = userId;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.ChangedColumns.Add(propertyName);
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.ChangedColumns.Add(propertyName);
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                
                                //auditEntry.OldValues[propertyName] = property.OriginalValue;
                                //auditEntry.NewValues[propertyName] = property.CurrentValue;

                                var originalValue = entry.GetDatabaseValues().GetValue<object>(propertyName.ToString());

                                if (property.OriginalValue == null && property.CurrentValue == null)
                                    continue;

                                if (property.OriginalValue == null || property.CurrentValue == null || originalValue == null || !originalValue.Equals(property.CurrentValue))
                                {
                                    auditEntry.OldValues[propertyName] = originalValue;
                                    auditEntry.NewValues[propertyName] = property.CurrentValue;

                                    auditEntry.ChangedColumns.Add(propertyName);
                                    auditEntry.AuditType = AuditType.Update;
                                }
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                if(auditEntry.ChangedColumns.Count > 0)
				{
                    AuditLogs.Add(auditEntry.ToAudit());
                }
                
            }
        }

        public override int SaveChanges()
		{
            //string userID = string.Empty;
            if ((UserId == null) || (UserId.Length == 0))
            {
                var user = this.Users.Where(u => u.UserName == UserName).SingleOrDefault();
                UserId = user != null ? user.Id : null;
            }

            var trackables = ChangeTracker.Entries<Entity>();

            if (trackables != null)
            {
                // added
                foreach (var item in trackables.Where(t => t.State == EntityState.Added))
                {
                    System.DateTime date = System.DateTime.Now;
                    item.Entity.CreatedAt = date;
                    item.Entity.CreatedBy = UserId;
                    item.Entity.ModifiedAt = date;
                    item.Entity.ModifiedBy = UserId;
                }
                // modified
                foreach (var item in trackables.Where(t => t.State == EntityState.Modified))
                {
                    item.Entity.ModifiedAt = System.DateTime.Now;
                    item.Entity.ModifiedBy = item.Entity.ModifiedBy != null ? item.Entity.ModifiedBy : UserId;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

            // optionsBuilder.UseSqlServer("data source=WOPTIMA01;initial catalog=ofa_v50_emag;user id=sa;password=Optima1234+;Connect Timeout=1440;multipleactiveresultsets=True;", b => b.UseRowNumberForPaging());
            // optionsBuilder.UseSqlServer("data source=WOPTIMA01;initial catalog=ofa_v50_emag_uat15;user id=sa;password=Optima1234+;Connect Timeout=1440;multipleactiveresultsets=True;", b => b.UseRowNumberForPaging());
            // optionsBuilder.UseSqlServer("data source=WOPTIMA01;initial catalog=ofa_v50_emag_it2;user id=sa;password=Optima1234+;Connect Timeout=1440;multipleactiveresultsets=True;", b => b.UseRowNumberForPaging());
            // optionsBuilder.UseSqlServer("data source=DESKTOP-TGGI2I5\\SQL2019;initial catalog=ofa_v50_emag;user id=sa;password=optima;Connect Timeout=1440;multipleactiveresultsets=True;", b => b.UseRowNumberForPaging());
            //  optionsBuilder.UseSqlServer("data source=DESKTOP-TGGI2I5\\SQL2019;initial catalog=ofa_v50_emag_it2;user id=sa;password=optima;Connect Timeout=1440;multipleactiveresultsets=True;", b => b.UseRowNumberForPaging());
            //optionsBuilder.UseSqlServer("data source=WOPTIMA01;initial catalog=ofa_v50_emag_prod_test;user id=sa;password=Optima1234+;Connect Timeout=1440;multipleactiveresultsets=True;", b => b.UseRowNumberForPaging());


            //de verificat daca ramane in versiunea finala
            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

            string conn = config.GetConnectionString("DefaultConnection");

            //define the database to use
            optionsBuilder.UseSqlServer(conn, b => b.UseRowNumberForPaging());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            ////Entity
            //builder.Entity<Entity>()
            //    .HasOne(i => i.CreatedByUser)
            //    .WithMany()
            //    .HasForeignKey(i => i.CreatedBy);
            //builder.Entity<Entity>()
            //    .HasOne(i => i.ModifiedByUser)
            //    .WithMany()
            //    .HasForeignKey(i => i.ModifiedBy);

            builder.Entity<ApplicationRole>()
                .Property(p => p.Id)
                .HasColumnName("Id");


            builder.Entity<ApplicationUser>()
                .Property(p => p.Id)
                .HasColumnName("Id");

            builder.Entity<ApplicationUserLogin>()
                .Property(p => p.LoginDate)
			    .HasDefaultValueSql("getdate()")
                .IsRequired();
            //builder.Entity<ApplicationUserLogin>()
            //   .HasOne(a => a.ApplicationUser)
            //   .WithMany()
            //   .HasForeignKey(a => a.UserId);


            builder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(e => e.Roles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

			builder.Entity<ApplicationUser>(b =>
			{
				b.HasMany(e => e.Logins)
					.WithOne()
					.HasForeignKey(ur => ur.UserId)
					.IsRequired();
			});


			//ColumnFilter
			builder.Entity<ColumnFilter>()
				.ToTable("ColumnFilter")
				.Property(p => p.Id)
				.HasColumnName("Id");
			builder.Entity<ColumnFilter>()
				.Property(p => p.Property)
				.IsRequired()
				.HasMaxLength(100);
			builder.Entity<ColumnFilter>()
				.Property(p => p.Type)
				.IsRequired()
				.HasMaxLength(100);

			//ReportBook
			builder.Entity<ReportBook>()
				.ToTable("ReportBook")
				.Property(p => p.Id)
				.HasColumnName("Id");
			builder.Entity<ReportBook>()
				.Property(p => p.Code)
				.IsRequired()
				.HasMaxLength(30);
			builder.Entity<ReportBook>()
				.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(100);


			//WFHCheck
			builder.Entity<WFHCheck>()
				.ToTable("WFHCheck")
				.Property(i => i.Id)
				.HasColumnName("Id");
			builder.Entity<WFHCheck>()
			   .HasOne(i => i.DictionaryItem)
			   .WithMany()
			   .HasForeignKey(i => i.DictionaryItemId);
			builder.Entity<WFHCheck>()
					.HasOne(i => i.Brand)
					.WithMany()
					.HasForeignKey(i => i.BrandId);
			builder.Entity<WFHCheck>()
			   .HasOne(i => i.Model)
			   .WithMany()
			   .HasForeignKey(i => i.ModelId);
			builder.Entity<WFHCheck>()
				 .HasOne(i => i.BudgetManager)
				 .WithMany()
				 .HasForeignKey(i => i.BudgetManagerId);
			builder.Entity<WFHCheck>()
			   .HasOne(i => i.Employee)
			   .WithMany()
			   .HasForeignKey(i => i.EmployeeId);

			//PrintLabel
			builder.Entity<PrintLabel>()
				.ToTable("PrintLabel")
				.Property(p => p.Id)
				.HasColumnName("Id");
			builder.Entity<PrintLabel>()
				.Property(p => p.Code)
				.IsRequired()
				.HasMaxLength(30);
			builder.Entity<PrintLabel>()
				.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(100);
			builder.Entity<PrintLabel>()
			   .HasOne(a => a.Asset)
			   .WithMany()
			   .HasForeignKey(a => a.AssetId);


			//InvColumnDefinitionRole
			builder.Entity<ColumnDefinitionRole>()
                .ToTable("ColumnDefinitionRole")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<ColumnDefinitionRole>()
                 .HasOne(a => a.Role)
                 .WithMany()
                 .HasForeignKey(a => a.RoleId);

            //InvCommittee
            builder.Entity<InvCommittee>()
            .ToTable("InvCommittee")
            .Property(p => p.Id)
            .HasColumnName("InvCommitteeId");
            builder.Entity<InvCommittee>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<InvCommittee>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //InvCommitteePosition
            builder.Entity<InvCommitteePosition>()
            .ToTable("InvCommitteePosition")
            .Property(p => p.Id)
            .HasColumnName("InvCommitteePositionId");
            builder.Entity<InvCommitteePosition>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<InvCommitteePosition>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            //InvCommitteeMember
            builder.Entity<InvCommitteeMember>()
                .ToTable("InvCommitteeMember")
                .Property(p => p.Id)
                .HasColumnName("InvCommitteeMemberId");

            //InventoryPlan
            builder.Entity<InventoryPlan>()
              .ToTable("InventoryPlan")
              .Property(p => p.Id)
              .HasColumnName("InventoryPlanId");
            builder.Entity<InventoryPlan>()
               .Property(p => p.DateStarted)
               .HasColumnType("datetime2(3)");
            builder.Entity<InventoryPlan>()
                .Property(p => p.DateFinished)
                .HasColumnType("datetime2(3)");

            //RouteRole
            builder.Entity<RouteRole>()
                .ToTable("RouteRole")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<RouteRole>()
                 .HasOne(a => a.Role)
                 .WithMany()
                 .HasForeignKey(a => a.RoleId);

            //RouteChildrenRole
            builder.Entity<RouteChildrenRole>()
                .ToTable("RouteChildrenRole")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<RouteChildrenRole>()
                 .HasOne(a => a.Role)
                 .WithMany()
                 .HasForeignKey(a => a.RoleId);


            //CreateAssetSAP
            builder.Entity<CreateAssetSAP>()
                .ToTable("CreateAssetSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<CreateAssetSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<CreateAssetSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<CreateAssetSAP>()
                    .HasOne(a => a.Asset)
                    .WithMany(a => a.CreateAssetSAP)
                    .HasForeignKey(a => a.AssetId);

            //TransferInStockSAP
            builder.Entity<TransferInStockSAP>()
                .ToTable("TransferInStockSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<TransferInStockSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<TransferInStockSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<TransferInStockSAP>()
                 .HasOne(i => i.CreateAssetSAP)
                 .WithMany()
                 .HasForeignKey(i => i.CreateAssetSAPId);
            builder.Entity<TransferInStockSAP>()
                   .HasOne(a => a.AssetStock)
                   .WithMany(a => a.TransferInStockSAP)
                   .HasForeignKey(a => a.AssetStockId);

            //TransferAssetSAP
            builder.Entity<TransferAssetSAP>()
                .ToTable("TransferAssetSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<TransferAssetSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<TransferAssetSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            //builder.Entity<TransferAssetSAP>()
            //     .HasOne(i => i.FromAsset)
            //     .WithMany(a => a.TransferAssetSAP);
            //     //.HasForeignKey(i => i.FromAssetId);
            //builder.Entity<TransferAssetSAP>()
            //    .HasOne(i => i.ToAsset)
            //    .WithMany(a => a.TransferAssetSAP);
                //.HasForeignKey(i => i.ToAssetId);
            //builder.Entity<TransferAssetSAP>()
            //       .HasOne(a => a.FromAsset)
            //       .WithMany(a => a.TransferAssetSAP)
            //       .HasForeignKey(a => a.FromAssetId)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Restrict);
            //builder.Entity<TransferAssetSAP>()
            //      .HasOne(a => a.ToAsset)
            //      .WithMany(a => a.TransferAssetSAP)
            //      .HasForeignKey(a => a.ToAssetId)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TransferAssetSAP>()
                 .HasOne(a => a.Asset)
                 .WithMany(a => a.TransferAssetSAP)
                 .HasForeignKey(a => a.AssetId);

            //RetireAssetSAP
            builder.Entity<RetireAssetSAP>()
                .ToTable("RetireAssetSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<RetireAssetSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<RetireAssetSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<RetireAssetSAP>()
                   .HasOne(a => a.Asset)
                   .WithMany(a => a.RetireAssetSAP)
                   .HasForeignKey(a => a.AssetId);


            //AssetInvPlusSAP
            builder.Entity<AssetInvPlusSAP>()
                .ToTable("AssetInvPlusSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<AssetInvPlusSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<AssetInvPlusSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<AssetInvPlusSAP>()
                    .HasOne(i => i.CreateAssetSAP)
                    .WithMany()
                    .HasForeignKey(i => i.CreateAssetSAPId);
            builder.Entity<AssetInvPlusSAP>()
                   .HasOne(a => a.Asset)
                   .WithMany(a => a.AssetInvPlusSAP)
                   .HasForeignKey(a => a.AssetId);


            //AssetInvMinusSAP
            builder.Entity<AssetInvMinusSAP>()
                .ToTable("AssetInvMinusSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<AssetInvMinusSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<AssetInvMinusSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<AssetInvMinusSAP>()
                   .HasOne(a => a.Asset)
                   .WithMany(a => a.AssetInvMinusSAP)
                   .HasForeignKey(a => a.AssetId);

            //AssetChangeSAP
            builder.Entity<AssetChangeSAP>()
                .ToTable("AssetChangeSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<AssetChangeSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<AssetChangeSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<AssetChangeSAP>()
                   .HasOne(a => a.Asset)
                   .WithMany(a => a.AssetChangeSAP)
                   .HasForeignKey(a => a.AssetId);

            //AcquisitionAssetSAP
            builder.Entity<AcquisitionAssetSAP>()
                .ToTable("AcquisitionAssetSAP")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<AcquisitionAssetSAP>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<AcquisitionAssetSAP>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<AcquisitionAssetSAP>()
                   .HasOne(a => a.Asset)
                   .WithMany(a => a.AcquisitionAssetSAP)
                   .HasForeignKey(a => a.AssetId);
            builder.Entity<AcquisitionAssetSAP>()
               .Property(p => p.EXCH_RATE)
               .HasColumnType("decimal(18, 4)");
            builder.Entity<AcquisitionAssetSAP>()
              .Property(p => p.NET_AMOUNT)
              .HasColumnType("decimal(18, 2)");
            builder.Entity<AcquisitionAssetSAP>()
              .Property(p => p.TOTAL_AMOUNT)
              .HasColumnType("decimal(18, 2)");
            builder.Entity<AcquisitionAssetSAP>()
             .Property(p => p.TAX_AMOUNT)
             .HasColumnType("decimal(18, 2)");
            //builder.Entity<AcquisitionAssetSAP>()
            //    .HasOne(p => p.Asset)
            //    .WithOne(c => c.AcquisitionAssetSAP)
            //    .HasForeignKey<Asset>(c => c.Ass);
            //.OnDelete(DeleteBehavior.Cascade);


            ////AcquisitionAssets
            //builder.Entity<AcquisitionAssets>()
            //    .ToTable("AcquisitionAssets")
            //    .Property(p => p.Id)
            //    .HasColumnName("Id");
            //builder.Entity<AcquisitionAssets>()
            //   .HasOne(a => a.AccMonth)
            //   .WithMany()
            //   .HasForeignKey(a => a.AccMonthId);
            //builder.Entity<AcquisitionAssets>()
            //      .HasOne(i => i.BudgetManager)
            //      .WithMany()
            //      .HasForeignKey(i => i.BudgetManagerId);
            //builder.Entity<AcquisitionAssets>()
            //         .HasOne(i => i.Asset)
            //         .WithMany()
            //         .HasForeignKey(i => i.AssetId);

            //BudgetBase
            builder.Entity<BudgetBase>()
                .ToTable("BudgetBase")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetBase>()
                .Property(p => p.Code)
                .HasMaxLength(120);
            builder.Entity<BudgetBase>()
                .Property(p => p.Name)
                .HasMaxLength(120);
            builder.Entity<BudgetBase>()
                .Property(p => p.Info)
                .HasMaxLength(120);
            builder.Entity<BudgetBase>()
                .Property(p => p.ValueIni)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<BudgetBase>()
                .Property(p => p.ValueFin)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<BudgetBase>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<BudgetBase>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<BudgetBase>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<BudgetBase>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<BudgetBase>()
                  .HasOne(i => i.User)
                  .WithMany()
                  .HasForeignKey(i => i.UserId);
            builder.Entity<BudgetBase>()
                  .HasOne(i => i.Company)
                  .WithMany()
                  .HasForeignKey(i => i.CompanyId);
            builder.Entity<BudgetBase>()
                  .HasOne(i => i.AppState)
                  .WithMany()
                  .HasForeignKey(i => i.AppStateId);
            builder.Entity<BudgetBase>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<BudgetBase>()
                     .HasOne(i => i.Country)
                     .WithMany()
                     .HasForeignKey(i => i.CountryId);
            builder.Entity<BudgetBase>()
                     .HasOne(i => i.Activity)
                     .WithMany()
                     .HasForeignKey(i => i.ActivityId);
            builder.Entity<BudgetBase>()
                     .HasOne(i => i.AdmCenter)
                     .WithMany()
                     .HasForeignKey(i => i.AdmCenterId);
            builder.Entity<BudgetBase>()
                     .HasOne(i => i.Region)
                     .WithMany()
                     .HasForeignKey(i => i.RegionId);
            builder.Entity<BudgetBase>()
                     .HasOne(i => i.AssetType)
                     .WithMany()
                     .HasForeignKey(i => i.AssetTypeId);
            //builder.Entity<BudgetBase>()
            //   .HasOne(p => p.BudgetMonthBase)
            //   .WithOne(c => c.BudgetBase)
            //   .HasForeignKey<BudgetMonthBase>(c => c.BudgetBaseId);
            //builder.Entity<BudgetBase>()
            //  .HasOne(p => p.BudgetForecast)
            //  .WithOne(c => c.BudgetBase)
            //  .HasForeignKey<BudgetForecast>(c => c.BudgetBaseId);


            //BudgetMonthBase
            builder.Entity<BudgetMonthBase>()
                .ToTable("BudgetMonthBase")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetMonthBase>()
             .HasOne(a => a.BudgetType)
             .WithMany()
             .HasForeignKey(a => a.BudgetTypeId)
             .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<BudgetMonthBase>()
                .HasOne(a => a.BudgetBase)
                .WithMany(a => a.BudgetMonthBase)
                .HasForeignKey(a => a.BudgetBaseId);

            //BudgetForecast
            builder.Entity<BudgetForecast>()
                .ToTable("BudgetForecast")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetForecast>()
               .HasOne(a => a.BudgetType)
               .WithMany()
               .HasForeignKey(a => a.BudgetTypeId)
               .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<BudgetForecast>()
               .HasOne(a => a.BudgetBase)
               .WithMany(a => a.BudgetForecast)
               .HasForeignKey(a => a.BudgetBaseId);

            //BudgetBaseAsset
            builder.Entity<BudgetBaseAsset>()
                .ToTable("BudgetBaseAsset")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetBaseAsset>()
               .HasOne(a => a.BudgetType)
               .WithMany()
               .HasForeignKey(a => a.BudgetTypeId)
               .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<BudgetBaseAsset>()
               .HasOne(a => a.BudgetBase)
               .WithMany(a => a.BudgetBaseAsset)
               .HasForeignKey(a => a.BudgetBaseId);
            builder.Entity<BudgetBaseAsset>()
               .HasOne(a => a.Asset)
               .WithMany()
               .HasForeignKey(a => a.AssetId)
               .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<BudgetBaseAsset>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId)
               .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<BudgetBaseAsset>()
              .HasOne(a => a.AppState)
              .WithMany()
              .HasForeignKey(a => a.AppStateId)
              .OnDelete(DeleteBehavior.Restrict);


            //Error
            builder.Entity<Error>()
                .ToTable("Error")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Error>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Error>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<Error>()
                .HasOne(a => a.Asset)
                .WithMany(a => a.Error)
                .HasForeignKey(a => a.AssetId);

            //ErrorType
            builder.Entity<ErrorType>()
                .ToTable("ErrorType")
                .Property(p => p.Id)
                .HasColumnName("ErrorTypeId");
            builder.Entity<ErrorType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<ErrorType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //AssetDepMDSync
            builder.Entity<AssetDepMDSync>()
                .ToTable("AssetDepMDSync")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<AssetDepMDSync>()
                .Property(p => p.InvNo)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<AssetDepMDSync>()
                .Property(p => p.SubNumber)
                .IsRequired()
                .HasMaxLength(200);

            //AssetDepMDCapSync
            builder.Entity<AssetDepMDCapSync>()
                .ToTable("AssetDepMDCapSync")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<AssetDepMDCapSync>()
                .Property(p => p.BUKRSH)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<AssetDepMDCapSync>()
                .Property(p => p.BLDAT)
                .IsRequired()
                .HasMaxLength(200);


            //Tax
            builder.Entity<Tax>()
                .ToTable("Tax")
                .Property(p => p.Id)
                .HasColumnName("TaxId");
            builder.Entity<Tax>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<SyncStatus>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);


            //SyncStatus
            builder.Entity<SyncStatus>()
                .ToTable("SyncStatus")
                .Property(p => p.Id)
                .HasColumnName("SyncStatusId");
            builder.Entity<SyncStatus>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<SyncStatus>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            //Device
            builder.Entity<Device>()
                .ToTable("Device")
                .Property(p => p.Id)
                .HasColumnName("DeviceId");
            builder.Entity<Device>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Device>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<Device>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);

            //DeviceType
            builder.Entity<DeviceType>()
                .ToTable("DeviceType")
                .Property(p => p.Id)
                .HasColumnName("DeviceTypeId");
            builder.Entity<DeviceType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<DeviceType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

			//MobilePhone
			builder.Entity<MobilePhone>()
				.ToTable("MobilePhone")
				.Property(p => p.Id)
				.HasColumnName("Id");
			builder.Entity<MobilePhone>()
				.Property(p => p.Code)
				.IsRequired()
				.HasMaxLength(30);
			builder.Entity<MobilePhone>()
				.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(100);

			//Info
			builder.Entity<Info>()
                .ToTable("Info")
                .Property(p => p.Id)
                .HasColumnName("InfoId");
            builder.Entity<Info>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Info>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            //InfoType
            builder.Entity<InfoType>()
                .ToTable("InfoType")
                .Property(p => p.Id)
                .HasColumnName("InfoTypeId");
            builder.Entity<InfoType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<InfoType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //Budget
            builder.Entity<Budget>()
                .ToTable("Budget")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Budget>()
                .Property(p => p.Code)
                .HasMaxLength(120);
            builder.Entity<Budget>()
                .Property(p => p.Name)
                .HasMaxLength(120);
            builder.Entity<Budget>()
                .Property(p => p.Info)
                .HasMaxLength(120);
            builder.Entity<Budget>()
                .Property(p => p.StartDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Budget>()
               .Property(p => p.EndDate)
               .IsRequired(false)
               .HasColumnType("date");
            builder.Entity<Budget>()
                .Property(p => p.ValueIni)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Budget>()
                .Property(p => p.ValueFin)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Budget>()
                .HasOne(a => a.Administration)
                .WithMany()
                .HasForeignKey(a => a.AdministrationId);
            builder.Entity<Budget>()
               .HasOne(a => a.SubType)
               .WithMany()
               .HasForeignKey(a => a.SubTypeId);
            builder.Entity<Budget>()
               .HasOne(a => a.Partner)
               .WithMany()
               .HasForeignKey(a => a.PartnerId);
            builder.Entity<Budget>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<Budget>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<Budget>()
               .HasOne(a => a.InterCompany)
               .WithMany()
               .HasForeignKey(a => a.InterCompanyId);
            builder.Entity<Budget>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<Budget>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<Budget>()
                  .HasOne(i => i.User)
                  .WithMany()
                  .HasForeignKey(i => i.UserId);
            builder.Entity<Budget>()
                  .HasOne(i => i.Company)
                  .WithMany()
                  .HasForeignKey(i => i.CompanyId);
            builder.Entity<Budget>()
                  .HasOne(i => i.Account)
                  .WithMany()
                  .HasForeignKey(i => i.AccountId);
            builder.Entity<Budget>()
                  .HasOne(i => i.AppState)
                  .WithMany()
                  .HasForeignKey(i => i.AppStateId);
            builder.Entity<Budget>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<Budget>()
                     .HasOne(i => i.Country)
                     .WithMany()
                     .HasForeignKey(i => i.CountryId);
            builder.Entity<Budget>()
                     .HasOne(i => i.Activity)
                     .WithMany()
                     .HasForeignKey(i => i.ActivityId);
            builder.Entity<Budget>()
                     .HasOne(i => i.AdmCenter)
                     .WithMany()
                     .HasForeignKey(i => i.AdmCenterId);
            builder.Entity<Budget>()
                     .HasOne(i => i.Region)
                     .WithMany()
                     .HasForeignKey(i => i.RegionId);
            builder.Entity<Budget>()
                     .HasOne(i => i.AssetType)
                     .WithMany()
                     .HasForeignKey(i => i.AssetTypeId);
            builder.Entity<Budget>()
                    .HasMany(a => a.BudgetMonths)
                    .WithOne();
            //.HasForeignKey(a => a.BudgetId);

            //OfferType
            builder.Entity<OfferType>()
                .ToTable("OfferType")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<OfferType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<OfferType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //Offer
            builder.Entity<Offer>()
                .ToTable("Offer")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Offer>()
                .Property(p => p.Code)
                .HasMaxLength(120);
            builder.Entity<Offer>()
                .Property(p => p.Name)
                .HasMaxLength(120);
            builder.Entity<Offer>()
                .Property(p => p.Info)
                .HasMaxLength(120);
            builder.Entity<Offer>()
                .Property(p => p.StartDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Offer>()
               .Property(p => p.EndDate)
               .IsRequired(false)
               .HasColumnType("date");
            builder.Entity<Offer>()
                .Property(p => p.ValueIni)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Offer>()
                .Property(p => p.ValueFin)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Offer>()
                .HasOne(a => a.Administration)
                .WithMany()
                .HasForeignKey(a => a.AdministrationId);
            builder.Entity<Offer>()
               .HasOne(a => a.SubType)
               .WithMany()
               .HasForeignKey(a => a.SubTypeId);
            builder.Entity<Offer>()
               .HasOne(a => a.Partner)
               .WithMany()
               .HasForeignKey(a => a.PartnerId);
            builder.Entity<Offer>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<Offer>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<Offer>()
               .HasOne(a => a.InterCompany)
               .WithMany()
               .HasForeignKey(a => a.InterCompanyId);
            builder.Entity<Offer>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<Offer>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<Offer>()
                  .HasOne(i => i.User)
                  .WithMany()
                  .HasForeignKey(i => i.UserId);
            builder.Entity<Offer>()
                  .HasOne(i => i.Company)
                  .WithMany()
                  .HasForeignKey(i => i.CompanyId);
            builder.Entity<Offer>()
                  .HasOne(i => i.Account)
                  .WithMany()
                  .HasForeignKey(i => i.AccountId);
            builder.Entity<Offer>()
                  .HasOne(i => i.AppState)
                  .WithMany()
                  .HasForeignKey(i => i.AppStateId);
            builder.Entity<Offer>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<Offer>()
                     .HasOne(i => i.AdmCenter)
                     .WithMany()
                     .HasForeignKey(i => i.AdmCenterId);
            builder.Entity<Offer>()
                     .HasOne(i => i.Region)
                     .WithMany()
                     .HasForeignKey(i => i.RegionId);
            builder.Entity<Offer>()
                     .HasOne(i => i.AssetType)
                     .WithMany()
                     .HasForeignKey(i => i.AssetTypeId);
            builder.Entity<Offer>()
                     .HasOne(i => i.ProjectType)
                     .WithMany()
                     .HasForeignKey(i => i.ProjectTypeId);

            //Matrix
            builder.Entity<Matrix>()
                .ToTable("Matrix")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Matrix>()
                .HasOne(a => a.Area)
                .WithMany()
                .HasForeignKey(a => a.AreaId);
            builder.Entity<Matrix>()
               .HasOne(a => a.AppState)
               .WithMany()
               .HasForeignKey(a => a.AppStateId);
            builder.Entity<Matrix>()
               .HasOne(a => a.Country)
               .WithMany()
               .HasForeignKey(a => a.CountryId);
            builder.Entity<Matrix>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<Matrix>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<Matrix>()
                .HasOne(i => i.Company)
                .WithMany()
                .HasForeignKey(i => i.CompanyId);
            builder.Entity<Matrix>()
                 .HasOne(i => i.AssetType)
                 .WithMany()
                 .HasForeignKey(i => i.AssetTypeId);
            builder.Entity<Matrix>()
               .HasOne(i => i.EmployeeL1)
               .WithMany()
               .HasForeignKey(i => i.EmployeeL1Id);
            builder.Entity<Matrix>()
              .HasOne(i => i.EmployeeL2)
              .WithMany()
              .HasForeignKey(i => i.EmployeeL2Id);
            builder.Entity<Matrix>()
              .HasOne(i => i.EmployeeL3)
              .WithMany()
              .HasForeignKey(i => i.EmployeeL3Id);
            builder.Entity<Matrix>()
              .HasOne(i => i.EmployeeL4)
              .WithMany()
              .HasForeignKey(i => i.EmployeeL4Id);
            builder.Entity<Matrix>()
              .HasOne(i => i.EmployeeS1)
              .WithMany()
              .HasForeignKey(i => i.EmployeeS1Id);
            builder.Entity<Matrix>()
             .HasOne(i => i.EmployeeS2)
             .WithMany()
             .HasForeignKey(i => i.EmployeeS2Id);
            builder.Entity<Matrix>()
             .HasOne(i => i.EmployeeS3)
             .WithMany()
             .HasForeignKey(i => i.EmployeeS3Id);
			builder.Entity<Matrix>()
			 .HasOne(i => i.EmployeeB1)
			 .WithMany()
			 .HasForeignKey(i => i.EmployeeB1Id);

			//MatrixLevel
			builder.Entity<MatrixLevel>()
                .ToTable("MatrixLevel")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<MatrixLevel>()
                .HasOne(a => a.Level)
                .WithMany()
                .HasForeignKey(a => a.LevelId);
            builder.Entity<MatrixLevel>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<MatrixLevel>()
                .HasOne(a => a.Uom)
                .WithMany()
                .HasForeignKey(a => a.UomId);
            builder.Entity<MatrixLevel>()
                .HasOne(a => a.Matrix)
                .WithMany(a => a.MatrixLevels)
            // builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
                 .HasForeignKey(a => a.MatrixId);


            //MatrixImport
            builder.Entity<MatrixImport>()
                .ToTable("MatrixImport")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<MatrixImport>()
                .Property(p => p.CompanyCode)
                .HasMaxLength(450);
            builder.Entity<MatrixImport>()
               .Property(p => p.CompanyName)
                .HasMaxLength(450);

            //RequestType
            builder.Entity<RequestType>()
                .ToTable("RequestType")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<RequestType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<RequestType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Request
            builder.Entity<Request>()
                .ToTable("Request")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Request>()
                .Property(p => p.Code)
                .HasMaxLength(120);
            builder.Entity<Request>()
                .Property(p => p.Name)
                .HasMaxLength(450);
            builder.Entity<Request>()
                .Property(p => p.Info)
                .HasMaxLength(450);
            builder.Entity<Request>()
                .Property(p => p.StartDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Request>()
               .Property(p => p.EndDate)
               .IsRequired(false)
               .HasColumnType("date");
            builder.Entity<Request>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<Request>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<Request>()
               .HasOne(a => a.Budget)
               .WithMany()
               .HasForeignKey(a => a.BudgetId);
            builder.Entity<Request>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<Request>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<Request>()
                  .HasOne(i => i.User)
                  .WithMany()
                  .HasForeignKey(i => i.UserId);
            builder.Entity<Request>()
                  .HasOne(i => i.Company)
                  .WithMany()
                  .HasForeignKey(i => i.CompanyId);
            builder.Entity<Request>()
                  .HasOne(i => i.AppState)
                  .WithMany()
                  .HasForeignKey(i => i.AppStateId);
            builder.Entity<Request>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<Request>()
              .HasOne(a => a.StartAccMonth)
              .WithMany()
              .HasForeignKey(a => a.StartAccMonthId);
            builder.Entity<Request>()
             .HasOne(a => a.RequestType)
             .WithMany()
             .HasForeignKey(a => a.RequestTypeId);


            //Contract
            builder.Entity<Contract>()
                .ToTable("Contract")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Contract>()
                .Property(p => p.Code)
                .HasMaxLength(120);
            builder.Entity<Contract>()
                .Property(p => p.Name)
                .HasMaxLength(120);
            builder.Entity<Contract>()
                .Property(p => p.Info)
                .HasMaxLength(120);
            builder.Entity<Contract>()
                .Property(p => p.EffectiveDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Contract>()
               .Property(p => p.AgreementDate)
               .IsRequired(false)
               .HasColumnType("date");
            builder.Entity<Contract>()
              .Property(p => p.ExpirationDate)
              .IsRequired(false)
              .HasColumnType("date");
            builder.Entity<Contract>()
               .Property(p => p.CreationDate)
               .IsRequired(false)
               .HasColumnType("date");
            builder.Entity<Contract>()
               .HasOne(a => a.Partner)
               .WithMany()
               .HasForeignKey(a => a.PartnerId);
            builder.Entity<Contract>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<Contract>()
                .HasOne(a => a.BusinessSystem)
                .WithMany()
                .HasForeignKey(a => a.BusinessSystemId);
            builder.Entity<Contract>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<Contract>()
                    .HasOne(a => a.Owner)
                    .WithMany()
                    .HasForeignKey(a => a.OwnerId);
            builder.Entity<Contract>()
                  .HasOne(i => i.ContractAmount)
                  .WithMany()
                  .HasForeignKey(i => i.ContractAmountId);
            builder.Entity<Contract>()
                  .HasOne(i => i.Company)
                  .WithMany()
                  .HasForeignKey(i => i.CompanyId);
            builder.Entity<Contract>()
                  .HasOne(i => i.AppState)
                  .WithMany()
                  .HasForeignKey(i => i.AppStateId);


            //Commodity
            builder.Entity<Commodity>()
                .ToTable("Commodity")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Commodity>()
                       .HasOne(a => a.Contract)
                       .WithMany(a => a.Commodities)
            // builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
            .HasForeignKey(a => a.ContractId);


            //ContractRegion
            builder.Entity<ContractRegion>()
                .ToTable("ContractRegion")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<ContractRegion>()
                       .HasOne(a => a.Contract)
                       .WithMany(a => a.ContractRegions)
            // builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
            .HasForeignKey(a => a.ContractId);


            //ContractDivision
            builder.Entity<ContractDivision>()
                .ToTable("ContractDivision")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<ContractDivision>()
                       .HasOne(a => a.Contract)
                       .WithMany(a => a.ContractDivisions)
            // builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
            .HasForeignKey(a => a.ContractId);

            //BusinessSystem
            builder.Entity<BusinessSystem>()
                .ToTable("BusinessSystem")
                .Property(p => p.Id)
                .HasColumnName("BusinessSystemId");
            builder.Entity<BusinessSystem>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<BusinessSystem>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //ContractAmount
            builder.Entity<ContractAmount>()
                .ToTable("ContractAmount")
                .Property(p => p.Id)
                .HasColumnName("ContractAmountId");
            builder.Entity<ContractAmount>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<ContractAmount>()
               .Property(p => p.Amount)
               .HasColumnType("decimal(18, 4)");
            builder.Entity<ContractAmount>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<ContractAmount>()
                .HasOne(a => a.Rate)
                .WithMany()
                .HasForeignKey(a => a.RateId);
            builder.Entity<ContractAmount>()
                .HasOne(a => a.RateRon)
                .WithMany()
                .HasForeignKey(a => a.RateRonId);

            //Rate
            builder.Entity<Rate>()
                .ToTable("Rate")
                .Property(p => p.Id)
                .HasColumnName("RateId");
            builder.Entity<Rate>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Rate>()
               .Property(p => p.Value)
               .HasColumnType("decimal(18, 4)");
            builder.Entity<Rate>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //Order
            builder.Entity<Order>()
                .ToTable("Order")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Order>()
                .Property(p => p.Code)
                .HasMaxLength(120);
            builder.Entity<Order>()
                .Property(p => p.Name)
                .HasMaxLength(120);
            builder.Entity<Order>()
                .Property(p => p.Info)
                .HasMaxLength(120);
            builder.Entity<Order>()
                .Property(p => p.StartDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Order>()
               .Property(p => p.EndDate)
               .IsRequired(false)
               .HasColumnType("date");
            builder.Entity<Order>()
                .Property(p => p.ValueIni)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Order>()
                .Property(p => p.ValueFin)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Order>()
                .HasOne(a => a.Administration)
                .WithMany()
                .HasForeignKey(a => a.AdministrationId);
            builder.Entity<Order>()
               .HasOne(a => a.SubType)
               .WithMany()
               .HasForeignKey(a => a.SubTypeId);
            builder.Entity<Order>()
               .HasOne(a => a.Partner)
               .WithMany()
               .HasForeignKey(a => a.PartnerId);
            builder.Entity<Order>()
               .HasOne(a => a.AccMonth)
               .WithMany()
               .HasForeignKey(a => a.AccMonthId);
            builder.Entity<Order>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<Order>()
               .HasOne(a => a.InterCompany)
               .WithMany()
               .HasForeignKey(a => a.InterCompanyId);
            builder.Entity<Order>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<Order>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<Order>()
                  .HasOne(i => i.User)
                  .WithMany()
                  .HasForeignKey(i => i.UserId);
            builder.Entity<Order>()
                  .HasOne(i => i.Company)
                  .WithMany()
                  .HasForeignKey(i => i.CompanyId);
            builder.Entity<Order>()
                  .HasOne(i => i.Account)
                  .WithMany()
                  .HasForeignKey(i => i.AccountId);
            builder.Entity<Order>()
                  .HasOne(i => i.AppState)
                  .WithMany()
                  .HasForeignKey(i => i.AppStateId);
            builder.Entity<Order>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<Order>()
                    .HasOne(i => i.Contract)
                    .WithMany()
                    .HasForeignKey(i => i.ContractId);
            builder.Entity<Order>()
                .HasOne(a => a.EmployeeL1)
                .WithMany()
                .HasForeignKey(a => a.EmployeeL1Id);
            builder.Entity<Order>()
                .HasOne(a => a.EmployeeL2)
                .WithMany()
                .HasForeignKey(a => a.EmployeeL2Id);
            builder.Entity<Order>()
                .HasOne(a => a.EmployeeL3)
                .WithMany()
                .HasForeignKey(a => a.EmployeeL3Id);
            builder.Entity<Order>()
                .HasOne(a => a.EmployeeL4)
                .WithMany()
                .HasForeignKey(a => a.EmployeeL4Id);
            builder.Entity<Order>()
                .HasOne(a => a.EmployeeS1)
                .WithMany()
                .HasForeignKey(a => a.EmployeeS1Id);
            builder.Entity<Order>()
                .HasOne(a => a.EmployeeS2)
                .WithMany()
                .HasForeignKey(a => a.EmployeeS2Id);
            builder.Entity<Order>()
                .HasOne(a => a.EmployeeS3)
                .WithMany()
                .HasForeignKey(a => a.EmployeeS3Id);
			builder.Entity<Order>()
			   .HasOne(a => a.EmployeeB1)
			   .WithMany()
			   .HasForeignKey(a => a.EmployeeB1Id);




			//Country
			builder.Entity<Country>()
                .ToTable("Country")
                .Property(p => p.Id)
                .HasColumnName("CountryId");
            builder.Entity<Country>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Country>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //County
            builder.Entity<County>()
                .ToTable("County")
                .Property(p => p.Id)
                .HasColumnName("CountyId");
            builder.Entity<County>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<County>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //City
            builder.Entity<City>()
                .ToTable("City")
                .Property(p => p.Id)
                .HasColumnName("CityId");
            builder.Entity<City>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<City>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //AccMonth
            builder.Entity<AccMonth>()
                .ToTable("AccMonth")
                .Property(p => p.Id)
                .HasColumnName("AccMonthId");

            ////AccountType
            //builder.Entity<AccountType>()
            //    .ToTable("AccountType")
            //    .Property(p => p.Id)
            //    .HasColumnName("AccountTypeId");
            //builder.Entity<AccountType>()
            //    .Property(p => p.Code)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<AccountType>()
            //    .Property(p => p.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);

            //AssetComponent
            builder.Entity<AssetComponent>()
                .ToTable("AssetComponent")
                .Property(p => p.Id)
                .HasColumnName("AssetComponentId");
            builder.Entity<AssetComponent>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AssetComponent>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<AssetComponent>()
                .Property(p => p.Prefix)
                .HasMaxLength(30);

            //AccSystem
            builder.Entity<AccSystem>()
                .ToTable("AccSystem")
                .Property(p => p.Id)
                .HasColumnName("AccSystemId");
            builder.Entity<AccSystem>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AccSystem>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //ConfigValue
            builder.Entity<ConfigValue>()
                .ToTable("ConfigValue")
                .Property(p => p.Id)
                .HasColumnName("ConfigValueId");
            builder.Entity<ConfigValue>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<ConfigValue>()
                .Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<ConfigValue>()
                 .HasOne(a => a.AspNetRole)
                 .WithMany()
                 .HasForeignKey(a => a.RoleId);

            //Badge
            builder.Entity<Badge>()
                .ToTable("Badge")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Badge>()
                .Property(p => p.Variant)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Badge>()
                .Property(p => p.Text)
                .HasMaxLength(100);

            //Committee
            builder.Entity<Committee>()
                .ToTable("Committees")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Committee>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId);
            builder.Entity<Committee>()
            .HasOne(a => a.Employee2)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId2);
            builder.Entity<Committee>()
            .HasOne(a => a.Employee3)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId3);
            builder.Entity<Committee>()
            .HasOne(a => a.Employee4)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId4);
            builder.Entity<Committee>()
            .HasOne(a => a.Employee5)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId5);
            builder.Entity<Committee>()
              .HasOne(a => a.Employee6)
              .WithMany()
              .HasForeignKey(a => a.EmployeeId6);
            builder.Entity<Committee>()
               .HasOne(a => a.Employee7)
               .WithMany()
               .HasForeignKey(a => a.EmployeeId7);
            builder.Entity<Committee>()
                  .HasOne(a => a.Room)
                  .WithMany()
                  .HasForeignKey(a => a.RoomId);
            builder.Entity<Committee>()
                  .HasOne(a => a.CostCenter)
                  .WithMany()
                  .HasForeignKey(a => a.CostCenterId);
            builder.Entity<Committee>()
                  .HasOne(a => a.Administration)
                  .WithMany()
                  .HasForeignKey(a => a.AdministrationId);

            //Route
            builder.Entity<Route>()
                .ToTable("Route")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Route>()
                .Property(p => p.Icon)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Route>()
                .Property(p => p.Href)
                .HasMaxLength(100);
            builder.Entity<Route>()
                 .HasOne(a => a.Role)
                 .WithMany()
                 .HasForeignKey(a => a.RoleId);
            builder.Entity<Route>()
                   .HasOne(a => a.Badge)
                   .WithMany()
                   .HasForeignKey(a => a.BadgeId);


            //RouteChildren
            builder.Entity<RouteChildren>()
                .ToTable("RouteChildren")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<RouteChildren>()
                .Property(p => p.Icon)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<RouteChildren>()
                .Property(p => p.Href)
                .HasMaxLength(100);
            builder.Entity<RouteChildren>()
                 .HasOne(a => a.Role)
                 .WithMany()
                 .HasForeignKey(a => a.RoleId);
            builder.Entity<RouteChildren>()
                       .HasOne(a => a.Badge)
                       .WithMany()
                       .HasForeignKey(a => a.BadgeId);
            builder.Entity<RouteChildren>()
                       .HasOne(a => a.IconRoute)
                       .WithMany()
                       .HasForeignKey(a => a.IconRouteId);
            //builder.Entity<RouteChildren>()
            //    .HasOne(a => a.Route)
            //    .WithMany()
            //    .HasForeignKey(a => a.RouteId);

            //EmailType
            builder.Entity<PermissionType>()
                .ToTable("PermissionType")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<PermissionType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<PermissionType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
           

            //Permission
            builder.Entity<Permission>()
                .ToTable("Permission")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Permission>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Permission>()
                .Property(p => p.Name)
                .HasMaxLength(100);


            //PermissionRole
            builder.Entity<PermissionRole>()
                .ToTable("PermissionRole")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<PermissionRole>()
                .Property(p => p.Code)
                .HasMaxLength(100);
            builder.Entity<PermissionRole>()
                .Property(p => p.Name)
                .HasMaxLength(100);
            builder.Entity<PermissionRole>()
                 .HasOne(a => a.Role)
                 .WithMany()
                 .HasForeignKey(a => a.RoleId);
            builder.Entity<PermissionRole>()
                .HasOne(a => a.Route)
                .WithMany()
                .HasForeignKey(a => a.RouteId);

            //AdmCenter
            builder.Entity<AdmCenter>()
                .ToTable("AdmCenter")
                .Property(p => p.Id)
                .HasColumnName("AdmCenterId");
            builder.Entity<AdmCenter>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AdmCenter>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<AdmCenter>()
                 .HasOne(a => a.Employee)
                 .WithMany()
                 .HasForeignKey(a => a.EmployeeId);

            //Administration
            builder.Entity<Administration>()
                .ToTable("Administration")
                .Property(p => p.Id)
                .HasColumnName("AdministrationId");
            builder.Entity<Administration>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Administration>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Storage
            builder.Entity<Storage>()
                .ToTable("Storage")
                .Property(p => p.Id)
                .HasColumnName("StorageId");
            builder.Entity<Storage>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Storage>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Plant
            builder.Entity<Plant>()
                .ToTable("Plant")
                .Property(p => p.Id)
                .HasColumnName("PlantId");
            builder.Entity<Plant>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Plant>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Area
            builder.Entity<Area>()
                .ToTable("Area")
                .Property(p => p.Id)
                .HasColumnName("AreaId");
            builder.Entity<Area>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Area>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            //InterCompany
            builder.Entity<InterCompany>()
                .ToTable("InterCompany")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<InterCompany>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<InterCompany>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<InterCompany>()
               .HasOne(a => a.InterCompanyEN)
               .WithMany()
               .HasForeignKey(a => a.InterCompanyENId);
            builder.Entity<InterCompany>()
                  .HasOne(a => a.Account)
                  .WithMany()
                  .HasForeignKey(a => a.AccountId);
            builder.Entity<InterCompany>()
              .HasOne(a => a.ExpAccount)
              .WithMany()
              .HasForeignKey(a => a.ExpAccountId);
            builder.Entity<InterCompany>()
              .HasOne(a => a.AssetCategory)
              .WithMany()
              .HasForeignKey(a => a.AssetCategoryId);
            builder.Entity<InterCompany>()
              .HasOne(a => a.AssetType)
              .WithMany()
              .HasForeignKey(a => a.AssetTypeId);

            //InterCompanyEN
            builder.Entity<InterCompanyEN>()
                .ToTable("InterCompanyEN")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<InterCompanyEN>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<InterCompanyEN>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //Category
            builder.Entity<Category>()
                .ToTable("Category")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Category>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Category>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Category>()
                .HasOne(a => a.InterCompany)
                .WithMany()
                .HasForeignKey(a => a.InterCompanyId);

            //CategoryEN
            builder.Entity<CategoryEN>()
                .ToTable("CategoryEN")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<CategoryEN>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<CategoryEN>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<CategoryEN>()
              .HasOne(a => a.InterCompanyEN)
              .WithMany()
              .HasForeignKey(a => a.InterCompanyENId);

            //SubCategory
            builder.Entity<SubCategory>()
                .ToTable("SubCategory")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<SubCategory>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<SubCategory>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<SubCategory>()
                .HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.CategoryId);
            builder.Entity<SubCategory>()
               .HasOne(a => a.CategoryEN)
               .WithMany()
               .HasForeignKey(a => a.CategoryENId);


            //SubCategoryEN
            builder.Entity<SubCategoryEN>()
                .ToTable("SubCategoryEN")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<SubCategoryEN>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<SubCategoryEN>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<SubCategoryEN>()
                .HasOne(a => a.CategoryEN)
                .WithMany()
                .HasForeignKey(a => a.CategoryENId);



            //InsuranceCategory
            builder.Entity<InsuranceCategory>()
                .ToTable("InsuranceCategory")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<InsuranceCategory>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<InsuranceCategory>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);




            //MasterType
            builder.Entity<MasterType>()
                .ToTable("MasterType")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<MasterType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<MasterType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Type
            builder.Entity<Model.Type>()
                .ToTable("Type")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Model.Type>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Model.Type>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            ////SubType
            //builder.Entity<SubType>()
            //	.ToTable("SubType")
            //	.Property(p => p.Id)
            //	.HasColumnName("Id");
            //builder.Entity<SubType>()
            //	.Property(p => p.Code)
            //	.IsRequired()
            //	.HasMaxLength(30);
            //builder.Entity<SubType>()
            //	.Property(p => p.Name)
            //	.IsRequired()
            //	.HasMaxLength(100);

            //SubTypePartner
            builder.Entity<SubTypePartner>()
                .ToTable("SubTypePartner")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<SubTypePartner>()
               .HasOne(a => a.SubType)
               .WithMany()
               .HasForeignKey(a => a.SubTypeId);
            builder.Entity<SubTypePartner>()
               .HasOne(a => a.Partner)
               .WithMany()
               .HasForeignKey(a => a.PartnerId);



            //DictionaryItem
            builder.Entity<DictionaryItem>()
                .ToTable("DictionaryItem")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<DictionaryItem>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<DictionaryItem>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Dimension
            builder.Entity<Dimension>()
                .ToTable("Dimension")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Dimension>()
                .Property(p => p.Length)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Dimension>()
                .Property(p => p.Width)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<Dimension>()
                .Property(p => p.Height)
                .HasMaxLength(30);

            ////AdministrationCostCenter
            //builder.Entity<AdministrationCostCenter>()
            //    .ToTable("AdministrationCostCenter")
            //    .HasKey(a => new { a.AdministrationId, a.CostCenterId });

            //builder.Entity<AdministrationCostCenter>()
            //    .HasOne(ae => ae.Administration)
            //    .WithMany(a => a.AdministrationCostCenters)
            //    .HasForeignKey(ae => ae.AdministrationId);

            //builder.Entity<AdministrationCostCenter>()
            //    .HasOne(ac => ac.CostCenter)
            //    .WithMany(c => c.AdministrationCostCenters)
            //    .HasForeignKey(ae => ae.CostCenterId);

            ////AdministrationEmployee
            //builder.Entity<AdministrationEmployee>()
            //    .ToTable("AdministrationEmployee")
            //    .HasKey(a => new { a.AdministrationId, a.EmployeeId });

            //builder.Entity<AdministrationEmployee>()
            //    .HasOne(ae => ae.Administration)
            //    .WithMany(a => a.AdministrationEmployees)
            //    .HasForeignKey(ae => ae.AdministrationId);

            //builder.Entity<AdministrationEmployee>()
            //    .HasOne(ae => ae.Employee)
            //    .WithMany(a => a.AdministrationEmployees)
            //    .HasForeignKey(ae => ae.EmployeeId);

            ////Analytic
            //builder.Entity<Analytic>()
            //    .ToTable("Analytic")
            //    .Property(p => p.Id)
            //    .HasColumnName("AnalyticId");
            //builder.Entity<Analytic>()
            //    .Property(p => p.Code)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<Analytic>()
            //    .Property(p => p.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);

            //AppState
            builder.Entity<AppState>()
                .ToTable("AppState")
                .Property(p => p.Id)
                .HasColumnName("AppStateId");
            builder.Entity<AppState>()
                .Property(p => p.Code)
                .HasMaxLength(30);
            builder.Entity<AppState>()
                .Property(p => p.ParentCode)
                .HasMaxLength(30);
            builder.Entity<AppState>()
                .Property(p => p.Name)
                .HasMaxLength(100);
            builder.Entity<AppState>()
                .Property(p => p.Mask)
                .HasMaxLength(200);

            //Asset
            builder.Entity<Asset>()
                .ToTable("Asset")
                .Property(p => p.Id)
                .HasColumnName("AssetId");

            //builder.Entity<Asset>()
            //    .HasOne(p => p.AssetInv)
            //    .WithOne(i => i.Asset)
            //    .HasForeignKey<AssetInv>(a => a.Id);

            builder.Entity<Asset>()
                .Property(p => p.InvNo)
                .HasMaxLength(30);
            builder.Entity<Asset>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(120);
            builder.Entity<Asset>()
                .Property(p => p.SerialNumber)
                .HasMaxLength(50);
            builder.Entity<Asset>()
                .Property(p => p.ERPCode)
                .HasMaxLength(50);
            builder.Entity<Asset>()
                .Property(p => p.PurchaseDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Asset>()
                .Property(p => p.ValueInv)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Asset>()
                .Property(p => p.ValueRem)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<Asset>()
                .HasOne(a => a.Document)
                .WithMany()
                .HasForeignKey(a => a.DocumentId);
            builder.Entity<Asset>()
                .HasOne(a => a.Administration)
                .WithMany()
                .HasForeignKey(a => a.AdministrationId);
            builder.Entity<Asset>()
               .HasOne(a => a.SubType)
               .WithMany()
               .HasForeignKey(a => a.SubTypeId);
            builder.Entity<Asset>()
               .HasOne(a => a.InsuranceCategory)
               .WithMany()
               .HasForeignKey(a => a.InsuranceCategoryId);
            builder.Entity<Asset>()
               .HasOne(a => a.Model)
               .WithMany()
               .HasForeignKey(a => a.ModelId);
            builder.Entity<Asset>()
               .HasOne(a => a.Brand)
               .WithMany()
               .HasForeignKey(a => a.BrandId);
            builder.Entity<Asset>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<Asset>()
               .HasOne(a => a.InterCompany)
               .WithMany()
               .HasForeignKey(a => a.InterCompanyId);
            builder.Entity<Asset>()
                .HasOne(a => a.AssetCategory)
                .WithMany()
                .HasForeignKey(a => a.AssetCategoryId);
            builder.Entity<Asset>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<Asset>()
                .HasOne(a => a.Department)
                .WithMany()
                .HasForeignKey(a => a.DepartmentId);
            builder.Entity<Asset>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<Asset>()
                .HasOne(a => a.Room)
                .WithMany()
                .HasForeignKey(a => a.RoomId);
            builder.Entity<Asset>()
                .HasOne(a => a.AssetType)
                .WithMany()
                .HasForeignKey(a => a.AssetTypeId);
            builder.Entity<Asset>()
                .HasOne(a => a.AssetState)
                .WithMany()
                .HasForeignKey(a => a.AssetStateId);
            builder.Entity<Asset>()
                .HasOne(a => a.InvState)
                .WithMany()
                .HasForeignKey(a => a.InvStateId);
            builder.Entity<Asset>()
                .HasOne(a => a.Uom)
                .WithMany()
                .HasForeignKey(a => a.UomId);
            builder.Entity<Asset>()
               .HasOne(a => a.Dimension)
               .WithMany()
               .HasForeignKey(a => a.DimensionId);
            builder.Entity<Asset>()
                .HasOne(a => a.AssetInv)
                .WithOne(a => a.Asset)
                .HasForeignKey<AssetInv>(a => a.AssetId);
            builder.Entity<Asset>()
                  .HasOne(i => i.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(i => i.CreatedUser);
            builder.Entity<Asset>()
                   .HasOne(a => a.Material)
                   .WithMany()
                   .HasForeignKey(a => a.MaterialId);
            builder.Entity<Asset>()
                   .HasOne(a => a.ReqBFMaterial)
                   .WithMany()
                   .HasForeignKey(a => a.ReqBFMaterialId);
			builder.Entity<Asset>()
			   .HasOne(i => i.TempUser)
			   .WithMany()
			   .HasForeignKey(i => i.TempUserId);
            builder.Entity<Asset>()
                .HasOne(a => a.WFHState)
                .WithMany()
                .HasForeignKey(a => a.WFHStateId);
            //builder.Entity<Asset>()
            //        .HasMany(a => a.CreateAssetSAP)
            //        .WithOne();
            //builder.Entity<Asset>()
            //        .HasMany(a => a.AssetChangeSAP)
            //        .WithOne();
            //builder.Entity<Asset>()
            //         .HasMany(a => a.AssetInvPlusSAP)
            //         .WithOne();
            //builder.Entity<Asset>()
            //         .HasMany(a => a.AssetInvMinusSAP)
            //         .WithOne();
            //builder.Entity<Asset>()
            //       .HasMany(a => a.RetireAssetSAP)
            //       .WithOne();
            //builder.Entity<Asset>()
            //        .HasMany(a => a.TransferAssetSAP)
            //        .WithOne();
            //builder.Entity<Asset>()
            //        .HasMany(a => a.TransferInStockSAP)
            //        .WithOne();
            //builder.Entity<Asset>()
            //       .HasMany(a => a.AcquisitionAssetSAP)
            //       .WithOne();
            //builder.Entity<Asset>()
            //    .HasOne(p => p.AcquisitionAssetSAP)
            //    .WithOne(c => c.Asset)
            //    .HasForeignKey<AcquisitionAssetSAP>(c => c.AssetId);
            //builder.Entity<Asset>().HasOne(e => e.AcquisitionAssetSAP)
            //        .WithMany()
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.SetNull);


            //AssetNi
            builder.Entity<AssetNi>()
                .ToTable("AssetNi")
                .Property(p => p.Id)
                .HasColumnName("AssetNiId");
            builder.Entity<AssetNi>()
                .Property(p => p.Code1)
                .HasMaxLength(30);
            builder.Entity<AssetNi>()
                .Property(p => p.Code2)
                .HasMaxLength(30);
            builder.Entity<AssetNi>()
                .Property(p => p.Name1)
                .IsRequired()
                .HasMaxLength(120);
            builder.Entity<AssetNi>()
                .Property(p => p.Name2)
                .IsRequired()
                .HasMaxLength(120);
            builder.Entity<AssetNi>()
                .Property(p => p.SerialNumber)
                .HasMaxLength(50);
            builder.Entity<AssetNi>()
                .Property(p => p.Producer)
                .HasMaxLength(30);
            builder.Entity<AssetNi>()
                .Property(p => p.Model)
                .HasMaxLength(30);
            builder.Entity<AssetNi>()
                .Property(p => p.Info)
                .HasMaxLength(200);

            //AssetOp
            builder.Entity<AssetOp>()
                .ToTable("AssetOp")
                .Property(i => i.Id)
                .HasColumnName("AssetOpId");
            //builder.Entity<AssetOp>()
            //    .HasOne(i => i.Document)
            //    .WithMany()
            //    .HasForeignKey(i => i.DocumentId);
            //builder.Entity<AssetOp>()
            //    .HasOne(i => i.Asset)
            //    .WithMany()
            //    .HasForeignKey(i => i.AssetId);

            builder.Entity<AssetOp>()
                .HasOne(i => i.AdministrationInitial)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);


            builder.Entity<AssetOp>()
              .HasOne(i => i.BudgetManagerInitial)
              .WithMany()
              .HasForeignKey(i => i.BudgetManagerIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.BudgetManagerFinal)
                .WithMany()
                .HasForeignKey(i => i.BudgetManagerIdFinal);


            builder.Entity<AssetOp>()
              .HasOne(i => i.ProjectInitial)
              .WithMany()
              .HasForeignKey(i => i.ProjectIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.ProjectFinal)
                .WithMany()
                .HasForeignKey(i => i.ProjectIdFinal);

            builder.Entity<AssetOp>()
              .HasOne(i => i.DimensionInitial)
              .WithMany()
              .HasForeignKey(i => i.DimensionIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.DimensionFinal)
                .WithMany()
                .HasForeignKey(i => i.DimensionIdFinal);

            builder.Entity<AssetOp>()
              .HasOne(i => i.AssetNatureInitial)
              .WithMany()
              .HasForeignKey(i => i.AssetNatureIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.AssetNatureFinal)
                .WithMany()
                .HasForeignKey(i => i.AssetNatureIdFinal);


            builder.Entity<AssetOp>()
                .HasOne(i => i.CostCenterInitial)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.CostCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdFinal);

            builder.Entity<AssetOp>()
                .HasOne(i => i.AssetStateInitial)
                .WithMany()
                .HasForeignKey(i => i.AssetStateIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.AssetStateFinal)
                .WithMany()
                .HasForeignKey(i => i.AssetStateIdFinal);

            builder.Entity<AssetOp>()
                .HasOne(i => i.DepartmentInitial)
                .WithMany()
                .HasForeignKey(i => i.DepartmentIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.DepartmentFinal)
                .WithMany()
                .HasForeignKey(i => i.DepartmentIdFinal);

            builder.Entity<AssetOp>()
                .HasOne(i => i.EmployeeInitial)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);

            builder.Entity<AssetOp>()
                .HasOne(i => i.RoomInitial)
                .WithMany()
                .HasForeignKey(i => i.RoomIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.RoomFinal)
                .WithMany()
                .HasForeignKey(i => i.RoomIdFinal);

            builder.Entity<AssetOp>()
                .HasOne(i => i.AssetCategoryInitial)
                .WithMany()
                .HasForeignKey(i => i.AssetCategoryIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.AssetCategoryFinal)
                .WithMany()
                .HasForeignKey(i => i.AssetCategoryIdFinal);

            builder.Entity<AssetOp>()
                .HasOne(i => i.InvStateInitial)
                .WithMany()
                .HasForeignKey(i => i.InvStateIdInitial);
            builder.Entity<AssetOp>()
                .HasOne(i => i.InvStateFinal)
                .WithMany()
                .HasForeignKey(i => i.InvStateIdFinal);
            builder.Entity<AssetOp>()
                .HasOne(i => i.AssetOpState)
                .WithMany()
                .HasForeignKey(i => i.AssetOpStateId);

            builder.Entity<AssetOp>()
                .HasOne(i => i.ReleaseConfUser)
                .WithMany()
                .HasForeignKey(i => i.ReleaseConfBy);
            builder.Entity<AssetOp>()
                .HasOne(i => i.SrcConfUser)
                .WithMany()
                .HasForeignKey(i => i.SrcConfBy);
            builder.Entity<AssetOp>()
                .HasOne(i => i.DstConfUser)
                .WithMany()
                .HasForeignKey(i => i.DstConfBy);
            builder.Entity<AssetOp>()
                .HasOne(i => i.RegisterConfUser)
                .WithMany()
                .HasForeignKey(i => i.RegisterConfBy);
            builder.Entity<AssetOp>()
                  .HasOne(i => i.EntityType)
                  .WithMany()
                  .HasForeignKey(i => i.EntityTypeId);


            //BudgetOp
            builder.Entity<BudgetOp>()
                .ToTable("BudgetOp")
                .Property(i => i.Id)
                .HasColumnName("Id");
			builder.Entity<BudgetOp>()
				.HasOne(i => i.Document)
				.WithMany()
				.HasForeignKey(i => i.DocumentId);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.AccMonth)
                .WithMany()
                .HasForeignKey(i => i.AccMonthId);
            builder.Entity<BudgetOp>()
				.HasOne(i => i.Budget)
				.WithMany()
				.HasForeignKey(i => i.BudgetId);
			builder.Entity<BudgetOp>()
                .HasOne(i => i.AdministrationInitial)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.AdministrationInitial)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.AccountInitial)
                .WithMany()
                .HasForeignKey(i => i.AccountIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.AccountFinal)
                .WithMany()
                .HasForeignKey(i => i.AccountIdFinal);
            builder.Entity<BudgetOp>()
                  .HasOne(i => i.BudgetManagerInitial)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.BudgetManagerFinal)
                .WithMany()
                .HasForeignKey(i => i.BudgetManagerIdFinal);
            builder.Entity<BudgetOp>()
                 .HasOne(i => i.CompanyInitial)
                 .WithMany()
                 .HasForeignKey(i => i.CompanyIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.CompanyFinal)
                .WithMany()
                .HasForeignKey(i => i.CompanyIdFinal);

            builder.Entity<BudgetOp>()
                 .HasOne(i => i.CostCenterInitial)
                 .WithMany()
                 .HasForeignKey(i => i.CostCenterIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.CostCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdFinal);
            builder.Entity<BudgetOp>()
              .HasOne(i => i.EmployeeInitial)
              .WithMany()
              .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.InterCompanyInitial)
                .WithMany()
                .HasForeignKey(i => i.InterCompanyIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.InterCompanyFinal)
                .WithMany()
                .HasForeignKey(i => i.InterCompanyIdFinal);
            builder.Entity<BudgetOp>()
                 .HasOne(i => i.PartnerInitial)
                 .WithMany()
                 .HasForeignKey(i => i.PartnerIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.PartnerFinal)
                .WithMany()
                .HasForeignKey(i => i.PartnerIdFinal);
            builder.Entity<BudgetOp>()
              .HasOne(i => i.ProjectInitial)
              .WithMany()
              .HasForeignKey(i => i.ProjectIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.ProjectFinal)
                .WithMany()
                .HasForeignKey(i => i.ProjectIdFinal);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.SubTypeInitial)
                .WithMany()
                .HasForeignKey(i => i.SubTypeIdInitial);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.SubTypeFinal)
                .WithMany()
                .HasForeignKey(i => i.SubTypeIdFinal);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.ReleaseConfUser)
                .WithMany()
                .HasForeignKey(i => i.ReleaseConfBy);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.SrcConfUser)
                .WithMany()
                .HasForeignKey(i => i.SrcConfBy);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.DstConfUser)
                .WithMany()
                .HasForeignKey(i => i.DstConfBy);
            builder.Entity<BudgetOp>()
                .HasOne(i => i.RegisterConfUser)
                .WithMany()
                .HasForeignKey(i => i.RegisterConfBy);

            builder.Entity<BudgetOp>()
                   .HasOne(i => i.CountryInitial)
                   .WithMany()
                   .HasForeignKey(i => i.CountryIdInitial);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.ActivityInitial)
                   .WithMany()
                   .HasForeignKey(i => i.ActivityIdInitial);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.AdmCenterInitial)
                   .WithMany()
                   .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.RegionInitial)
                   .WithMany()
                   .HasForeignKey(i => i.RegionIdInitial);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.AssetTypeInitial)
                   .WithMany()
                   .HasForeignKey(i => i.AssetTypeIdInitial);

            builder.Entity<BudgetOp>()
                 .HasOne(i => i.CountryFinal)
                 .WithMany()
                 .HasForeignKey(i => i.CountryIdFinal);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.ActivityFinal)
                   .WithMany()
                   .HasForeignKey(i => i.ActivityIdFinal);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.AdmCenterFinal)
                   .WithMany()
                   .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.RegionFinal)
                   .WithMany()
                   .HasForeignKey(i => i.RegionIdFinal);
            builder.Entity<BudgetOp>()
                   .HasOne(i => i.AssetTypeFinal)
                   .WithMany()
                   .HasForeignKey(i => i.AssetTypeIdFinal);
            builder.Entity<BudgetOp>()
                     .HasOne(i => i.BudgetStateFinal)
                     .WithMany()
                     .HasForeignKey(i => i.BudgetStateIdFinal);
            builder.Entity<BudgetOp>()
               .HasOne(i => i.BudgetBase)
               .WithMany()
               .HasForeignKey(i => i.BudgetBaseId);


            //BudgetBaseOp
            builder.Entity<BudgetBaseOp>()
                .ToTable("BudgetBaseOp")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetBaseOp>()
                .HasOne(i => i.Document)
                .WithMany()
                .HasForeignKey(i => i.DocumentId);
			builder.Entity<BudgetBaseOp>()
				.HasOne(i => i.AccMonth)
				.WithMany()
				.HasForeignKey(i => i.AccMonthId);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.Budget)
			//    .WithMany()
			//    .HasForeignKey(i => i.BudgetId);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.AdministrationInitial)
			//    .WithMany()
			//    .HasForeignKey(i => i.AdministrationIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.AdministrationFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.AdministrationIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.AdministrationInitial)
			//    .WithMany()
			//    .HasForeignKey(i => i.AdministrationIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.AdministrationFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.AdministrationIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.AccountInitial)
			//    .WithMany()
			//    .HasForeignKey(i => i.AccountIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.AccountFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.AccountIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//      .HasOne(i => i.BudgetManagerInitial)
			//      .WithMany()
			//      .HasForeignKey(i => i.BudgetManagerIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.BudgetManagerFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.BudgetManagerIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//     .HasOne(i => i.CompanyInitial)
			//     .WithMany()
			//     .HasForeignKey(i => i.CompanyIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.CompanyFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.CompanyIdFinal);

			//builder.Entity<BudgetBaseOp>()
			//     .HasOne(i => i.CostCenterInitial)
			//     .WithMany()
			//     .HasForeignKey(i => i.CostCenterIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.CostCenterFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.CostCenterIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//  .HasOne(i => i.EmployeeInitial)
			//  .WithMany()
			//  .HasForeignKey(i => i.EmployeeIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.EmployeeFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.EmployeeIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.InterCompanyInitial)
			//    .WithMany()
			//    .HasForeignKey(i => i.InterCompanyIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.InterCompanyFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.InterCompanyIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//     .HasOne(i => i.PartnerInitial)
			//     .WithMany()
			//     .HasForeignKey(i => i.PartnerIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.PartnerFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.PartnerIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//  .HasOne(i => i.ProjectInitial)
			//  .WithMany()
			//  .HasForeignKey(i => i.ProjectIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.ProjectFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.ProjectIdFinal);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.SubTypeInitial)
			//    .WithMany()
			//    .HasForeignKey(i => i.SubTypeIdInitial);
			//builder.Entity<BudgetBaseOp>()
			//    .HasOne(i => i.SubTypeFinal)
			//    .WithMany()
			//    .HasForeignKey(i => i.SubTypeIdFinal);
			builder.Entity<BudgetBaseOp>()
                .HasOne(i => i.ReleaseConfUser)
                .WithMany()
                .HasForeignKey(i => i.ReleaseConfBy);
            builder.Entity<BudgetBaseOp>()
                .HasOne(i => i.SrcConfUser)
                .WithMany()
                .HasForeignKey(i => i.SrcConfBy);
            builder.Entity<BudgetBaseOp>()
                .HasOne(i => i.DstConfUser)
                .WithMany()
                .HasForeignKey(i => i.DstConfBy);
            builder.Entity<BudgetBaseOp>()
                .HasOne(i => i.RegisterConfUser)
                .WithMany()
                .HasForeignKey(i => i.RegisterConfBy);

            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.CountryInitial)
            //       .WithMany()
            //       .HasForeignKey(i => i.CountryIdInitial);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.ActivityInitial)
            //       .WithMany()
            //       .HasForeignKey(i => i.ActivityIdInitial);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.AdmCenterInitial)
            //       .WithMany()
            //       .HasForeignKey(i => i.AdministrationIdInitial);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.RegionInitial)
            //       .WithMany()
            //       .HasForeignKey(i => i.RegionIdInitial);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.AssetTypeInitial)
            //       .WithMany()
            //       .HasForeignKey(i => i.AssetTypeIdInitial);

            //builder.Entity<BudgetBaseOp>()
            //     .HasOne(i => i.CountryFinal)
            //     .WithMany()
            //     .HasForeignKey(i => i.CountryIdFinal);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.ActivityFinal)
            //       .WithMany()
            //       .HasForeignKey(i => i.ActivityIdFinal);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.AdmCenterFinal)
            //       .WithMany()
            //       .HasForeignKey(i => i.AdministrationIdFinal);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.RegionFinal)
            //       .WithMany()
            //       .HasForeignKey(i => i.RegionIdFinal);
            //builder.Entity<BudgetBaseOp>()
            //       .HasOne(i => i.AssetTypeFinal)
            //       .WithMany()
            //       .HasForeignKey(i => i.AssetTypeIdFinal);
            builder.Entity<BudgetBaseOp>()
                     .HasOne(i => i.BudgetStateFinal)
                     .WithMany()
                     .HasForeignKey(i => i.BudgetStateIdFinal);
            builder.Entity<BudgetBaseOp>()
               .HasOne(i => i.BudgetBase)
               .WithMany()
               .HasForeignKey(i => i.BudgetBaseId);
			builder.Entity<BudgetBaseOp>()
			   .HasOne(i => i.BudgetForecastFin)
			   .WithMany()
			   .HasForeignKey(i => i.BudgetForecastFinId);


			//OrderOp
			builder.Entity<OrderOp>()
                .ToTable("OrderOp")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<OrderOp>()
                .HasOne(i => i.Document)
                .WithMany()
                .HasForeignKey(i => i.DocumentId);
            builder.Entity<OrderOp>()
                .HasOne(i => i.AccMonth)
                .WithMany()
                .HasForeignKey(i => i.AccMonthId);
            builder.Entity<OrderOp>()
                .HasOne(i => i.Order)
                .WithMany()
                .HasForeignKey(i => i.OrderId);
            builder.Entity<OrderOp>()
                .HasOne(i => i.AdministrationInitial)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<OrderOp>()
                .HasOne(i => i.AdministrationInitial)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<OrderOp>()
                .HasOne(i => i.AccountInitial)
                .WithMany()
                .HasForeignKey(i => i.AccountIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.AccountFinal)
                .WithMany()
                .HasForeignKey(i => i.AccountIdFinal);
            builder.Entity<OrderOp>()
                  .HasOne(i => i.BudgetManagerInitial)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.BudgetManagerFinal)
                .WithMany()
                .HasForeignKey(i => i.BudgetManagerIdFinal);
            builder.Entity<OrderOp>()
                 .HasOne(i => i.CompanyInitial)
                 .WithMany()
                 .HasForeignKey(i => i.CompanyIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.CompanyFinal)
                .WithMany()
                .HasForeignKey(i => i.CompanyIdFinal);

            builder.Entity<OrderOp>()
                 .HasOne(i => i.CostCenterInitial)
                 .WithMany()
                 .HasForeignKey(i => i.CostCenterIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.CostCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdFinal);
            builder.Entity<OrderOp>()
              .HasOne(i => i.EmployeeInitial)
              .WithMany()
              .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);
            builder.Entity<OrderOp>()
                .HasOne(i => i.InterCompanyInitial)
                .WithMany()
                .HasForeignKey(i => i.InterCompanyIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.InterCompanyFinal)
                .WithMany()
                .HasForeignKey(i => i.InterCompanyIdFinal);
            builder.Entity<OrderOp>()
                 .HasOne(i => i.PartnerInitial)
                 .WithMany()
                 .HasForeignKey(i => i.PartnerIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.PartnerFinal)
                .WithMany()
                .HasForeignKey(i => i.PartnerIdFinal);
            builder.Entity<OrderOp>()
              .HasOne(i => i.ProjectInitial)
              .WithMany()
              .HasForeignKey(i => i.ProjectIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.ProjectFinal)
                .WithMany()
                .HasForeignKey(i => i.ProjectIdFinal);
            builder.Entity<OrderOp>()
                .HasOne(i => i.SubTypeInitial)
                .WithMany()
                .HasForeignKey(i => i.SubTypeIdInitial);
            builder.Entity<OrderOp>()
                .HasOne(i => i.SubTypeFinal)
                .WithMany()
                .HasForeignKey(i => i.SubTypeIdFinal);
            builder.Entity<OrderOp>()
                .HasOne(i => i.ReleaseConfUser)
                .WithMany()
                .HasForeignKey(i => i.ReleaseConfBy);
            builder.Entity<OrderOp>()
                .HasOne(i => i.SrcConfUser)
                .WithMany()
                .HasForeignKey(i => i.SrcConfBy);
            builder.Entity<OrderOp>()
                .HasOne(i => i.DstConfUser)
                .WithMany()
                .HasForeignKey(i => i.DstConfBy);
            builder.Entity<OrderOp>()
                .HasOne(i => i.RegisterConfUser)
                .WithMany()
                .HasForeignKey(i => i.RegisterConfBy);


            //RequestOp
            builder.Entity<RequestOp>()
                .ToTable("RequestOp")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<RequestOp>()
                .HasOne(i => i.Document)
                .WithMany()
                .HasForeignKey(i => i.DocumentId);
            builder.Entity<RequestOp>()
                .HasOne(i => i.AccMonth)
                .WithMany()
                .HasForeignKey(i => i.AccMonthId);
            builder.Entity<RequestOp>()
                .HasOne(i => i.Request)
                .WithMany()
                .HasForeignKey(i => i.RequestId);
            builder.Entity<RequestOp>()
                  .HasOne(i => i.BudgetManager)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerId);
            builder.Entity<RequestOp>()
                 .HasOne(i => i.Company)
                 .WithMany()
                 .HasForeignKey(i => i.CompanyId);
            builder.Entity<RequestOp>()
                 .HasOne(i => i.CostCenterInitial)
                 .WithMany()
                 .HasForeignKey(i => i.CostCenterIdInitial);
            builder.Entity<RequestOp>()
                .HasOne(i => i.CostCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdFinal);
            builder.Entity<RequestOp>()
              .HasOne(i => i.EmployeeInitial)
              .WithMany()
              .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<RequestOp>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);
            builder.Entity<RequestOp>()
              .HasOne(i => i.ProjectInitial)
              .WithMany()
              .HasForeignKey(i => i.ProjectIdInitial);
            builder.Entity<RequestOp>()
                .HasOne(i => i.ProjectFinal)
                .WithMany()
                .HasForeignKey(i => i.ProjectIdFinal);
            builder.Entity<RequestOp>()
                .HasOne(i => i.ReleaseConfUser)
                .WithMany()
                .HasForeignKey(i => i.ReleaseConfBy);
            builder.Entity<RequestOp>()
                .HasOne(i => i.SrcConfUser)
                .WithMany()
                .HasForeignKey(i => i.SrcConfBy);
            builder.Entity<RequestOp>()
                .HasOne(i => i.DstConfUser)
                .WithMany()
                .HasForeignKey(i => i.DstConfBy);
            builder.Entity<RequestOp>()
                .HasOne(i => i.RegisterConfUser)
                .WithMany()
                .HasForeignKey(i => i.RegisterConfBy);
            builder.Entity<RequestOp>()
            .HasOne(i => i.BudgetInitial)
            .WithMany()
            .HasForeignKey(i => i.BudgetIdInitial);
            builder.Entity<RequestOp>()
                .HasOne(i => i.BudgetFinal)
                .WithMany()
                .HasForeignKey(i => i.BudgetIdFinal);


            //OfferOp
            builder.Entity<OfferOp>()
                .ToTable("OfferOp")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<OfferOp>()
                .HasOne(i => i.Document)
                .WithMany()
                .HasForeignKey(i => i.DocumentId);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AccMonth)
                .WithMany()
                .HasForeignKey(i => i.AccMonthId);
            builder.Entity<OfferOp>()
                .HasOne(i => i.Offer)
                .WithMany()
                .HasForeignKey(i => i.OfferId);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AdministrationInitial)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AdministrationInitial)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AccountInitial)
                .WithMany()
                .HasForeignKey(i => i.AccountIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AccountFinal)
                .WithMany()
                .HasForeignKey(i => i.AccountIdFinal);
            builder.Entity<OfferOp>()
                  .HasOne(i => i.BudgetManagerInitial)
                  .WithMany()
                  .HasForeignKey(i => i.BudgetManagerIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.BudgetManagerFinal)
                .WithMany()
                .HasForeignKey(i => i.BudgetManagerIdFinal);
            builder.Entity<OfferOp>()
                 .HasOne(i => i.CompanyInitial)
                 .WithMany()
                 .HasForeignKey(i => i.CompanyIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.CompanyFinal)
                .WithMany()
                .HasForeignKey(i => i.CompanyIdFinal);

            builder.Entity<OfferOp>()
                 .HasOne(i => i.CostCenterInitial)
                 .WithMany()
                 .HasForeignKey(i => i.CostCenterIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.CostCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdFinal);
            builder.Entity<OfferOp>()
              .HasOne(i => i.EmployeeInitial)
              .WithMany()
              .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);
            builder.Entity<OfferOp>()
                .HasOne(i => i.InterCompanyInitial)
                .WithMany()
                .HasForeignKey(i => i.InterCompanyIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.InterCompanyFinal)
                .WithMany()
                .HasForeignKey(i => i.InterCompanyIdFinal);
            builder.Entity<OfferOp>()
                 .HasOne(i => i.PartnerInitial)
                 .WithMany()
                 .HasForeignKey(i => i.PartnerIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.PartnerFinal)
                .WithMany()
                .HasForeignKey(i => i.PartnerIdFinal);
            builder.Entity<OfferOp>()
              .HasOne(i => i.ProjectInitial)
              .WithMany()
              .HasForeignKey(i => i.ProjectIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.ProjectFinal)
                .WithMany()
                .HasForeignKey(i => i.ProjectIdFinal);
            builder.Entity<OfferOp>()
                .HasOne(i => i.SubTypeInitial)
                .WithMany()
                .HasForeignKey(i => i.SubTypeIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.SubTypeFinal)
                .WithMany()
                .HasForeignKey(i => i.SubTypeIdFinal);
            builder.Entity<OfferOp>()
                .HasOne(i => i.ReleaseConfUser)
                .WithMany()
                .HasForeignKey(i => i.ReleaseConfBy);
            builder.Entity<OfferOp>()
                .HasOne(i => i.SrcConfUser)
                .WithMany()
                .HasForeignKey(i => i.SrcConfBy);
            builder.Entity<OfferOp>()
                .HasOne(i => i.DstConfUser)
                .WithMany()
                .HasForeignKey(i => i.DstConfBy);
            builder.Entity<OfferOp>()
                .HasOne(i => i.RegisterConfUser)
                .WithMany()
                .HasForeignKey(i => i.RegisterConfBy);
            builder.Entity<OfferOp>()
              .HasOne(i => i.AdmCenterInitial)
              .WithMany()
              .HasForeignKey(i => i.AdmCenterIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AdmCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.AdmCenterIdFinal);
            builder.Entity<OfferOp>()
             .HasOne(i => i.RegionInitial)
             .WithMany()
             .HasForeignKey(i => i.RegionIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.RegionFinal)
                .WithMany()
                .HasForeignKey(i => i.RegionIdFinal);
            builder.Entity<OfferOp>()
             .HasOne(i => i.AssetTypeInitial)
             .WithMany()
             .HasForeignKey(i => i.AssetTypeIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.AssetTypeFinal)
                .WithMany()
                .HasForeignKey(i => i.AssetTypeIdFinal);
            builder.Entity<OfferOp>()
             .HasOne(i => i.ProjectTypeInitial)
             .WithMany()
             .HasForeignKey(i => i.ProjectTypeIdInitial);
            builder.Entity<OfferOp>()
                .HasOne(i => i.ProjectTypeFinal)
                .WithMany()
                .HasForeignKey(i => i.ProjectTypeIdFinal);


            //ContractOp
            builder.Entity<ContractOp>()
                .ToTable("ContractOp")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<ContractOp>()
                .HasOne(i => i.Document)
                .WithMany()
                .HasForeignKey(i => i.DocumentId);
            builder.Entity<ContractOp>()
                .HasOne(i => i.AccMonth)
                .WithMany()
                .HasForeignKey(i => i.AccMonthId);
            builder.Entity<ContractOp>()
                .HasOne(i => i.Contract)
                .WithMany()
                .HasForeignKey(i => i.ContractId);
            builder.Entity<ContractOp>()
                .HasOne(i => i.ContractDivision)
                .WithMany()
                .HasForeignKey(i => i.ContractDivisionId);
            builder.Entity<ContractOp>()
                .HasOne(i => i.Commodity)
                .WithMany()
                .HasForeignKey(i => i.CommodityId);
            builder.Entity<ContractOp>()
                 .HasOne(i => i.ContractRegion)
                 .WithMany()
                 .HasForeignKey(i => i.ContractRegionId);
            builder.Entity<ContractOp>()
                .HasOne(i => i.Company)
                .WithMany()
                .HasForeignKey(i => i.CompanyId);
            builder.Entity<ContractOp>()
              .HasOne(i => i.Employee)
              .WithMany()
              .HasForeignKey(i => i.EmployeeId);
            builder.Entity<ContractOp>()
                .HasOne(i => i.BusinessSystem)
                .WithMany()
                .HasForeignKey(i => i.BusinessSystemId);
            builder.Entity<ContractOp>()
                 .HasOne(i => i.ContractAmount)
                 .WithMany()
                 .HasForeignKey(i => i.ContractAmountId);
            builder.Entity<ContractOp>()
                .HasOne(i => i.ReleaseConfUser)
                .WithMany()
                .HasForeignKey(i => i.ReleaseConfBy);
            builder.Entity<ContractOp>()
                .HasOne(i => i.SrcConfUser)
                .WithMany()
                .HasForeignKey(i => i.SrcConfBy);
            builder.Entity<ContractOp>()
                .HasOne(i => i.DstConfUser)
                .WithMany()
                .HasForeignKey(i => i.DstConfBy);
            builder.Entity<ContractOp>()
                .HasOne(i => i.RegisterConfUser)
                .WithMany()
                .HasForeignKey(i => i.RegisterConfBy);

            //AssetComponentOp
            builder.Entity<AssetComponentOp>()
                .ToTable("AssetComponentOp")
                .Property(i => i.Id)
                .HasColumnName("AssetComponentOpId");
            builder.Entity<AssetComponentOp>()
               .HasOne(i => i.EmployeeInitial)
               .WithMany()
               .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<AssetComponentOp>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);


            //EmailManager
            builder.Entity<EmailManager>()
                .ToTable("EmailManager")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<EmailManager>()
               .HasOne(i => i.EmployeeInitial)
               .WithMany()
               .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<EmailManager>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);
            builder.Entity<EmailManager>()
              .HasOne(i => i.RoomInitial)
              .WithMany()
              .HasForeignKey(i => i.RoomIdInitial);
            builder.Entity<EmailManager>()
            .HasOne(i => i.RoomFinal)
            .WithMany()
            .HasForeignKey(i => i.RoomIdFinal);

            //EmailStatus
            builder.Entity<EmailStatus>()
                .ToTable("EmailStatus")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<EmailStatus>()
               .HasOne(i => i.EmployeeInitial)
               .WithMany()
               .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<EmailStatus>()
                    .HasOne(i => i.EmployeeFinal)
                    .WithMany()
                    .HasForeignKey(i => i.EmployeeIdFinal);
            builder.Entity<EmailStatus>()
                  .HasOne(i => i.CostCenterInitial)
                  .WithMany()
                  .HasForeignKey(i => i.CostCenterIdInitial);
            builder.Entity<EmailStatus>()
                .HasOne(i => i.CostCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdFinal);
            builder.Entity<EmailStatus>()
               .HasOne(i => i.SrcEmployeeValidateUser)
               .WithMany()
               .HasForeignKey(i => i.SrcEmployeeValidateBy);
            builder.Entity<EmailStatus>()
               .HasOne(i => i.SrcManagerValidateUser)
               .WithMany()
               .HasForeignKey(i => i.SrcManagerValidateBy);
            builder.Entity<EmailStatus>()
               .HasOne(i => i.DstEmployeeValidateUser)
               .WithMany()
               .HasForeignKey(i => i.DstEmployeeValidateBy);
            builder.Entity<EmailStatus>()
               .HasOne(i => i.DstManagerValidateUser)
               .WithMany()
               .HasForeignKey(i => i.DstManagerValidateBy);
            builder.Entity<EmailStatus>()
               .HasOne(i => i.FinalValidateUser)
               .WithMany()
               .HasForeignKey(i => i.FinalValidateBy);

            //EmailOrderStatus
            builder.Entity<EmailOrderStatus>()
                .ToTable("EmailOrderStatus")
                .Property(i => i.Id)
                .HasColumnName("Id");
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.Matrix)
               .WithMany()
               .HasForeignKey(i => i.MatrixId);
            builder.Entity<EmailOrderStatus>()
                    .HasOne(i => i.Order)
                    .WithMany()
                    .HasForeignKey(i => i.OrderId);
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.EmployeeL1ValidateUser)
               .WithMany()
               .HasForeignKey(i => i.EmployeeL1ValidateBy);
            builder.Entity<EmailOrderStatus>()
                 .HasOne(i => i.EmployeeL2ValidateUser)
                 .WithMany()
                 .HasForeignKey(i => i.EmployeeL2ValidateBy);
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.EmployeeL3ValidateUser)
               .WithMany()
               .HasForeignKey(i => i.EmployeeL3ValidateBy);
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.EmployeeL4ValidateUser)
               .WithMany()
               .HasForeignKey(i => i.EmployeeL4ValidateBy);
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.EmployeeS1ValidateUser)
               .WithMany()
               .HasForeignKey(i => i.EmployeeS1ValidateBy);
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.EmployeeS2ValidateUser)
               .WithMany()
               .HasForeignKey(i => i.EmployeeS2ValidateBy);
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.EmployeeS3ValidateUser)
               .WithMany()
               .HasForeignKey(i => i.EmployeeS3ValidateBy);
            builder.Entity<EmailOrderStatus>()
               .HasOne(i => i.FinalValidateUser)
               .WithMany()
               .HasForeignKey(i => i.FinalValidateBy);
			builder.Entity<EmailOrderStatus>()
			  .HasOne(i => i.EmployeeB1ValidateUser)
			  .WithMany()
			  .HasForeignKey(i => i.EmployeeB1ValidateBy);


			//EmailRequestStatus
			builder.Entity<EmailRequestStatus>()
				.ToTable("EmailRequestStatus")
				.Property(i => i.Id)
				.HasColumnName("Id");
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.RequestBudgetForecast)
			   .WithMany()
			   .HasForeignKey(i => i.RequestBudgetForecastId);
			builder.Entity<EmailRequestStatus>()
					.HasOne(i => i.Request)
					.WithMany()
					.HasForeignKey(i => i.RequestId);
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.EmployeeL1ValidateUser)
			   .WithMany()
			   .HasForeignKey(i => i.EmployeeL1ValidateBy);
			builder.Entity<EmailRequestStatus>()
				 .HasOne(i => i.EmployeeL2ValidateUser)
				 .WithMany()
				 .HasForeignKey(i => i.EmployeeL2ValidateBy);
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.EmployeeL3ValidateUser)
			   .WithMany()
			   .HasForeignKey(i => i.EmployeeL3ValidateBy);
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.EmployeeL4ValidateUser)
			   .WithMany()
			   .HasForeignKey(i => i.EmployeeL4ValidateBy);
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.EmployeeS1ValidateUser)
			   .WithMany()
			   .HasForeignKey(i => i.EmployeeS1ValidateBy);
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.EmployeeS2ValidateUser)
			   .WithMany()
			   .HasForeignKey(i => i.EmployeeS2ValidateBy);
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.EmployeeS3ValidateUser)
			   .WithMany()
			   .HasForeignKey(i => i.EmployeeS3ValidateBy);
			builder.Entity<EmailRequestStatus>()
			   .HasOne(i => i.FinalValidateUser)
			   .WithMany()
			   .HasForeignKey(i => i.FinalValidateBy);

			//EmailOfferStatus
			builder.Entity<EmailOfferStatus>()
				.ToTable("EmailOfferStatus")
				.Property(i => i.Id)
				.HasColumnName("Id");
			builder.Entity<EmailOfferStatus>()
			   .HasOne(i => i.Partner)
			   .WithMany()
			   .HasForeignKey(i => i.PartnerId);
			builder.Entity<EmailOfferStatus>()
					.HasOne(i => i.Offer)
					.WithMany()
					.HasForeignKey(i => i.OfferId);
			builder.Entity<EmailOfferStatus>()
			   .HasOne(i => i.Request)
			   .WithMany()
			   .HasForeignKey(i => i.RequestId);
			builder.Entity<EmailOfferStatus>()
				 .HasOne(i => i.Employee)
				 .WithMany()
				 .HasForeignKey(i => i.EmployeeId);
			builder.Entity<EmailOfferStatus>()
				 .HasOne(i => i.Owner)
				 .WithMany()
				 .HasForeignKey(i => i.OwnerId);
			builder.Entity<EmailOfferStatus>()
				 .HasOne(i => i.Company)
				 .WithMany()
				 .HasForeignKey(i => i.CompanyId);


			//EmailType
			builder.Entity<EmailType>()
                .ToTable("EmailType")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<EmailType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<EmailType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<EmailType>()
                .Property(p => p.UploadFolder)
                .HasMaxLength(200);


            //AssetInv
            builder.Entity<AssetInv>()
                .ToTable("AssetInv")
                .HasKey(a => a.AssetId);
            //builder.Entity<AssetInv>()
            //    .HasOne(ai => ai.Asset)
            //    .WithOne(a => a.AssetInv)
            //    .HasForeignKey<Asset>(a => a.AssetInvId);
            //builder.Entity<AssetInv>()
            //    .ToTable("AssetInv")
            //    .HasKey(a => a.Id);
            //builder.Entity<AssetInv>()
            //    .Property(p => p.Id)
            //    .HasColumnName("AssetId");
            builder.Entity<AssetInv>()
                .Property(p => p.InvNoOld)
                .HasMaxLength(30);
            builder.Entity<AssetInv>()
                .Property(p => p.InvName)
                .HasMaxLength(120);
            builder.Entity<AssetInv>()
                .Property(p => p.Barcode)
                .HasMaxLength(30);
            builder.Entity<AssetInv>()
                .Property(p => p.Producer)
                .HasMaxLength(30);
            builder.Entity<AssetInv>()
                .Property(p => p.Model)
                .HasMaxLength(30);
            builder.Entity<AssetInv>()
                .Property(p => p.Info)
                .HasMaxLength(200);

            //AssetAdmIn
            builder.Entity<AssetAdmIn>()
                .ToTable("AssetAdmIn")
                .HasKey(a => a.AssetId);
            builder.Entity<AssetAdmIn>()
                .Property(a => a.AssetId)
                .HasColumnName("AssetId");
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.Asset)
                .WithMany()
                .HasForeignKey(a => a.AssetId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.Administration)
                .WithMany()
                .HasForeignKey(a => a.AdministrationId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.AssetCategory)
                .WithMany()
                .HasForeignKey(a => a.AssetCategoryId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.Department)
                .WithMany()
                .HasForeignKey(a => a.DepartmentId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.Room)
                .WithMany()
                .HasForeignKey(a => a.RoomId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.AssetType)
                .WithMany()
                .HasForeignKey(a => a.AssetTypeId);
            builder.Entity<AssetAdmIn>()
                .HasOne(a => a.AssetState)
                .WithMany()
                .HasForeignKey(a => a.AssetStateId);

            //AssetAdmMD
            builder.Entity<AssetAdmMD>()
                .ToTable("AssetAdmMD")
                .HasKey(a => new { a.AccMonthId, a.AssetId });
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.AccMonth)
                .WithMany()
                .HasForeignKey(a => a.AccMonthId);
            //builder.Entity<AssetAdmMD>()
            //    .HasOne(a => a.Asset)
            //    .WithMany()
            //    .HasForeignKey(a => a.AssetId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.Administration)
                .WithMany()
                .HasForeignKey(a => a.AdministrationId);
            //builder.Entity<AssetAdmMD>()
            //  .HasOne(a => a.Company)
            //  .WithMany()
            //  .HasForeignKey(a => a.CompanyId);
            builder.Entity<AssetAdmMD>()
               .HasOne(a => a.SubType)
               .WithMany()
               .HasForeignKey(a => a.SubTypeId);
            builder.Entity<AssetAdmMD>()
               .HasOne(a => a.InsuranceCategory)
               .WithMany()
               .HasForeignKey(a => a.InsuranceCategoryId);
            builder.Entity<AssetAdmMD>()
               .HasOne(a => a.Model)
               .WithMany()
               .HasForeignKey(a => a.ModelId);
            builder.Entity<AssetAdmMD>()
               .HasOne(a => a.Brand)
               .WithMany()
               .HasForeignKey(a => a.BrandId);
            builder.Entity<AssetAdmMD>()
               .HasOne(a => a.Project)
               .WithMany()
               .HasForeignKey(a => a.ProjectId);
            builder.Entity<AssetAdmMD>()
               .HasOne(a => a.InterCompany)
               .WithMany()
               .HasForeignKey(a => a.InterCompanyId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.AssetCategory)
                .WithMany()
                .HasForeignKey(a => a.AssetCategoryId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.CostCenter)
                .WithMany()
                .HasForeignKey(a => a.CostCenterId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.Department)
                .WithMany()
                .HasForeignKey(a => a.DepartmentId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.Room)
                .WithMany()
                .HasForeignKey(a => a.RoomId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.AssetType)
                .WithMany()
                .HasForeignKey(a => a.AssetTypeId);
            builder.Entity<AssetAdmMD>()
                .HasOne(a => a.AssetState)
                .WithMany()
                .HasForeignKey(a => a.AssetStateId);

            //BudgetMonth
            builder.Entity<BudgetMonth>()
                .ToTable("BudgetMonth")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetMonth>()
                       .HasOne(a => a.AccMonth)
                       .WithMany()
                       .HasForeignKey(a => a.AccMonthId);
            builder.Entity<BudgetMonth>()
                       .HasOne(a => a.Budget)
                       .WithMany(a => a.BudgetMonths)
            // builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
            .HasForeignKey(a => a.BudgetId);

            //OfferMaterial
            builder.Entity<OfferMaterial>()
                .ToTable("OfferMaterial")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<OfferMaterial>()
                       .HasOne(a => a.Material)
                       .WithMany()
                       .HasForeignKey(a => a.MaterialId);
            builder.Entity<OfferMaterial>()
                          .HasOne(a => a.EmailManager)
                          .WithMany()
                          .HasForeignKey(a => a.EmailManagerId);
            builder.Entity<OfferMaterial>()
                             .HasOne(a => a.AppState)
                             .WithMany()
                             .HasForeignKey(a => a.AppStateId);
            builder.Entity<OfferMaterial>()
                       .HasOne(a => a.Offer)
                       .WithMany(a => a.OfferMaterials)
            // builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
            .HasForeignKey(a => a.OfferId);


            //Accountancy
            builder.Entity<Accountancy>()
                .ToTable("Accountancy")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Accountancy>()
                       .HasOne(a => a.Account)
                       .WithMany()
                       .HasForeignKey(a => a.AccountId);
            builder.Entity<Accountancy>()
                          .HasOne(a => a.ExpAccount)
                          .WithMany()
                          .HasForeignKey(a => a.ExpAccountId);
            builder.Entity<Accountancy>()
                             .HasOne(a => a.AssetType)
                             .WithMany()
                             .HasForeignKey(a => a.AssetTypeId);
            builder.Entity<Accountancy>()
                                .HasOne(a => a.AssetCategory)
                                .WithMany()
                                .HasForeignKey(a => a.AssetCategoryId);
            //builder.Entity<Accountancy>()
            //           .HasOne(a => a.InterCompany)
            //           .WithMany(a => a.Accountancies)
            //// builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
            //.HasForeignKey(a => a.InterCompanyId);



            //OrderMatwrial
            builder.Entity<OrderMaterial>()
                .ToTable("OrderMaterial")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<OrderMaterial>()
                       .HasOne(a => a.Material)
                       .WithMany()
                       .HasForeignKey(a => a.MaterialId);
            builder.Entity<OrderMaterial>()
                             .HasOne(a => a.AppState)
                             .WithMany()
                             .HasForeignKey(a => a.AppStateId);
            builder.Entity<OrderMaterial>()
                                .HasOne(a => a.Request)
                                .WithMany()
                                .HasForeignKey(a => a.RequestId);
            builder.Entity<OrderMaterial>()
                                .HasOne(a => a.OfferMaterial)
                                .WithMany()
                                .HasForeignKey(a => a.OfferMaterialId);
            builder.Entity<OrderMaterial>()
                       .HasOne(a => a.Order)
                       .WithMany(a => a.OrderMaterials)
            // builder.Entity<BudgetMonth>().Ignore(a => a.Budget);
            .HasForeignKey(a => a.OrderId);

            ////BudgetMonth
            //builder.Entity<BudgetMonth>()
            //    .ToTable("BudgetMonth")
            //    .HasKey(a => new { a.AccMonthId, a.BudgetId });
            //builder.Entity<BudgetMonth>()
            //    .HasOne(a => a.AccMonth)
            //    .WithMany()
            //    .HasForeignKey(a => a.AccMonthId);
            //builder.Entity<BudgetMonth>()
            //  .HasOne(a => a.Budget)
            //  .WithMany()
            //  .HasForeignKey(a => a.BudgetId);

            //AssetAC
            builder.Entity<AssetAC>()
                .ToTable("AssetAC")
                .HasKey(v => new { v.AssetClassTypeId, v.AssetId });
            builder.Entity<AssetAC>()
                .HasOne(v => v.AssetClassIn)
                .WithMany()
                .HasForeignKey(v => v.AssetClassIdIn)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<AssetAC>()
                .HasOne(v => v.AssetClass)
                .WithMany()
                .HasForeignKey(v => v.AssetClassId)
                .OnDelete(DeleteBehavior.Restrict);

            //AssetDep
            builder.Entity<AssetDep>()
                .ToTable("AssetDep")
                .HasKey(v => new { v.AccSystemId, v.AssetId });
            builder.Entity<AssetDep>()
                .Property(p => p.UsageStartDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<AssetDep>()
                .Property(p => p.UsageEndDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueInvIn)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueRemIn)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueDepPUIn)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueDepIn)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueDepYTDIn)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueInv)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueRem)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueDepPU)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueDep)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDep>()
                .Property(p => p.ValueDepYTD)
                .HasColumnType("decimal(18, 4)");

            //AssetDepMD
            builder.Entity<AssetDepMD>()
                .ToTable("AssetDepMD")
                .HasKey(a => new { a.AccMonthId, a.AccSystemId, a.AssetId });
            builder.Entity<AssetDepMD>()
                .Property(p => p.CurrentAPC)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDepMD>()
                .Property(p => p.PosCap)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDepMD>()
                .Property(p => p.BkValFYStart)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDepMD>()
                .Property(p => p.AccumulDep)
                .HasColumnType("decimal(18, 4)");
            builder.Entity<AssetDepMD>()
                .Property(p => p.DepForYear)
                .HasColumnType("decimal(18, 4)");

            ////AssetInv
            //builder.Entity<AssetInv>()
            //    .ToTable("AssetInv")
            //    .HasKey(v => v.Id);

            //builder.Entity<AssetInv>()
            //    .Property(v => v.Id)
            //    .HasColumnName("AssetId");

            //builder.Entity<AssetInv>().Ignore(a => a.Id);

            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.Asset)
            //    .WithOne(i => i.).HasForeignKey("Asset", "AssetId");

            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.AdministrationIn)
            //    .WithMany()
            //    .HasForeignKey(v => v.AdministrationIdIn);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.Administration)
            //    .WithMany()
            //    .HasForeignKey(v => v.AdministrationId);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.AssetCategoryIn)
            //    .WithMany()
            //    .HasForeignKey(v => v.AssetCategoryIdIn);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.AssetCategory)
            //    .WithMany()
            //    .HasForeignKey(v => v.AssetCategoryId);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.CostCenterIn)
            //    .WithMany()
            //    .HasForeignKey(v => v.CostCenterIdIn);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.CostCenter)
            //    .WithMany()
            //    .HasForeignKey(v => v.CostCenterId);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.DepartmentIn)
            //    .WithMany()
            //    .HasForeignKey(v => v.DepartmentIdIn);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.Department)
            //    .WithMany()
            //    .HasForeignKey(v => v.DepartmentId);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.EmployeeIn)
            //    .WithMany()
            //    .HasForeignKey(v => v.EmployeeIdIn);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.Employee)
            //    .WithMany()
            //    .HasForeignKey(v => v.EmployeeId);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.RoomIn)
            //    .WithMany()
            //    .HasForeignKey(v => v.RoomIdIn);
            //builder.Entity<AssetInv>()
            //    .HasOne(v => v.Room)
            //    .WithMany()
            //    .HasForeignKey(v => v.RoomId);

            //AssetState
            builder.Entity<AssetState>()
                .ToTable("AssetState")
                .Property(p => p.Id)
                .HasColumnName("AssetStateId");
            builder.Entity<AssetState>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AssetState>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //AssetCategory
            builder.Entity<AssetCategory>()
                .ToTable("AssetCategory")
                .Property(p => p.Id)
                .HasColumnName("AssetCategoryId");
            builder.Entity<AssetCategory>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AssetCategory>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<AssetCategory>()
                .Property(p => p.Prefix)
                .HasMaxLength(30);

            ////AssetCategoryAC
            //builder.Entity<AssetCategoryAC>()
            //    .ToTable("AssetCategoryAC")
            //    .HasKey(a => new { a.AssetClassTypeId, a.AssetCategoryId });
            //builder.Entity<AssetCategoryAC>()
            //    .HasOne(a => a.AssetClassType)
            //    .WithMany()
            //    .HasForeignKey(a => a.AssetClassTypeId)
            //    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //builder.Entity<AssetCategoryAC>()
            //    .HasOne(a => a.AssetCategory)
            //    .WithMany()
            //    .HasForeignKey(a => a.AssetCategoryId)
            //    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //builder.Entity<AssetCategoryAC>()
            //    .HasOne(a => a.AssetClass)
            //    .WithMany()
            //    .HasForeignKey(a => a.AssetClassId)
            //    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            ////AssetCategoryDep
            //builder.Entity<AssetCategoryDep>()
            //    .ToTable("AssetCategoryDep")
            //    .HasKey(a => new { a.AccSystemId, a.AssetCategoryId });
            //builder.Entity<AssetCategoryDep>()
            //    .HasOne(a => a.AccSystem)
            //    .WithMany()
            //    .HasForeignKey(a => a.AccSystemId)
            //    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //builder.Entity<AssetCategoryDep>()
            //    .HasOne(a => a.AssetCategory)
            //    .WithMany()
            //    .HasForeignKey(a => a.AssetCategoryId)
            //    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //builder.Entity<AssetCategoryDep>()
            //    .HasOne(a => a.AssetAccState)
            //    .WithMany()
            //    .HasForeignKey(a => a.AssetAccStateId)
            //    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            //AssetClass
            builder.Entity<AssetClass>()
                .ToTable("AssetClass")
                .Property(p => p.Id)
                .HasColumnName("AssetClassId");
            builder.Entity<AssetClass>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AssetClass>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255);

            //AssetClassType
            builder.Entity<AssetClassType>()
                .ToTable("AssetClassType")
                .Property(p => p.Id)
                .HasColumnName("AssetClassTypeId");
            builder.Entity<AssetClassType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AssetClassType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //AssetType
            builder.Entity<AssetType>()
                .ToTable("AssetType")
                .Property(p => p.Id)
                .HasColumnName("AssetTypeId");
            builder.Entity<AssetType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AssetType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //ColumnDefinition
            builder.Entity<ColumnDefinition>()
                .ToTable("ColumnDefinition")
                .Property(p => p.Id)
                .HasColumnName("ColumnDefinitionId");
            builder.Entity<ColumnDefinition>()
                .Property(p => p.HeaderCode)
                .IsRequired()
                .HasMaxLength(50);
            builder.Entity<ColumnDefinition>()
                .Property(p => p.Property)
                .IsRequired()
                .HasMaxLength(50);
            builder.Entity<ColumnDefinition>()
                .Property(p => p.Include)
                .HasMaxLength(100);
            builder.Entity<ColumnDefinition>()
                .Property(p => p.SortBy)
                .HasMaxLength(50);
            builder.Entity<ColumnDefinition>()
                .Property(p => p.Pipe)
                .HasMaxLength(50);
            builder.Entity<ColumnDefinition>()
                .Property(p => p.Format)
                .HasMaxLength(50);
            builder.Entity<ColumnDefinition>()
                .Property(p => p.TextAlign)
                .HasMaxLength(10);
            builder.Entity<ColumnDefinition>()
                .Property(p => p.Active)
                .IsRequired();
            builder.Entity<ColumnDefinition>()
                .HasOne(a => a.AspNetRole)
                .WithMany()
                .HasForeignKey(a => a.RoleId);
			builder.Entity<ColumnDefinition>()
			 .HasOne(a => a.ColumnFilter)
			 .WithMany()
			 .HasForeignKey(a => a.ColumnFilterId);




			//Company
			builder.Entity<Company>()
                .ToTable("Company")
                .Property(p => p.Id)
                .HasColumnName("CompanyId");
            builder.Entity<Company>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Company>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //ConfigValue
            builder.Entity<ConfigValue>()
                .ToTable("ConfigValue")
                .Property(p => p.Id)
                .HasColumnName("ConfigValueId");
            builder.Entity<ConfigValue>()
                .Property(p => p.Group)
                .HasMaxLength(50);
            builder.Entity<ConfigValue>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);
            builder.Entity<ConfigValue>()
                .Property(p => p.Description)
                .HasMaxLength(200);
            builder.Entity<ConfigValue>()
                .Property(p => p.ValueType)
                .IsRequired()
                .HasMaxLength(20);
            builder.Entity<ConfigValue>()
                .Property(p => p.TextValue)
                .HasMaxLength(200);
            builder.Entity<ConfigValue>()
                .Property(p => p.NumericValue)
                .HasColumnType("decimal(18,4)");
            builder.Entity<ConfigValue>()
                .Property(p => p.DateValue)
                .HasColumnType("date");

            //CostCenter
            builder.Entity<CostCenter>()
                .ToTable("CostCenter")
                .Property(p => p.Id)
                .HasColumnName("CostCenterId");
            builder.Entity<CostCenter>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<CostCenter>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<CostCenter>()
                .Property(p => p.ERPCode)
                .HasMaxLength(50);
            builder.Entity<CostCenter>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId);
            //  builder.Entity<CostCenter>()
            //       .HasOne(a => a.Employee2)
            //       .WithMany()
            //       .HasForeignKey(a => a.EmployeeId2);
            // builder.Entity<CostCenter>()
            //       .HasOne(a => a.Employee3)
            //       .WithMany()
            //       .HasForeignKey(a => a.EmployeeId3);
            // builder.Entity<CostCenter>()
            //       .HasOne(a => a.Employee4)
            //       .WithMany()
            //       .HasForeignKey(a => a.EmployeeId4);
            // builder.Entity<CostCenter>()
            //       .HasOne(a => a.Employee5)
            //       .WithMany()
            //       .HasForeignKey(a => a.EmployeeId5);
            //builder.Entity<CostCenter>()
            //       .HasOne(a => a.Employee6)
            //       .WithMany()
            //       .HasForeignKey(a => a.EmployeeId6);
            // builder.Entity<CostCenter>()
            //       .HasOne(a => a.Employee7)
            //       .WithMany()
            //      .HasForeignKey(a => a.EmployeeId7);

            // EntityFilePartner
            builder.Entity<EntityFilePartner>()
                .ToTable("EntityFilePartner")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<EntityFilePartner>()
               .HasOne(a => a.EntityFile)
               .WithMany()
               .HasForeignKey(a => a.EntityFileId);
            builder.Entity<EntityFilePartner>()
               .HasOne(a => a.Partner)
               .WithMany()
               .HasForeignKey(a => a.PartnerId);


            // EmployeeCostCenter
            builder.Entity<EmployeeCostCenter>()
                .ToTable("EmployeeCostCenter")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<EmployeeCostCenter>()
               .HasOne(a => a.Employee)
               .WithMany()
               .HasForeignKey(a => a.EmployeeId);
            builder.Entity<EmployeeCostCenter>()
               .HasOne(a => a.CostCenter)
               .WithMany()
               .HasForeignKey(a => a.CostCenterId);


			// EmployeeCompany
			builder.Entity<EmployeeCompany>()
				.ToTable("EmployeeCompany")
				.Property(p => p.Id)
				.HasColumnName("Id");
			builder.Entity<EmployeeCompany>()
			   .HasOne(a => a.Employee)
			   .WithMany()
			   .HasForeignKey(a => a.EmployeeId);
			builder.Entity<EmployeeCompany>()
			   .HasOne(a => a.Company)
			   .WithMany()
			   .HasForeignKey(a => a.CompanyId);

			// EmployeeDivision
			builder.Entity<EmployeeDivision>()
                .ToTable("EmployeeDivision")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<EmployeeDivision>()
               .HasOne(a => a.Employee)
               .WithMany()
               .HasForeignKey(a => a.EmployeeId);
            builder.Entity<EmployeeDivision>()
               .HasOne(a => a.Division)
               .WithMany()
               .HasForeignKey(a => a.DivisionId);

            // EmployeeStorage
            builder.Entity<EmployeeStorage>()
                .ToTable("EmployeeStorage")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<EmployeeStorage>()
               .HasOne(a => a.Employee)
               .WithMany()
               .HasForeignKey(a => a.EmployeeId);
            builder.Entity<EmployeeStorage>()
               .HasOne(a => a.Storage)
               .WithMany()
               .HasForeignKey(a => a.StorageId);

            // RequestBudgetForecast
            builder.Entity<RequestBudgetForecast>()
                .ToTable("RequestBudgetForecast")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<RequestBudgetForecast>()
               .HasOne(a => a.Request)
               .WithMany()
               .HasForeignKey(a => a.RequestId);
            builder.Entity<RequestBudgetForecast>()
               .HasOne(a => a.BudgetForecast)
               .WithMany()
               .HasForeignKey(a => a.BudgetForecastId);
            builder.Entity<RequestBudgetForecast>()
              .HasOne(a => a.AccMonth)
              .WithMany()
              .HasForeignKey(a => a.AccMonthId);
            builder.Entity<RequestBudgetForecast>()
              .HasOne(a => a.BudgetManager)
              .WithMany()
              .HasForeignKey(a => a.BudgetManagerId);

			// RequestBudgetForecastMaterial
			builder.Entity<RequestBudgetForecastMaterial>()
				.ToTable("RequestBudgetForecastMaterial")
				.Property(p => p.Id)
				.HasColumnName("Id");
            //builder.Entity<RequestBudgetForecastMaterial>()
            //   .HasOne(a => a.RequestBudgetForecast)
            //   .WithMany()
            //   .HasForeignKey(a => a.RequestBudgetForecastId);
            //builder.Entity<RequestBudgetForecastMaterial>()
            //   .HasOne(a => a.Material)
            //   .WithMany()
            //   .HasForeignKey(a => a.MaterialId);

            // RequestBFMaterialCostCenter
            builder.Entity<RequestBFMaterialCostCenter>()
                .ToTable("RequestBFMaterialCostCenter")
                .Property(p => p.Id)
                .HasColumnName("Id");
            //builder.Entity<RequestBudgetForecastMaterial>()
            //   .HasOne(a => a.RequestBudgetForecast)
            //   .WithMany()
            //   .HasForeignKey(a => a.RequestBudgetForecastId);
            //builder.Entity<RequestBudgetForecastMaterial>()
            //   .HasOne(a => a.Material)
            //   .WithMany()
            //   .HasForeignKey(a => a.MaterialId);

            // ProjectTypeDivision
            builder.Entity<ProjectTypeDivision>()
                .ToTable("ProjectTypeDivision")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<ProjectTypeDivision>()
               .HasOne(a => a.ProjectType)
               .WithMany()
               .HasForeignKey(a => a.ProjectTypeId);
            builder.Entity<ProjectTypeDivision>()
               .HasOne(a => a.Division)
               .WithMany()
               .HasForeignKey(a => a.DivisionId);

            ////Currency
            //builder.Entity<Currency>()
            //    .ToTable("Currency")
            //    .Property(p => p.Id)
            //    .HasColumnName("CurrencyId");
            //builder.Entity<Currency>()
            //    .Property(p => p.Code)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<Currency>()
            //    .Property(p => p.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);

            //Department
            builder.Entity<Department>()
                .ToTable("Department")
                .Property(d => d.Id)
                .HasColumnName("DepartmentId");
            builder.Entity<Department>()
                .Property(d => d.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Department>()
                .Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Department>()
                .Property(d => d.ERPCode)
                .HasMaxLength(50);
            builder.Entity<Department>()
                .HasOne(d => d.TeamLeader)
                .WithMany()
                .HasForeignKey(d => d.TeamLeaderId);

            //Division
            builder.Entity<Division>()
                .ToTable("Division")
                .Property(p => p.Id)
                .HasColumnName("DivisionId");
            builder.Entity<Division>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Division>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //DictionaryType
            builder.Entity<DictionaryType>()
                .ToTable("DictionaryType")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<DictionaryType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<DictionaryType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //BudgetType
            builder.Entity<BudgetType>()
                .ToTable("BudgetType")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<BudgetType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Document
            builder.Entity<Document>()
                .ToTable("Document")
                .Property(p => p.Id)
                .HasColumnName("DocumentId");
            builder.Entity<Document>()
                .Property(p => p.DocNo1)
                .IsRequired()
                .HasMaxLength(50);
            builder.Entity<Document>()
                .Property(p => p.DocNo2)
                .IsRequired()
                .HasMaxLength(50);
            builder.Entity<Document>()
                .Property(p => p.CreationDate)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Document>()
                .Property(p => p.DocumentDate)
                .IsRequired(true)
                .HasColumnType("date");
            builder.Entity<Document>()
                .Property(p => p.RegisterDate)
                .IsRequired(true)
                .HasColumnType("date");
            builder.Entity<Document>()
                .Property(p => p.Details)
                .HasMaxLength(255);

            //DocumentType
            builder.Entity<DocumentType>()
                .ToTable("DocumentType")
                .Property(p => p.Id)
                .HasColumnName("DocumentTypeId");
            builder.Entity<DocumentType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<DocumentType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<DocumentType>()
                .Property(p => p.ParentCode)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<DocumentType>()
                .Property(p => p.Mask)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<DocumentType>()
                .Property(p => p.Prefix)
                .HasMaxLength(10);
            builder.Entity<DocumentType>()
                .Property(p => p.Suffix)
                .HasMaxLength(10);

            //Employee
            builder.Entity<Employee>()
                .ToTable("Employee")
                .Property(p => p.Id)
                .HasColumnName("EmployeeId");
            builder.Entity<Employee>()
                .Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Employee>()
                .Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Employee>()
                .Property(p => p.InternalCode)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Employee>()
                .Property(p => p.ERPCode)
                .HasMaxLength(50);
            builder.Entity<Employee>()
                .Property(p => p.Email)
                .HasMaxLength(100);

            //Manager
            builder.Entity<Manager>()
                .ToTable("Manager")
                .Property(p => p.Id)
                .HasColumnName("ManagerId");
            builder.Entity<Manager>()
                .Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Manager>()
                .Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Manager>()
                .Property(p => p.InternalCode)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Manager>()
                .Property(p => p.Email)
                .HasMaxLength(100);


            //Owner
            builder.Entity<Owner>()
                .ToTable("Owner")
                .Property(p => p.Id)
                .HasColumnName("OwnerId");
            builder.Entity<Owner>()
                .Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Owner>()
                .Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Owner>()
                .Property(p => p.UniqueName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Owner>()
                .Property(p => p.Email)
                .HasMaxLength(100);


            //Address
            builder.Entity<Address>()
                .ToTable("Address")
                .Property(p => p.Id)
                .HasColumnName("AddressId");
            builder.Entity<Address>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Address>()
                .Property(p => p.Phone)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Address>()
                .Property(p => p.UniqueName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Address>()
                .Property(p => p.Fax)
                .HasMaxLength(100);

            //EntityType
            builder.Entity<EntityType>()
                .ToTable("EntityType")
                .Property(p => p.Id)
                .HasColumnName("EntityTypeId");
            builder.Entity<EntityType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<EntityType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<EntityType>()
                .Property(p => p.UploadFolder)
                .HasMaxLength(100);

            //EntityFile
            builder.Entity<EntityFile>()
                .ToTable("EntityFile")
                .Property(p => p.Id)
                .HasColumnName("EntityFileId");
            //builder.Entity<EntityFile>()
            //    .Property(p => p.EntityId)
            //    .IsRequired();
            builder.Entity<EntityFile>()
                .Property(p => p.EntityTypeId)
                .IsRequired();
            builder.Entity<EntityFile>()
                .Property(p => p.FileType)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<EntityFile>()
               .Property(p => p.StoredAs)
               .IsRequired()
               .HasMaxLength(50);
            builder.Entity<EntityFile>()
               .Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(100);
            builder.Entity<EntityFile>()
               .Property(p => p.Size)
               .IsRequired();
            builder.Entity<EntityFile>()
                .Property(p => p.Info)
                .HasMaxLength(100);

            ////EntityNumber
            //builder.Entity<EntityNumber>()
            //    .ToTable("EntityNumber")
            //    .Property(e => e.Id)
            //    .HasColumnName("EntityNumberId");
            //builder.Entity<EntityNumber>()
            //    .Property(e => e.Entity)
            //    .IsRequired()
            //    .HasMaxLength(100);
            //builder.Entity<EntityNumber>()
            //    .Property(e => e.Prefix)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<EntityNumber>()
            //    .Property(e => e.Suffix)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<EntityNumber>()
            //    .Property(e => e.FormatChar)
            //    .HasMaxLength(1);

            //Inventory
            builder.Entity<Inventory>()
                .ToTable("Inventory")
                .Property(p => p.Id)
                .HasColumnName("InventoryId");
            builder.Entity<Inventory>()
                .Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Inventory>()
                .Property(p => p.Start)
                .IsRequired(false)
                .HasColumnType("date");
            builder.Entity<Inventory>()
                .Property(p => p.End)
                .IsRequired(false)
                .HasColumnType("date");

			//InventoryAsset
			    //builder.Entity<InventoryAsset>()
			    //   .ToTable("InventoryAsset")
			    //   .HasKey(p => p.Id);
			    //builder.Entity<InventoryAsset>()
			    //.Property(b => b.UpdatedAt)
			    //.HasDefaultValueSql("getdate()");
			    //builder.Entity<InventoryAsset>()
			    //.Property(b => b.InInventory)
			    //.HasDefaultValueSql("1");
			builder.Entity<InventoryAsset>()
                .ToTable("InventoryAsset")
                .HasKey(i => new { i.InventoryId, i.AssetId });
            builder.Entity<InventoryAsset>()
                .Property(i => i.SerialNumber)
                .HasMaxLength(50);
            builder.Entity<InventoryAsset>()
                .Property(i => i.Producer)
                .HasMaxLength(100);
            builder.Entity<InventoryAsset>()
                .Property(i => i.Model)
                .HasMaxLength(100);
            builder.Entity<InventoryAsset>()
               .HasOne(i => i.AdministrationInitial)
               .WithMany()
               .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.EmployeeInitial)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdInitial);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.EmployeeFinal)
                .WithMany()
                .HasForeignKey(i => i.EmployeeIdFinal);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.RoomInitial)
                .WithMany()
                .HasForeignKey(i => i.RoomIdInitial);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.RoomFinal)
                .WithMany()
                .HasForeignKey(i => i.RoomIdFinal);

            builder.Entity<InventoryAsset>()
                .HasOne(i => i.CostCenterInitial)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdInitial);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.CostCenterFinal)
                .WithMany()
                .HasForeignKey(i => i.CostCenterIdFinal);
            builder.Entity<InventoryAsset>()
              .HasOne(i => i.AdministrationInitial)
              .WithMany()
              .HasForeignKey(i => i.AdministrationIdInitial);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.AdministrationFinal)
                .WithMany()
                .HasForeignKey(i => i.AdministrationIdFinal);

            builder.Entity<InventoryAsset>()
                .HasOne(i => i.StateInitial)
                .WithMany()
                .HasForeignKey(i => i.StateIdInitial);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.StateFinal)
                .WithMany()
                .HasForeignKey(i => i.StateIdFinal);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.DetailState)
                .WithMany()
                .HasForeignKey(i => i.DetailStateId);
            builder.Entity<InventoryAsset>()
                .HasOne(i => i.ModifiedByUser)
                .WithMany()
                .HasForeignKey(i => i.ModifiedBy);
            builder.Entity<InventoryAsset>()
             .HasOne(i => i.UomFinal)
             .WithMany()
             .HasForeignKey(i => i.UomIdFinal);
            builder.Entity<InventoryAsset>()
              .HasOne(i => i.DimensionFinal)
              .WithMany()
              .HasForeignKey(i => i.DimensionIdFinal);
            builder.Entity<InventoryAsset>()
               .HasOne(i => i.TempUser)
               .WithMany()
               .HasForeignKey(i => i.TempUserId);
			builder.Entity<InventoryAsset>()
			   .HasOne(i => i.InventoryTeamManager)
			   .WithMany()
			   .HasForeignKey(i => i.InventoryTeamManagerId);
			builder.Entity<InventoryAsset>()
			   .HasOne(i => i.InventoryResponsable)
			   .WithMany()
			   .HasForeignKey(i => i.InventoryResponsableId);
			//builder.Entity<InventoryAsset>()
			//   .HasOne(i => i.ScanByUser)
			//   .WithMany()
			//   .HasForeignKey(i => i.ScanBy);

			//InvState
			builder.Entity<InvState>()
                .ToTable("InvState")
                .Property(p => p.Id)
                .HasColumnName("InvStateId");
            builder.Entity<InvState>()
                .Property(p => p.Code)
                .HasMaxLength(30);
            builder.Entity<InvState>()
                .Property(p => p.ParentCode)
                .HasMaxLength(30);
            builder.Entity<InvState>()
                .Property(p => p.Name)
                .HasMaxLength(100);
            builder.Entity<InvState>()
                .Property(p => p.Mask)
                .HasMaxLength(200);

            //Region
            builder.Entity<Region>()
                .ToTable("Region")
                .Property(p => p.Id)
                .HasColumnName("RegionId");
            builder.Entity<Region>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Region>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Location
            builder.Entity<Location>()
                .ToTable("Location")
                .Property(p => p.Id)
                .HasColumnName("LocationId");
            builder.Entity<Location>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Location>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Location>()
                .Property(p => p.Address)
                .HasMaxLength(200);
            builder.Entity<Location>()
                .Property(p => p.Prefix)
                .HasMaxLength(30);
            builder.Entity<Location>()
                .Property(p => p.ERPCode)
                .HasMaxLength(50);

            //LocationType
            builder.Entity<LocationType>()
                .ToTable("LocationType")
                .Property(p => p.Id)
                .HasColumnName("LocationTypeId");
            builder.Entity<LocationType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<LocationType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //AssetNature
            builder.Entity<AssetNature>()
                .ToTable("AssetNature")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<AssetNature>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<AssetNature>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //BudgetManager
            builder.Entity<BudgetManager>()
                .ToTable("BudgetManager")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<BudgetManager>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<BudgetManager>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //MaterialType
            builder.Entity<MaterialType>()
                .ToTable("MaterialType")
                .Property(p => p.Id)
                .HasColumnName("MaterialTypeId");
            builder.Entity<MaterialType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<MaterialType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Material
            builder.Entity<Material>()
                .ToTable("Material")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Material>()
                .Property(p => p.Code);
            builder.Entity<Material>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(400);
            builder.Entity<Material>()
               .HasOne(a => a.SubCategory)
               .WithMany()
               .HasForeignKey(a => a.SubCategoryId);
            builder.Entity<Material>()
                  .HasOne(a => a.SubCategoryEN)
                  .WithMany()
                  .HasForeignKey(a => a.SubCategoryENId);
            builder.Entity<Material>()
                 .HasOne(a => a.SubType)
                 .WithMany()
                 .HasForeignKey(a => a.SubTypeId);
            builder.Entity<Material>()
                 .HasOne(a => a.Account)
                 .WithMany()
                 .HasForeignKey(a => a.AccountId);
            builder.Entity<Material>()
                 .HasOne(a => a.ExpAccount)
                 .WithMany()
                 .HasForeignKey(a => a.ExpAccountId);
            builder.Entity<Material>()
                .HasOne(a => a.AssetCategory)
                .WithMany()
                .HasForeignKey(a => a.AssetCategoryId);
            builder.Entity<Material>()
                  .HasOne(a => a.MaterialType)
                  .WithMany()
                  .HasForeignKey(a => a.MaterialTypeId);


            //Stock
            builder.Entity<Stock>()
                .ToTable("Stock")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Stock>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(200);
            builder.Entity<Stock>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(400);
            builder.Entity<Stock>()
                   .Property(p => p.LongName)
                   .IsRequired()
                   .HasMaxLength(400);

            //Account
            builder.Entity<Account>()
                .ToTable("Account")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Account>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Account>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //ExpAccount
            builder.Entity<ExpAccount>()
                .ToTable("ExpAccount")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<ExpAccount>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<ExpAccount>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Articles
            builder.Entity<Article>()
                .ToTable("Article")
                .Property(p => p.Id)
                .HasColumnName("Id");
            builder.Entity<Article>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Article>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Partner
            builder.Entity<Partner>()
                .ToTable("Partner")
                .Property(p => p.Id)
                .HasColumnName("PartnerId");
            builder.Entity<Partner>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Partner>()
                .Property(p => p.FiscalCode)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Partner>()
                .Property(p => p.RegistryNumber)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Partner>()
                .Property(p => p.Address)
                .HasMaxLength(200);
            builder.Entity<Partner>()
                .Property(p => p.ContactInfo)
                .HasMaxLength(200);
            builder.Entity<Partner>()
                .Property(p => p.Bank)
                .HasMaxLength(100);
            builder.Entity<Partner>()
                .Property(p => p.BankAccount)
                .HasMaxLength(30);
            builder.Entity<Partner>()
                .Property(p => p.PayingAccount)
                .HasMaxLength(30);
            builder.Entity<Partner>()
                .Property(p => p.ErpCode)
                .HasMaxLength(30);

            //Partner
            builder.Entity<PartnerLocation>()
                .ToTable("PartnerLocation")
                .Property(p => p.Id)
                .HasColumnName("Id");
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.FiscalCode)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.RegistryNumber)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.Address)
            //    .HasMaxLength(200);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.ContactInfo)
            //    .HasMaxLength(200);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.Bank)
            //    .HasMaxLength(100);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.BankAccount)
            //    .HasMaxLength(30);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.PayingAccount)
            //    .HasMaxLength(30);
            //builder.Entity<PartnerLocation>()
            //    .Property(p => p.ErpCode)
            //    .HasMaxLength(30);

            ////Product
            //builder.Entity<Product>()
            //    .ToTable("Product")
            //    .Property(p => p.Id)
            //    .HasColumnName("ProductId");
            //builder.Entity<Product>()
            //    .Property(p => p.Code)
            //    .HasMaxLength(30);
            //builder.Entity<Product>()
            //    .Property(p => p.Name)
            //    .IsRequired()
            //    .HasMaxLength(200);

            ////Purchase
            //builder.Entity<Purchase>()
            //    .ToTable("Purchase")
            //    .HasKey(p => p.Id);
            //builder.Entity<Purchase>()
            //    .Property(p => p.Id)
            //    .HasColumnName("PurchaseId");
            //builder.Entity<Purchase>()
            //    .HasOne(p => p.Document)
            //    .WithMany()
            //    .HasForeignKey(p => p.Id);

            //builder.Entity<PurchaseEntry>()
            //    .ToTable("PurchaseEntry")
            //    .HasKey(p => p.Id);
            //builder.Entity<PurchaseEntry>()
            //    .Property(p => p.Id)
            //    .HasColumnName("PurchaseEntryId");
            //builder.Entity<PurchaseEntry>()
            //    .HasOne(p => p.Document)
            //    .WithMany()
            //    .HasForeignKey(p => p.Id);
            //builder.Entity<PurchaseEntry>()
            //    .HasOne(p => p.Purchase)
            //    .WithMany()
            //    .HasForeignKey(p => p.PurchaseId)
            //    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            ////PurchaseEntryType
            //builder.Entity<PurchaseEntryType>()
            //    .ToTable("PurchaseEntryType")
            //    .Property(p => p.Id)
            //    .HasColumnName("PurchaseEntryTypeId");
            //builder.Entity<PurchaseEntryType>()
            //    .Property(p => p.Code)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<PurchaseEntryType>()
            //    .Property(p => p.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);
            //builder.Entity<PurchaseEntryType>()
            //    .Property(p => p.ParentCode)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<PurchaseEntryType>()
            //    .Property(p => p.Mask)
            //    .HasMaxLength(200);

            //ProjectType
            builder.Entity<ProjectType>()
                .ToTable("ProjectType")
                .Property(p => p.Id)
                .HasColumnName("ProjectTypeId");
            builder.Entity<ProjectType>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<ProjectType>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            //Activity
            builder.Entity<Activity>()
                .ToTable("Activity")
                .Property(p => p.Id)
                .HasColumnName("ActivityId");
            builder.Entity<Activity>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Activity>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);


            //Room
            builder.Entity<Room>()
                .ToTable("Room")
                .Property(p => p.Id)
                .HasColumnName("RoomId");
            builder.Entity<Room>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Room>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<Room>()
                .Property(p => p.ERPCode)
                .HasMaxLength(50);

            //TableDefinition
            builder.Entity<TableDefinition>()
                .ToTable("TableDefinition")
                .Property(p => p.Id)
                .HasColumnName("TableDefinitionId");
            builder.Entity<TableDefinition>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<TableDefinition>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Entity<TableDefinition>()
                .Property(p => p.Description)
                .HasMaxLength(200);

            //Uom
            builder.Entity<Uom>()
                .ToTable("Uom")
                .Property(p => p.Id)
                .HasColumnName("UomId");
            builder.Entity<Uom>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(30);
            builder.Entity<Uom>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            ////Synthetic
            //builder.Entity<Synthetic>()
            //    .ToTable("Synthetic")
            //    .Property(p => p.Id)
            //    .HasColumnName("SyntheticId");
            //builder.Entity<Synthetic>()
            //    .Property(p => p.Code)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<Synthetic>()
            //    .Property(p => p.Name)
            //    .IsRequired()
            //    .HasMaxLength(100);

            ////Vat
            //builder.Entity<Vat>()
            //    .ToTable("Vat")
            //    .Property(p => p.Id)
            //    .HasColumnName("VatId");
            //builder.Entity<Vat>()
            //    .Property(p => p.Code)
            //    .IsRequired()
            //    .HasMaxLength(30);
            //builder.Entity<Vat>()
            //    .Property(p => p.Name)
            //    .HasMaxLength(100);
        }
    }
}
