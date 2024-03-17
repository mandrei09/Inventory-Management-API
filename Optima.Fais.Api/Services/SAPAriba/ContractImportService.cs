using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.SAPAriba
{
	public class ContractImportService : IContractImportService
    {
        //private const string BaseUrl = @"https://eu.openapi.ariba.com/api/retrieve-contract-workspaces/v1/prod/contractWorkspaces?realm=dante-T&user=sorina.manzicu@emag.ro&passwordAdapter=PasswordAdapter1&$count=true"; // UAT
        //private const string BaseUrl = @"https://eu.openapi.ariba.com/api/retrieve-contract-workspaces/v1/prod/contractWorkspaces?realm=dante&user=daniela.niculescu@emag.ro&passwordAdapter=PasswordAdapter1&$count=true";
        private readonly string _BASEURL;
        private readonly string _AUTHORIZATION;
        private readonly string _API_KEY;
        private readonly string _ADDRESS;
        private readonly IConfiguration _configuration;

		public ContractImportService(IServiceProvider services, IConfiguration configuration)
        {
            Services = services;
			_configuration = configuration;
			this._BASEURL = configuration.GetSection("SAP_ARIBA").GetValue<string>("URL");
            this._AUTHORIZATION = configuration.GetSection("SAP_ARIBA").GetValue<string>("AUTHORIZATION");
            this._API_KEY = configuration.GetSection("SAP_ARIBA").GetValue<string>("API_KEY");
            this._ADDRESS = configuration.GetSection("SAP_ARIBA").GetValue<string>("ADDRESS");
        }

        public IServiceProvider Services { get; }

        public async Task<int> ContractImportAsync()
        {
            int countChanges = 0;

            using (var scope = Services.CreateScope())
            {
                var dbContext =
                   scope.ServiceProvider
                       .GetRequiredService<ApplicationDbContext>();

                Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "CONTRACT").SingleOrDefault();

                if (syncStatus == null)
                {
                    syncStatus = new Model.SyncStatus()
                    {
                        Code = "CONTRACT",
                        Name = "Contracte SAP Ariba",
                        SyncEnabled = true,
                        SyncInterval = 1440
                    };

                    dbContext.Add(syncStatus);
                    dbContext.SaveChanges();
                }

                if (syncStatus.SyncEnabled)
                {
                    var lastSync = syncStatus.SyncLast;

                    if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
                    {
                        syncStatus.SyncStart = DateTime.Now;

                        var IdContract = await dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Contract").SingleAsync();
                        var IdContractAmount = await dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "ContractAmount").SingleAsync();
                        var IdCommodity = await dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Commodity").SingleAsync();
                        var IdPartner = await dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Partner").SingleAsync();
                        var IdPartnerLocation = await dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "PartnerLocation").SingleAsync();
                        var IdPartnerActive = await dbContext.Set<Model.DimensionERP>().FromSql("UpdatePartnerAsActive {0}", "Partner").SingleAsync();

                        const int CONTRACT_SYNC_TURNS = 5;

                        for (int i = 1; i <= CONTRACT_SYNC_TURNS; i++)
                        {
                            int skipContracts = 500 * (i - 1); 
                            string skipContractsString = "&$skip=" + skipContracts.ToString();

                            var contracts = await GetContractsAsync(skipContractsString);

                            Model.Address address = null;
                            Model.Owner owner = null;
                            Model.BusinessSystem businessSystem = null;
                            Model.Partner partner = null;
                            Model.PartnerLocation partnerLocation = null;
                            Model.ContractAmount contractAmount = null;
                            Model.Contract contract = null;
                            Model.Rate rate = null;
                            Model.Company companyCode = null;
                            List<CustomFields> customFieldExpenceTypes = null;
                            List<CustomFields> customFieldPaymentTerms = null;
                            List<CustomFields> customFieldCompanyCodes = null;
                            string customFieldExpenceType = string.Empty;
                            decimal customFieldPaymentTerm = 0;
                            string customFieldCompanyCode = string.Empty;

                            if (contracts.Count > 0)
                            {
                                for (int c = 0; c < contracts.Count; c++)
                                {
                                    
                                    if (contracts[c].ContractId == "CW48820")
                                    {
                                        var test = 10;
                                    }
                                    
                                    if (contracts[c].Owner != null)
                                    {
                                        owner = dbContext.Set<Model.Owner>().Where(com => com.UniqueName == contracts[c].Owner.UniqueName).FirstOrDefault();

                                        if (owner == null)
                                        {
                                            owner = new Model.Owner()
                                            {
                                                UniqueName = contracts[c].Owner.UniqueName,
                                                FullName = contracts[c].Owner.Name,
                                                Email = contracts[c].Owner.EmailAddress,
                                                Organization = contracts[c].Owner.Organization,
                                                OrgANId = contracts[c].Owner.OrgANId,
                                                OrgName = contracts[c].Owner.OrgName,
                                                FirstName = string.Empty,
                                                LastName = string.Empty
                                            };

                                            dbContext.Add(owner);
                                        }
                                    }

                                    if (contracts[c].BusinessSystem != null)
                                    {
                                        businessSystem = dbContext.Set<Model.BusinessSystem>().Where(com => com.Code == contracts[c].BusinessSystem.BusinessSystemId).FirstOrDefault();

                                        if (businessSystem == null)
                                        {
                                            businessSystem = new Model.BusinessSystem()
                                            {
                                                Code = contracts[c].BusinessSystem.BusinessSystemId,
                                                Name = contracts[c].BusinessSystem.BusinessSystemName,
                                                IsDeleted = false
                                            };

                                            dbContext.Add(businessSystem);
                                        }
                                    }

                                    if (contracts[c].Supplier != null)
                                    {

                                        if (contracts[c].Supplier.Address != null)
                                        {
                                            Model.Company company = dbContext.Set<Model.Company>().Where(com => com.Code == contracts[c].Supplier.Address.Country + contracts[c].Supplier.Address.State).FirstOrDefault();

                                            if (company == null)
                                            {
                                                company = new Model.Company()
                                                {
                                                    Code = contracts[c].Supplier.Address.Country + contracts[c].Supplier.Address.State,
                                                    Name = contracts[c].Supplier.Address.Country + contracts[c].Supplier.Address.State,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(company);
                                            }

                                            Model.Country country = dbContext.Set<Model.Country>().Where(com => com.Code == contracts[c].Supplier.Address.Country).FirstOrDefault();

                                            if (country == null)
                                            {
                                                country = new Model.Country()
                                                {
                                                    Code = contracts[c].Supplier.Address.Country,
                                                    Name = contracts[c].Supplier.Address.Country,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(country);
                                            }


                                            Model.County county = dbContext.Set<Model.County>().Include(county => county.Country).Where(com => com.Name == contracts[c].Supplier.Address.City && com.Country.Code == contracts[c].Supplier.Address.Country).FirstOrDefault();

                                            if (county == null)
                                            {
                                                county = new Model.County()
                                                {
                                                    Code = contracts[c].Supplier.Address.City,
                                                    Name = contracts[c].Supplier.Address.City,
                                                    Country = country,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(county);
                                            }


                                            Model.City city = dbContext.Set<Model.City>().Include(c => c.County).Where(com => com.Name == contracts[c].Supplier.Address.City && com.County.Name == contracts[c].Supplier.Address.City).FirstOrDefault();

                                            if (city == null)
                                            {
                                                city = new Model.City()
                                                {
                                                    Code = contracts[c].Supplier.Address.City,
                                                    Name = contracts[c].Supplier.Address.City,
                                                    County = county,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(city);
                                            }

                                            address = dbContext.Set<Model.Address>().Where(com => com.UniqueName == contracts[c].Supplier.Address.UniqueName).FirstOrDefault();

                                            if (address == null)
                                            {
                                                address = new Model.Address()
                                                {
                                                    UniqueName = contracts[c].Supplier.Address.UniqueName,
                                                    Name = contracts[c].Supplier.Address.Name,
                                                    Phone = contracts[c].Supplier.Address.Phone,
                                                    Fax = contracts[c].Supplier.Address.Fax,
                                                    AddressDetail = contracts[c].Supplier.Address.Lines != null ? contracts[c].Supplier.Address.Lines[0] : "",
                                                    PostalCode = contracts[c].Supplier.Address.PostalCode,
                                                    City = city,
                                                    Company = company,
                                                    IsDeleted = false,
                                                    Code = string.Empty
                                                };

                                                dbContext.Add(address);
                                            }
                                        }

                                        partnerLocation = dbContext.Set<Model.PartnerLocation>().Where(com => com.Cui == contracts[c].Supplier.SystemID).FirstOrDefault();

                                        if (partnerLocation == null)
                                        {
                                            partnerLocation = new Model.PartnerLocation()
                                            {
                                                Cui = contracts[c].Supplier.SystemID,
                                                Denumire = contracts[c].Supplier.Name,
                                                IsDeleted = false
                                                //RegistryNumber = contracts[c].Supplier.SystemID,
                                                //FiscalCode = contracts[c].Supplier.SystemID,
                                                //AddressDetail = address,
                                            };

                                            dbContext.Add(partnerLocation);
                                        }
                                        else
                                        {
                                            partnerLocation.IsDeleted = false;
                                            dbContext.Update(partnerLocation);
                                        }

                                        partner = dbContext.Set<Model.Partner>().Where(com => com.ErpCode == contracts[c].Supplier.SystemID).FirstOrDefault();

                                        if (partner == null)
                                        {
                                            partner = new Model.Partner()
                                            {
                                                ErpCode = contracts[c].Supplier.SystemID,
                                                Name = contracts[c].Supplier.Name,
                                                RegistryNumber = contracts[c].Supplier.SystemID,
                                                FiscalCode = contracts[c].Supplier.SystemID,
                                                AddressDetail = address,
                                                PartnerLocation = partnerLocation,
                                                IsDeleted = false
                                            };

                                            dbContext.Add(partner);
										}
										else
										{
                                            partner.Name = contracts[c].Supplier.Name;
                                            partner.IsDeleted = false;
                                            dbContext.Update(partner);
										}
                                    }

                                    Model.AppState appState = dbContext.Set<Model.AppState>().Where(com => com.Name == contracts[c].ContractStatus).FirstOrDefault();

                                    if (appState == null)
                                    {
                                        appState = new Model.AppState()
                                        {
                                            Code = contracts[c].ContractStatus,
                                            Name = contracts[c].ContractStatus,
                                            IsDeleted = false
                                        };

                                        dbContext.Add(appState);
                                    }

									if (contracts[c].CustomFields.Count > 0)
									{
                                        decimal n1;

                                        customFieldExpenceTypes = contracts[c].CustomFields.Where(c => c.FieldId == "cus_ExpenseType").ToList();
                                        customFieldExpenceType = customFieldExpenceTypes.Count > 0 ? customFieldExpenceTypes[0].TextValue.Count > 0 ? customFieldExpenceTypes[0].TextValue[0] : "" : "";

                                        customFieldPaymentTerms = contracts[c].CustomFields.Where(c => c.FieldId == "cus_Paymentterms").ToList();
                                        if(customFieldPaymentTerms.Count > 0 && customFieldPaymentTerms[0].BigDecimalValue != null)
										{
                                            bool result = Decimal.TryParse(customFieldPaymentTerms[0].BigDecimalValue, out n1);

											if (result)
											{
                                                customFieldPaymentTerm = n1;
                                            }

                                        }

                                        customFieldCompanyCodes = contracts[c].CustomFields.Where(c => c.FieldId == "cus_Companycode").ToList();
                                        customFieldCompanyCode = customFieldCompanyCodes.Count > 0 ? customFieldCompanyCodes[0].TextValue.Count > 0 ? customFieldCompanyCodes[0].TextValue[0] : "" : "";

                                        if(customFieldCompanyCode != "")
										{
                                            companyCode = dbContext.Set<Model.Company>().Where(com => com.Code == customFieldCompanyCode).FirstOrDefault();

                                            if (companyCode == null)
                                            {
                                                companyCode = new Model.Company()
                                                {
                                                    Code = customFieldCompanyCode,
                                                    Name = customFieldCompanyCode,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(companyCode);
                                            }
                                        }
                                    }
                                       
                                    contract = dbContext.Set<Model.Contract>().Where(com => com.ContractId == contracts[c].ContractId).FirstOrDefault();

                                    if (contract == null)
                                    {

                                        if (contracts[c].ContractAmount != null)
                                        {
                                            Model.Uom uom = dbContext.Set<Model.Uom>().Where(com => com.Code == contracts[c].ContractAmount.Currency).FirstOrDefault();
                                            rate = dbContext.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == contracts[c].ContractAmount.Currency && com.IsLast == true).FirstOrDefault();

                                            if (uom == null)
                                            {
                                                uom = new Model.Uom()
                                                {
                                                    Code = contracts[c].ContractAmount.Currency,
                                                    Name = contracts[c].ContractAmount.Currency,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(uom);
                                            }
                                            else
                                            {
                                                uom.IsDeleted = false;
                                                dbContext.Update(uom);
                                            }


                                            contractAmount = new Model.ContractAmount()
                                            {
                                                Amount = contracts[c].ContractAmount.Amount,
                                                Uom = uom,
                                                Rate = rate,
                                                IsDeleted = false,
                                                Code = string.Empty,
                                                Name = string.Empty,
                                                AmountRem = contracts[c].ContractAmount.Amount
                                            };

                                            dbContext.Add(contractAmount);
                                        }


                                        contract = new Model.Contract()
                                        {
                                            ContractId = contracts[c].ContractId,
                                            Title = contracts[c].Title,
                                            Name = contracts[c].Description,
                                            AppState = appState,
                                            EffectiveDate = contracts[c].EffectiveDate,
                                            AgreementDate = contracts[c].AgreementDate,
                                            ExpirationDate = contracts[c].ExpirationDate,
                                            CreationDate = contracts[c].CreationDate,
                                            Version = contracts[c].Version,
                                            TemplateId = contracts[c].TemplateId,
                                            AmendmentType = contracts[c].AmendmentType,
                                            AmendmentReason = contracts[c].AmendmentReason,
                                            Origin = contracts[c].Origin,
                                            HierarchicalType = contracts[c].HierarchicalType,
                                            ExpirationTermType = contracts[c].ExpirationTermType,
                                            RelatedId = contracts[c].RelatedId,
                                            MaximumNumberOfRenewals = contracts[c].MaximumNumberOfRenewals,
                                            AutoRenewalInterval = contracts[c].AutoRenewalInterval,
                                            IsTestProject = contracts[c].IsTestProject,
                                            Owner = owner,
                                            Partner = partner,
                                            BusinessSystem = businessSystem,
                                            ContractAmount = contractAmount,
                                            Code = customFieldExpenceType,
                                            PaymentTerms = customFieldPaymentTerm,
                                            Company = companyCode
                                        };

                                        dbContext.Add(contract);
                                    }
                                    else
                                    {
                                        contractAmount = dbContext.Set<Model.ContractAmount>().Where(com => com.Id == contract.ContractAmountId).FirstOrDefault();

                                        if (contracts[c].ContractAmount != null)
                                        {
                                            Model.Uom uom = dbContext.Set<Model.Uom>().Where(com => com.Code == contracts[c].ContractAmount.Currency).FirstOrDefault();
                                            rate = dbContext.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == contracts[c].ContractAmount.Currency && com.IsLast == true).FirstOrDefault();

                                            if (uom == null)
                                            {
                                                uom = new Model.Uom()
                                                {
                                                    Code = contracts[c].ContractAmount.Currency,
                                                    Name = contracts[c].ContractAmount.Currency,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(uom);
                                            }
                                            else
                                            {
                                                uom.IsDeleted = false;
                                                dbContext.Update(uom);
                                            }

                                            if (contractAmount == null)
                                            {
                                                contractAmount = new Model.ContractAmount()
                                                {
                                                    Amount = contracts[c].ContractAmount.Amount,
                                                    Uom = uom,
                                                    Rate = rate,
                                                    IsDeleted = false,
                                                    Code = string.Empty,
                                                    Name = string.Empty,
                                                    AmountRem = contracts[c].ContractAmount.Amount
                                                };

                                                dbContext.Add(contractAmount);
                                            }
                                            else
                                            {
                                                contractAmount.Amount = contracts[c].ContractAmount.Amount;
                                                contractAmount.Uom = uom;
                                                contractAmount.Rate = rate;
                                                contractAmount.IsDeleted = false;

                                                dbContext.Update(contractAmount);

                                            }
                                        }

                                        contract.ModifiedAt = DateTime.Now;
                                        contract.Title = contracts[c].Title;
                                        contract.Name = contracts[c].Description;
                                        contract.AppState = appState;
                                        contract.EffectiveDate = contracts[c].EffectiveDate;
                                        contract.AgreementDate = contracts[c].AgreementDate;
                                        contract.ExpirationDate = contracts[c].ExpirationDate;
                                        contract.CreationDate = contracts[c].CreationDate;
                                        contract.Version = contracts[c].Version;
                                        contract.TemplateId = contracts[c].TemplateId;
                                        contract.AmendmentType = contracts[c].AmendmentType;
                                        contract.AmendmentReason = contracts[c].AmendmentReason;
                                        contract.Origin = contracts[c].Origin;
                                        contract.HierarchicalType = contracts[c].HierarchicalType;
                                        contract.ExpirationTermType = contracts[c].ExpirationTermType;
                                        contract.RelatedId = contracts[c].RelatedId;
                                        contract.MaximumNumberOfRenewals = contracts[c].MaximumNumberOfRenewals;
                                        contract.AutoRenewalInterval = contracts[c].AutoRenewalInterval;
                                        contract.IsTestProject = contracts[c].IsTestProject;
                                        contract.Owner = owner;
                                        contract.Partner = partner;
                                        contract.BusinessSystem = businessSystem;
                                        contract.ContractAmount = contractAmount;
                                        contract.IsDeleted = false;
                                        contract.Code = customFieldExpenceType;
                                        contract.PaymentTerms = customFieldPaymentTerm;
                                        contract.Company = companyCode;

                                        dbContext.Update(contract);

                                    }

                                    if (contracts[c].Commodities.Count > 0)
                                    {
                                        for (int m = 0; m < contracts[c].Commodities.Count; m++)
                                        {
                                            Model.Commodity commodity = dbContext.Set<Model.Commodity>().Include(c => c.Contract).Where(com => com.UniqueName == contracts[c].Commodities[m].UniqueName && com.ContractId == contract.Id).FirstOrDefault();

                                            if (commodity == null)
                                            {
                                                commodity = new Model.Commodity()
                                                {
                                                    Code = contracts[c].Commodities[m].UniqueName,
                                                    UniqueName = contracts[c].Commodities[m].UniqueName,
                                                    Domain = contracts[c].Commodities[m].Domain,
                                                    Name = contracts[c].Commodities[m].Name,
                                                    Contract = contract,
                                                    IsDeleted = false
                                                };

                                                dbContext.Add(commodity);
											}
											else
											{
                                                commodity.Code = contracts[c].Commodities[m].UniqueName;
                                                commodity.Domain = contracts[c].Commodities[m].Domain;
                                                commodity.Name = contracts[c].Commodities[m].Name;
                                                commodity.ContractId = contract.Id;
                                                commodity.IsDeleted = false;
                                                dbContext.Update(commodity);

                                            }
                                        }
                                    }

                                    if (contracts[c].Regions.Count > 0)
                                    {
                                        for (int m = 0; m < contracts[c].Regions.Count; m++)
                                        {
                                            Model.ContractRegion contractRegion = dbContext.Set<Model.ContractRegion>().Where(com => com.UniqueName == contracts[c].Regions[m].UniqueName).FirstOrDefault();

                                            if (contractRegion == null)
                                            {
                                                contractRegion = new Model.ContractRegion()
                                                {
                                                    Code = contracts[c].Regions[m].UniqueName,
                                                    UniqueName = contracts[c].Regions[m].UniqueName,
                                                    Name = contracts[c].Regions[m].Name,
                                                    IsDeleted = false,
                                                    Contract = contract,
                                                };

                                                dbContext.Add(contractRegion);
											}
											else
											{
                                                contractRegion.Code = contracts[c].Regions[m].UniqueName;
                                                contractRegion.Name = contracts[c].Regions[m].Name;
                                                contractRegion.ContractId = contract.Id;

                                                dbContext.Update(contractRegion);
                                            }
                                        }
                                    }

                                    if (contracts[c].Departments.Count > 0)
                                    {
                                        for (int m = 0; m < contracts[c].Departments.Count; m++)
                                        {
                                            Model.ContractDivision contractDivision = dbContext.Set<Model.ContractDivision>().Where(com => com.UniqueName == contracts[c].Departments[m].UniqueName).FirstOrDefault();

                                            if (contractDivision == null)
                                            {
                                                contractDivision = new Model.ContractDivision()
                                                {
                                                    Code = contracts[c].Departments[m].UniqueName,
                                                    UniqueName = contracts[c].Departments[m].UniqueName,
                                                    Name = contracts[c].Departments[m].Name,
                                                    IsDeleted = false,
                                                    Contract = contract,
                                                };

                                                dbContext.Add(contractDivision);
                                            }
                                            else
                                            {
                                                contractDivision.Code = contracts[c].Departments[m].UniqueName;
                                                contractDivision.Name = contracts[c].Departments[m].Name;
                                                contractDivision.ContractId = contract.Id;

                                                dbContext.Update(contractDivision);
                                            }
                                        }
                                    }

                                    dbContext.SaveChanges();

                                    countChanges++;
                                }

                                //for (int i = 0; i < legalEntity.Count; i++)
                                //{
                                //    var companyId = dbContext.Set<Model.ERPImportResult>().FromSql("ImportLocation {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                                //        legalEntity[i].FinancialDimension, legalEntity[i].LegalEntityId, legalEntity[i].DimensionValue, legalEntity[i].GroupDimension, legalEntity[i].IsSuspended, legalEntity[i].Description, legalEntity[i].Owner).Single();

                                //    countChanges++;
                                //}
                        }
                    }

                    var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();
                    syncStatus.SyncEnd = DateTime.Now;
                    syncStatus.SyncLast = DateTime.Now;
                    dbContext.Update(syncStatus);
                    dbContext.SaveChanges();

                    }
                }
            }

            return countChanges;
        }

        public async Task<List<ContractDetail>> GetContractsAsync(string skip)
        {
            HttpClient clientContract = null;

            var bearerToken = "";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_ADDRESS);

                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;

                var keysValues = new List<KeyValuePair<string, string>>();
                // keysValues.Add(new KeyValuePair<string, string>("client_id", "1bfe66b6-7814-4d29-8a7a-60c9f4fa55ed"));
                // keysValues.Add(new KeyValuePair<string, string>("resource", _ADDRESS));
                //keysValues.Add(new KeyValuePair<string, string>("client_secret", "W~g9Q6-1V8-hnU-FIGZAXePO4ez~-347AV"));
                keysValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                // keysValues.Add(new KeyValuePair<string, string>("Authorization", "Basic NGQwYTQwYWMtOGRkNy00YTFhLTg2MzktNzY5ZGExYjdmZmNiOnBUTFZiWGh6alV0WWgycHBTcFl6bjVyZUVScXB4U2E5"));

                request.Content = new FormUrlEncodedContent(keysValues);
                //request.Headers.Add("Authorization", "Basic NGQwYTQwYWMtOGRkNy00YTFhLTg2MzktNzY5ZGExYjdmZmNiOnBUTFZiWGh6alV0WWgycHBTcFl6bjVyZUVScXB4U2E5");// uat
                request.Headers.Add("Authorization", "Basic " + _AUTHORIZATION);

                var bearerResult = await client.SendAsync(request);
                var bearerData = await bearerResult.Content.ReadAsStringAsync();
                bearerToken = JObject.Parse(bearerData)["access_token"].ToString();
            }

            using (clientContract = new HttpClient())
            {
                clientContract.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
                //clientContract.DefaultRequestHeaders.Add("apikey", "ymgtoKPybgdursz3U0n7NXB4uzIjdIyV");// UAT
                clientContract.DefaultRequestHeaders.Add("apikey", _API_KEY);

                var httpResponse = await clientContract.GetAsync(_BASEURL + skip);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Cannot retrieve tasks");
                }

                var content = await httpResponse.Content.ReadAsStringAsync();
                var contractEntity = JsonConvert.DeserializeObject<ODataResponse<ContractDetail>>(content);

                return contractEntity.Value;
            }

        }
    }
}

