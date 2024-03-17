using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Optima.Fais.Data;
using Optima.Fais.EfRepository.Inventory;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers.Inventory
{
    [Route("api/inventoryplans")]
    public partial class InventoryPlansController : GenericApiController<InventoryPlan, Dto.InventoryPlan>
    {
        public InventoryPlansController(ApplicationDbContext context, IInventoryPlansRepository itemsRepository,
            IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, int? administrationId, int? costCenterId, string filter, string includes)
        {
            List<InventoryPlan> items = null;
            IEnumerable<Dto.InventoryPlan> itemsResult = null;

            includes = includes + "CostCenter,Administration,InvCommittee";
            items = (_itemsRepository as IInventoryPlansRepository).GetByFilters(administrationId, costCenterId, filter, includes, sortColumn, sortDirection, page, pageSize).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.InventoryPlan>(i));

            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IInventoryPlansRepository).GetCountByFilters(administrationId, costCenterId, filter);
                var pagedResult = new Dto.PagedResult<Dto.InventoryPlan>(itemsResult, new Dto.PagingInfo()
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

        [HttpPut("updateinventoryplan")]
        public virtual IActionResult UpdateInventoryPlan([FromBody] Dto.InventoryPlan item)
        {

            int inventoryPlanId = (_itemsRepository as IInventoryPlansRepository).UpdateInventoryPlan(item);
            return Ok();
        }
    }
}
