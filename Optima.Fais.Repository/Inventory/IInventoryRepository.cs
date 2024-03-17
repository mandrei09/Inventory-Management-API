using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IInventoryRepository : IRepository<Model.Inventory>
    {
        IEnumerable<Model.Inventory> GetSyncDetails(int pageSize, int lastId, System.DateTime lastModifiedAt);
        int GetCountByFilters(string filter, List<int> locationIds);
        IEnumerable<Model.Inventory> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> accMonthIds);
    }
}
