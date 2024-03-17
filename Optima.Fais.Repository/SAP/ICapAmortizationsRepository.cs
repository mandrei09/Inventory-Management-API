using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ICapAmortizationsRepository : IRepository<AssetDepMDCapSync>
    {
        int GetCountByFilters(string filter);
        IEnumerable<AssetDepMDCapSync> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
