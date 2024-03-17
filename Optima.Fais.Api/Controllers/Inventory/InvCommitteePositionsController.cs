using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers.Inventory
{
    [Route("api/committeepositions")]
    public class InvCommitteePositionsController : GenericApiController<InvCommitteePosition, Dto.InvCommitteePostition>
    {
        public InvCommitteePositionsController(ApplicationDbContext context, IInvCommitteePositionsRepository itemsRepository,
          IMapper mapper)
          : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string includes)
        {
            List<InvCommitteePosition> items = null;
            IEnumerable<Dto.InvCommitteePostition> itemsResult = null;

            items = (_itemsRepository as IInvCommitteePositionsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.InvCommitteePostition>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IInvCommitteePositionsRepository).GetCountByFilters(filter);
                var pagedResult = new Dto.PagedResult<Dto.InvCommitteePostition>(itemsResult, new Dto.PagingInfo()
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
