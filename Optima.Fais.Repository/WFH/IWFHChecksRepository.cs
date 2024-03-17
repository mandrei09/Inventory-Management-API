using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IWFHChecksRepository : IRepository<WFHCheck>
    {
        int GetCountByFilters(string filter, List<int> deviceTypeIds);
        IEnumerable<WFHCheck> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> deviceTypeIds);
    }
}
