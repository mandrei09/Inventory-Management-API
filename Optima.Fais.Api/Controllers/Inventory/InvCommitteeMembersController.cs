using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Collections.Generic;
using Optima.Faia.Model;
using System.Linq;

namespace Optima.Fais.Api.Controllers.Inventory
{
    [Route("api/committeemembers")]
    public partial class InvCommitteeMembersController : GenericApiController<InvCommitteeMember, Dto.InvCommitteeMember>
    {
        public InvCommitteeMembersController(ApplicationDbContext context, IInvCommitteeMembersRepository itemsRepository, 
            IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, int? invCommitteeId, string filter, string includes)
        {
            List<InvCommitteeMember> items = null;
            IEnumerable<Dto.InvCommitteeMember> itemsResult = null;

            includes = includes + "InvCommittee,Employee,InvCommitteePosition";
            items = (_itemsRepository as IInvCommitteeMembersRepository).GetByFilters(invCommitteeId, filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.InvCommitteeMember>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IInvCommitteeMembersRepository).GetCountByFilters(invCommitteeId, filter);
                var pagedResult = new Dto.PagedResult<Dto.InvCommitteeMember>(itemsResult, new Dto.PagingInfo()
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
