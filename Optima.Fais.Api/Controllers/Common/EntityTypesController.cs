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
    [Route("api/entitytypes")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EntityTypesController : GenericApiController<Model.EntityType, Dto.EntityType>
    {
        public EntityTypesController(ApplicationDbContext context, IEntityTypesRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes, string uploadFolder)
        {
            List<Model.EntityType> items = null;
            IEnumerable<Dto.EntityType> itemsResult = null;


            items = (_itemsRepository as IEntityTypesRepository).GetByFilters(filter, uploadFolder, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.EntityType>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IEntityTypesRepository).GetCountByFilters(filter, uploadFolder);
                var pagedResult = new Dto.PagedResult<Dto.EntityType>(itemsResult, new Dto.PagingInfo()
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
