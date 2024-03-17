using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetInvPlusSAPsRepository : IRepository<AssetInvPlusSAP>
    {
        int GetCountByFilters(string filter);
        IEnumerable<AssetInvPlusSAP> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
