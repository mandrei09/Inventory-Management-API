using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Utilities;
using PdfSharp;
using System.Drawing;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/partners")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class PartnersController : GenericApiController<Model.Partner, Dto.Partner>
    {
		private readonly IConfiguration _configuration;
        private readonly string _BASEURL;
        private readonly string _TOKEN;

        public PartnersController(ApplicationDbContext context, IPartnersRepository itemsRepository, IMapper mapper, IConfiguration configuration)
            : base(context, itemsRepository, mapper)
        {
            _configuration = configuration;
            this._BASEURL = configuration.GetSection("SAP").GetValue<string>("URL");
            this._TOKEN = configuration.GetSection("SAP").GetValue<string>("SAP-PROXY-AUTH-TOKEN");
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.Partner> items = null;
            IEnumerable<Dto.Partner> itemsResult = null;

            includes = "PartnerLocation";

            items = (_itemsRepository as IPartnersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, null, false).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.Partner>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IPartnersRepository).GetCountByFilters(filter, null, false);
                var pagedResult = new Dto.PagedResult<Dto.Partner>(itemsResult, new Dto.PagingInfo()
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


        [HttpGet]
        [Route("offer", Order = -1)]
        public virtual IActionResult GetOfferPartner(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes, bool showAll = false)
        {
            List<Model.Partner> items = null;
            IEnumerable<Dto.Partner> itemsResult = null;
            List<int> rIds = null;
            includes = "PartnerLocation";


            List<int> partnersIds = _context.Set<Model.Contract>().Where(a => a.AppStateId == 27 && a.Code == "Capex").Select(a => a.PartnerId.Value).ToList();
            partnersIds.Add(1927);

            items = (_itemsRepository as IPartnersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, partnersIds, showAll).ToList();

           
            itemsResult = items.Select(i => _mapper.Map<Dto.Partner>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IPartnersRepository).GetCountByFilters(filter, partnersIds, showAll);
                var pagedResult = new Dto.PagedResult<Dto.Partner>(itemsResult, new Dto.PagingInfo()
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

        //     [HttpPost("requestByCUI")]
        //     public async Task<bool> ProcessStock([FromBody] Dto.PartnerRequest partnerRequest)
        //     {
        //         Dto.PartnerResponse response = null;
        //         Dto.PartnerCorrelationResponse partnerCorrelation = null;
        //         Model.PartnerLocation partnerLocation = null;
        //         Model.Partner partner = null;

        //         string result = "";
        //         string res = "";
        //         if(partnerRequest != null && partnerRequest.PartnerId > 0)
        //{
        //             partner = _context.Set<Model.Partner>().Where(p => p.Id == partnerRequest.PartnerId).Single();
        //             partner.FiscalCode = "14735487";
        //             result = await GetID(partner.FiscalCode);
        //             if (result != "")
        //             {
        //                 try
        //                 {
        //                     response = JsonConvert.DeserializeObject<Dto.PartnerResponse>(result);
        //                     if (response.Cod == "200")
        //                     {
        //                         res = await GetCorrelationID(response.CorrelationId);
        //                         if (res != "")
        //                         {
        //                             try
        //                             {
        //                                 partnerCorrelation = JsonConvert.DeserializeObject<Dto.PartnerCorrelationResponse>(res);
        //                                 if (partnerCorrelation.Cod == "200")
        //                                 {
        //                                     partnerLocation = _context.Set<Model.PartnerLocation>().Where(p => p.Cui == partner.FiscalCode).SingleOrDefault();

        //                                     if(partnerLocation == null)
        //							{
        //                                         partnerLocation = new Model.PartnerLocation()
        //                                         {
        //                                             Cui = partnerCorrelation.Found[0].Cui,
        //                                             Data = partnerCorrelation.Found[0].Data,
        //                                             Denumire = partnerCorrelation.Found[0].Denumire,
        //                                             Adresa = partnerCorrelation.Found[0].Adresa,
        //                                             NrRegCom = partnerCorrelation.Found[0].NrRegCom,
        //                                             Telefon = partnerCorrelation.Found[0].Telefon,
        //                                             Fax = partnerCorrelation.Found[0].Fax,
        //                                             CodPostal = partnerCorrelation.Found[0].CodPostal,
        //                                             Act = partnerCorrelation.Found[0].Act,
        //                                             Stare_inregistrare = partnerCorrelation.Found[0].Stare_inregistrare,
        //                                             ScpTVA = partnerCorrelation.Found[0].ScpTVA,
        //                                             Data_inceput_ScpTVA = partnerCorrelation.Found[0].Data_inceput_ScpTVA,
        //                                             Data_sfarsit_ScpTVA = partnerCorrelation.Found[0].Data_sfarsit_ScpTVA,
        //                                             Data_anul_imp_ScpTVA = partnerCorrelation.Found[0].Data_anul_imp_ScpTVA,
        //                                             Mesaj_ScpTVA = partnerCorrelation.Found[0].Mesaj_ScpTVA,
        //                                             DataInceputTvaInc = partnerCorrelation.Found[0].DataInceputTvaInc,
        //                                             DataSfarsitTvaInc = partnerCorrelation.Found[0].DataSfarsitTvaInc,
        //                                             DataActualizareTvaInc = partnerCorrelation.Found[0].DataActualizareTvaInc,
        //                                             DataPublicareTvaInc = partnerCorrelation.Found[0].DataPublicareTvaInc,
        //                                             TipActTvaInc = partnerCorrelation.Found[0].TipActTvaInc,
        //                                             StatusTvaIncasare = partnerCorrelation.Found[0].StatusTvaIncasare,
        //                                             DataInactivare = partnerCorrelation.Found[0].DataInactivare,
        //                                             DataReactivare = partnerCorrelation.Found[0].DataReactivare,
        //                                             DataPublicare = partnerCorrelation.Found[0].DataPublicare,
        //                                             DataRadiere = partnerCorrelation.Found[0].DataRadiere,
        //                                             StatusInactivi = partnerCorrelation.Found[0].StatusInactivi,
        //                                             DataInceputSplitTVA = partnerCorrelation.Found[0].DataInceputSplitTVA,
        //                                             DataAnulareSplitTVA = partnerCorrelation.Found[0].DataAnulareSplitTVA,
        //                                             Iban = partnerCorrelation.Found[0].Iban,
        //                                             StatusRO_e_Factura = partnerCorrelation.Found[0].StatusRO_e_Factura,
        //                                             CreatedAt = DateTime.Now,
        //                                             ModifiedAt = DateTime.Now
        //                                         };

        //                                         partner.PartnerLocation = partnerLocation;
        //                                         _context.Add(partnerLocation);
        //                                         _context.Update(partner);
        //                                         _context.SaveChanges();
        //							}
        //							else
        //							{
        //                                         partnerLocation.Cui = partnerCorrelation.Found[0].Cui;
        //                                         partnerLocation.Data = partnerCorrelation.Found[0].Data;
        //                                         partnerLocation.Denumire = partnerCorrelation.Found[0].Denumire;
        //                                         partnerLocation.Adresa = partnerCorrelation.Found[0].Adresa;
        //                                         partnerLocation.NrRegCom = partnerCorrelation.Found[0].NrRegCom;
        //                                         partnerLocation.Telefon = partnerCorrelation.Found[0].Telefon;
        //                                         partnerLocation.Fax = partnerCorrelation.Found[0].Fax;
        //                                         partnerLocation.CodPostal = partnerCorrelation.Found[0].CodPostal;
        //                                         partnerLocation.Act = partnerCorrelation.Found[0].Act;
        //                                         partnerLocation.Stare_inregistrare = partnerCorrelation.Found[0].Stare_inregistrare;
        //                                         partnerLocation.ScpTVA = partnerCorrelation.Found[0].ScpTVA;
        //                                         partnerLocation.Data_inceput_ScpTVA = partnerCorrelation.Found[0].Data_inceput_ScpTVA;
        //                                         partnerLocation.Data_sfarsit_ScpTVA = partnerCorrelation.Found[0].Data_sfarsit_ScpTVA;
        //                                         partnerLocation.Data_anul_imp_ScpTVA = partnerCorrelation.Found[0].Data_anul_imp_ScpTVA;
        //                                         partnerLocation.Mesaj_ScpTVA = partnerCorrelation.Found[0].Mesaj_ScpTVA;
        //                                         partnerLocation.DataInceputTvaInc = partnerCorrelation.Found[0].DataInceputTvaInc;
        //                                         partnerLocation.DataSfarsitTvaInc = partnerCorrelation.Found[0].DataSfarsitTvaInc;
        //                                         partnerLocation.DataActualizareTvaInc = partnerCorrelation.Found[0].DataActualizareTvaInc;
        //                                         partnerLocation.DataPublicareTvaInc = partnerCorrelation.Found[0].DataPublicareTvaInc;
        //                                         partnerLocation.TipActTvaInc = partnerCorrelation.Found[0].TipActTvaInc;
        //                                         partnerLocation.StatusTvaIncasare = partnerCorrelation.Found[0].StatusTvaIncasare;
        //                                         partnerLocation.DataInactivare = partnerCorrelation.Found[0].DataInactivare;
        //                                         partnerLocation.DataReactivare = partnerCorrelation.Found[0].DataReactivare;
        //                                         partnerLocation.DataPublicare = partnerCorrelation.Found[0].DataPublicare;
        //                                         partnerLocation.DataRadiere = partnerCorrelation.Found[0].DataRadiere;
        //                                         partnerLocation.StatusInactivi = partnerCorrelation.Found[0].StatusInactivi;
        //                                         partnerLocation.DataInceputSplitTVA = partnerCorrelation.Found[0].DataInceputSplitTVA;
        //                                         partnerLocation.DataAnulareSplitTVA = partnerCorrelation.Found[0].DataAnulareSplitTVA;
        //                                         partnerLocation.Iban = partnerCorrelation.Found[0].Iban;
        //                                         partnerLocation.StatusRO_e_Factura = partnerCorrelation.Found[0].StatusRO_e_Factura;
        //                                         partnerLocation.ModifiedAt = DateTime.Now;

        //                                         partner.PartnerLocationId = partnerLocation.Id;
        //                                         _context.Update(partner);
        //                                         _context.Update(partnerLocation);
        //                                         _context.SaveChanges();

        //                                     }
        //                                     return true;
        //                                 }
        //                             }
        //                             catch (Exception)
        //                             {

        //                                 throw;
        //                             }
        //                         }
        //                         return true;
        //                     }
        //                     else
        //                     {
        //                         return false;
        //                     }
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     return false;
        //                 }
        //             }
        //             else
        //             {
        //                 return false;
        //             }
        //}
        //else
        //{
        //             return false;
        //}

        //     }

        [HttpPost("GetID")]
        public async Task<string> GetID(string cui)
        {
            HttpClient clientContract = null;

            var baseUrl = "https://webservicesp.anaf.ro/AsynchWebService/api/v6/ws/tva";
            string result = "";
            
            using (clientContract = new HttpClient())
            {
                IList<Dto.PartnerAPIRequest> request = new List<Dto.PartnerAPIRequest>
                {
                    new Dto.PartnerAPIRequest()
                        {
                            Cui = cui,
                            Data = DateTime.Now.ToString("yyyy-MM-dd")
                         }
                };
                JsonContent contentJson = JsonContent.Create(request);
                try
                {
                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                    return result;

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

                    return e.Message;
                }
            }
        }

        [HttpPost("GetCorrelationID")]
        public async Task<string> GetCorrelationID(string id)
        {
            HttpClient clientContract = null;

            var baseUrl = "https://webservicesp.anaf.ro/AsynchWebService/api/v6/ws/tva?id=" + id;
            string result = "";

            using (clientContract = new HttpClient())
            {
                try
                {
                    var httpResponse = await clientContract.GetAsync(baseUrl);
                    result = await httpResponse.Content.ReadAsStringAsync();
                    clientContract.Dispose();
                    return result;
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
                    return e.Message;
                }
            }
        }

        [HttpPost("requestByCUI")]
        public async Task<bool> ProcessStock([FromBody] Dto.PartnerRequest partnerRequest)
        {
            var result = await SyncDataFromSAP(partnerRequest.Cui);
            var count = 0;
            GetPartnersResult saveResult = null;

            if (result != "")
            {

                try
                {
					//using (var errorfile = System.IO.File.CreateText("partner-result-SAP-API-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented));

					//};

					//var path = Path.Combine("upload", DateTime.UtcNow.ToString("yyyyMMdd"));

					//using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"\" + path + @"\responseSAPOK.txt"))
					//{
					//	string json = r.ReadToEnd();
     //                   saveResult = JsonConvert.DeserializeObject<GetPartnersResult>(json);
					//}



					saveResult = JsonConvert.DeserializeObject<GetPartnersResult>(result);

                    if (saveResult.Data != null && saveResult.Data.Return_Code == "1")
                    {
                        Model.Partner partner = null;
                        Model.PartnerLocation partnerLocation = null;

                        string resultNew = "";
                        string res = "";
                        Dto.PartnerResponse response = null;
                        Dto.PartnerCorrelationResponse partnerCorrelation = null;

                        partner = _context.Set<Model.Partner>().Include(p => p.PartnerLocation).AsNoTracking().Where(a => a.RegistryNumber == partnerRequest.Cui && a.IsDeleted == false).SingleOrDefault();

                        count = saveResult.Data.E_OUTPUT.Count;

                        for (int c = 0; c < saveResult.Data.E_OUTPUT.Count; c++)
                        {
                            if(saveResult.Data.E_OUTPUT[c].STCEG != "")
							{
								if (saveResult.Data.E_OUTPUT[c].STCEG.StartsWith("RO"))
								{
                                    saveResult.Data.E_OUTPUT[c].STCEG = saveResult.Data.E_OUTPUT[c].STCEG.Replace("RO", "");

                                }
                                // partnerLocation = _context.Set<Model.PartnerLocation>().AsNoTracking().Where(a => a.Cui == saveResult.Data.E_OUTPUT[c].STCEG.Trim() && a.IsDeleted == false).SingleOrDefault();

                                //if (partnerLocation != null)
                                //{
                                //    partner.PartnerLocationId = partnerLocation.Id;
                                //    _context.Update(partner);
                                //    _context.SaveChanges();
                                //}
                                //else
                                //{
                                //    partnerLocation = new Model.PartnerLocation()
                                //    {
                                //        Cui = saveResult.Data.E_OUTPUT[c].STCEG,
                                //        CreatedAt = DateTime.Now,
                                //        ModifiedAt = DateTime.Now
                                //    };

                                //    partner.PartnerLocation = partnerLocation;
                                //    _context.Add(partnerLocation);
                                //    _context.Update(partner);
                                //    _context.SaveChanges();
                                //}


                                resultNew = await GetID(saveResult.Data.E_OUTPUT[c].STCEG.Trim());
                                if (resultNew != "")
                                {
                                    try
                                    {
                                        response = JsonConvert.DeserializeObject<Dto.PartnerResponse>(resultNew);
                                        if (response.Cod == "200")
                                        {
                                            res = await GetCorrelationID(response.CorrelationId);


                                            using (var errorfile = System.IO.File.CreateText("ANAF-" + DateTime.Now.Ticks + ".txt"))
                                            {
                                                errorfile.WriteLine(res);

                                            };
                                            if (res != "")
                                            {
                                                try
                                                {
                                                    partnerCorrelation = JsonConvert.DeserializeObject<Dto.PartnerCorrelationResponse>(res);

													if (partnerCorrelation.Cod == "200")
                                                    {
                                                        partnerLocation = _context.Set<Model.PartnerLocation>().Where(p => p.Cui == saveResult.Data.E_OUTPUT[c].STCEG.Trim()).SingleOrDefault();

                                                        if (partnerLocation == null)
                                                        {
                                                            partnerLocation = new Model.PartnerLocation()
                                                            {
                                                                Cui = partnerCorrelation.Found[0].Cui,
                                                                Data = partnerCorrelation.Found[0].Data,
                                                                Denumire = partnerCorrelation.Found[0].Denumire,
                                                                Adresa = partnerCorrelation.Found[0].Adresa,
                                                                NrRegCom = partnerCorrelation.Found[0].NrRegCom,
                                                                Telefon = partnerCorrelation.Found[0].Telefon,
                                                                Fax = partnerCorrelation.Found[0].Fax,
                                                                CodPostal = partnerCorrelation.Found[0].CodPostal,
                                                                Act = partnerCorrelation.Found[0].Act,
                                                                Stare_inregistrare = partnerCorrelation.Found[0].Stare_inregistrare,
                                                                ScpTVA = partnerCorrelation.Found[0].ScpTVA,
                                                                Data_inceput_ScpTVA = partnerCorrelation.Found[0].Data_inceput_ScpTVA,
                                                                Data_sfarsit_ScpTVA = partnerCorrelation.Found[0].Data_sfarsit_ScpTVA,
                                                                Data_anul_imp_ScpTVA = partnerCorrelation.Found[0].Data_anul_imp_ScpTVA,
                                                                Mesaj_ScpTVA = partnerCorrelation.Found[0].Mesaj_ScpTVA,
                                                                DataInceputTvaInc = partnerCorrelation.Found[0].DataInceputTvaInc,
                                                                DataSfarsitTvaInc = partnerCorrelation.Found[0].DataSfarsitTvaInc,
                                                                DataActualizareTvaInc = partnerCorrelation.Found[0].DataActualizareTvaInc,
                                                                DataPublicareTvaInc = partnerCorrelation.Found[0].DataPublicareTvaInc,
                                                                TipActTvaInc = partnerCorrelation.Found[0].TipActTvaInc,
                                                                StatusTvaIncasare = partnerCorrelation.Found[0].StatusTvaIncasare,
                                                                DataInactivare = partnerCorrelation.Found[0].DataInactivare,
                                                                DataReactivare = partnerCorrelation.Found[0].DataReactivare,
                                                                DataPublicare = partnerCorrelation.Found[0].DataPublicare,
                                                                DataRadiere = partnerCorrelation.Found[0].DataRadiere,
                                                                StatusInactivi = partnerCorrelation.Found[0].StatusInactivi,
                                                                DataInceputSplitTVA = partnerCorrelation.Found[0].DataInceputSplitTVA,
                                                                DataAnulareSplitTVA = partnerCorrelation.Found[0].DataAnulareSplitTVA,
                                                                Iban = partnerCorrelation.Found[0].Iban,
                                                                StatusRO_e_Factura = partnerCorrelation.Found[0].StatusRO_e_Factura,
                                                                CreatedAt = DateTime.Now,
                                                                ModifiedAt = DateTime.Now
                                                            };

                                                            partner.PartnerLocation = partnerLocation;
                                                            _context.Add(partnerLocation);
                                                            _context.Update(partner);
                                                            _context.SaveChanges();
                                                        }
                                                        else
                                                        {
                                                            partnerLocation.Cui = partnerCorrelation.Found[0].Cui;
                                                            partnerLocation.Data = partnerCorrelation.Found[0].Data;
                                                            partnerLocation.Denumire = partnerCorrelation.Found[0].Denumire;
                                                            partnerLocation.Adresa = partnerCorrelation.Found[0].Adresa;
                                                            partnerLocation.NrRegCom = partnerCorrelation.Found[0].NrRegCom;
                                                            partnerLocation.Telefon = partnerCorrelation.Found[0].Telefon;
                                                            partnerLocation.Fax = partnerCorrelation.Found[0].Fax;
                                                            partnerLocation.CodPostal = partnerCorrelation.Found[0].CodPostal;
                                                            partnerLocation.Act = partnerCorrelation.Found[0].Act;
                                                            partnerLocation.Stare_inregistrare = partnerCorrelation.Found[0].Stare_inregistrare;
                                                            partnerLocation.ScpTVA = partnerCorrelation.Found[0].ScpTVA;
                                                            partnerLocation.Data_inceput_ScpTVA = partnerCorrelation.Found[0].Data_inceput_ScpTVA;
                                                            partnerLocation.Data_sfarsit_ScpTVA = partnerCorrelation.Found[0].Data_sfarsit_ScpTVA;
                                                            partnerLocation.Data_anul_imp_ScpTVA = partnerCorrelation.Found[0].Data_anul_imp_ScpTVA;
                                                            partnerLocation.Mesaj_ScpTVA = partnerCorrelation.Found[0].Mesaj_ScpTVA;
                                                            partnerLocation.DataInceputTvaInc = partnerCorrelation.Found[0].DataInceputTvaInc;
                                                            partnerLocation.DataSfarsitTvaInc = partnerCorrelation.Found[0].DataSfarsitTvaInc;
                                                            partnerLocation.DataActualizareTvaInc = partnerCorrelation.Found[0].DataActualizareTvaInc;
                                                            partnerLocation.DataPublicareTvaInc = partnerCorrelation.Found[0].DataPublicareTvaInc;
                                                            partnerLocation.TipActTvaInc = partnerCorrelation.Found[0].TipActTvaInc;
                                                            partnerLocation.StatusTvaIncasare = partnerCorrelation.Found[0].StatusTvaIncasare;
                                                            partnerLocation.DataInactivare = partnerCorrelation.Found[0].DataInactivare;
                                                            partnerLocation.DataReactivare = partnerCorrelation.Found[0].DataReactivare;
                                                            partnerLocation.DataPublicare = partnerCorrelation.Found[0].DataPublicare;
                                                            partnerLocation.DataRadiere = partnerCorrelation.Found[0].DataRadiere;
                                                            partnerLocation.StatusInactivi = partnerCorrelation.Found[0].StatusInactivi;
                                                            partnerLocation.DataInceputSplitTVA = partnerCorrelation.Found[0].DataInceputSplitTVA;
                                                            partnerLocation.DataAnulareSplitTVA = partnerCorrelation.Found[0].DataAnulareSplitTVA;
                                                            partnerLocation.Iban = partnerCorrelation.Found[0].Iban;
                                                            partnerLocation.StatusRO_e_Factura = partnerCorrelation.Found[0].StatusRO_e_Factura;
                                                            partnerLocation.ModifiedAt = DateTime.Now;

                                                            partner.PartnerLocationId = partnerLocation.Id;
                                                            _context.Update(partner);
                                                            _context.Update(partnerLocation);
                                                            _context.SaveChanges();

                                                        }
                                                        return true;
                                                    }
                                                }
                                                catch (Exception)
                                                {

                                                    throw;
                                                }
                                            }
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (saveResult.Meta.Code == 400)
                        {
                            Model.ErrorType errorType = null;
                            Model.Error error = null;

                            errorType = await _context.Set<Model.ErrorType>().Where(e => e.Code == "GETPARTNER").SingleOrDefaultAsync();

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
                                    Name = saveResult.Errors[0].Detail,
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



                return false;
            }
            else
            {
                //using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
                //{
                //    errorfile.WriteLine(result);
                //    errorfile.WriteLine(result);

                //};

                return false;
            }
        }

        [HttpPost("SyncDataFromSAP")]
        public async Task<string> SyncDataFromSAP(string registryNumber)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.PartnersData> oIList1 = new List<Dto.PartnersData>
                {
                    new Dto.PartnersData()
                        {
                            I_INPUT = new Dto.PartnersInputs()
							{
                                VENDORS = new List<Dto.PartnersModel>
								{
                                    new Dto.PartnersModel()
									{
                                        // Blank = "",
                                        REGISTRYNUMBER = registryNumber
									}
								}
							}
                         }
                };

                

                var postUser = new Dto.GetPartners
                {
                    Sap_function = "ZFIF_FIXED_ASSET_VENDOR_VATNO",
                    data = oIList1,
                    Options = new Dto.PartnersOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    
                };

                //using (var errorfile = System.IO.File.CreateText("before-acquisition-create-" + DateTime.Now.Ticks + ".txt"))
                //{
                //    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(postUser, Formatting.Indented));

                //};

                JsonContent contentJson = JsonContent.Create(postUser);

                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {

					//using (var errorfile = System.IO.File.CreateText("BEFORE: " + DateTime.Now.Ticks + ".txt"))
					//{
     //                   errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

     //               };

					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();


                    //using (var errorfile = System.IO.File.CreateText("RESULT: " + DateTime.Now.Ticks + ".txt"))
                    //{
                    //    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented));

                    //};

                    clientContract.Dispose();

                    return result;

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

                    return result;
                }

               
            }
        }

		[HttpGet("export")]
		public IActionResult Export(string filter, string administrationIds, string admCenterIds)
		{
			List<int> aIds = null;
			List<int> admIds = null;
			List<Model.Partner> partners = null;

			if ((admCenterIds != null) && (admCenterIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(admCenterIds).ToList().Select(int.Parse).ToList();
			if ((administrationIds != null) && (administrationIds.Length > 0)) admIds = JsonConvert.DeserializeObject<string[]>(administrationIds).ToList().Select(int.Parse).ToList();
			using (ExcelPackage package = new ExcelPackage())
			{
				partners = (_itemsRepository as IPartnersRepository).GetByFilters(filter, "PartnerLocation", null, null, null, null, null, false).ToList();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("furnizori");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Descriere";
				worksheet.Cells[1, 2].Value = "CUI";
				worksheet.Cells[1, 3].Value = "Nr. Inmatriculare";
				worksheet.Cells[1, 4].Value = "Adresa";
				worksheet.Cells[1, 5].Value = "Date contact 1";
				worksheet.Cells[1, 6].Value = "Date contact 2";
				worksheet.Cells[1, 7].Value = "Date contact 3";

				int recordIndex = 2;
				foreach (var item in partners)
				{
					worksheet.Cells[recordIndex, 1].Value = item.Name;
					worksheet.Cells[recordIndex, 2].Value = item.FiscalCode;
					worksheet.Cells[recordIndex, 3].Value = item.RegistryNumber;
					worksheet.Cells[recordIndex, 4].Value = item.Address;
					worksheet.Cells[recordIndex, 5].Value = item.ContactInfo != null ? item.ContactInfo : "";
					worksheet.Cells[recordIndex, 6].Value = item.Bank != null ? item.Bank : "";
					worksheet.Cells[recordIndex, 7].Value = item.BankAccount != null ? item.BankAccount : "";
					//worksheet.Cells[recordIndex, 8].Value = item.AdmCenter != null ? item.AdmCenter.Name : "";
					//worksheet.Cells[recordIndex, 9].Value = item.Region != null ? item.Region.Name : "";
					recordIndex++;
				}

				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();
				worksheet.Column(5).AutoFit();
				worksheet.Column(6).AutoFit();
				worksheet.Column(7).AutoFit();

				using (var cells = worksheet.Cells[1, 1, 1, 7])
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
					FileDownloadName = "cost_centers.xlsx"
				};

				return result;

			}
		}
	}
}
