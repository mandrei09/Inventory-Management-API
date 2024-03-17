using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
	[Route("api/mastertypes")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class MasterTypesController : GenericApiController<Model.MasterType, Dto.MasterType>
	{
		public MasterTypesController(ApplicationDbContext context, IMasterTypesRepository itemsRepository, IMapper mapper)
			: base(context, itemsRepository, mapper)
		{
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
		{
			List<Model.MasterType> items = null;
			IEnumerable<Dto.MasterType> itemsResult = null;

			items = (_itemsRepository as IMasterTypesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.MasterType>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IMasterTypesRepository).GetCountByFilters(filter);
				var pagedResult = new Dto.PagedResult<Dto.MasterType>(itemsResult, new Dto.PagingInfo()
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
