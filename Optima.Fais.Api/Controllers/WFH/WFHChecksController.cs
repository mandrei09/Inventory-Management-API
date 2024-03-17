using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/wfhchecks")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class WFHChecksController : GenericApiController<Model.WFHCheck, Dto.WFHCheck>
    {
        public WFHChecksController(ApplicationDbContext context, IWFHChecksRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string infoTypeIds, string includes)
        {
            List<Model.WFHCheck> items = null;
            IEnumerable<Dto.WFHCheck> itemsResult = null;
            List<int> cIds = null;

            if (infoTypeIds != null && !infoTypeIds.StartsWith("["))
            {
                infoTypeIds = "[" + infoTypeIds + "]";
            }

            includes = includes ?? "DictionaryItem,Brand,Model,BudgetManager,Employee";

            if ((infoTypeIds != null) && (infoTypeIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(infoTypeIds).ToList().Select(int.Parse).ToList();


            items = (_itemsRepository as IWFHChecksRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.WFHCheck>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IWFHChecksRepository).GetCountByFilters(filter, cIds);
                var pagedResult = new Dto.PagedResult<Dto.WFHCheck>(itemsResult, new Dto.PagingInfo()
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
    }
}
