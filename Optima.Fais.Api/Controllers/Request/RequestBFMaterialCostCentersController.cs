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
    [Route("api/requestbudgetforecastmaterialcostcenters")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RequestBFMaterialCostCentersController : GenericApiController<Model.RequestBFMaterialCostCenter, Dto.RequestBFMaterialCostCenter>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public RequestBFMaterialCostCentersController(ApplicationDbContext context, IRequestBFMaterialCostCentersRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string costCenterIds, string requestBudgetForecastMaterialIds, int? orderId, string includes, bool reception = false)
        {
            List<Model.RequestBFMaterialCostCenter> items = null;
            IEnumerable<Dto.RequestBFMaterialCostCenter> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;
            List<int?> oIds = null;

            includes = includes + ",OfferType,RequestBudgetForecastMaterial";

            if (costCenterIds != null && !costCenterIds.StartsWith("["))
			{
                costCenterIds = "[" + costCenterIds + "]";
			}

			if (requestBudgetForecastMaterialIds != null && !requestBudgetForecastMaterialIds.StartsWith("["))
			{
                requestBudgetForecastMaterialIds = "[" + requestBudgetForecastMaterialIds + "]";
			}

            if (requestBudgetForecastMaterialIds == null)
            {
                if(orderId == null || orderId == 0)
                {
					requestBudgetForecastMaterialIds = "[-1]";
				}
                
            }

            if ((costCenterIds != null) && (costCenterIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(costCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((requestBudgetForecastMaterialIds != null) && (requestBudgetForecastMaterialIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(requestBudgetForecastMaterialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IRequestBFMaterialCostCentersRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds, orderId, reception).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.RequestBFMaterialCostCenter>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRequestBFMaterialCostCentersRepository).GetCountByFilters(filter, cIds, eIds, orderId, reception);
                var pagedResult = new Dto.PagedResult<Dto.RequestBFMaterialCostCenter>(itemsResult, new Dto.PagingInfo()
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
        [Route("reception", Order = -1)]
        public virtual IActionResult GetReception(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string orderId, string includes, bool reception = false)
        {
            List<Model.RequestBFMaterialCostCenter> items = null;
            IEnumerable<Dto.RequestBFMaterialCostCenter> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;
            List<int?> oIds = null;

            includes = includes + ",OfferType,RequestBudgetForecastMaterial";

            if (orderId != null && !orderId.StartsWith("["))
            {
                orderId = "[" + orderId + "]";
            }

            if ((orderId != null) && (orderId.Length > 0)) oIds = JsonConvert.DeserializeObject<string[]>(orderId).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IRequestBFMaterialCostCentersRepository).GetByReceptionFilters(filter, includes, sortColumn, sortDirection, page, pageSize, oIds, reception).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.RequestBFMaterialCostCenter>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRequestBFMaterialCostCentersRepository).GetCountByReceptionFilters(filter, oIds, reception);
                var pagedResult = new Dto.PagedResult<Dto.RequestBFMaterialCostCenter>(itemsResult, new Dto.PagingInfo()
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
        public async virtual Task<IActionResult> AddEmployeeDivision([FromBody] Dto.RequestBFMaterialCostCenterAdd requestBFMaterialCostCenterAdd)
        {
            Model.RequestBFMaterialCostCenter budgetFor = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            Model.Contract contract = null;
            Model.AppState appState = null;

            appState = await _context.Set<Model.AppState>().Where(u => u.Code == "PENDING").SingleAsync();
            requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>().Include(b => b.RequestBudgetForecast).ThenInclude(b => b.BudgetForecast).Where(u => u.Id == requestBFMaterialCostCenterAdd.RequestBudgetForecastMaterialId).SingleAsync();

            for (int i = 0; i < requestBFMaterialCostCenterAdd.CostCenterIds.Length; i++)
            {
                budgetFor = new Model.RequestBFMaterialCostCenter()
                {
                    RequestBudgetForecastMaterialId = requestBFMaterialCostCenterAdd.RequestBudgetForecastMaterialId,
                    CostCenterId = requestBFMaterialCostCenterAdd.CostCenterIds[i],
                    Price = requestBudgetForecastMaterial.Price,
                    PriceRon = requestBudgetForecastMaterial.PriceRon,
                    Quantity = (int)requestBudgetForecastMaterial.Quantity,
                    QuantityRem = (int)requestBudgetForecastMaterial.QuantityRem,
                    Value = requestBudgetForecastMaterial.Value,
                    ValueRon = requestBudgetForecastMaterial.ValueRon,
                    ValueRem = requestBudgetForecastMaterial.ValueRem,
                    ValueRemRon = requestBudgetForecastMaterial.ValueRemRon,
                    Guid = requestBudgetForecastMaterial.Guid,
                    AppStateId = appState.Id,
                    OfferTypeId = requestBudgetForecastMaterial.OfferTypeId,
                    MaxQuantity = (int)requestBudgetForecastMaterial.MaxQuantity,
                    MaxValueRon = requestBudgetForecastMaterial.MaxValueRon,
                    MaxValue = requestBudgetForecastMaterial.MaxValue,
                    OfferMaterialId = requestBudgetForecastMaterial.OfferMaterialId,
                    OrderId = requestBudgetForecastMaterial.OrderId,
                    OrderMaterialId = requestBudgetForecastMaterial.OrderMaterialId,
                    Multiple = requestBudgetForecastMaterial.Multiple
                };

                _context.Add(budgetFor);

                _context.SaveChanges();
            }

			int sumQuantity = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == requestBFMaterialCostCenterAdd.RequestBudgetForecastMaterialId).Sum(a => a.Quantity);
			decimal sumValue = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == requestBFMaterialCostCenterAdd.RequestBudgetForecastMaterialId).Sum(a => a.Value);
			decimal sumValueRon = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == requestBFMaterialCostCenterAdd.RequestBudgetForecastMaterialId).Sum(a => a.ValueRon);

			requestBudgetForecastMaterial.TotalCostCenterQuantity = sumQuantity;
			requestBudgetForecastMaterial.TotalCostCenterValue = sumValue;
			requestBudgetForecastMaterial.TotalCostCenterValueRon = sumValueRon;

			_context.Update(requestBudgetForecastMaterial);
			_context.SaveChanges();

            /*
			//List<Model.RequestBudgetForecastMaterial> requestBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.Guid == requestBudgetForecastMaterial.Guid).ToListAsync();

			//int sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.Quantity);
			//decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.Value);
			//decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.ValueRon);

			//for (int o = 0; o < requestBFMaterials.Count; o++)
			//{
			//	requestBFMaterials[o].TotalCostCenterQuantity = sumTotalOrderQuantity;
			//	requestBFMaterials[o].TotalCostCenterValue = sumTotalOrderValue;
			//	requestBFMaterials[o].TotalCostCenterValueRon = sumTotalOrderValueRon;

			//	_context.Update(requestBFMaterials[o]);
			//	_context.SaveChanges();
			//}
            */

			return Ok(StatusCode(200));
        }

		[HttpPost]
		[Route("updateemployee")]
		public async virtual Task<IActionResult> UpdateEmployee([FromBody] Dto.RequestBFMaterialCostCenterEmployeeUpdate requestBFMaterialCostCenterEmployeeUpdate)
		{
			Model.RequestBFMaterialCostCenter requestBFMaterialCost = null;

			requestBFMaterialCost = await _context.Set<Model.RequestBFMaterialCostCenter>()
                .Where(u => u.Id == requestBFMaterialCostCenterEmployeeUpdate.RequestBFMaterialCostCenterId)
                .SingleAsync();

            if(requestBFMaterialCost != null)
            {
                requestBFMaterialCost.EmployeeId = requestBFMaterialCostCenterEmployeeUpdate.EmployeeId;
                requestBFMaterialCost.ModifiedAt = DateTime.Now;
                _context.Update(requestBFMaterialCost);
                _context.SaveChanges();

			}

			return Ok(StatusCode(200));
		}

		[HttpDelete("remove/{id}")]
        public async virtual Task<IActionResult> DeleteEmployeeDivision(int id)
        {
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            Model.RequestBFMaterialCostCenter reqBFMaterialCostCenter = null;
            Model.Contract contract = null;

            reqBFMaterialCostCenter = await _context.Set<Model.RequestBFMaterialCostCenter>().Where(a => a.Id == id).SingleAsync();
            requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>().Include(b => b.RequestBudgetForecast).ThenInclude(b => b.BudgetForecast).Where(a => a.Id == reqBFMaterialCostCenter.RequestBudgetForecastMaterialId).SingleAsync();

            if (reqBFMaterialCostCenter != null)
            {
                reqBFMaterialCostCenter.IsDeleted = true;
                reqBFMaterialCostCenter.ModifiedAt = DateTime.Now;
                _context.Update(reqBFMaterialCostCenter);
                _context.SaveChanges();


                int sumQuantity = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == reqBFMaterialCostCenter.RequestBudgetForecastMaterialId).Sum(a => a.Quantity);
                decimal sumValue = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == reqBFMaterialCostCenter.RequestBudgetForecastMaterialId).Sum(a => a.Value);
                decimal sumValueRon = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == reqBFMaterialCostCenter.RequestBudgetForecastMaterialId).Sum(a => a.ValueRon);

                requestBudgetForecastMaterial.TotalCostCenterQuantity = sumQuantity;
                requestBudgetForecastMaterial.TotalCostCenterValue = sumValue;
                requestBudgetForecastMaterial.TotalCostCenterValueRon = sumValueRon;

                _context.Update(requestBudgetForecastMaterial);
                _context.SaveChanges();

                //List<Model.RequestBudgetForecastMaterial> requestBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.Guid == requestBudgetForecastMaterial.Guid).ToListAsync();

                //int sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.Quantity);
                //decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.Value);
                //decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.ValueRon);

                //for (int o = 0; o < requestBFMaterials.Count; o++)
                //{
                //	requestBFMaterials[o].TotalCostCenterQuantity = sumTotalOrderQuantity;
                //	requestBFMaterials[o].TotalCostCenterValue = sumTotalOrderValue;
                //	requestBFMaterials[o].TotalCostCenterValueRon = sumTotalOrderValueRon;

                //	_context.Update(requestBFMaterials[o]);
                //	_context.SaveChanges();
                //}


            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }

		[HttpDelete("removeemployee/{id}")]
		public async virtual Task<IActionResult> DeleteEmployee(int id)
		{
			Model.RequestBFMaterialCostCenter reqBFMaterialCostCenter = null;

			reqBFMaterialCostCenter = await _context.Set<Model.RequestBFMaterialCostCenter>().Where(a => a.Id == id).SingleAsync();

			if (reqBFMaterialCostCenter != null)
			{
				reqBFMaterialCostCenter.EmployeeId = null;
				reqBFMaterialCostCenter.ModifiedAt = DateTime.Now;
				_context.Update(reqBFMaterialCostCenter);
				_context.SaveChanges();
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
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            Model.RequestBFMaterialCostCenter requestBFMaterialCostCenter = null;

            for (int i = 0; i < materialToUpdates.Count; i++)
            {
                requestBFMaterialCostCenter = await _context.Set<Model.RequestBFMaterialCostCenter>()
                    .Where(a => a.Id == materialToUpdates[i].Id).SingleAsync();

                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Include(b => b.RequestBudgetForecast).ThenInclude(b => b.BudgetForecast)
                    .Include(o => o.OfferType)
                    .Where(u => u.Id == requestBFMaterialCostCenter.RequestBudgetForecastMaterialId).SingleAsync();

                if (requestBFMaterialCostCenter != null)
                {
                    requestBFMaterialCostCenter.OfferTypeId = requestBudgetForecastMaterial.OfferTypeId;
                    requestBFMaterialCostCenter.Quantity = (int)materialToUpdates[i].Quantity;
                    requestBFMaterialCostCenter.QuantityRem = (int)materialToUpdates[i].Quantity;
                    requestBFMaterialCostCenter.Value = requestBFMaterialCostCenter.Price * requestBFMaterialCostCenter.Quantity;
                    requestBFMaterialCostCenter.ValueRon = requestBFMaterialCostCenter.PriceRon * requestBFMaterialCostCenter.Quantity;
                    requestBFMaterialCostCenter.ValueRem = requestBFMaterialCostCenter.Price * requestBFMaterialCostCenter.QuantityRem;
                    requestBFMaterialCostCenter.ValueRemRon = requestBFMaterialCostCenter.PriceRon * requestBFMaterialCostCenter.QuantityRem;

					if (requestBudgetForecastMaterial.OfferType.Code == "O-V")
                    {
                        requestBFMaterialCostCenter.Value = materialToUpdates[i].ValueRon;
                        requestBFMaterialCostCenter.ValueRon = materialToUpdates[i].ValueRon;
                        requestBFMaterialCostCenter.ValueRem = materialToUpdates[i].ValueRon;
                        requestBFMaterialCostCenter.ValueRemRon = materialToUpdates[i].ValueRon;
                    }

                    _context.Update(requestBFMaterialCostCenter);
                    _context.SaveChanges();
                }
            }


            int sumQuantity = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == requestBFMaterialCostCenter.RequestBudgetForecastMaterialId).Sum(a => a.Quantity);
            decimal sumValue = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == requestBFMaterialCostCenter.RequestBudgetForecastMaterialId).Sum(a => a.Value);
            decimal sumValueRon = _context.Set<Model.RequestBFMaterialCostCenter>().AsNoTracking().Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == requestBFMaterialCostCenter.RequestBudgetForecastMaterialId).Sum(a => a.ValueRon);

            requestBudgetForecastMaterial.TotalCostCenterQuantity = sumQuantity;
            requestBudgetForecastMaterial.TotalCostCenterValue = sumValue;
            requestBudgetForecastMaterial.TotalCostCenterValueRon = sumValueRon;

            _context.Update(requestBudgetForecastMaterial);
            _context.SaveChanges();

            //List<Model.RequestBudgetForecastMaterial> requestBFMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.Guid == requestBudgetForecastMaterial.Guid).ToListAsync();

            //int sumTotalOrderQuantity = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.Quantity);
            //decimal sumTotalOrderValue = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.Value);
            //decimal sumTotalOrderValueRon = _context.Set<Model.RequestBudgetForecastMaterial>().AsNoTracking().Where(a => a.IsDeleted == false && a.Guid == requestBudgetForecastMaterial.Guid).Sum(a => a.ValueRon);

            //for (int o = 0; o < requestBFMaterials.Count; o++)
            //{
            //	requestBFMaterials[o].TotalCostCenterQuantity = sumTotalOrderQuantity;
            //	requestBFMaterials[o].TotalCostCenterValue = sumTotalOrderValue;
            //	requestBFMaterials[o].TotalCostCenterValueRon = sumTotalOrderValueRon;

            //	_context.Update(requestBFMaterials[o]);
            //	_context.SaveChanges();
            //}



            return new Model.RequestResult { Success = true, Message = "Datele au fost actualizate cu sucess!", RequestId = 0 };
        }

        [HttpPost("updatemultiple")]
        public async virtual Task<RequestResult> MultipleUpdate([FromBody] List<RequestBudgetForecastMaterialMultiple> materialToUpdates)
        {
            Model.RequestBFMaterialCostCenter requestBFMaterialCostCenter = null;

            for (int i = 0; i < materialToUpdates.Count; i++)
            {
                requestBFMaterialCostCenter = await _context.Set<Model.RequestBFMaterialCostCenter>().Where(a => a.Id == materialToUpdates[i].Id).SingleAsync();

                if (requestBFMaterialCostCenter != null)
                {
                    requestBFMaterialCostCenter.Multiple = materialToUpdates[i].Multiple;

                    _context.Update(requestBFMaterialCostCenter);
                    _context.SaveChanges();
                }
            }
            return new Model.RequestResult { Success = true, Message = "Datele au fost actualizate cu sucess!", RequestId = 0 };
        }
	}
}
