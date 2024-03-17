using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAccountanciesRepository : IRepository<Accountancy>
    {
        IEnumerable<Accountancy> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> offerIds, List<int?> materialIds, List<int?> subCategoryIds);
        int GetCountByFilters(string filter, List<int?> offerIds, List<int?> materialIds, List<int?> subCategoryIds);
    }
}
