using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/printlabels")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class PrintLabelsController : GenericApiController<Model.PrintLabel, Dto.PrintLabel>
    {
        public PrintLabelsController(ApplicationDbContext context, IPrintLabelsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.PrintLabel> items = null;
            IEnumerable<Dto.PrintLabel> itemsResult = null;

            includes = includes ?? "Asset,Company";


            items = (_itemsRepository as IPrintLabelsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.PrintLabel>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IPrintLabelsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.PrintLabel>(itemsResult, new Dto.PagingInfo()
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
        [Route("deletePrintLabels")]
        public async Task<int> DeleteAllLabels()
		{
			int countChanges = 0;

			var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllLabels").ToList();

			countChanges = count[0].Count;
			return countChanges;
		}
	}
}
