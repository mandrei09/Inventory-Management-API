using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
	[Authorize]
	[Route("api/brands")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class BrandsController : GenericApiController<Model.Brand, Dto.Brand>
	{
		public BrandsController(ApplicationDbContext context, IBrandsRepository itemsRepository, IMapper mapper)
			: base(context, itemsRepository, mapper)
		{
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string dictionaryItemIds, string filter, string includes)
		{
			List<Model.Brand> items = null;
			IEnumerable<Dto.Brand> itemsResult = null;
			List<int> dIds = null;

			includes = includes + ",DictionaryItem";

			if (dictionaryItemIds != null && !dictionaryItemIds.StartsWith("["))
			{
				dictionaryItemIds = "[" + dictionaryItemIds + "]";
			}

			if ((dictionaryItemIds != null) && (dictionaryItemIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(dictionaryItemIds).ToList().Select(int.Parse).ToList();

			items = (_itemsRepository as IBrandsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Brand>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IBrandsRepository).GetCountByFilters(filter, dIds);
				var pagedResult = new Dto.PagedResult<Dto.Brand>(itemsResult, new Dto.PagingInfo()
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

		[AllowAnonymous]
		[HttpGet]
		[Route("allowAnonymous", Order = -1)]
		public virtual IActionResult GetAllowAnonymous(int? page, int? pageSize, string sortColumn, string sortDirection, string dictionaryItemIds, string filter, string includes)
		{
			List<Model.Brand> items = null;
			IEnumerable<Dto.Brand> itemsResult = null;

			List<int> dIds = null;

			includes = includes + ",DictionaryItem";

			if (dictionaryItemIds != null && !dictionaryItemIds.StartsWith("["))
			{
				dictionaryItemIds = "[" + dictionaryItemIds + "]";
			}

			if ((dictionaryItemIds != null) && (dictionaryItemIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(dictionaryItemIds).ToList().Select(int.Parse).ToList();

			items = (_itemsRepository as IBrandsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Brand>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as IBrandsRepository).GetCountByFilters(filter, dIds);
				var pagedResult = new Dto.PagedResult<Dto.Brand>(itemsResult, new Dto.PagingInfo()
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
