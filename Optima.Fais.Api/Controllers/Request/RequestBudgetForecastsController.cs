using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/requestbudgetforecasts")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class RequestBudgetForecastsController : GenericApiController<Model.RequestBudgetForecast, Dto.RequestBudgetForecast>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public RequestBudgetForecastsController(ApplicationDbContext context, IRequestBudgetForecastsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string requestIds, string budgetForecastIds, string includes, bool needBudget = false)
        {
            RequestBudgetForecastTotal depTotal = null;
            List<Model.RequestBudgetForecast> items = null;
            IEnumerable<Dto.RequestBudgetForecast> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;

            includes = includes + ",OfferType";

            if (requestIds != null && !requestIds.StartsWith("["))
			{
                requestIds = "[" + requestIds + "]";
			}

            if(requestIds == null)
			{
                requestIds = "[" + -1 + "]";
            }

			if (budgetForecastIds != null && !budgetForecastIds.StartsWith("["))
			{
                budgetForecastIds = "[" + budgetForecastIds + "]";
			}

			if ((requestIds != null) && (requestIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(requestIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((budgetForecastIds != null) && (budgetForecastIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(budgetForecastIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IRequestBudgetForecastsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds, needBudget).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.RequestBudgetForecast>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRequestBudgetForecastsRepository).GetCountByFilters(filter, cIds, eIds, needBudget);
                var pagedResult = new Dto.PagedResult<Dto.RequestBudgetForecast>(itemsResult, new Dto.PagingInfo()
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
        [Route("detailneedbudgetui", Order = -1)]
        public virtual IActionResult GetDetailneedbudgetui(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string requestIds, string budgetForecastIds, string includes, bool needBudget = false)
        {
            RequestBudgetForecastTotal depTotal = null;
            List<Model.RequestBudgetForecast> items = null;
            IEnumerable<Dto.RequestBudgetForecast> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;

            includes = includes + ",OfferType";

            if (requestIds != null && !requestIds.StartsWith("["))
            {
                requestIds = "[" + requestIds + "]";
            }

            if (budgetForecastIds != null && !budgetForecastIds.StartsWith("["))
            {
                budgetForecastIds = "[" + budgetForecastIds + "]";
            }

            if ((requestIds != null) && (requestIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(requestIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((budgetForecastIds != null) && (budgetForecastIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(budgetForecastIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IRequestBudgetForecastsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds, needBudget).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.RequestBudgetForecast>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRequestBudgetForecastsRepository).GetCountByFilters(filter, cIds, eIds, needBudget);
                var pagedResult = new Dto.PagedResult<Dto.RequestBudgetForecast>(itemsResult, new Dto.PagingInfo()
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

        [HttpGet("data")]
        public virtual async Task<IActionResult> GetDownloadData()
        {
            IRequestBudgetForecastsRepository repo = _itemsRepository as IRequestBudgetForecastsRepository;
            var items = await repo.GetAllIncludingChildrensAsync();

            return Ok(items.Select(i => _mapper.Map<Dto.RequestBudgetForecastData>(i)));
        }

        [HttpGet]
        [Route("filtered", Order = -1)]
        public virtual IActionResult GetFiltered(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string requestIds, string budgetForecastIds, string includes, bool needBudget = false)
        {
            List<Model.RequestBudgetForecast> items = null;
            IEnumerable<Dto.RequestBudgetForecast> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;


            if (requestIds != null && !requestIds.StartsWith("["))
            {
                requestIds = "[" + requestIds + "]";
            }

            if (budgetForecastIds != null && !budgetForecastIds.StartsWith("["))
            {
                budgetForecastIds = "[" + budgetForecastIds + "]";
            }

            if ((requestIds != null) && (requestIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(requestIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((budgetForecastIds != null) && (budgetForecastIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(budgetForecastIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IRequestBudgetForecastsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds, needBudget).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.RequestBudgetForecast>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IRequestBudgetForecastsRepository).GetCountByFilters(filter, cIds, eIds, needBudget);
                var pagedResult = new Dto.PagedResult<Dto.RequestBudgetForecast>(itemsResult, new Dto.PagingInfo()
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
        public async virtual Task<IActionResult> AddEmployeeDivision([FromBody] Dto.RequestBudgetForecastAdd requestBudgetForecastAdd)
        {
            Model.RequestBudgetForecast budgetFor = null;

            for (int i = 0; i < requestBudgetForecastAdd.BudgetForecastIds.Length; i++)
            {
                var request = await _context.Set<Model.Request>().Where(u => u.Id == requestBudgetForecastAdd.RequestId).SingleAsync();


                budgetFor = new Model.RequestBudgetForecast()
                {
                    RequestId = requestBudgetForecastAdd.RequestId,
                    BudgetForecastId = requestBudgetForecastAdd.BudgetForecastIds[i]
                };

                _context.Add(budgetFor);

                _context.SaveChanges();

               
            }

            return Ok(StatusCode(200));
        }

        [HttpPost]
        [Route("contractAdd")]
        public async virtual Task<RequestResult> AddContract([FromBody] Dto.RequestBudgetForecastContractAdd requestBudgetForecastContractAdd)
        {
            Model.RequestResult requestResult = null;
            Model.RequestBudgetForecast budgetFor = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = null;
            Model.Contract contract = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

                if (user != null)
                {
                    _context.UserId = user.Id.ToString();


                    budgetFor = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(u => u.Id == requestBudgetForecastContractAdd.RequestBudgetForecastId).SingleAsync();

                    requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Include(c => c.Contract).ThenInclude(c => c.ContractAmount).Where(b => b.Guid == budgetFor.Guid).ToListAsync();

					for (int i = 0; i < requestBudgetForecasts.Count; i++)
					{
						requestBudgetForecasts[i].ContractId = requestBudgetForecastContractAdd.ContractId;
                        requestBudgetForecasts[i].NeedBudget = false;
                        requestBudgetForecasts[i].NeedContract = false;
                        requestBudgetForecasts[i].NeedContractValue = 0;

                        _context.Update(budgetFor);

                        contract = await _context.Set<Model.Contract>().Include(c => c.ContractAmount).Where(u => u.Id == requestBudgetForecastContractAdd.ContractId).SingleAsync();

                        if (contract.ContractAmount.AmountRonRem < requestBudgetForecasts[i].TotalOrderValueRon)
                        {
                            requestBudgetForecasts[i].NeedContract = true;
                            requestBudgetForecasts[i].NeedContractValue = requestBudgetForecasts[i].TotalOrderValueRon - requestBudgetForecasts[i].Contract.ContractAmount.AmountRonRem;
                        }

                        if (requestBudgetForecasts[i].BudgetForecast.TotalRem < requestBudgetForecasts[i].ValueRon)
                        {
                            requestBudgetForecasts[i].NeedBudget = true;

                            
                        }

                        _context.Update(requestBudgetForecasts[i]);
                        _context.SaveChanges();
                    }

                    

                    return new Model.RequestResult { Success = true, Message = "Contractul a fost mapat cu sucess!", RequestId = 0 };
                }
                else
                {
                    return new Model.RequestResult { Success = false, Message = $"Userul nu exista!", RequestId = 0 };
                }


            }
            else
            {
                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!", RequestId = 0 };
            }

           
        }

        [HttpPost("needBudgetUpdate")]
        public async virtual Task<RequestResult> NeedBudgetUpdate([FromBody] Dto.RequestBudgetForecastNeedBudget needBudget)
        {

            Model.RequestBudgetForecast reqBgt = await _context.Set<Model.RequestBudgetForecast>().Include(b => b.BudgetForecast).Where(a => a.Id == needBudget.RequestBudgetForecastId).SingleAsync();

            if (reqBgt != null)
            {
                reqBgt.NeedBudget = true;
                reqBgt.NeedBudgetValue = reqBgt.ValueRon - reqBgt.BudgetForecast.TotalRem;
                reqBgt.ModifiedAt = DateTime.Now;
                _context.Update(reqBgt);
                _context.SaveChanges();

                return new RequestResult { RequestId = 0, Success = true, Message = "Datele au fost salvate cu succes!" };
            }
            else
            {
                return new RequestResult { RequestId = 0, Success = false, Message = "WBS nu exista!" };
            }
        }


    }
}