//title, owner, contractId, contractStatus, supplier, effectiveDate, expirationDate, creationDate, contractAmount, version, templateId, 
//    businessSystemId, amendmentType, amendmentReason, origin, hierarchicalType, parentAgreement, expirationTermType, commodity, region


public class ContractImport
{
    List<ContractDetail> Value { get; set; }
}

public class ContractDetail
{
    public string Title { get; set; }
    public string Description { get; set; }
	public List<Commodities> Commodities { get; set; }
    public List<Regions> Regions { get; set; }
    public List<Departments> Departments { get; set; }
    public Owner Owner { get; set; }
    public string ContractId { get; set; }
    public string ContractStatus { get; set; }
    public Supplier Supplier { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? AgreementDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? CreationDate { get; set; }
	public ContractAmount ContractAmount { get; set; }
    public string Version { get; set; }
    public string TemplateId { get; set; }
    public BusinessSystem BusinessSystem { get; set; }
    public string AmendmentType { get; set; }
    public string AmendmentReason { get; set; }
    public int Origin { get; set; }
    public string HierarchicalType { get; set; }
    public string ExpirationTermType { get; set; }
	public string RelatedId { get; set; }
    public int MaximumNumberOfRenewals { get; set; }
	public int AutoRenewalInterval { get; set; }
	public bool IsTestProject { get; set; }
    public List<CustomFields> CustomFields { get; set; }
}

public class Commodities
{
    public string UniqueName { get; set; }
    public string Domain { get; set; }
    public string Name { get; set; }
}

public class Regions
{
    public string UniqueName { get; set; }
    public string Name { get; set; }
}

public class Departments
{
    public string UniqueName { get; set; }
    public string Name { get; set; }
}

public class CustomFields
{
    public string FieldId { get; set; }
    public string FieldName { get; set; }
	public string BigDecimalValue { get; set; }
	public List<string> TextValue { get; set; }
}

public class Owner
{
    public string UniqueName { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
	public string Organization { get; set; }
	public string OrgANId { get; set; }
	public string OrgName { get; set; }
	public string TimeZoneID { get; set; }
}

public class Supplier
{
    public string Name { get; set; }
    public string SystemID { get; set; }
	public Address Address { get; set; }
}

public class Address
{
    public string Name { get; set; }
    public string UniqueName { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
	public List<string> Lines { get; set; }
}

public class ContractAmount
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}

public class BusinessSystem
{
    public string BusinessSystemId { get; set; }
    public string BusinessSystemName { get; set; }
}

internal class ODataResponse<T>
{
    public List<T> Value { get; set; }
}

