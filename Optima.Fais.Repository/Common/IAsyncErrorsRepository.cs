using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAsyncErrorsRepository : IRepository<AssetSyncError>
    {
        int GetCountByFilters(string filter, List<int> errorTypeIds);
        IEnumerable<AssetSyncError> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> errorTypeIds);
    }
}
