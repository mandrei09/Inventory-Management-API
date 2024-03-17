using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/asyncerrors")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AsyncErrorsController : GenericApiController<Model.AssetSyncError, Dto.AssetSyncError>
    {
        private readonly IConfiguration _configuration;
        private readonly string _BASEURL;
        private readonly string _TOKEN;
        HttpClient clientContract = null;

        public AsyncErrorsController(ApplicationDbContext context, IAsyncErrorsRepository itemsRepository, IMapper mapper, IConfiguration configuration)
            : base(context, itemsRepository, mapper)
        {
            _configuration = configuration;
            this._BASEURL = configuration.GetSection("SAP").GetValue<string>("URL");
            this._TOKEN = configuration.GetSection("SAP").GetValue<string>("SAP-PROXY-AUTH-TOKEN");
        }
        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string errorTypeIds, string includes)
        {
            List<Model.AssetSyncError> items = null;
            IEnumerable<Dto.AssetSyncError> itemsResult = null;
            List<int> cIds = null;

            if (errorTypeIds != null && !errorTypeIds.StartsWith("["))
            {
                errorTypeIds = "[" + errorTypeIds + "]";
            }



            includes = includes ?? "ErrorType,Asset";
            includes = "";
            // if ((errorTypeIds != null) && (errorTypeIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(errorTypeIds).ToList().Select(int.Parse).ToList();


            items = (_itemsRepository as IAsyncErrorsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds).ToList();
            items.Where(d => d.IsDeleted == false);
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Model.AssetSyncError, Dto.AssetSyncError>(); 
            });

            var mapper = new Mapper(config);
            itemsResult = items.Select(i => mapper.Map<Dto.AssetSyncError>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAsyncErrorsRepository).GetCountByFilters(filter, cIds);
                var pagedResult = new Dto.PagedResult<Dto.AssetSyncError>(itemsResult, new Dto.PagingInfo()
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

        [HttpGet("delete/{id}")]
        public void DeleteElement(int Id)
        {
            var item = _context.AssetSyncErrors.FirstOrDefault(d => d.Id == Id);
            item.IsDeleted = true;
            _context.SaveChanges();
        }
        [HttpGet("getacquisitionassetsapdata/{takeacquisitionassetId}")]
        public async Task<List<Dto.AcquisitionAssetSAP>> getAcquisitionAssetSAPData(int takeAcquisitionAssetId)
        {
            List<Dto.AcquisitionAssetSAP> asset = _context.Set<Model.AcquisitionAssetSAP>().FromSql($"EXEC TakeAcquisitionAssetById @TakeAcquisitionAssetId = {takeAcquisitionAssetId}")
                        .AsEnumerable()
                        .GroupBy(item => item.Guid,
                            (key, group) => new Dto.AcquisitionAssetSAP()
                            {
                                STORNO = group.FirstOrDefault().STORNO,
                                COMPANYCODE = group.FirstOrDefault().COMPANYCODE,
                                DOC_DATE = group.FirstOrDefault().DOC_DATE,
                                PSTNG_DATE = group.FirstOrDefault().PSTNG_DATE,
                                REF_DOC_NO = group.FirstOrDefault().REF_DOC_NO,
                                HEADER_TXT = group.FirstOrDefault().HEADER_TXT,
                                VENDOR_NO = group.FirstOrDefault().VENDOR_NO,
                                CURRENCY = group.FirstOrDefault().CURRENCY,
                                EXCH_RATE = group.FirstOrDefault().EXCH_RATE,
                                TOTAL_AMOUNT = group.FirstOrDefault().TOTAL_AMOUNT,
                                AssetId = group.FirstOrDefault().AssetId,
                                ASSETS = group.ToList().Select(a => new Dto.AcquisitionAssets
                                {
                                    ASSET = a.ASSET,
                                    SUBNUMBER = a.SUBNUMBER,
                                    ITEM_TEXT = a.ITEM_TEXT,
                                    TAX_CODE = a.TAX_CODE,
                                    NET_AMOUNT = a.NET_AMOUNT,
                                    TAX_AMOUNT = a.TAX_AMOUNT,
                                    GL_ACCOUNT = a.GL_ACCOUNT,
                                    ASVAL_DATE = a.ASVAL_DATE,
                                    WBS_ELEMENT = a.WBS_ELEMENT
                                }).ToList()
                            })
                        .ToList();
            return asset;
        }

        [HttpGet("getchangeassetsapdata/{takeacquisitionassetId}")]
        public async Task<List<Dto.AssetChangeSAP>> getChangeAssetSapData(int takeAcquisitionAssetId)
        {
            List<Dto.AssetChangeSAP> asset = _context.Set<Model.AssetChangeSAP>()
                            .Where(com => com.IsDeleted == false && com.NotSync == true && com.IsTesting == false && com.AssetId == takeAcquisitionAssetId).Select(a => new Dto.AssetChangeSAP()
                            {

                                COMPANYCODE = a.COMPANYCODE,
                                ASSET = a.ASSET,
                                SUBNUMBER = a.SUBNUMBER,
                                ASSETCLASS = a.ASSETCLASS,
                                POSTCAP = a.POSTCAP,
                                DESCRIPT = a.DESCRIPT,
                                DESCRIPT2 = a.DESCRIPT2,
                                INVENT_NO = a.INVENT_NO,
                                SERIAL_NO = a.SERIAL_NO,
                                QUANTITY = a.QUANTITY,
                                BASE_UOM = a.BASE_UOM,
                                LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
                                LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
                                CAP_DATE = a.CAP_DATE,
                                COSTCENTER = a.COSTCENTER,
                                INTERN_ORD = a.INTERN_ORD,
                                PLANT = a.PLANT,
                                LOCATION = a.LOCATION,
                                ROOM = a.ROOM,
                                PERSON_NO = a.PERSON_NO,
                                PLATE_NO = a.PLATE_NO,
                                ZZCLAS = a.ZZCLAS,
                                IN_CONSERVATION = a.IN_CONSERVATION,
                                PROP_IND = a.PROP_IND,
                                OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
                                OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
                                VENDOR_NO = a.VENDOR_NO,
                                RESP_CCTR = a.RESP_CCTR,
                                FROM_DATE = a.FROM_DATE,
                                INVOICE = a.INVOICE,
                                AssetId = a.AssetId

                            }).ToList();
            return asset;
        }
        public class AssetAcquisitionModel
        {
            [JsonPropertyName("assetId")]
            public int AssetId { get; set; }
            [JsonPropertyName("synccode")]
            public string Synccode { get; set; }


            [JsonPropertyName("companycode")]
            public string Companycode { get; set; }

            [JsonPropertyName("currency")]
            public string Currency { get; set; }


            [JsonPropertyName("storno")]
            public string Storno { get; set; } 

            [JsonPropertyName("header_TXT")]
            public string HeaderTxt { get; set; }

            [JsonPropertyName("ref_DOC_NO")]
            public string RefDocNo { get; set; }

            [JsonPropertyName("doc_DATE")]
            public string DocDate { get; set; }

            [JsonPropertyName("arraycode")]
            public string Arraycode { get; set; }

            [JsonPropertyName("asval_DATE")]
            public string AsvalDate { get; set; } 

            [JsonPropertyName("gL_ACCOUNT")]
            public string GlAccount { get; set; }

            [JsonPropertyName("item_TEXT")]
            public string ItemText { get; set; }

            [JsonPropertyName("net_AMOUNT")]
            public decimal? NetAmount { get; set; } 

            [JsonPropertyName("subnumber")]
            public string Subnumber { get; set; }

            [JsonPropertyName("tax_AMOUNT")]
            public decimal? TaxAmount { get; set; } 

            [JsonPropertyName("tax_CODE")]
            public string TaxCode { get; set; }

            [JsonPropertyName("vendor_NO")]
            public string VendorNo { get; set; }

            [JsonPropertyName("wbs_ELEMENT")]
            public string WbsElement { get; set; }
            [JsonPropertyName("selectedItemId")]
            public int ErrorAsyncId { get; set; }
        }

        [HttpPost("updateacquisitionasset")]
        public async Task<IActionResult> UpdateAcquisitionAssetSAP([FromBody] AssetAcquisitionModel model)
        {
            using (clientContract = new HttpClient())
            {
                var baseUrl = _BASEURL;
                if (model == null)
                {
                    return BadRequest("Invalid data");
                }
                var asset = _context.AcquisitionAssetsSAPs.FromSql($"EXEC TakeAcquisitionAssetById @TakeAcquisitionAssetId = {model.AssetId}")
                           .AsNoTracking()
                           .GroupBy(item => item.Guid,
                               (key, group) => new Model.AcquisitionAssetSAP()
                               {
                                   Id = group.FirstOrDefault().Id,
                                   AccMonthId = group.FirstOrDefault().AccMonthId,
                                   BudgetManagerId = group.FirstOrDefault().BudgetManagerId,
                                   ErrorId = group.FirstOrDefault().ErrorId,
                                   CreatedAt = group.FirstOrDefault().CreatedAt,
                                   CreatedBy = group.FirstOrDefault().CreatedBy,
                                   Guid = group.FirstOrDefault().Guid,
                                   IsDeleted = group.FirstOrDefault().IsDeleted,
                                   ModifiedAt = group.FirstOrDefault().ModifiedAt,
                                   ModifiedBy = group.FirstOrDefault().ModifiedBy,
                                   IsTesting = group.FirstOrDefault().IsTesting,
                                   STORNO = group.FirstOrDefault().STORNO,
                                   COMPANYCODE = group.FirstOrDefault().COMPANYCODE,
                                   DOC_DATE = group.FirstOrDefault().DOC_DATE,
                                   PSTNG_DATE = group.FirstOrDefault().PSTNG_DATE,
                                   REF_DOC_NO = group.FirstOrDefault().REF_DOC_NO,
                                   HEADER_TXT = group.FirstOrDefault().HEADER_TXT,
                                   VENDOR_NO = group.FirstOrDefault().VENDOR_NO,
                                   CURRENCY = group.FirstOrDefault().CURRENCY,
                                   EXCH_RATE = group.FirstOrDefault().EXCH_RATE,
                                   TOTAL_AMOUNT = group.FirstOrDefault().TOTAL_AMOUNT,
                                   AssetId = group.FirstOrDefault().AssetId,
                                   ASSET = group.FirstOrDefault().ASSET,
                                   SUBNUMBER = group.FirstOrDefault().SUBNUMBER,
                                   ITEM_TEXT = group.FirstOrDefault().ITEM_TEXT,
                                   TAX_CODE = group.FirstOrDefault().TAX_CODE,
                                   NET_AMOUNT = group.FirstOrDefault().NET_AMOUNT,
                                   TAX_AMOUNT = group.FirstOrDefault().TAX_AMOUNT,
                                   GL_ACCOUNT = group.FirstOrDefault().GL_ACCOUNT,
                                   ASVAL_DATE = group.FirstOrDefault().ASVAL_DATE,
                                   WBS_ELEMENT = group.FirstOrDefault().WBS_ELEMENT
                               })
                           .FirstOrDefault();
                if (asset == null)
                {
                    return NotFound($"Asset with ID {model.AssetId} not found.");
                }
                if (asset != null)
                {
                    asset.STORNO = model.Storno ?? asset.STORNO;
                    asset.COMPANYCODE = !string.IsNullOrEmpty(model.Companycode) ? model.Companycode : asset.COMPANYCODE;
                    if (!string.IsNullOrEmpty(model.DocDate))
                    {
                        DateTime parsedDate = DateTime.ParseExact(model.DocDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        string formattedDocDate = parsedDate.ToString("yyyyMMdd");
                        asset.DOC_DATE = !string.IsNullOrEmpty(formattedDocDate) ? formattedDocDate : asset.DOC_DATE;
                    }
                    asset.REF_DOC_NO = !string.IsNullOrEmpty(model.RefDocNo) ? model.RefDocNo : asset.REF_DOC_NO;
                    asset.HEADER_TXT = !string.IsNullOrEmpty(model.HeaderTxt) ? model.HeaderTxt : asset.HEADER_TXT;
                    asset.VENDOR_NO = !string.IsNullOrEmpty(model.VendorNo) ? model.VendorNo : asset.VENDOR_NO;
                    asset.CURRENCY = !string.IsNullOrEmpty(model.Currency) ? model.Currency : asset.CURRENCY;
                    asset.SUBNUMBER = !string.IsNullOrEmpty(model.Subnumber) ? model.Subnumber : asset.SUBNUMBER;
                    asset.ITEM_TEXT = !string.IsNullOrEmpty(model.ItemText) ? model.ItemText : asset.ITEM_TEXT;
                    asset.TAX_CODE = !string.IsNullOrEmpty(model.TaxCode) ? model.TaxCode : asset.TAX_CODE;
                    asset.NET_AMOUNT = model.NetAmount ?? asset.NET_AMOUNT;
                    asset.TAX_AMOUNT = model.TaxAmount ?? asset.TAX_AMOUNT;
                    asset.GL_ACCOUNT = !string.IsNullOrEmpty(model.GlAccount) ? model.GlAccount : asset.GL_ACCOUNT;
                    asset.ASVAL_DATE = !string.IsNullOrEmpty(model.AsvalDate) ? model.AsvalDate : asset.ASVAL_DATE;
                    asset.WBS_ELEMENT = !string.IsNullOrEmpty(model.WbsElement) ? model.WbsElement : asset.WBS_ELEMENT;
                    asset.NotSync = true;
                    _context.AcquisitionAssetsSAPs.Update(asset);
                    await _context.SaveChangesAsync();

                    IList<AcquisitionAssets> assets = new List<AcquisitionAssets>();

                    assets.Add(new AcquisitionAssets
                    {
                        ASSET = asset.ASSET,
                        SUBNUMBER = asset.SUBNUMBER,
                        ITEM_TEXT = asset.ITEM_TEXT,
                        TAX_CODE = asset.TAX_CODE,
                        NET_AMOUNT = asset.NET_AMOUNT,
                        TAX_AMOUNT = asset.TAX_AMOUNT,
                        GL_ACCOUNT = asset.GL_ACCOUNT,
                        ASVAL_DATE = asset.ASVAL_DATE,
                        WBS_ELEMENT = asset.WBS_ELEMENT
                    });

                    IList<Dto.AcquisitionAssetData> oIList1 = new List<Dto.AcquisitionAssetData>
                {
                    new Dto.AcquisitionAssetData()
                        {
                            I_INPUT = new Dto.AcquisitionAssetInput()
                                {
                                    STORNO = asset.STORNO,
                                    COMPANYCODE = asset.COMPANYCODE,
                                    DOC_DATE = asset.DOC_DATE,
                                    PSTNG_DATE = asset.PSTNG_DATE,
                                    REF_DOC_NO = asset.REF_DOC_NO,
                                    HEADER_TXT = asset.HEADER_TXT,
                                    VENDOR_NO = asset.VENDOR_NO,
                                    CURRENCY = asset.CURRENCY,
                                    EXCH_RATE = asset.EXCH_RATE,
                                    TOTAL_AMOUNT = asset.TOTAL_AMOUNT,
                                    ASSETS = assets
                                }
                         }
                };

                    var postUser = new Dto.AcquisitionAsset
                    {
                        Sap_function = "ZFIF_FIXED_ASSET_ACQUISITION",
                        Options = new Dto.AcquisitionAssetDataOptions()
                        {
                            Api_call_timeout = 180
                        },
                        Remote_host_name = "test",
                        Data = oIList1
                    };
                    JsonContent contentJson = JsonContent.Create(postUser);
                    clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);
                    string result = "";

                    try
                    {
                        var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == model.ErrorAsyncId);

                        var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                        result = await httpResponse.Content.ReadAsStringAsync();

                        clientContract.Dispose();


                        if (result != "")
                        {
                            var createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);
                           
                            if (createAssetResultNew.Meta.Code == 400)
                            {
                                asset.NotSync = true;
                                _context.AcquisitionAssetsSAPs.Update(asset);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {

                            }
                            assetSyncErrorToUpdate.ManualSyncErrors = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == model.ErrorAsyncId);
                        assetSyncErrorToUpdate.ManualSyncErrors = JsonConvert.SerializeObject(ex.Message, Formatting.Indented).ToString();
                        _context.SaveChanges();
                        return BadRequest(JsonConvert.SerializeObject(ex, Formatting.Indented).ToString());

                    }
                }
                return Ok("Ok");
            }
        }



        [HttpGet("getcreateassetsapdata/{takeacquisitionassetId}")]
        public async Task<List<Dto.CreateAssetSAP>> getCreateAssetSAPData(int takeAcquisitionAssetId)
        {
            List<Dto.CreateAssetSAP> assets = _context.Set<Model.CreateAssetSAP>()
                             .Where(com => com.IsDeleted == false && com.NotSync == true && com.IsTesting == false && com.AssetId == takeAcquisitionAssetId).Select(a => new Dto.CreateAssetSAP()
                             {
                                 AssetId = a.AssetId,
                                 XSUBNO = a.XSUBNO,
                                 COMPANYCODE = a.COMPANYCODE,
                                 ASSET = a.ASSET,
                                 SUBNUMBER = a.SUBNUMBER,
                                 ASSETCLASS = a.ASSETCLASS,
                                 POSTCAP = a.POSTCAP,
                                 DESCRIPT = a.DESCRIPT,
                                 DESCRIPT2 = a.DESCRIPT2,
                                 INVENT_NO = a.INVENT_NO,
                                 SERIAL_NO = a.SERIAL_NO,
                                 QUANTITY = a.QUANTITY,
                                 BASE_UOM = a.BASE_UOM,
                                 LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
                                 LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
                                 CAP_DATE = a.CAP_DATE,
                                 COSTCENTER = a.COSTCENTER,
                                 RESP_CCTR = a.RESP_CCTR,
                                 INTERN_ORD = a.INTERN_ORD,
                                 PLANT = a.PLANT,
                                 LOCATION = a.LOCATION,
                                 ROOM = a.ROOM,
                                 PERSON_NO = a.PERSON_NO,
                                 PLATE_NO = a.PLATE_NO,
                                 ZZCLAS = a.ZZCLAS,
                                 IN_CONSERVATION = a.IN_CONSERVATION,
                                 PROP_IND = a.PROP_IND,
                                 OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
                                 OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
                                 TESTRUN = a.TESTRUN,
                                 VENDOR_NO = a.VENDOR_NO,
                                 INVOICE = a.INVOICE

                             }).ToList();
            return assets;
        }
        public class AssetCreateModel
        {
            [JsonPropertyName("assetId")]
            public int AssetId { get; set; }

            [JsonPropertyName("asset")]
            public string Asset { get; set; }

            [JsonPropertyName("assetclass")]
            public string Assetclass { get; set; }

            [JsonPropertyName("base_UOM")]
            public string BaseUom { get; set; }

            [JsonPropertyName("cap_DATE")]
            public string CapDate { get; set; }

            [JsonPropertyName("companycode")]
            public string Companycode { get; set; }

            [JsonPropertyName("costcenter")]
            public string Costcenter { get; set; }

            [JsonPropertyName("descripT2")]
            public string Descript2 { get; set; }

            [JsonPropertyName("descript")]
            public string Descript { get; set; }

            [JsonPropertyName("in_CONSERVATION")]
            public string InConservation { get; set; }

            [JsonPropertyName("interN_ORD")]
            public string InternOrd { get; set; }

            [JsonPropertyName("invenT_NO")]
            public string InventNo { get; set; }

            [JsonPropertyName("invoice")]
            public string Invoice { get; set; }

            [JsonPropertyName("lasT_INVENTORY_DATE")]
            public string LastInventoryDate { get; set; }

            [JsonPropertyName("lasT_INVENTORY_DOCNO")]
            public string LastInventoryDocno { get; set; }

            [JsonPropertyName("location")]
            public string Location { get; set; }

            [JsonPropertyName("notSync")]
            public bool NotSync { get; set; }

            [JsonPropertyName("optimA_ASSET_NO")]
            public string OptimaAssetNo { get; set; }

            [JsonPropertyName("optimA_ASSET_PARENT_NO")]
            public string OptimaAssetParentNo { get; set; }

            [JsonPropertyName("persoN_NO")]
            public string PersonNo { get; set; }

            [JsonPropertyName("plant")]
            public string Plant { get; set; }

            [JsonPropertyName("platE_NO")]
            public string PlateNo { get; set; }

            [JsonPropertyName("postcap")]
            public string Postcap { get; set; }

            [JsonPropertyName("proP_IND")]
            public string PropInd { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }

            [JsonPropertyName("resP_CCTR")]
            public string RespCctr { get; set; }

            [JsonPropertyName("room")]
            public string Room { get; set; }
            [JsonPropertyName("selectedItemId")]
            public int ErrorAsyncId { get; set; }

            [JsonPropertyName("seriaL_NO")]
            public string SerialNo { get; set; }

            [JsonPropertyName("subnumber")]
            public string Subnumber { get; set; }

            [JsonPropertyName("testrun")]
            public string Testrun { get; set; }

            [JsonPropertyName("vendoR_NO")]
            public string VendorNo { get; set; }

            [JsonPropertyName("xsubno")]
            public string Xsubno { get; set; }

            [JsonPropertyName("zzclas")]
            public string Zzclas { get; set; }
            
        }


        [HttpPost("updatecreateasset")]
        public async Task<IActionResult> UpdateCreateAssetSAP([FromBody] AssetCreateModel model)
        {
            using (clientContract = new HttpClient())
            {
                var baseUrl = _BASEURL;
                if (model == null)
                {
                    return BadRequest("Invalid data");
                }
                var asset = _context.CreateAssetSAPs
                                  .Where(com => com.IsDeleted == false && com.NotSync == true && com.IsTesting == false && com.AssetId == model.AssetId).Select(a => new Model.CreateAssetSAP()
                                  {
                                      Id = a.Id,
                                      AccMonthId = a.AccMonthId,
                                      BudgetManagerId = a.BudgetManagerId,
                                      AssetId = a.AssetId,
                                      XSUBNO = a.XSUBNO,
                                      COMPANYCODE = a.COMPANYCODE,
                                      ASSET = a.ASSET,
                                      SUBNUMBER = a.SUBNUMBER,
                                      ASSETCLASS = a.ASSETCLASS,
                                      POSTCAP = a.POSTCAP,
                                      DESCRIPT = a.DESCRIPT,
                                      DESCRIPT2 = a.DESCRIPT2,
                                      INVENT_NO = a.INVENT_NO,
                                      SERIAL_NO = a.SERIAL_NO,
                                      QUANTITY = a.QUANTITY,
                                      BASE_UOM = a.BASE_UOM,
                                      LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
                                      LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
                                      CAP_DATE = a.CAP_DATE,
                                      COSTCENTER = a.COSTCENTER,
                                      RESP_CCTR = a.RESP_CCTR,
                                      INTERN_ORD = a.INTERN_ORD,
                                      PLANT = a.PLANT,
                                      LOCATION = a.LOCATION,
                                      ROOM = a.ROOM,
                                      PERSON_NO = a.PERSON_NO,
                                      PLATE_NO = a.PLATE_NO,
                                      ZZCLAS = a.ZZCLAS,
                                      IN_CONSERVATION = a.IN_CONSERVATION,
                                      PROP_IND = a.PROP_IND,
                                      OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
                                      OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
                                      TESTRUN = a.TESTRUN,
                                      VENDOR_NO = a.VENDOR_NO,
                                      INVOICE = a.INVOICE

                                  }).FirstOrDefault();
                if (asset != null)
                {
                    asset.XSUBNO = model.Xsubno ?? asset.XSUBNO;
                    asset.COMPANYCODE = !string.IsNullOrEmpty(model.Companycode) ? model.Companycode : asset.COMPANYCODE;
                    asset.ASSET = !string.IsNullOrEmpty(model.Asset) ? model.Asset : asset.ASSET;
                    asset.SUBNUMBER = !string.IsNullOrEmpty(model.Subnumber) ? model.Subnumber : asset.SUBNUMBER;
                    asset.ASSETCLASS = !string.IsNullOrEmpty(model.Assetclass) ? model.Assetclass : asset.ASSETCLASS;
                    asset.POSTCAP = !string.IsNullOrEmpty(model.Postcap) ? model.Postcap : asset.POSTCAP;
                    asset.DESCRIPT = !string.IsNullOrEmpty(model.Descript) ? model.Descript : asset.DESCRIPT;
                    asset.DESCRIPT2 = !string.IsNullOrEmpty(model.Descript2) ? model.Descript2 : asset.DESCRIPT2;
                    asset.INVENT_NO = !string.IsNullOrEmpty(model.InventNo) ? model.InventNo : asset.INVENT_NO;
                    asset.SERIAL_NO = !string.IsNullOrEmpty(model.SerialNo) ? model.SerialNo : asset.SERIAL_NO;
                    asset.QUANTITY = model.Quantity != 0 ? model.Quantity : asset.QUANTITY;
                    asset.BASE_UOM = !string.IsNullOrEmpty(model.BaseUom) ? model.BaseUom : asset.BASE_UOM;
                    asset.LAST_INVENTORY_DATE = !string.IsNullOrEmpty(model.LastInventoryDate) ? model.LastInventoryDate : asset.LAST_INVENTORY_DATE;
                    asset.LAST_INVENTORY_DOCNO = !string.IsNullOrEmpty(model.LastInventoryDocno) ? model.LastInventoryDocno : asset.LAST_INVENTORY_DOCNO;
                    asset.CAP_DATE = !string.IsNullOrEmpty(model.CapDate) ? model.CapDate : asset.CAP_DATE;
                    asset.COSTCENTER = !string.IsNullOrEmpty(model.Costcenter) ? model.Costcenter : asset.COSTCENTER;
                    asset.RESP_CCTR = !string.IsNullOrEmpty(model.RespCctr) ? model.RespCctr : asset.RESP_CCTR;
                    asset.INTERN_ORD = !string.IsNullOrEmpty(model.InternOrd) ? model.InternOrd : asset.INTERN_ORD;
                    asset.PLANT = !string.IsNullOrEmpty(model.Plant) ? model.Plant : asset.PLANT;
                    asset.LOCATION = !string.IsNullOrEmpty(model.Location) ? model.Location : asset.LOCATION;
                    asset.ROOM = !string.IsNullOrEmpty(model.Room) ? model.Room : asset.ROOM;
                    asset.PERSON_NO = !string.IsNullOrEmpty(model.PersonNo) ? model.PersonNo : asset.PERSON_NO;
                    asset.PLATE_NO = !string.IsNullOrEmpty(model.PlateNo) ? model.PlateNo : asset.PLATE_NO;
                    asset.ZZCLAS = !string.IsNullOrEmpty(model.Zzclas) ? model.Zzclas : asset.ZZCLAS;
                    asset.IN_CONSERVATION = !string.IsNullOrEmpty(model.InConservation) ? model.InConservation : asset.IN_CONSERVATION;
                    asset.PROP_IND = !string.IsNullOrEmpty(model.PropInd) ? model.PropInd : asset.PROP_IND;
                    asset.OPTIMA_ASSET_NO = !string.IsNullOrEmpty(model.OptimaAssetNo) ? model.OptimaAssetNo : asset.OPTIMA_ASSET_NO;
                    asset.OPTIMA_ASSET_PARENT_NO = !string.IsNullOrEmpty(model.OptimaAssetParentNo) ? model.OptimaAssetParentNo : asset.OPTIMA_ASSET_PARENT_NO;
                    asset.TESTRUN = !string.IsNullOrEmpty(model.Testrun) ? model.Testrun : asset.TESTRUN;
                    asset.VENDOR_NO = !string.IsNullOrEmpty(model.VendorNo) ? model.VendorNo : asset.VENDOR_NO;
                    asset.INVOICE = !string.IsNullOrEmpty(model.Invoice) ? model.Invoice : asset.INVOICE;
                    asset.NotSync = true;
                    _context.CreateAssetSAPs.Update(asset);
                    await _context.SaveChangesAsync();

                    IList<Dto.CreateAssetData> oIList1 = new List<Dto.CreateAssetData>
                {
                    new Dto.CreateAssetData()
                        {
                            I_INPUT = new Dto.CreateAssetInput()
                                {
                                    XSUBNO = asset.XSUBNO,
                                    COMPANYCODE = asset.COMPANYCODE,
                                    ASSET = asset.ASSET,
                                    SUBNUMBER = asset.SUBNUMBER,
                                    ASSETCLASS = asset.ASSETCLASS,
                                    POSTCAP = asset.POSTCAP,
                                    DESCRIPT = asset.DESCRIPT,
                                    DESCRIPT2 = asset.DESCRIPT2,
                                    INVENT_NO = asset.INVENT_NO,
                                    SERIAL_NO = asset.SERIAL_NO,
                                    QUANTITY = asset.QUANTITY,
                                    BASE_UOM = asset.BASE_UOM,
                                    LAST_INVENTORY_DATE = asset.LAST_INVENTORY_DATE,
                                    LAST_INVENTORY_DOCNO = asset.LAST_INVENTORY_DOCNO,
                                    CAP_DATE = asset.CAP_DATE,
                                    COSTCENTER = asset.COSTCENTER,
                                    RESP_CCTR = asset.RESP_CCTR,
                                    INTERN_ORD = asset.INTERN_ORD,
                                    PLANT = asset.PLANT,
                                    LOCATION = asset.LOCATION,
                                    ROOM = asset.ROOM,
                                    PERSON_NO = asset.PERSON_NO,
                                    PLATE_NO = asset.PLATE_NO,
                                    ZZCLAS = asset.ZZCLAS,
                                    IN_CONSERVATION = asset.IN_CONSERVATION,
                                    PROP_IND = asset.PROP_IND,
                                    OPTIMA_ASSET_NO = asset.OPTIMA_ASSET_NO,
                                    OPTIMA_ASSET_PARENT_NO = asset.OPTIMA_ASSET_PARENT_NO,
                                    TESTRUN = asset.TESTRUN,
                                    VENDOR_NO = asset.VENDOR_NO,
                                    INVOICE = asset.INVOICE
                                }
                         }
                };

                    var postUser = new Dto.CreateAsset
                    {
                        Sap_function = "ZFIF_FIXED_ASSET_CREATE",
                        Options = new Dto.CreateAssetDataOptions()
                        {
                            Api_call_timeout = 180
                        },
                        Remote_host_name = "test",
                        Data = oIList1
                    };

                    JsonContent contentJson = JsonContent.Create(postUser);
                    clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);
                    var result = "";
                    try
                    {
                        var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == model.ErrorAsyncId);

                        var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                        result = await httpResponse.Content.ReadAsStringAsync();

                        clientContract.Dispose();


                        if (result != "")
                        {
                            var createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);
                            
                            if (createAssetResultNew.Meta.Code == 400)
                            {
                                asset.NotSync = true;
                                _context.CreateAssetSAPs.Update(asset);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {

                            }
                            assetSyncErrorToUpdate.ManualSyncErrors = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex) {
                        var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == model.ErrorAsyncId);
                        assetSyncErrorToUpdate.ManualSyncErrors = JsonConvert.SerializeObject(ex.Message, Formatting.Indented).ToString();
                        _context.SaveChanges();
                        return BadRequest(JsonConvert.SerializeObject(ex, Formatting.Indented).ToString());
                    
                    }
                }
                return Ok(model);
            }
        }
        public class AssetChangeModel
        {
            [JsonPropertyName("assetId")]
            public int AssetId { get; set; }

            [JsonPropertyName("asset")]
            public string Asset { get; set; }

            [JsonPropertyName("assetclass")]
            public string Assetclass { get; set; }

            [JsonPropertyName("basE_UOM")]
            public string BaseUom { get; set; }

            [JsonPropertyName("caP_DATE")]
            public string CapDate { get; set; }

            [JsonPropertyName("companycode")]
            public string Companycode { get; set; }

            [JsonPropertyName("costcenter")]
            public string Costcenter { get; set; }

            [JsonPropertyName("descripT2")]
            public string Descript2 { get; set; }

            [JsonPropertyName("descript")]
            public string Descript { get; set; }

            [JsonPropertyName("iN_CONSERVATION")]
            public string InConservation { get; set; }

            [JsonPropertyName("interN_ORD")]
            public string InternOrd { get; set; }

            [JsonPropertyName("invenT_NO")]
            public string InventNo { get; set; }

            [JsonPropertyName("invoice")]
            public string Invoice { get; set; }

            [JsonPropertyName("lasT_INVENTORY_DATE")]
            public string LastInventoryDate { get; set; }

            [JsonPropertyName("lasT_INVENTORY_DOCNO")]
            public string LastInventoryDocno { get; set; }

            [JsonPropertyName("location")]
            public string Location { get; set; }

            [JsonPropertyName("notSync")]
            public bool? NotSync { get; set; }

            [JsonPropertyName("optimA_ASSET_NO")]
            public string OptimaAssetNo { get; set; }

            [JsonPropertyName("optimA_ASSET_PARENT_NO")]
            public string OptimaAssetParentNo { get; set; }

            [JsonPropertyName("persoN_NO")]
            public string PersonNo { get; set; }

            [JsonPropertyName("plant")]
            public string Plant { get; set; }

            [JsonPropertyName("platE_NO")]
            public string PlateNo { get; set; }

            [JsonPropertyName("postcap")]
            public string Postcap { get; set; }

            [JsonPropertyName("proP_IND")]
            public string PropInd { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }

            [JsonPropertyName("resP_CCTR")]
            public string RespCctr { get; set; }

            [JsonPropertyName("room")]
            public string Room { get; set; }

            [JsonPropertyName("seriaL_NO")]
            public string SerialNo { get; set; }

            [JsonPropertyName("subnumber")]
            public string Subnumber { get; set; }

            [JsonPropertyName("vendor_NO")]
            public string VendorNo { get; set; }

            [JsonPropertyName("zzclas")]
            public string Zzclas { get; set; }
            [JsonPropertyName("selectedItemId")]
            public int ErrorAsyncId { get; set; }
        }

        [HttpPost("updatechangeasset")]
        public async Task<IActionResult> UpdateChangeAssetSAP([FromBody] AssetChangeModel model)
        {
            using (clientContract = new HttpClient())
            {
                var baseUrl = _BASEURL;
                if (model == null)
                {
                    return BadRequest("Invalid data");
                }
                var asset = _context.AssetChangeSAPs
                                .Where(com => com.IsDeleted == false && com.NotSync == true && com.IsTesting == false && com.AssetId == model.AssetId).Select(a => new Model.AssetChangeSAP()
                                {
                                    Id = a.Id,
                                    AccMonthId = a.AccMonthId,
                                    BudgetManagerId = a.BudgetManagerId,
                                    COMPANYCODE = a.COMPANYCODE,
                                    ASSET = a.ASSET,
                                    SUBNUMBER = a.SUBNUMBER,
                                    ASSETCLASS = a.ASSETCLASS,
                                    POSTCAP = a.POSTCAP,
                                    DESCRIPT = a.DESCRIPT,
                                    DESCRIPT2 = a.DESCRIPT2,
                                    INVENT_NO = a.INVENT_NO,
                                    SERIAL_NO = a.SERIAL_NO,
                                    QUANTITY = a.QUANTITY,
                                    BASE_UOM = a.BASE_UOM,
                                    LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
                                    LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
                                    CAP_DATE = a.CAP_DATE,
                                    COSTCENTER = a.COSTCENTER,
                                    INTERN_ORD = a.INTERN_ORD,
                                    PLANT = a.PLANT,
                                    LOCATION = a.LOCATION,
                                    ROOM = a.ROOM,
                                    PERSON_NO = a.PERSON_NO,
                                    PLATE_NO = a.PLATE_NO,
                                    ZZCLAS = a.ZZCLAS,
                                    IN_CONSERVATION = a.IN_CONSERVATION,
                                    PROP_IND = a.PROP_IND,
                                    OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
                                    OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
                                    VENDOR_NO = a.VENDOR_NO,
                                    RESP_CCTR = a.RESP_CCTR,
                                    FROM_DATE = a.FROM_DATE,
                                    INVOICE = a.INVOICE,
                                    AssetId = a.AssetId

                                }).FirstOrDefault();
                if (asset != null)
                {
                    asset.COMPANYCODE = !string.IsNullOrEmpty(model.Companycode) ? model.Companycode : asset.COMPANYCODE;
                    asset.ASSET = !string.IsNullOrEmpty(model.Asset) ? model.Asset : asset.ASSET;
                    asset.SUBNUMBER = !string.IsNullOrEmpty(model.Subnumber) ? model.Subnumber : asset.SUBNUMBER;
                    asset.ASSETCLASS = !string.IsNullOrEmpty(model.Assetclass) ? model.Assetclass : asset.ASSETCLASS;
                    asset.POSTCAP = !string.IsNullOrEmpty(model.Postcap) ? model.Postcap : asset.POSTCAP;
                    asset.DESCRIPT = !string.IsNullOrEmpty(model.Descript) ? model.Descript : asset.DESCRIPT;
                    asset.DESCRIPT2 = !string.IsNullOrEmpty(model.Descript2) ? model.Descript2 : asset.DESCRIPT2;
                    asset.INVENT_NO = !string.IsNullOrEmpty(model.InventNo) ? model.InventNo : asset.INVENT_NO;
                    asset.SERIAL_NO = !string.IsNullOrEmpty(model.SerialNo) ? model.SerialNo : asset.SERIAL_NO;
                    asset.QUANTITY = model.Quantity != 0 ? model.Quantity : asset.QUANTITY;
                    asset.BASE_UOM = !string.IsNullOrEmpty(model.BaseUom) ? model.BaseUom : asset.BASE_UOM;
                    asset.LAST_INVENTORY_DATE = !string.IsNullOrEmpty(model.LastInventoryDate) ? model.LastInventoryDate : asset.LAST_INVENTORY_DATE;
                    asset.LAST_INVENTORY_DOCNO = !string.IsNullOrEmpty(model.LastInventoryDocno) ? model.LastInventoryDocno : asset.LAST_INVENTORY_DOCNO;
                    asset.CAP_DATE = !string.IsNullOrEmpty(model.CapDate) ? model.CapDate : asset.CAP_DATE;
                    asset.COSTCENTER = !string.IsNullOrEmpty(model.Costcenter) ? model.Costcenter : asset.COSTCENTER;
                    asset.RESP_CCTR = !string.IsNullOrEmpty(model.RespCctr) ? model.RespCctr : asset.RESP_CCTR;
                    asset.INTERN_ORD = !string.IsNullOrEmpty(model.InternOrd) ? model.InternOrd : asset.INTERN_ORD;
                    asset.PLANT = !string.IsNullOrEmpty(model.Plant) ? model.Plant : asset.PLANT;
                    asset.LOCATION = !string.IsNullOrEmpty(model.Location) ? model.Location : asset.LOCATION;
                    asset.ROOM = !string.IsNullOrEmpty(model.Room) ? model.Room : asset.ROOM;
                    asset.PERSON_NO = !string.IsNullOrEmpty(model.PersonNo) ? model.PersonNo : asset.PERSON_NO;
                    asset.PLATE_NO = !string.IsNullOrEmpty(model.PlateNo) ? model.PlateNo : asset.PLATE_NO;
                    asset.ZZCLAS = !string.IsNullOrEmpty(model.Zzclas) ? model.Zzclas : asset.ZZCLAS;
                    asset.IN_CONSERVATION = !string.IsNullOrEmpty(model.InConservation) ? model.InConservation : asset.IN_CONSERVATION;
                    asset.PROP_IND = !string.IsNullOrEmpty(model.PropInd) ? model.PropInd : asset.PROP_IND;
                    asset.OPTIMA_ASSET_NO = !string.IsNullOrEmpty(model.OptimaAssetNo) ? model.OptimaAssetNo : asset.OPTIMA_ASSET_NO;
                    asset.OPTIMA_ASSET_PARENT_NO = !string.IsNullOrEmpty(model.OptimaAssetParentNo) ? model.OptimaAssetParentNo : asset.OPTIMA_ASSET_PARENT_NO;
                    asset.VENDOR_NO = !string.IsNullOrEmpty(model.VendorNo) ? model.VendorNo : asset.VENDOR_NO;
                    asset.INVOICE = !string.IsNullOrEmpty(model.Invoice) ? model.Invoice : asset.INVOICE;
                    asset.NotSync = true;
                    _context.AssetChangeSAPs.Update(asset);
                    await _context.SaveChangesAsync();


                    IList<Dto.AssetChangeData> oIList1 = new List<Dto.AssetChangeData>
                {
                    new Dto.AssetChangeData()
                        {
                            I_INPUT = new Dto.AssetChangeInput()
                                {
                                    COMPANYCODE = asset.COMPANYCODE,
                                    ASSET = asset.ASSET,
                                    SUBNUMBER = asset.SUBNUMBER,
                                    ASSETCLASS = asset.ASSETCLASS,
                                    POSTCAP = asset.POSTCAP,
                                    DESCRIPT = asset.DESCRIPT,
                                    DESCRIPT2 = asset.DESCRIPT2,
                                    INVENT_NO = asset.INVENT_NO,
                                    SERIAL_NO = asset.SERIAL_NO,
                                    QUANTITY = asset.QUANTITY,
                                    BASE_UOM = asset.BASE_UOM,
                                    LAST_INVENTORY_DATE = asset.LAST_INVENTORY_DATE,
                                    LAST_INVENTORY_DOCNO = asset.LAST_INVENTORY_DOCNO,
                                    CAP_DATE = asset.CAP_DATE,
                                    COSTCENTER = asset.COSTCENTER,
                                    INTERN_ORD = asset.INTERN_ORD,
                                    PLANT = asset.PLANT,
                                    LOCATION = asset.LOCATION,
                                    ROOM = asset.ROOM,
                                    PERSON_NO = asset.PERSON_NO,
                                    PLATE_NO = asset.PLATE_NO,
                                    ZZCLAS = asset.ZZCLAS,
                                    IN_CONSERVATION = asset.IN_CONSERVATION,
                                    PROP_IND = asset.PROP_IND,
                                    OPTIMA_ASSET_NO = asset.OPTIMA_ASSET_NO,
                                    OPTIMA_ASSET_PARENT_NO = asset.OPTIMA_ASSET_PARENT_NO,
                                    VENDOR_NO = asset.VENDOR_NO,
                                    RESP_CCTR = asset.RESP_CCTR,
                                    FROM_DATE = asset.FROM_DATE,
                                    INVOICE  = asset.INVOICE,
                                }
                         }
                };

                    var postUser = new Dto.AssetChange
                    {
                        Sap_function = "ZFIF_FIXED_ASSET_CHANGE",
                        Options = new Dto.AssetChangeDataOptions()
                        {
                            Api_call_timeout = 180
                        },
                        Remote_host_name = "test",
                        Data = oIList1
                    };
                    var result = "";
                    JsonContent contentJson = JsonContent.Create(postUser);
                    clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                    try
                    {
                        var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == model.ErrorAsyncId);

                        var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                        result = await httpResponse.Content.ReadAsStringAsync();

                        clientContract.Dispose();


                        if (result != "")
                        {
                            var createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);
                            
                            if (createAssetResultNew.Meta.Code == 400)
                            {
                                asset.NotSync = true;
                                _context.AssetChangeSAPs.Update(asset);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {

                            }
                            assetSyncErrorToUpdate.ManualSyncErrors = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == model.ErrorAsyncId);
                        assetSyncErrorToUpdate.ManualSyncErrors = JsonConvert.SerializeObject(ex.Message, Formatting.Indented).ToString();
                        _context.SaveChanges();
                        return BadRequest(JsonConvert.SerializeObject(ex, Formatting.Indented).ToString());

                    }
                    return Ok("Ok");
                }
                return Ok("No Asset");
            }
        }
    }
}
