using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Optima.Fais.Api.Controllers.Identity;
using Optima.Fais.Api.HubConfig;
using Optima.Fais.Api.Identity;
using Optima.Fais.Api.Services;
using Optima.Fais.Api.Services.BNR;
using Optima.Fais.Api.Services.Flow;
using Optima.Fais.Api.Services.LDAP;
using Optima.Fais.Api.Services.SAPAriba;
using Optima.Fais.Data;
using Optima.Fais.EfRepository;
using Optima.Fais.EfRepository.Inventory;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using Serilog;

namespace Optima.Fais.Api
{
	public class Startup
	{
		public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			 services.AddCors();

			//services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
			//	builder
			//	.WithOrigins("http://localhost:8101", "http://localhost:4200", "http://localhost", "https://localhost", "https://webservicesp.anaf.ro/AsynchWebService/api/v6/ws/tva", "http://woptima01.emag.local", "https://woptima01.emag.local")
			//	.AllowAnyMethod()
			//	.AllowAnyHeader()
			//	.AllowCredentials();
			//}));

			services.AddSignalR();
			services.AddControllers()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.IncludeFields = true;
				});

			// BLAZOR //

			//services.AddRazorPages();
			//services.AddServerSideBlazor();

			// BLAZOR //

			services
				.AddDbContext<ApplicationDbContext>(options =>
				options
				.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.UseRowNumberForPaging()));
				

			services.AddIdentity<ApplicationUser, ApplicationRole>()
							.AddEntityFrameworkStores<ApplicationDbContext>()
							.AddDefaultTokenProviders();

			var emailConfig = Configuration
				 .GetSection("EmailConfiguration")
				 .Get<EmailConfiguration>();

			//services.AddMvc().AddJsonOptions(options =>
			//{
			//	//Set date configurations
			//	//options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
			//	options.SerializerSettings.DateTimeZoneHandling = "yyyy-MM-dd HH:mm:ss"; // month must be capital. otherwise it gives minutes.
			//});

			// Identity options.
			services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = false;
			});

			services.AddAutoMapper(typeof(Startup));

			// services.AddSignalR();

			services.AddMvcCore(
				//    options => {
				//    var policy = new AuthorizationPolicyBuilder()
				//    .RequireAuthenticatedUser()
				//    .Build();
				//}
				)
				.AddAuthorization();
			//.AddJsonFormatters();

			//// IdentityServer4.AccessTokenValidation: authentication middleware for the API.
			//app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
			//{
			//    Authority = Configuration.GetSection("Authority").GetValue<string>("Url"),
			//    //Authority = "http://assets.europe.odcorp.net/api/",
			//    AllowedScopes = { "WebAPI" },
			//    RequireHttpsMetadata = false
			//});

			services.AddAuthentication("Bearer")
				.AddIdentityServerAuthentication(options =>
				{
					options.Authority = Configuration.GetSection("Authority").GetValue<string>("Url");
					options.RequireHttpsMetadata = false;

					options.ApiName = "WebAPI";
				});

			// Claims-Based Authorization: role claims.
			services.AddAuthorization(options =>
			{
				// Policy for dashboard: only administrator role.
				options.AddPolicy("Manage Accounts", policy => policy.RequireClaim("role", "administrator"));
				//options.AddPolicy("Asset.Edit", policy => policy.RequireClaim("role", "administrator"));


				// Policy for resources: user or administrator role. 
				options.AddPolicy("Access Resources", policyBuilder => policyBuilder.RequireAssertion(
						context => context.User.HasClaim(claim => (claim.Type == "role" && claim.Value == "user")
						   || (claim.Type == "role" && claim.Value == "administrator"))
					)
				);
			});



            // services.AddSingleton<IHostedService, SFTPService>();

            // services.AddSingleton<IHostedService, EmployeeNotValidatedEmailService>();


#if RELEASE
			services.AddSingleton<IHostedService, LdapBackgroundService>();
			services.AddSingleton<IHostedService, EmployeeEmailService>();
			services.AddSingleton<IHostedService, RequestEmailService>();
			services.AddSingleton<IHostedService, OfferEmailService>();
			services.AddSingleton<IHostedService, BNRRateAPIService>();
			services.AddSingleton<IHostedService, ApiSAPAribaService>();
			services.AddSingleton<IHostedService, OrderEmailService>();
			services.AddSingleton<IHostedService, CreateAssetAPIService>();
