using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/ordermaterials")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class OrderMaterialsController : GenericApiController<Model.OrderMaterial, Dto.OrderMaterial>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public OrderMaterialsController(ApplicationDbContext context, IOrderMaterialsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string orderIds, string materialIds, string subCategoryIds, string includes)
        {
            List<Model.OrderMaterial> items = null;
            IEnumerable<Dto.OrderMaterial> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;
            List<int?> catIds = null;

            includes = "Order,Material,AppState,Rate.Uom";


			if (orderIds != null && !orderIds.StartsWith("["))
			{
                orderIds = "[" + orderIds + "]";
			}

            if (orderIds == null)
            {
                orderIds = "[-1]";
            }

            if (materialIds != null && !materialIds.StartsWith("["))
			{
                materialIds = "[" + materialIds + "]";
			}

            if (subCategoryIds != null && !subCategoryIds.StartsWith("["))
            {
                subCategoryIds = "[" + subCategoryIds + "]";
            }

            if ((orderIds != null) && (orderIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(orderIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((materialIds != null) && (materialIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(materialIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((subCategoryIds != null) && (subCategoryIds.Length > 0)) catIds = JsonConvert.DeserializeObject<string[]>(subCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IOrderMaterialsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds, catIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.OrderMaterial>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IOrderMaterialsRepository).GetCountByFilters(filter, cIds, eIds, catIds);
                var pagedResult = new Dto.PagedResult<Dto.OrderMaterial>(itemsResult, new Dto.PagingInfo()
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
