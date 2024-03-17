using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
	[Route("api/activities")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class ActivitiesController : GenericApiController<Model.Activity, Dto.Activity>
	{
		public ActivitiesController(ApplicationDbContext context, IActivitiesRepository itemsRepository, IMapper mapper)
			: base(context, itemsRepository, mapper)
		{
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
		{
			List<Model.Activity> items = null;
			IEnumerable<Dto.Activity> itemsResult = null;

			items = (_itemsRepository as IActivitiesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Activity>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IActivitiesRepository).GetCountByFilters(filter);
				var pagedResult = new Dto.PagedResult<Dto.Activity>(itemsResult, new Dto.PagingInfo()
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
