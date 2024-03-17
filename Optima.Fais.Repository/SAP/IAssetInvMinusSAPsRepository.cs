using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetInvMinusSAPsRepository : IRepository<AssetInvMinusSAP>
    {
        int GetCountByFilters(string filter);
        IEnumerable<AssetInvMinusSAP> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
