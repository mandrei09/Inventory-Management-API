using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Repository.Inventory
{
    public interface IInventoryPlansRepository : IRepository<Model.InventoryPlan>
    {
        Task<Model.InventoryPlan> GetByAdministrationAndCostCenterAsync(int? administrationId, int? costCenterId);
        Task<Model.InventoryPlan> GetByFinalReportAsync();
        int GetCountByFilters(int? administrationId, int? costCentrId, string filter);
        IEnumerable<Model.InventoryPlan> GetByFilters(int? administrationId, int? costCenterId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        int UpdateInventoryPlan(Dto.InventoryPlan item);
    }
}
