using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
	[Route("api/companies")]
	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
	public partial class CompaniesController : GenericApiController<Model.Company, Dto.Company>
	{
		public CompaniesController(ApplicationDbContext context, ICompaniesRepository itemsRepository, IMapper mapper)
			: base(context, itemsRepository, mapper)
		{
		}

		[HttpGet]
		[Route("", Order = -1)]
		public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes, string exceptCompanyIds)
		{
			List<Model.Company> items = null;
			IEnumerable<Dto.Company> itemsResult = null;
			List<int?> exceptIds = null;

			if ((exceptCompanyIds != null) && (exceptCompanyIds.Length > 0)) exceptIds = JsonConvert.DeserializeObject<string[]>(exceptCompanyIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

			items = (_itemsRepository as ICompaniesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, exceptIds).ToList();
			itemsResult = items.Select(i => _mapper.Map<Dto.Company>(i));



			if (page.HasValue && pageSize.HasValue)
			{
				int totalItems = (_itemsRepository as ICompaniesRepository).GetCountByFilters(filter, exceptIds);
				var pagedResult = new Dto.PagedResult<Dto.Company>(itemsResult, new Dto.PagingInfo()
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
		[Route("sync")]
		public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
		{
			int totalItems = 0;
			List<Model.Company> items = (_itemsRepository as ICompaniesRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
			var itemsResult = items.Select(i => _mapper.Map<CompanySync>(i));
			var pagedResult = new Dto.PagedResult<CompanySync>(itemsResult, new Dto.PagingInfo()
			{
				TotalItems = totalItems,
				CurrentPage = 1,
				PageSize = pageSize
			});
			return Ok(pagedResult);
		}
	}
}
