using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IBudgetManagersRepository : IRepository<BudgetManager>
    {
        IEnumerable<BudgetManager> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> uomIds);
        int GetCountByFilters(string filter, List<int?> uomIds);
    }
}