#endif
            services.AddTransient<IBudgetsRepository, BudgetsRepository>();
			services.AddTransient<IBudgetBasesRepository, BudgetBasesRepository>();
			services.AddTransient<IBudgetMonthBasesRepository, BudgetMonthBasesRepository>();
			services.AddTransient<IBudgetForecastsRepository, BudgetForecastsRepository>();
			//services.AddTransient<IRepository<Budget>, Repository<Budget>>();
			services.AddTransient<IBudgetOpsRepository, BudgetOpsRepository>();
			services.AddTransient<IBudgetBaseOpsRepository, BudgetBaseOpsRepository>();
			services.AddTransient<IOrdersRepository, OrdersRepository>();
			services.AddTransient<IOrderOpsRepository, OrderOpsRepository>();
			services.AddTransient<IOrderTypesRepository, OrderTypesRepository>();
			services.AddTransient<IOfferTypesRepository, OfferTypesRepository>();
			services.AddTransient<IRequestsRepository, RequestsRepository>();
			services.AddTransient<IRequestOpsRepository, RequestOpsRepository>();
			services.AddTransient<IContractsRepository, ContractsRepository>();
			services.AddTransient<IContractOpsRepository, ContractOpsRepository>();
			services.AddTransient<IOffersRepository, OffersRepository>();
			services.AddTransient<IOfferOpsRepository, OfferOpsRepository>();
			services.AddTransient<IRepository<Model.Article>, Repository<Model.Article>>();
			services.AddTransient<IAccountsRepository, AccountsRepository>();
			services.AddTransient<IExpAccountsRepository, ExpAccountsRepository>();
			services.AddTransient<IAssetTypesRepository, AssetTypesRepository>();
			services.AddTransient<IAccMonthsRepository, AccMonthsRepository>();
			services.AddTransient<IAdmCentersRepository, AdmCentersRepository>();
			services.AddTransient<IAssetsRepository, AssetsRepository>();
			services.AddTransient<IAssetNiRepository, AssetNiRepository>();
			services.AddTransient<IAssetOpsRepository, AssetOpsRepository>();
			services.AddTransient<IAssetComponentOpsRepository, AssetComponentOpsRepository>();
			services.AddTransient<IAssetCategoriesRepository, AssetCategoriesRepository>();
			services.AddTransient<IUomsRepository, UomsRepository>();
			services.AddTransient<IAssetClassesRepository, AssetClassesRepository>();
			services.AddTransient<ICostCentersRepository, CostCentersRepository>();
			services.AddTransient<IDepartmentsRepository, DepartmentsRepository>();
			services.AddTransient<IDocumentsRepository, DocumentsRepository>();
			services.AddTransient<IDocumentTypesRepository, DocumentTypesRepository>();
			services.AddTransient<IAppStatesRepository, AppStatesRepository>();
			services.AddTransient<IEntityFilesRepository, EntityFilesRepository>();
			//services.AddTransient<IInvCompOpsRepository, InvCompOpsRepository>();
			//services.AddTransient<IInvCompsRepository, InvCompsRepository>();
			services.AddTransient<IDivisionsRepository, DivisionsRepository>();
			services.AddTransient<IDictionaryTypesRepository, DictionaryTypesRepository>();
			services.AddTransient<IPartnerLocationsRepository, PartnerLocationsRepository>();
			services.AddTransient<IInventoryRepository, InventoryRepository>();
			services.AddTransient<IEmployeesRepository, EmployeesRepository>();
			services.AddTransient<ILocationsRepository, LocationsRepository>();
			services.AddTransient<IPartnersRepository, PartnersRepository>();
			services.AddTransient<IRegionsRepository, RegionsRepository>();
			services.AddTransient<IRoomsRepository, RoomsRepository>();
			services.AddTransient<IReportingRepository, ReportingRepository>();
			services.AddTransient<IAssetNiRepository, AssetNiRepository>();
			services.AddTransient<IAssetStatesRepository, AssetStatesRepository>();
			services.AddTransient<IInvStatesRepository, InvStatesRepository>();
			services.AddTransient<IAdministrationsRepository, AdministrationsRepository>();
			services.AddTransient<IDictionaryItemsRepository, DictionaryItemsRepository>();
			services.AddTransient<IBrandsRepository, BrandsRepository>();
			services.AddTransient<IModelsRepository, ModelsRepository>();
			services.AddTransient<IInsuranceCategoriesRepository, InsuranceCategoriesRepository>();
			services.AddTransient<ISubCategoriesRepository, SubCategoriesRepository>();
			services.AddTransient<ISubCategoriesENRepository, SubCategoriesENRepository>();
			services.AddTransient<IMasterTypesRepository, MasterTypesRepository>();
			services.AddTransient<IProjectsRepository, ProjectsRepository>();
			services.AddTransient<IProjectTypesRepository, ProjectTypesRepository>();
			services.AddTransient<IActivitiesRepository, ActivitiesRepository>();
			services.AddTransient<ITaxsRepository, TaxsRepository>();
			services.AddTransient<ISubTypesRepository, SubTypesRepository>();
			services.AddTransient<ISubTypePartnersRepository, SubTypePartnersRepository>();
			services.AddTransient<ITypesRepository, TypesRepository>();
			services.AddTransient<ICompaniesRepository, CompaniesRepository>();
			services.AddTransient<IAssetNaturesRepository, AssetNaturesRepository>();
			services.AddTransient<IBudgetManagersRepository, BudgetManagersRepository>();
			services.AddTransient<IDimensionsRepository, DimensionsRepository>();
			services.AddTransient<IConfigValuesRepository, ConfigValuesRepository>();
			services.AddTransient<IAssetComponentsRepository, AssetComponentsRepository>();
			services.AddTransient<ICountriesRepository, CountriesRepository>();
			services.AddTransient<ICountiesRepository, CountiesRepository>();
			services.AddTransient<ICitiesRepository, CitiesRepository>();
			services.AddTransient<IEmailTypesRepository, EmailTypesRepository>();
			services.AddTransient<IEntityTypesRepository, EntityTypesRepository>();
			services.AddTransient<IEmailManagersRepository, EmailManagersRepository>();
			services.AddTransient<IEmailStatusRepository, EmailStatusRepository>();
			services.AddTransient<IEmailOrderStatusRepository, EmailOrderStatusRepository>();
			services.AddTransient<IEmployeeService, EmployeeService>();
			services.AddTransient<IEmployeeNotValidatedService, EmployeeNotValidatedService>();
			services.AddScoped<ITableDefinitionsRepository, TableDefinitionsRepository>();
			services.AddScoped<IColumnDefinitionsRepository, ColumnDefinitionsRepository>();
			services.AddTransient<IRoutesRepository, RoutesRepository>();
			services.AddTransient<IRouteChildrensRepository, RouteChildrensRepository>();
			services.AddTransient<IPermissionsRepository, PermissionsRepository>();
			services.AddTransient<IPermissionTypesRepository, PermissionTypesRepository>();
			services.AddTransient<IPermissionRolesRepository, PermissionRolesRepository>();
			services.AddTransient<IColumnDefinitionRolesRepository, ColumnDefinitionRolesRepository>();
			services.AddTransient<IRouteRolesRepository, RouteRolesRepository>();
			services.AddTransient<IRouteChildrenRolesRepository, RouteChildrenRolesRepository>();
			services.AddTransient<IDashboardsRepository, DashboardsRepository>();
			services.AddTransient<IDashboardWHsRepository, DashboardWHsRepository>();
			services.AddTransient<IBadgesRepository, BadgesRepository>();
			services.AddTransient<IMaterialsRepository, MaterialsRepository>();
			services.AddTransient<IEmployeeCostCentersRepository, EmployeeCostCentersRepository>();
			services.AddTransient<IEmployeeCompaniesRepository, EmployeeCompaniesRepository>();
			services.AddTransient<IEmployeeDivisionsRepository, EmployeeDivisionsRepository>();
			services.AddTransient<IEmployeeStoragesRepository, EmployeeStoragesRepository>();
			services.AddTransient<IRequestBudgetForecastsRepository, RequestBudgetForecastsRepository>();
			services.AddTransient<IRequestBudgetForecastMaterialsRepository, RequestBudgetForecastMaterialsRepository>();
			services.AddTransient<IRequestBFMaterialCostCentersRepository, RequestBFMaterialCostCentersRepository>();
			services.AddTransient<IProjectTypeDivisionsRepository, ProjectTypeDivisionsRepository>();
			services.AddTransient<IOfferMaterialsRepository, OfferMaterialsRepository>();
			services.AddTransient<IOrderMaterialsRepository, OrderMaterialsRepository>();
			services.AddTransient<IDevicesRepository, DevicesRepository>();
			services.AddTransient<IDevicesTypesRepository, DeviceTypesRepository>();
			services.AddTransient<IMobilePhonesRepository, MobilePhonesRepository>();
			services.AddTransient<IPrintLabelsRepository, PrintLabelsRepository>();
			services.AddTransient<IStocksRepository, StocksRepository>();
			services.AddTransient<IRatesRepository, RatesRepository>();
			services.AddTransient<IStoragesRepository, StoragesRepository>();
			services.AddTransient<IMatrixRepository, MatrixRepository>();
			services.AddTransient<ILevelsRepository, LevelsRepository>();
			services.AddTransient<IMatrixLevelsRepository, MatrixLevelsRepository>();
			services.AddTransient<IColumnFiltersRepository, ColumnFiltersRepository>();

			services.AddTransient<IErrorsRepository, ErrorsRepository>();
			services.AddTransient<IErrorTypesRepository, ErrorTypesRepository>();
			services.AddTransient<IAmortizationsRepository, AmortizationsRepository>();
			services.AddTransient<ICapAmortizationsRepository, CapAmortizationsRepository>();
			services.AddTransient<IWFHChecksRepository, WFHChecksRepository>();
            services.AddScoped<IAsyncErrorsRepository, AsyncErrorsRepository>();

            //services.AddTransient<IInventoryAssetsRepository, InventoryAssetsRepository>();

            services.AddTransient<ITransferInStockSAPsRepository, TransferInStockSAPsRepository>();
			services.AddTransient<ITransferAssetSAPsRepository, TransferAssetSAPsRepository>();
			services.AddTransient<IRetireAssetSAPsRepository, RetireAssetSAPsRepository>();
			services.AddTransient<ICreateAssetSAPsRepository, CreateAssetSAPsRepository>();
			services.AddTransient<IAssetInvPlusSAPsRepository, AssetInvPlusSAPsRepository>();
			services.AddTransient<IAssetInvMinusSAPsRepository, AssetInvMinusSAPsRepository>();
			services.AddTransient<IAssetChangeSAPsRepository, AssetChangeSAPsRepository>();
			services.AddTransient<IAquisitionAssetSAPsRepository, AquisitionAssetSAPsRepository>();
			services.AddTransient<IPlantsRepository, PlantsRepository>();
			services.AddTransient<IBNRRatesImportService, BNRRatesImportService>();
			services.AddTransient<ICreateAssetSAPService, CreateAssetSAPService>();
			services.AddTransient<IContractImportService, ContractImportService>();
			services.AddTransient<IAssetService, AssetService>();

			services.AddScoped<ILdapAuthenticationService, LdapAuthenticationService>();
			services.AddScoped<ILocalAuthenticationService, LocalAuthenticationService>();
			services.AddTransient<ILdapSyncService, LdapSyncService>();
			services.AddTransient<INotifyService, NotifyService>();
			services.AddTransient<IAppendix1Generator, Appendix1Generator>();
			services.AddTransient<IAppendixAGenerator, AppendixAGenerator>();
			services.AddTransient<IAllowLabelGenerator, AllowLabelGenerator>();
			services.AddTransient<IAppendixMFGenerator, AppendixMFGenerator>();
			services.AddTransient<IAppendixBookBeforeInvGenerator, AppendixBookBeforeInvGenerator>();
			services.AddTransient<IAppendixBookAfterInvGenerator, AppendixBookAfterInvGenerator>();
			services.AddTransient<IAppendixAMinusGenerator, AppendixAMinusGenerator>();
			services.AddTransient<IAppendixAPlusGenerator, AppendixAPlusGenerator>();
			services.AddTransient<IAppendixPVGenerator, AppendixPVGenerator>();
			services.AddTransient<IAppendixPVFinalGenerator, AppendixPVFinalGenerator>();
			services.AddTransient<IOrderValidationService, OrderValidationService>();
			services.AddTransient<IOfferService, OfferService>();
			services.AddScoped<IRequestsService, RequestsService>();
			services.AddScoped<IOrdersService, OrdersService>();
			services.AddScoped<IInventoryService, InventoryService>();
			services.AddScoped<IDocumentHelperService, DocumentHelperService>();
			services.AddScoped<IEmailStatusService, EmailStatusService>();
			
			services.AddScoped<IEmailOrderStatusService, EmailOrderStatusService>();
			services.AddScoped<IEmailOfferStatusService, EmailOfferStatusService>();

			services.AddTransient<IRequestValidationService, RequestValidationService>();
			services.AddScoped<IEmailRequestStatusService, EmailRequestStatusService>();
			services.AddTransient<IOrderFlowService, OrderFlowService>();
            services.AddScoped<IInvCommitteeMembersRepository, InvCommitteeMembersRepository>();
            services.AddScoped<IInvCommitteePositionsRepository, InvCommitteePostionsRepository>();
            services.AddScoped<IInventoryPlansRepository, InventoryPlansRepository>();
			services.AddScoped<IInvCommitteesRepository, InvCommitteesRepository>();

            //services.AddScoped<RoleManager<IdentityRole>>();

            //services.AddScoped<RoleManager<IdentityRole>>();

            services.AddSingleton<IEmailSender, EmailSender>();
			services.AddSingleton(emailConfig);



			services.Configure<FormOptions>(o => {
				o.ValueLengthLimit = int.MaxValue;
				o.MultipartBodyLengthLimit = int.MaxValue;
				o.MemoryBufferThreshold = int.MaxValue;
			});

			// services.Configure<LdapConfig>(this.Configuration.GetSection("ldap"));
			services.Configure<LdapConfig>(this.Configuration.GetSection("ldap"));

			Log.Logger = new LoggerConfiguration()
				   .ReadFrom.Configuration(Configuration)
				   .CreateLogger();


			services.AddIdentityServer(options =>
			{
				options.Events.RaiseErrorEvents = true;
				options.Events.RaiseInformationEvents = true;
				options.Events.RaiseFailureEvents = true;
				options.Events.RaiseSuccessEvents = true;
			})
				.AddDeveloperSigningCredential()
				.AddInMemoryIdentityResources(Config.GetIdentityResources())
				.AddInMemoryApiResources(Config.GetApiResources())
				.AddInMemoryClients(Config.GetClients())
				//.AddAspNetIdentity<ApplicationUser>();// MODIFICAT
			   .Services.AddTransient<IResourceOwnerPasswordValidator, CustomUserValidator>();

			services.AddAuthentication(o =>
			{
				o.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
				o.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
			});

			// services.AddScoped<LogUserActivity>();

			services.AddControllers()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.IncludeFields = true;
				});


		}


		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//app.UseForwardedHeaders();
			app.UseRouting();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			// global cors policy
			// app.UseCors("CorsPolicy");

			// global cors policy
			app.UseCors(x => x
				.AllowAnyMethod()
				.AllowAnyHeader()
				.SetIsOriginAllowed(origin => true) // allow any origin
				.AllowCredentials()); // allow credentials

			app.UseAuthentication();
			app.UseAuthorization();

			//app.UseEndpoints(x => x.MapControllers());

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();

				// BLAZOR //
				//endpoints.MapBlazorHub();
				//endpoints.MapFallbackToPage("/_Host");

				//endpoints.MapRazorPages(); // existing endpoints
				//endpoints.MapBlazorHub();
				// BLAZOR //
				endpoints.MapHub<NotifyHub>("/notify");
			});

			app.UseIdentityServer();

			// app.UseMvc();

			// app.UseEndpoints();




			//app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

			// Adds Identity.
			//app.UseAuthentication();

			// Adds IdentityServer.

		}
	}
}
