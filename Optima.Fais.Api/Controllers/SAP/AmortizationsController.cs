using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/amortizations")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AmortizationsController : GenericApiController<Model.AssetDepMDSync, Dto.AssetDepMDSync>
    {
		private readonly IConfiguration _configuration;
        private readonly string _BASEURL;
        private readonly string _TOKEN;

        public AmortizationsController(ApplicationDbContext context, IAmortizationsRepository itemsRepository, IMapper mapper, IConfiguration configuration)
            : base(context, itemsRepository, mapper)
        {
			_configuration = configuration;
            this._BASEURL = configuration.GetSection("SAP").GetValue<string>("URL");
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, GenericFilter colDefFilter, string includes)
        {
            List<Model.AssetDepMDSync> items = null;
            IEnumerable<Dto.AssetDepMDSync> itemsResult = null;

            includes = includes + "AccMonth,BudgetManager,Company";


            items = (_itemsRepository as IAmortizationsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.AssetDepMDSync>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAmortizationsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.AssetDepMDSync>(itemsResult, new Dto.PagingInfo()
                {
                    TotalItems = totalItems,
                    CurrentPage = page.Value,
                    PageSize = pageSize.Value
                });
                return Ok(pagedResult);
            }
            else
            {
                return Ok(itemsResult);
            }
        }

        [HttpGet("getDataFromSAP/{companyCode}/{year}/{month}")]
        public async Task<IActionResult> GetAmortization(string companyCode, string year, string month)
        {

            var result = await SyncAmortization(companyCode, year, month);
            var count = 0;
            GetAmortizationResult saveStockResult = null;

            if (result != "")
            {

                try
                {
					//using (var errorfile = System.IO.File.CreateText("stock-result-API-" + DateTime.Now.Ticks + ".txt"))
					//{
					//    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented));

					//};

					//var path = Path.Combine("upload", DateTime.UtcNow.ToString("yyyyMMdd"));

					//using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"\" + path + @"\responseSAPOK.txt"))
					//{
					//	string json = r.ReadToEnd();
     //                   saveStockResult = JsonConvert.DeserializeObject<GetAmortizationResult>(json);
     //               }

                    

                     saveStockResult = JsonConvert.DeserializeObject<GetAmortizationResult>(result);


                    //using (var errorfile = System.IO.File.CreateText("stock-result-API-" + DateTime.Now.Ticks + ".txt"))
                    //{
                    //	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(saveStockResult, Formatting.Indented));

                    //};

                    //using (var errorfile = System.IO.File.CreateText("stock-result-API-" + DateTime.Now.Ticks + ".txt"))
                    //{
                    //	errorfile.WriteLine(saveStockResult.Meta.ToString());
                    //	errorfile.WriteLine(saveStockResult.Data.ToString());

                    //};

                    if (saveStockResult.Data != null && saveStockResult.Data.Return_Code == "1")
                    {
                        Model.AssetDepMDSync assetDepMDSync = null;
                        Model.AccMonth accMonth = null;
                        Model.BudgetManager budgetManager = null;
                        Model.Company company = null;

                        accMonth = _context.Set<Model.AccMonth>().AsNoTracking().Where(a =>a.Month == int.Parse(month.Substring(1,2)) && a.Year == int.Parse(year)).SingleOrDefault();
                        budgetManager = _context.Set<Model.BudgetManager>().AsNoTracking().Where(a => a.Name == year).SingleOrDefault();
                        company = _context.Set<Model.Company>().AsNoTracking().Where(a => a.Code == companyCode).SingleOrDefault();

                        count = saveStockResult.Data.E_FA_AMORTIZATION.Count;

                        for (int c = 0; c < saveStockResult.Data.E_FA_AMORTIZATION.Count; c++)
                        {
                            assetDepMDSync = new Model.AssetDepMDSync()
                            {
                                AccSystemId = 3,
                                BudgetManagerId = budgetManager.Id,
                                AccMonthId = accMonth.Id,
                                CompanyId = company.Id,
                                InvNo = saveStockResult.Data.E_FA_AMORTIZATION[c].ANLN1.Trim(),
                                SubNumber = saveStockResult.Data.E_FA_AMORTIZATION[c].ANLN2.Trim(),
                                NDJAR = saveStockResult.Data.E_FA_AMORTIZATION[c].NDJAR.Trim(),
                                NDPER = saveStockResult.Data.E_FA_AMORTIZATION[c].NDPER.Trim(),
                                NDABJ = saveStockResult.Data.E_FA_AMORTIZATION[c].NDABJ.Trim(),
                                APC_FY_START = saveStockResult.Data.E_FA_AMORTIZATION[c].APC_FY_START,
                                DEP_FY_START = saveStockResult.Data.E_FA_AMORTIZATION[c].DEP_FY_START,
                                ACQUISITION = saveStockResult.Data.E_FA_AMORTIZATION[c].ACQUISITION,
                                DEP_FY = saveStockResult.Data.E_FA_AMORTIZATION[c].DEP_FY,
                                ANBTR_R = saveStockResult.Data.E_FA_AMORTIZATION[c].ANBTR_R,
                                DEPRET = saveStockResult.Data.E_FA_AMORTIZATION[c].DEPRET,
                                ANBTR_T = saveStockResult.Data.E_FA_AMORTIZATION[c].ANBTR_T,
                                DEPTRANS = saveStockResult.Data.E_FA_AMORTIZATION[c].DEPTRANS,
                                ANLN1 = saveStockResult.Data.E_FA_AMORTIZATION[c].ANLN1.Trim(),
                                ANLN2 = saveStockResult.Data.E_FA_AMORTIZATION[c].ANLN2.Trim()
                            };

                            _context.Add(assetDepMDSync);
                            _context.SaveChanges();


                        }
                    }
                    else
                    {
                        if (saveStockResult.Meta.Code == 400)
                        {
                            Model.ErrorType errorType = null;
                            Model.Error error = null;

                            errorType = await _context.Set<Model.ErrorType>().Where(e => e.Code == "CREATEASSET").SingleOrDefaultAsync();

                            if (errorType != null)
                            {
                                error = new Model.Error()
                                {
                                    AssetId = null,
                                    Code = "-",
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = null,
                                    ErrorTypeId = errorType.Id,
                                    IsDeleted = false,
                                    ModifiedAt = DateTime.Now,
                                    ModifiedBy = null,
                                    Name = saveStockResult.Errors[0].Detail,
                                    UserId = null

                                };

                                await _context.AddAsync(error);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    using (var errorfile = System.IO.File.CreateText("errorStock-" + DateTime.Now.Ticks + ".txt"))
                    {
                        errorfile.WriteLine(ex.StackTrace);
                        errorfile.WriteLine(ex.ToString());

                    };
                }



                return Ok(count);
            }
            else
            {
                //using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
                //{
                //    errorfile.WriteLine(result);
                //    errorfile.WriteLine(result);

                //};

                return Ok(count);
            }
        }

        public async Task<string> SyncAmortization(string companyCode, string year, string month)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.AmortizationData> oIList1 = new List<Dto.AmortizationData>
                {
                    new Dto.AmortizationData()
                        {
                            I_BUKRS = companyCode,
                            I_GJAHR = year,
                            I_AFBLPE = month
                         }
                };

                var postUser = new Dto.GetAmortization
                {
                    Sap_function = "ZFIF_FIXED_ASSET_AMORTIZATION",
                    Options = new Dto.AmortizationOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {

                    //using (var errorfile = System.IO.File.CreateText("contentJson" + DateTime.Now.Ticks + ".txt"))
                    //{
                    //	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

                    //};

                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }
        }

        [HttpPost("updateOptima")]
        public async Task<int> SyncOptima()
        {
            int countChanges = 0;
            List<Model.RecordCount> counts = new List<Model.RecordCount>();


            List<int> locations = await _context.Set<Model.Location>().Select(a => a.Id).ToListAsync();

            for (int i = 0; i < locations.Count; i++)
            {
                counts = await _context.Set<Model.RecordCount>().FromSql("UpdateAssetsAmortization").ToListAsync();
                countChanges += counts.Count;
            }

            return countChanges;
        }
    }
}
