using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
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
    [Route("api/emailorderstatus")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EmailOrderStatusController : GenericApiController<Model.EmailOrderStatus, Dto.EmailOrderStatus>
    {
		private readonly UserManager<ApplicationUser> _userManager;

		public EmailOrderStatusController(ApplicationDbContext context, IEmailOrderStatusRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
			this._userManager = userManager;
		}

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string emailTypeIds, string appStateIds, string includes)
        {
            List<Model.EmailOrderStatus> items = null;
            IEnumerable<Dto.EmailOrderStatus> itemsResult = null;
            List<int?> dIds = null;
            List<int?> appIds = null;
            includes = "EmailType,Order,Matrix,AppState";

            if ((emailTypeIds != null) && (emailTypeIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(emailTypeIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((appStateIds != null) && (appStateIds.Length > 0)) appIds = JsonConvert.DeserializeObject<string[]>(appStateIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IEmailOrderStatusRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, dIds, appIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EmailOrderStatus>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEmailOrderStatusRepository).GetCountByFilters(filter, dIds, appIds);
                var pagedResult = new Dto.PagedResult<Dto.EmailOrderStatus>(itemsResult, new Dto.PagingInfo()
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
