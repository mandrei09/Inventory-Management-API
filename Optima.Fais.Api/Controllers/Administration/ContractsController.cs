using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Helpers;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/contracts")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class ContractsController : GenericApiController<Model.Contract, Dto.Contract>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;

        private readonly IEmailSender _emailSender;

        public ContractsController(ApplicationDbContext context,
            IContractsRepository itemsRepository, IMapper mapper, IWebHostEnvironment hostingEnvironment, UserManager<Model.ApplicationUser> userManager, IEmailSender emailSender)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._emailSender = emailSender;
        }
        
        [HttpGet]
        [Route("", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            ContractFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ContractFilter>(jsonFilter) : new ContractFilter();

            if (filter != null)
			{
                assetFilter.Filter = filter;
            }


            var items = (_itemsRepository as IContractsRepository)
                .GetContract(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.ContractDetail>, List<Dto.Contract>>(items);

            var result = new ContractPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("detailui", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetailUIs(int page, int pageSize, string sortColumn, string sortDirection,
           string includes, string jsonFilter, string filter)
        {
            AssetDepTotal depTotal = null;
            AssetCategoryTotal catTotal = null;
            ContractFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            //var countContracts = _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToList();
            //var countOffer = _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();


            includes = includes + ",Contract.Partner,Contract.ContractAmount.RateRon,Contract.ContractAmount.Rate.Uom";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ContractFilter>(jsonFilter) : new ContractFilter();


            if (filter != null)
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IContractsRepository)
                .GetContractUI(assetFilter, includes, sorting, paging, out depTotal, out catTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.ContractDetail>, List<Dto.ContractUI>>(items);

            var result = new ContractUIPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal, catTotal);

            return Ok(result);
        }


        [HttpPost("detail")]
        public async Task<IActionResult> PostDetail([FromBody] ContractSave offer)
        {
            var userName = HttpContext.User.Identity.Name;

            if (userName != null && userName != "")
			{
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                if (user != null)
				{
                    offer.UserId = user.Id;

                    int id = (_itemsRepository as IContractsRepository).CreateOrUpdateContract(offer);

                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }


            }
			else
			{
                return BadRequest();
			}

           
        }

        [HttpPut("detail")]
        public virtual async Task<IActionResult> PutDetail([FromBody] ContractSave offer)
        {
            var userName = HttpContext.User.Identity.Name;

            if (userName != null && userName != "")
            {
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                if (user != null)
                {
                    offer.UserId = user.Id;

                    int id = (_itemsRepository as IContractsRepository).CreateOrUpdateContract(offer);
                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return BadRequest();
            }

            
        }

        [HttpGet("detail/{id:int}")]
        public virtual IActionResult GetDetail(int id, string includes)
        {
            var asset = (_itemsRepository as IContractsRepository).GetDetailsById(id, includes);
            var result = _mapper.Map<Dto.Contract>(asset);

            return Ok(result);
        }

      

        //[HttpGet("getAllCurrency")]
        //public async Task<BNRCurrency.getallResponseGetallResult> GetBnrAllCurrency()
        //{
        //    BNRCurrency.CursBCESoapClient cursBCESoapClient = new(BNRCurrency.CursBCESoapClient.EndpointConfiguration.CursBCESoap);

        //    var currency = await cursBCESoapClient.getallAsync(DateTime.Now);

        //    return currency;
        //}


        //[HttpGet("getAllCurrency")]
        //// [Consumes("application/xml")]
        //public IActionResult ReturnXmlDocument()
        //{
        //    WebClient webClient = new WebClient();
        //     string url = "http://www.bnr.ro/files/xml/years/nbrfxrates" + "2021" + ".xml";
        //    //string url = "http://www.bnro.ro/nbrfxrates10days.xml";
        //    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        //    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

        //    XDocument systemXml = XDocument.Load(response.GetResponseStream());

        //    XElement cube = (from cubeElement in systemXml.Elements("Cube")
        //                     where cubeElement.Attribute("date").ToString().Equals("2021-11-05")
        //                     select cubeElement).SingleOrDefault();

        //    XElement rate = (from rateElement in cube.Elements("Rate")
        //                     where rateElement.Attribute("currency").ToString().Equals("EUR")
        //                     select rateElement).SingleOrDefault();

        //    return Ok();

        //    //         var doc = new XmlDocument();

        //    //FileStream xmlStream = new FileStream("OpenAPIs/nbrfxrates.xml", FileMode.Open);
        //    //doc.Load(xmlStream);
        //    //         return Ok(doc.DocumentElement.OuterXml.ToString());
        //}


        //private void get10DaysData()
        //{
        //    XmlParser last10XML = new XmlParser("http://www.bnro.ro/nbrfxrates10days.xml", "Cube", "date", "Rate", "currency");
        //    Dictionary<string, Dictionary<string, decimal>> last10Rates = last10XML.parse();

        //    Dictionary<string, string> d = new Dictionary<string, string>();

        //    foreach (var date in last10Rates)
        //    {
        //        d["date"] = date.Key;

        //        foreach (var currency in date.Value)
        //        {
        //            d["rate"] = currency.Value.ToString();
        //            db.insert(currency.Key, d, true);
        //        }
        //    }
        //}

        [HttpGet("getContractByID/{contractID}")]
        public async Task<List<ContractDetail>> GetContractsAsync(string contractID)
        {
            HttpClient clientContract = null;
            //var BaseUrl = @"https://eu.openapi.ariba.com/api/retrieve-contract-workspaces/v1/prod/contractWorkspaces/" + contractID + @"?realm=dante-T&user=sorina.manzicu@emag.ro&passwordAdapter=PasswordAdapter1&$count=true";
            var BaseUrl = @"https://eu.openapi.ariba.com/api/retrieve-contract-workspaces/v1/prod/contractWorkspaces/" + contractID + @"?realm=dante&user=daniela.niculescu@emag.ro&passwordAdapter=PasswordAdapter1&$count=true";

            var bearerToken = "";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api-eu.ariba.com/v2/oauth/token");

                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;

                var keysValues = new List<KeyValuePair<string, string>>();
                // keysValues.Add(new KeyValuePair<string, string>("client_id", "1bfe66b6-7814-4d29-8a7a-60c9f4fa55ed"));
                // keysValues.Add(new KeyValuePair<string, string>("resource", "https://api-eu.ariba.com/v2/oauth/token"));
                //keysValues.Add(new KeyValuePair<string, string>("client_secret", "W~g9Q6-1V8-hnU-FIGZAXePO4ez~-347AV"));
                keysValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                // keysValues.Add(new KeyValuePair<string, string>("Authorization", "Basic NGQwYTQwYWMtOGRkNy00YTFhLTg2MzktNzY5ZGExYjdmZmNiOnBUTFZiWGh6alV0WWgycHBTcFl6bjVyZUVScXB4U2E5"));

                request.Content = new FormUrlEncodedContent(keysValues);
                // request.Headers.Add("Authorization", "Basic NGQwYTQwYWMtOGRkNy00YTFhLTg2MzktNzY5ZGExYjdmZmNiOnBUTFZiWGh6alV0WWgycHBTcFl6bjVyZUVScXB4U2E5");
                request.Headers.Add("Authorization", "Basic ODdiYWY5NmQtNDE5MS00ZWQ2LWJiY2MtMzk0NzdiY2Y3ZmY1OktaVWM5bWJvQnVTY3FQU3VTYzVZYXpDdnowb3AzMFAz");

                var bearerResult = await client.SendAsync(request);
                var bearerData = await bearerResult.Content.ReadAsStringAsync();
                bearerToken = JObject.Parse(bearerData)["access_token"].ToString();
            }

            using (clientContract = new HttpClient())
            {
                clientContract.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
                //clientContract.DefaultRequestHeaders.Add("apikey", "ymgtoKPybgdursz3U0n7NXB4uzIjdIyV");
                clientContract.DefaultRequestHeaders.Add("apikey", "DLMXNmVpqHf7aAxgq91v8Ztj0pQRW1VS");

                var httpResponse = await clientContract.GetAsync(BaseUrl);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Cannot retrieve tasks");
                }

                var content = await httpResponse.Content.ReadAsStringAsync();
                var contractEntity = JsonConvert.DeserializeObject<ODataResponse<ContractDetail>>(content);

                Model.Address address = null;
                Model.Owner owner = null;
                Model.BusinessSystem businessSystem = null;
                Model.Partner partner = null;
                Model.PartnerLocation partnerLocation = null;
                Model.ContractAmount contractAmount = null;
                Model.Contract contract = null;

                for (int c = 0; c < contractEntity.Value.Count; c++)
                {

                    if (contractEntity.Value[c].Owner != null)
                    {
                        owner = _context.Set<Model.Owner>().Where(com => com.UniqueName == contractEntity.Value[c].Owner.UniqueName).FirstOrDefault();

                        if (owner == null)
                        {
                            owner = new Model.Owner()
                            {
                                UniqueName = contractEntity.Value[c].Owner.UniqueName,
                                FullName = contractEntity.Value[c].Owner.Name,
                                Email = contractEntity.Value[c].Owner.EmailAddress,
                                Organization = contractEntity.Value[c].Owner.Organization,
                                OrgANId = contractEntity.Value[c].Owner.OrgANId,
                                OrgName = contractEntity.Value[c].Owner.OrgName,
                                FirstName = string.Empty,
                                LastName = string.Empty
                            };

                            _context.Add(owner);
						}
						else
						{
                            owner.FullName = contractEntity.Value[c].Owner.Name;
                            owner.Email = contractEntity.Value[c].Owner.EmailAddress;
                            owner.Organization = contractEntity.Value[c].Owner.Organization;
                            owner.OrgANId = contractEntity.Value[c].Owner.OrgANId;
                            owner.OrgName = contractEntity.Value[c].Owner.OrgName;

                            _context.Update(owner);
                        }
                    }

                    if (contractEntity.Value[c].BusinessSystem != null)
                    {
                        businessSystem = _context.Set<Model.BusinessSystem>().Where(com => com.Code == contractEntity.Value[c].BusinessSystem.BusinessSystemId).FirstOrDefault();

                        if (businessSystem == null)
                        {
                            businessSystem = new Model.BusinessSystem()
                            {
                                Code = contractEntity.Value[c].BusinessSystem.BusinessSystemId,
                                Name = contractEntity.Value[c].BusinessSystem.BusinessSystemName,
                                IsDeleted = false
                            };

                            _context.Add(businessSystem);
                        }
                    }

                    if (contractEntity.Value[c].Supplier != null)
                    {

                        if (contractEntity.Value[c].Supplier.Address != null)
                        {
                            Model.Company company = _context.Set<Model.Company>().Where(com => com.Code == contractEntity.Value[c].Supplier.Address.Country + contractEntity.Value[c].Supplier.Address.State).FirstOrDefault();

                            if (company == null)
                            {
                                company = new Model.Company()
                                {
                                    Code = contractEntity.Value[c].Supplier.Address.Country + contractEntity.Value[c].Supplier.Address.State,
                                    Name = contractEntity.Value[c].Supplier.Address.Country + contractEntity.Value[c].Supplier.Address.State,
                                    IsDeleted = false
                                };

                                _context.Add(company);
                            }

                            Model.Country country = _context.Set<Model.Country>().Where(com => com.Code == contractEntity.Value[c].Supplier.Address.Country).FirstOrDefault();

                            if (country == null)
                            {
                                country = new Model.Country()
                                {
                                    Code = contractEntity.Value[c].Supplier.Address.Country,
                                    Name = contractEntity.Value[c].Supplier.Address.Country,
                                    IsDeleted = false
                                };

                                _context.Add(country);
                            }


                            Model.County county = _context.Set<Model.County>().Include(county => county.Country).Where(com => com.Name == contractEntity.Value[c].Supplier.Address.City && com.Country.Code == contractEntity.Value[c].Supplier.Address.Country).FirstOrDefault();

                            if (county == null)
                            {
                                county = new Model.County()
                                {
                                    Code = contractEntity.Value[c].Supplier.Address.City,
                                    Name = contractEntity.Value[c].Supplier.Address.City,
                                    Country = country,
                                    IsDeleted = false
                                };

                                _context.Add(county);
                            }


                            Model.City city = _context.Set<Model.City>().Include(c => c.County).Where(com => com.Name == contractEntity.Value[c].Supplier.Address.City && com.County.Name == contractEntity.Value[c].Supplier.Address.City).FirstOrDefault();

                            if (city == null)
                            {
                                city = new Model.City()
                                {
                                    Code = contractEntity.Value[c].Supplier.Address.Country,
                                    Name = contractEntity.Value[c].Supplier.Address.Country,
                                    County = county,
                                    IsDeleted = false
                                };

                                _context.Add(city);
                            }

                            address = _context.Set<Model.Address>().Where(com => com.UniqueName == contractEntity.Value[c].Supplier.Address.UniqueName).FirstOrDefault();

                            if (address == null)
                            {
                                address = new Model.Address()
                                {
                                    UniqueName = contractEntity.Value[c].Supplier.Address.UniqueName,
                                    Name = contractEntity.Value[c].Supplier.Address.Name,
                                    Phone = contractEntity.Value[c].Supplier.Address.Phone,
                                    Fax = contractEntity.Value[c].Supplier.Address.Fax,
                                    AddressDetail = contractEntity.Value[c].Supplier.Address.Lines != null ? contractEntity.Value[c].Supplier.Address.Lines[0] : "",
                                    PostalCode = contractEntity.Value[c].Supplier.Address.PostalCode,
                                    City = city,
                                    Company = company,
                                    IsDeleted = false,
                                    Code = string.Empty
                                };

                                _context.Add(address);
                            }
                        }


                        partnerLocation = _context.Set<Model.PartnerLocation>().Where(com => com.Cui == contractEntity.Value[c].Supplier.SystemID).FirstOrDefault();

                        if (partnerLocation == null)
                        {
                            partnerLocation = new Model.PartnerLocation()
                            {
                                Cui = contractEntity.Value[c].Supplier.SystemID,
                                Denumire = contractEntity.Value[c].Supplier.Name,
                                //RegistryNumber = contractEntity.Value[c].Supplier.SystemID,
                                //FiscalCode = contractEntity.Value[c].Supplier.SystemID,
                                //AddressDetail = address,
                            };

                            _context.Add(partnerLocation);
                        }

                        partner = _context.Set<Model.Partner>().Where(com => com.ErpCode == contractEntity.Value[c].Supplier.SystemID).FirstOrDefault();

                        if (partner == null)
                        {
                            partner = new Model.Partner()
                            {
                                ErpCode = contractEntity.Value[c].Supplier.SystemID,
                                Name = contractEntity.Value[c].Supplier.Name,
                                RegistryNumber = contractEntity.Value[c].Supplier.SystemID,
                                FiscalCode = contractEntity.Value[c].Supplier.SystemID,
                                AddressDetail = address,
                                PartnerLocation = partnerLocation
                            };

                            _context.Add(partner);
                        }
                    }

      //              if (contractEntity.Value[c].ContractAmount != null)
      //              {
      //                  Model.Uom uomRon = _context.Set<Model.Uom>().Where(a => a.Code == "RON").FirstOrDefault();
      //                  Model.Rate rateRon = _context.Set<Model.Rate>().Include(u => u.Uom).Where(com => com.UomId == uomRon.Id && com.IsLast == true).FirstOrDefault();

      //                  Model.Uom uom = _context.Set<Model.Uom>().Where(com => com.Code == contractEntity.Value[c].ContractAmount.Currency).FirstOrDefault();
      //                  Model.Rate rate = _context.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == contractEntity.Value[c].ContractAmount.Currency && com.IsLast == true).FirstOrDefault();

      //                  if (uom == null)
      //                  {
      //                      uom = new Model.Uom()
      //                      {
      //                          Code = contractEntity.Value[c].ContractAmount.Currency,
      //                          Name = contractEntity.Value[c].ContractAmount.Currency,
      //                          IsDeleted = false
      //                      };

      //                      _context.Add(uom);
      //                  }


      //                  contractAmount = _context.Set<Model.ContractAmount>().Include(u => u.Uom).Include(r => r.Rate).Where(com => com.Amount == contractEntity.Value[c].ContractAmount.Amount && com.Uom.Code == contractEntity.Value[c].ContractAmount.Currency).FirstOrDefault();

      //                  if (contractAmount == null)
      //                  {
      //                      contractAmount = new Model.ContractAmount()
      //                      {
      //                          Amount = contractEntity.Value[c].ContractAmount.Amount,
      //                          Uom = uom,
      //                          Rate = rate,
      //                          RateRon = rateRon,
      //                          IsDeleted = false,
      //                          Code = string.Empty,
      //                          Name = string.Empty
      //                      };

      //                      _context.Add(contractAmount);
						//}
						//else
						//{
      //                      contractAmount.Amount = contractEntity.Value[c].ContractAmount.Amount;
      //                      contractAmount.Uom = uom;
      //                      contractAmount.Rate = rate;
      //                      contractAmount.RateRon = rateRon;
      //                      contractAmount.IsDeleted = false;

      //                      _context.Update(contractAmount);
      //                  }
      //              }

                    Model.AppState appState = _context.Set<Model.AppState>().Where(com => com.Name == contractEntity.Value[c].ContractStatus).FirstOrDefault();

                    if (appState == null)
                    {
                        appState = new Model.AppState()
                        {
                            Code = contractEntity.Value[c].ContractStatus,
                            Name = contractEntity.Value[c].ContractStatus,
                            IsDeleted = false
                        };

                        _context.Add(appState);
                    }

                    contract = _context.Set<Model.Contract>().Where(com => com.ContractId == contractEntity.Value[c].ContractId).FirstOrDefault();

                    if (contract == null)
                    {
                        contract = new Model.Contract()
                        {
                            ContractId = contractEntity.Value[c].ContractId,
                            Title = contractEntity.Value[c].Title,
                            Name = contractEntity.Value[c].Description,
                            AppState = appState,
                            EffectiveDate = contractEntity.Value[c].EffectiveDate,
                            AgreementDate = contractEntity.Value[c].AgreementDate,
                            ExpirationDate = contractEntity.Value[c].ExpirationDate,
                            CreationDate = contractEntity.Value[c].CreationDate,
                            Version = contractEntity.Value[c].Version,
                            TemplateId = contractEntity.Value[c].TemplateId,
                            AmendmentType = contractEntity.Value[c].AmendmentType,
                            AmendmentReason = contractEntity.Value[c].AmendmentReason,
                            Origin = contractEntity.Value[c].Origin,
                            HierarchicalType = contractEntity.Value[c].HierarchicalType,
                            ExpirationTermType = contractEntity.Value[c].ExpirationTermType,
                            RelatedId = contractEntity.Value[c].RelatedId,
                            MaximumNumberOfRenewals = contractEntity.Value[c].MaximumNumberOfRenewals,
                            AutoRenewalInterval = contractEntity.Value[c].AutoRenewalInterval,
                            IsTestProject = contractEntity.Value[c].IsTestProject,
                            Owner = owner,
                            Partner = partner,
                            BusinessSystem = businessSystem,
                            ContractAmount = contractAmount,
                            Code = string.Empty
                        };

                        _context.Add(contract);
                    }
                    else
                    {

                        contractAmount = _context.Set<Model.ContractAmount>().Where(com => com.Id == contract.ContractAmountId).FirstOrDefault();

                        if (contractEntity.Value[c].ContractAmount != null)
                        {
                            Model.Uom uomRon = _context.Set<Model.Uom>().Where(a => a.Code == "RON").FirstOrDefault();
                            Model.Rate rateRon = _context.Set<Model.Rate>().Include(u => u.Uom).Where(com => com.UomId == uomRon.Id && com.IsLast == true).FirstOrDefault();

                            Model.Uom uom = _context.Set<Model.Uom>().Where(com => com.Code == contractEntity.Value[c].ContractAmount.Currency).FirstOrDefault();
                            Model.Rate rate = _context.Set<Model.Rate>().Include(r => r.Uom).Where(com => com.Uom.Code == contractEntity.Value[c].ContractAmount.Currency && com.IsLast == true).FirstOrDefault();

                            if (uom == null)
                            {
                                uom = new Model.Uom()
                                {
                                    Code = contractEntity.Value[c].ContractAmount.Currency,
                                    Name = contractEntity.Value[c].ContractAmount.Currency,
                                    IsDeleted = false
                                };

                                _context.Add(uom);
                            }
                            else
                            {
                                uom.IsDeleted = false;
                                _context.Update(uom);
                            }

                            if (contractAmount == null)
                            {
                                contractAmount = new Model.ContractAmount()
                                {
                                    Amount = contractEntity.Value[c].ContractAmount.Amount,
                                    Uom = uom,
                                    Rate = rate,
                                    RateRon = rateRon,
                                    IsDeleted = false,
                                    Code = string.Empty,
                                    Name = string.Empty,
                                    AmountRem = contractEntity.Value[c].ContractAmount.Amount
                                };

                                _context.Add(contractAmount);
                            }
                            else
                            {
                                contractAmount.Amount = contractEntity.Value[c].ContractAmount.Amount;
                                contractAmount.Uom = uom;
                                contractAmount.Rate = rate;
                                contractAmount.RateRon = rateRon;
                                contractAmount.IsDeleted = false;

                                _context.Update(contractAmount);

                            }

                        }


                        contract.ModifiedAt = DateTime.Now;
                        contract.Title = contractEntity.Value[c].Title;
                        contract.Name = contractEntity.Value[c].Description;
                        contract.AppState = appState;
                        contract.EffectiveDate = contractEntity.Value[c].EffectiveDate;
                        contract.AgreementDate = contractEntity.Value[c].AgreementDate;
                        contract.ExpirationDate = contractEntity.Value[c].ExpirationDate;
                        contract.CreationDate = contractEntity.Value[c].CreationDate;
                        contract.Version = contractEntity.Value[c].Version;
                        contract.TemplateId = contractEntity.Value[c].TemplateId;
                        contract.AmendmentType = contractEntity.Value[c].AmendmentType;
                        contract.AmendmentReason = contractEntity.Value[c].AmendmentReason;
                        contract.Origin = contractEntity.Value[c].Origin;
                        contract.HierarchicalType = contractEntity.Value[c].HierarchicalType;
                        contract.ExpirationTermType = contractEntity.Value[c].ExpirationTermType;
                        contract.RelatedId = contractEntity.Value[c].RelatedId;
                        contract.MaximumNumberOfRenewals = contractEntity.Value[c].MaximumNumberOfRenewals;
                        contract.AutoRenewalInterval = contractEntity.Value[c].AutoRenewalInterval;
                        contract.IsTestProject = contractEntity.Value[c].IsTestProject;
                        contract.Owner = owner;
                        contract.Partner = partner;
                        contract.BusinessSystem = businessSystem;
                        contract.ContractAmount = contractAmount;

                        _context.Update(contract);

                    }


                    if (contractEntity.Value[c].Commodities.Count > 0)
                    {
                        for (int m = 0; m < contractEntity.Value[c].Commodities.Count; m++)
                        {
                            Model.Commodity commodity = _context.Set<Model.Commodity>().Where(com => com.UniqueName == contractEntity.Value[c].Commodities[m].UniqueName).FirstOrDefault();

                            if (commodity == null)
                            {
                                commodity = new Model.Commodity()
                                {
                                    Code = contractEntity.Value[c].Commodities[m].UniqueName,
                                    UniqueName = contractEntity.Value[c].Commodities[m].UniqueName,
                                    Domain = contractEntity.Value[c].Commodities[m].Domain,
                                    Name = contractEntity.Value[c].Commodities[m].Name,
                                    Contract = contract,
                                    IsDeleted = false
                                };

                                _context.Add(commodity);
                            }
                        }
                    }

                    if (contractEntity.Value[c].Regions.Count > 0)
                    {
                        for (int m = 0; m < contractEntity.Value[c].Regions.Count; m++)
                        {
                            Model.ContractRegion contractRegion = _context.Set<Model.ContractRegion>().Where(com => com.UniqueName == contractEntity.Value[c].Regions[m].UniqueName).FirstOrDefault();

                            if (contractRegion == null)
                            {
                                contractRegion = new Model.ContractRegion()
                                {
                                    Code = contractEntity.Value[c].Regions[m].UniqueName,
                                    UniqueName = contractEntity.Value[c].Regions[m].UniqueName,
                                    Name = contractEntity.Value[c].Regions[m].Name,
                                    IsDeleted = false,
                                    Contract = contract,
                                };

                                _context.Add(contractRegion);
                            }
                        }
                    }

                    if (contractEntity.Value[c].Departments.Count > 0)
                    {
                        for (int m = 0; m < contractEntity.Value[c].Departments.Count; m++)
                        {
                            Model.ContractDivision contractDivision = _context.Set<Model.ContractDivision>().Where(com => com.UniqueName == contractEntity.Value[c].Departments[m].UniqueName).FirstOrDefault();

                            if (contractDivision == null)
                            {
                                contractDivision = new Model.ContractDivision()
                                {
                                    Code = contractEntity.Value[c].Departments[m].UniqueName,
                                    UniqueName = contractEntity.Value[c].Departments[m].UniqueName,
                                    Name = contractEntity.Value[c].Departments[m].Name,
                                    IsDeleted = false,
                                    Contract = contract,
                                };

                                _context.Add(contractDivision);
                            }
                        }
                    }

                    _context.SaveChanges();
                }

                return contractEntity.Value;
            }

        }

		[HttpGet("export")]
		public IActionResult Export(string filter, string administrationIds, string admCenterIds, string jsonFilter)
		{
			List<int> aIds = null;
			List<int> admIds = null;
			List<Model.ContractDetail> contracts = null;
			AssetDepTotal depTotal = null;
			AssetCategoryTotal catTotal = null;
			ContractFilter assetFilter = null;

			if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			if ((administrationIds != null) && (administrationIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(administrationIds).ToList().Select(int.Parse).ToList();


			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<ContractFilter>(jsonFilter) : new ContractFilter();
			using (ExcelPackage package = new ExcelPackage())
			{
				contracts = (_itemsRepository as IContractsRepository)
			   .GetContract(assetFilter, "Contract.AppState,Contract.Owner,Contract.Partner,Contract.BusinessSystem,Contract.ContractAmount.Rate,Contract.ContractAmount.Uom,", null, null, out depTotal, out catTotal).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("contracte");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Contract";
				worksheet.Cells[1, 2].Value = "Status";
				worksheet.Cells[1, 3].Value = "Tip";
				worksheet.Cells[1, 4].Value = "ContractID";
				worksheet.Cells[1, 5].Value = "Title";
				worksheet.Cells[1, 6].Value = "EffectiveDate";
				worksheet.Cells[1, 7].Value = "AgreementDate";
				worksheet.Cells[1, 8].Value = "ExpirationDate";
				worksheet.Cells[1, 9].Value = "CreationDate";
				worksheet.Cells[1, 10].Value = "Version";
				worksheet.Cells[1, 11].Value = "TemplateId";
				worksheet.Cells[1, 12].Value = "AmendmentType";
				worksheet.Cells[1, 13].Value = "AmendmentReason";
				worksheet.Cells[1, 14].Value = "ExpirationTermType";
				worksheet.Cells[1, 15].Value = "Owner";
				worksheet.Cells[1, 16].Value = "ID supplier";
				worksheet.Cells[1, 17].Value = "Supplier";
				worksheet.Cells[1, 18].Value = "BusinessSystem";
				worksheet.Cells[1, 19].Value = "Amount";
				worksheet.Cells[1, 20].Value = "Amount RON";
                worksheet.Cells[1, 21].Value = "Amount Rem";
                worksheet.Cells[1, 22].Value = "Amount Rem RON";
                worksheet.Cells[1, 23].Value = "Amount Used";
                worksheet.Cells[1, 24].Value = "Amount RON Used";
                worksheet.Cells[1, 25].Value = "Rata";
                worksheet.Cells[1, 26].Value = "Multiplicator";
                worksheet.Cells[1, 27].Value = "Currency";
                worksheet.Cells[1, 28].Value = "Data Rata";

                int recordIndex = 2, columnsNumber = 28;
				foreach (var item in contracts)
				{
                    //Values
                    worksheet.Cells[recordIndex, 1].Value = item.Contract.Code;
                    worksheet.Cells[recordIndex, 2].Value = item.Contract.AppState != null ? item.Contract.AppState.Name : "";
                    worksheet.Cells[recordIndex, 3].Value = item.Contract.Code;
                    worksheet.Cells[recordIndex, 4].Value = item.Contract.ContractId;
                    worksheet.Cells[recordIndex, 5].Value = item.Contract.Title;
                    worksheet.Cells[recordIndex, 6].Value = item.Contract.EffectiveDate;
					worksheet.Cells[recordIndex, 7].Value = item.Contract.AgreementDate;
					worksheet.Cells[recordIndex, 8].Value = item.Contract.ExpirationDate;
					worksheet.Cells[recordIndex, 9].Value = item.Contract.CreationDate;
					worksheet.Cells[recordIndex, 10].Value = item.Contract.Version;
					worksheet.Cells[recordIndex, 11].Value = item.Contract.TemplateId;
					worksheet.Cells[recordIndex, 12].Value = item.Contract.AmendmentType;
					worksheet.Cells[recordIndex, 13].Value = item.Contract.AmendmentReason;
					worksheet.Cells[recordIndex, 14].Value = item.Contract.ExpirationTermType;
					worksheet.Cells[recordIndex, 15].Value = item.Contract.Owner != null ? item.Contract.Owner.Email : "";
					worksheet.Cells[recordIndex, 16].Value = item.Contract.Partner != null ? item.Contract.Partner.RegistryNumber : "";
					worksheet.Cells[recordIndex, 17].Value = item.Contract.Partner != null ? item.Contract.Partner.Name : "";
					worksheet.Cells[recordIndex, 18].Value = item.Contract.BusinessSystem != null ? item.Contract.BusinessSystem.Name : "";
					worksheet.Cells[recordIndex, 19].Value = item.Contract.ContractAmount != null ? item.Contract.ContractAmount.Amount : "";
					worksheet.Cells[recordIndex, 20].Value = item.Contract.ContractAmount != null ? item.Contract.ContractAmount.AmountRon : "";
                    worksheet.Cells[recordIndex, 21].Value = item.Contract.ContractAmount != null ? item.Contract.ContractAmount.AmountRem : "";
                    worksheet.Cells[recordIndex, 22].Value = item.Contract.ContractAmount != null ? item.Contract.ContractAmount.AmountRonRem : "";
                    worksheet.Cells[recordIndex, 23].Value = item.Contract.ContractAmount != null ? item.Contract.ContractAmount.AmountUsed : "";
                    worksheet.Cells[recordIndex, 24].Value = item.Contract.ContractAmount != null ? item.Contract.ContractAmount.AmountRonUsed : "";
                    worksheet.Cells[recordIndex, 25].Value = item.Contract.ContractAmount.Rate != null ? item.Contract.ContractAmount.Rate.Value : "";
                    worksheet.Cells[recordIndex, 26].Value = item.Contract.ContractAmount.Rate != null ? item.Contract.ContractAmount.Rate.Multiplier : ""; ;
                    worksheet.Cells[recordIndex, 27].Value = item.Contract.ContractAmount != null && item.Contract.ContractAmount.Uom != null ? item.Contract.ContractAmount.Uom.Code : "";
                    worksheet.Cells[recordIndex, 28].Value = item.Contract.ContractAmount.Rate != null ? item.Contract.ContractAmount.Rate.Code : "";
                    //
                    
                    //Styles
                    worksheet.Cells[recordIndex, 6].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 7].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Cells[recordIndex, 19].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 21].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 28].Style.Numberformat.Format = "mm/dd/yyyy";
                    //

                    recordIndex++;
				}

                worksheet.Row(1).Height = 35.00;
                worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.View.FreezePanes(2, 1);

                for (int i = 1 ; i<=columnsNumber ; i++)
                {
                    worksheet.Column(i).AutoFit();
                }

				using (var cells = worksheet.Cells[1, 1, 1, columnsNumber])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "contracte.xlsx"
				};

				return result;

			}
		}
	}
}
