using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IRetireAssetSAPsRepository : IRepository<RetireAssetSAP>
    {
        int GetCountByFilters(string filter);
        IEnumerable<RetireAssetSAP> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
