using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Dto.Common;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/requestbudgetforecastmaterials")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RequestBudgetForecastMaterialsController : GenericApiController<Model.RequestBudgetForecastMaterial, Dto.RequestBudgetForecastMaterial>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public RequestBudgetForecastMaterialsController(ApplicationDbContext context, IRequestBudgetForecastMaterialsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string jsonFilter, string materialIds, int? orderId, string requestBudgetForecastIds, string includes, bool showAll = true)
        {
            List<Model.RequestBudgetForecastMaterial> items = null;
            IEnumerable<Dto.RequestBudgetForecastMaterial> itemsResult = null;
            RequestFilter requestFilter = null;
            List<int?> cIds = null;
            List<int?> eIds = null;

            includes = includes + ",OfferType";

            requestFilter = jsonFilter != null ? JsonConvert.DeserializeObject<RequestFilter>(jsonFilter) : new RequestFilter();


            if (materialIds != null && !materialIds.StartsWith("["))
			{
                materialIds = "[" + materialIds + "]";
			}

            if (requestBudgetForecastIds != null && !requestBudgetForecastIds.StartsWith("["))
			{
                requestBudgetForecastIds = "[" + requestBudgetForecastIds + "]";
			}

            if (requestFilter.RequestBudgetForecastIds == null || requestFilter.RequestBudgetForecastIds.Count == 0)
            {
                if(orderId == null || orderId == 0)
                {
					requestBudgetForecastIds = "[-1]";
				}
                
            }

            if ((materialIds != null) && (materialIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(materialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((requestBudgetForecastIds != null) && (requestBudgetForecastIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(requestBudgetForecastIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IRequestBudgetForecastMaterialsRepository).GetByFilters(requestFilter, filter, includes, sortColumn, sortDirection, page, pageSize, cIds, orderId, eIds, showAll).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.RequestBudgetForecastMaterial>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRequestBudgetForecastMaterialsRepository).GetCountByFilters(requestFilter, filter, cIds, orderId, eIds, showAll);
                var pagedResult = new Dto.PagedResult<Dto.RequestBudgetForecastMaterial>(itemsResult, new Dto.PagingInfo()
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


        [HttpPost]
        [Route("add")]
        public async virtual Task<IActionResult> AddEmployeeDivision([FromBody] Dto.RequestBudgetForecastMaterialAdd requestBudgetForecastMaterialAdd)
        {
            Model.RequestBudgetForecastMaterial budgetFor = null;
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.OfferMaterial offerMaterial = null;
            Model.Contract contract = null;
            Model.AppState appState = null;
            Model.Offer offer = null;
            int sumQuantity = 0;
            int sumTotalOrderQuantity = 0;

            offer = await _context.Set<Model.Offer>().Where(u => u.Id == requestBudgetForecastMaterialAdd.OfferId).SingleAsync();
            appState = await _context.Set<Model.AppState>().Where(u => u.Code == "NEW").SingleAsync();
            requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Include(o => o.OfferType).Include(b => b.BudgetForecast).Where(u => u.Id == requestBudgetForecastMaterialAdd.RequestBudgetForecastId).SingleAsync();

            for (int i = 0; i < requestBudgetForecastMaterialAdd.MaterialIds.Length; i++)
            {
                offerMaterial = await _context.Set<Model.OfferMaterial>().Where(o => o.IsDeleted == false && o.Guid == offer.Guid && o.Validated == true && o.OfferId == requestBudgetForecastMaterialAdd.OfferId && o.MaterialId == requestBudgetForecastMaterialAdd.MaterialIds[i]).SingleAsync();

                budgetFor = new Model.RequestBudgetForecastMaterial()
                {
                    RequestBudgetForecastId = requestBudgetForecastMaterialAdd.RequestBudgetForecastId,
                    MaterialId = requestBudgetForecastMaterialAdd.MaterialIds[i],
                    Price = offerMaterial.Price,
                    PriceRon = offerMaterial.PriceRon,
                    Quantity = (int)offerMaterial.Quantity,
                    QuantityRem = (int)offerMaterial.Quantity,
                    Value = offerMaterial.Value,
                    ValueRon = offerMaterial.ValueRon,
                    ValueRem = offerMaterial.ValueRon,
                    ValueRemRon = offerMaterial.ValueRon,
                    Guid = requestBudgetForecast.Guid,
                    AppStateId = appState.Id,
                    OfferMaterialId = offerMaterial.Id,
                    OfferTypeId = requestBudgetForecast.OfferTypeId,
                    MaxQuantity = (int)requestBudgetForecast.MaxQuantity,
                    MaxValueRon = requestBudgetForecast.MaxValueRon,
                    MaxValue = requestBudgetForecast.MaxValue
                };

                
                _context.Add(budgetFor);

                _context.SaveChanges();
            }

            List<string> materials = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterialAdd.RequestBudgetForecastId).Select(a => a.Material.Code).ToListAsync();

            var mat = string.Join(", ", materials);

            requestBudgetForecast.Materials = mat;

            if(requestBudgetForecast.OfferType.Code == "O-V")
			{
                sumQuantity = 1;
            }
			else
			{
                sumQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterialAdd.RequestBudgetForecastId).Sum(a => a.Quantity);
            }
            
            decimal sumValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterialAdd.RequestBudgetForecastId).Sum(a => a.Value);
            decimal sumValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterialAdd.RequestBudgetForecastId).Sum(a => a.ValueRon);

            requestBudgetForecast.Quantity = sumQuantity;
            requestBudgetForecast.Value = sumValue;
            requestBudgetForecast.ValueRon = sumValueRon;

            _context.Update(requestBudgetForecast);
            _context.SaveChanges();

            List<Model.RequestBudgetForecast> requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(a => a.Guid == requestBudgetForecast.Guid).ToListAsync();


            if (requestBudgetForecast.OfferType.Code == "O-V")
            {
                sumTotalOrderQuantity = 1;
            }
            else
            {
                sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.Quantity);
            }

            decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.Value);
            decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.ValueRon);

            for (int o = 0; o < requestBudgetForecasts.Count; o++)
			{
                requestBudgetForecasts[o].NeedBudget = false;
                requestBudgetForecasts[o].NeedBudgetValue = 0;

                requestBudgetForecasts[o].TotalOrderQuantity = sumTotalOrderQuantity;
                requestBudgetForecasts[o].TotalOrderValue = sumTotalOrderValue;
                requestBudgetForecasts[o].TotalOrderValueRon = sumTotalOrderValueRon;

                requestBudgetForecasts[o].NeedContract = false;
                requestBudgetForecasts[o].NeedContractValue = 0;

                contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBudgetForecasts[o].ContractId).FirstOrDefaultAsync();

                if (contract != null && contract.ContractAmount != null && contract.ContractAmount.AmountRonRem < requestBudgetForecasts[0].TotalOrderValueRon)
                {
                    requestBudgetForecasts[o].NeedContract = true;
                    requestBudgetForecasts[o].NeedContractValue = requestBudgetForecasts[o].TotalOrderValueRon - contract.ContractAmount.AmountRonRem;
                }

                if (requestBudgetForecasts[o].BudgetForecast.TotalRem < requestBudgetForecasts[o].ValueRon)
                {
                    requestBudgetForecasts[o].NeedBudget = true;
                    requestBudgetForecasts[o].NeedBudgetValue = requestBudgetForecasts[o].ValueRon - requestBudgetForecasts[o].BudgetForecast.TotalRem;
                }

                _context.Update(requestBudgetForecasts[o]);
                _context.SaveChanges();
            }

            /*
            //contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBudgetForecast.ContractId).SingleAsync();

            //if (contract.ContractAmount.AmountRonRem < requestBudgetForecast.MaxValueRon)
            //{
            //    requestBudgetForecast.NeedContract = true;

            //    _context.Update(requestBudgetForecast);

            //    _context.SaveChanges();
            //}

            //if (requestBudgetForecast.BudgetForecast.TotalRem < requestBudgetForecast.ValueRon)
            //{
            //    requestBudgetForecast.NeedBudget = true;
            //    requestBudgetForecast.NeedBudgetValue = 0;

            //    requestBudgetForecast.NeedBudgetValue = requestBudgetForecast.ValueRon - requestBudgetForecast.BudgetForecast.TotalRem;

            //    _context.Update(requestBudgetForecast);

            //    _context.SaveChanges();
            //}
            */

            return Ok(StatusCode(200));
        }

        [HttpDelete("remove/{id}")]
        public async virtual Task<IActionResult> DeleteEmployeeDivision(int id)
        {
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.RequestBudgetForecastMaterial reqBgt = null;
            Model.Contract contract = null;

            reqBgt = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.Id == id).SingleAsync();
            requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(a => a.Id == reqBgt.RequestBudgetForecastId).SingleAsync();

            if (reqBgt != null)
            {
                reqBgt.IsDeleted = true;
                reqBgt.ModifiedAt = DateTime.Now;
                _context.Update(reqBgt);
                _context.SaveChanges();

                List<string> materials = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == reqBgt.RequestBudgetForecastId).Select(a => a.Material.Code).ToListAsync();


                var mat = string.Join(", ", materials);

                requestBudgetForecast.Materials = mat;


                int sumQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == reqBgt.RequestBudgetForecastId).Sum(a => a.Quantity);
                decimal sumValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == reqBgt.RequestBudgetForecastId).Sum(a => a.Value);
                decimal sumValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == reqBgt.RequestBudgetForecastId).Sum(a => a.ValueRon);

                requestBudgetForecast.Quantity = sumQuantity;
                requestBudgetForecast.Value = sumValue;
                requestBudgetForecast.ValueRon = sumValueRon;
                requestBudgetForecast.NeedContract = false;
                requestBudgetForecast.NeedBudget = false;

                _context.Update(requestBudgetForecast);
                _context.SaveChanges();

                List<Model.RequestBudgetForecast> requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Where(a => a.Guid == requestBudgetForecast.Guid).ToListAsync();

                int sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.Quantity);
                decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.Value);
                decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.ValueRon);

                for (int o = 0; o < requestBudgetForecasts.Count; o++)
                {
                    requestBudgetForecasts[o].TotalOrderQuantity = sumTotalOrderQuantity;
                    requestBudgetForecasts[o].TotalOrderValue = sumTotalOrderValue;
                    requestBudgetForecasts[o].TotalOrderValueRon = sumTotalOrderValueRon;

                    requestBudgetForecasts[o].NeedContract = false;
                    requestBudgetForecasts[o].NeedContractValue = 0;

                    contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBudgetForecasts[o].ContractId).SingleAsync();

                    if (contract.ContractAmount.AmountRonRem < requestBudgetForecasts[0].TotalOrderValueRon)
                    {
                        requestBudgetForecasts[o].NeedContract = true;
                        requestBudgetForecasts[o].NeedContractValue = requestBudgetForecasts[o].TotalOrderValueRon - contract.ContractAmount.AmountRonRem;
                    }

                    _context.Update(requestBudgetForecasts[o]);
                    _context.SaveChanges();
                }

                //contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBudgetForecast.ContractId).SingleAsync();

                //if (contract.ContractAmount.AmountRonRem < requestBudgetForecast.MaxValueRon)
                //{
                //    requestBudgetForecast.NeedContract = true;

                //    _context.Update(requestBudgetForecast);

                //    _context.SaveChanges();
                //}

                if (requestBudgetForecast.BudgetForecast.TotalRem < requestBudgetForecast.ValueRon)
                {
                    requestBudgetForecast.NeedBudget = true;

                    _context.Update(requestBudgetForecast);

                    _context.SaveChanges();
                }
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }


        [HttpPost("update")]
        public async virtual Task<RequestResult> MaterialUpdate([FromBody] List<RequestBudgetForecastMaterialUpdate> materialToUpdates)
        {
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            Model.Contract contract = null;
            int sumQuantity = 0;
            int sumTotalOrderQuantity = 0;

            for (int i = 0; i < materialToUpdates.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Include(o => o.OfferMaterial).ThenInclude(o => o.Offer).ThenInclude(o => o.OfferType)
                    .Where(a => a.Id == materialToUpdates[i].Id).SingleAsync();

                requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(u => u.Id == requestBudgetForecastMaterial.RequestBudgetForecastId).SingleAsync();

                if (requestBudgetForecastMaterial != null)
                {
                    requestBudgetForecastMaterial.OfferTypeId = requestBudgetForecast.OfferTypeId;
                    requestBudgetForecastMaterial.Quantity = (int)materialToUpdates[i].Quantity;
                    requestBudgetForecastMaterial.QuantityRem = (int)materialToUpdates[i].Quantity;
                    requestBudgetForecastMaterial.Value = requestBudgetForecastMaterial.Price * requestBudgetForecastMaterial.Quantity;
                    requestBudgetForecastMaterial.ValueRon = requestBudgetForecastMaterial.PriceRon * requestBudgetForecastMaterial.Quantity;
                    requestBudgetForecastMaterial.ValueRem = requestBudgetForecastMaterial.Price * requestBudgetForecastMaterial.QuantityRem;
                    requestBudgetForecastMaterial.ValueRemRon = requestBudgetForecastMaterial.PriceRon * requestBudgetForecastMaterial.QuantityRem;

                    if(requestBudgetForecastMaterial.OfferMaterial.Offer.OfferType.Code == "O-V")
					{
                        requestBudgetForecastMaterial.Value = materialToUpdates[i].ValueRon;
                        requestBudgetForecastMaterial.ValueRon = materialToUpdates[i].ValueRon;
                        requestBudgetForecastMaterial.ValueRem = materialToUpdates[i].ValueRon;
                        requestBudgetForecastMaterial.ValueRemRon = materialToUpdates[i].ValueRon;
                    }

                    _context.Update(requestBudgetForecastMaterial);
                    _context.SaveChanges();
                }
            }


            List<string> materials = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterial.RequestBudgetForecastId).Select(a => a.Material.Code).ToListAsync();


            var mat = string.Join(", ", materials);

            requestBudgetForecast.Materials = mat;

            if (requestBudgetForecast.OfferType.Code == "O-V")
            {
                sumQuantity = 1;
            }
            else
            {
                sumQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterial.RequestBudgetForecastId).Sum(a => a.Quantity);
            }



            decimal sumValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterial.RequestBudgetForecastId).Sum(a => a.Value);
            decimal sumValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecastMaterial.RequestBudgetForecastId).Sum(a => a.ValueRon);

            requestBudgetForecast.Quantity = sumQuantity;
            requestBudgetForecast.Value = sumValue;
            requestBudgetForecast.ValueRon = sumValueRon;

            _context.Update(requestBudgetForecast);
            _context.SaveChanges();

            List<Model.RequestBudgetForecast> requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(a => a.Guid == requestBudgetForecast.Guid).ToListAsync();

            if (requestBudgetForecast.OfferType.Code == "O-V")
            {
                sumTotalOrderQuantity = 1;
            }
            else
            {
                sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.Quantity);
            }

            
            decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.Value);
            decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecast.Guid).Sum(a => a.ValueRon);

            for (int o = 0; o < requestBudgetForecasts.Count; o++)
            {
                requestBudgetForecasts[o].NeedBudget = false;
                requestBudgetForecasts[o].NeedBudgetValue = 0;
                requestBudgetForecasts[o].TotalOrderQuantity = sumTotalOrderQuantity;
                requestBudgetForecasts[o].TotalOrderValue = sumTotalOrderValue;
                requestBudgetForecasts[o].TotalOrderValueRon = sumTotalOrderValueRon;

                requestBudgetForecasts[o].NeedContract = false;
                requestBudgetForecasts[o].NeedContractValue = 0;

                contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBudgetForecasts[o].ContractId).SingleAsync();

                if (contract.ContractAmount.AmountRonRem < requestBudgetForecasts[0].TotalOrderValueRon)
                {
                    requestBudgetForecasts[o].NeedContract = true;
                    requestBudgetForecasts[o].NeedContractValue = requestBudgetForecasts[o].TotalOrderValueRon - contract.ContractAmount.AmountRonRem;
                }

                if (requestBudgetForecasts[o].BudgetForecast.TotalRem < requestBudgetForecasts[o].ValueRon)
                {
                    requestBudgetForecasts[o].NeedBudget = true;
                    requestBudgetForecasts[o].NeedBudgetValue = requestBudgetForecasts[o].ValueRon - requestBudgetForecasts[o].BudgetForecast.TotalRem;
                }

                _context.Update(requestBudgetForecasts[o]);
                _context.SaveChanges();
            }

            

            return new Model.RequestResult { Success = true, Message = "Datele au fost actualizate cu sucess!", RequestId = 0 };
        }

        [HttpPost("updateAssetParent")]
        public async virtual Task<RequestResult> MaterialUpdateAssetParent([FromBody] List<RequestBudgetForecastMaterialUpdateAssetParent> materialToUpdates)
        {
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            Model.Asset asset = null;

            for (int i = 0; i < materialToUpdates.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Include(o => o.OfferMaterial).ThenInclude(o => o.Offer).ThenInclude(o => o.OfferType)
                    .Where(a => a.Id == materialToUpdates[i].Id).SingleAsync();

                asset = await _context.Set<Model.Asset>()
                   .Where(a => a.Id == materialToUpdates[i].AssetId).SingleOrDefaultAsync();

                if (requestBudgetForecastMaterial != null)
                {
                    requestBudgetForecastMaterial.HasParentAsset = asset != null ? true : false;
                    requestBudgetForecastMaterial.ParentAsset = asset != null ? asset.InvNo : String.Empty;

                    _context.Update(requestBudgetForecastMaterial);
                    _context.SaveChanges();
                }
            }

            return new Model.RequestResult { Success = true, Message = "Datele au fost actualizate cu sucess!", RequestId = 0 };
        }
    }
}
