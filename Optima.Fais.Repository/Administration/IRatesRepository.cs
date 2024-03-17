using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IRatesRepository : IRepository<Rate>
    {
        IEnumerable<Rate> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> uomIds, string date, bool showLast);
        int GetCountByFilters(string filter, List<int?> uomIds, string date, bool showLast);
    }
}
