using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAmortizationsRepository : IRepository<AssetDepMDSync>
    {
        int GetCountByFilters(string filter);
        IEnumerable<AssetDepMDSync> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
