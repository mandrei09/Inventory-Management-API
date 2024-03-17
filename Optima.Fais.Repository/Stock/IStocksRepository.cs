using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IStocksRepository : IRepository<Stock>
    {
        int GetCountByFilters(string filter, List<int?> categoryIds, List<int?> plantInitialIds, List<int?> plantActualIds, List<int?> exceptEmployeeIds, List<int?> storageInitialIds, bool showStock);
        IEnumerable<Stock> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> categoryIds, List<int?> plantInitialIds, List<int?> plantActualIds, List<int?> exceptEmployeeIds, List<int?> storageInitialIds, bool showStock);
    }
}
