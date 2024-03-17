using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetChangeSAPsRepository : IRepository<AssetChangeSAP>
    {
        int GetCountByFilters(string filter);
        IEnumerable<AssetChangeSAP> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
