using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/permissiontypes")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class PermissionTypesController : GenericApiController<Model.PermissionType, Dto.PermissionType>
    {
        public PermissionTypesController(ApplicationDbContext context, IPermissionTypesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<Model.PermissionType> items = null;
            IEnumerable<Dto.PermissionType> itemsResult = null;


            items = (_itemsRepository as IPermissionTypesRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.PermissionType>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IPermissionTypesRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.PermissionType>(itemsResult, new Dto.PagingInfo()
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
