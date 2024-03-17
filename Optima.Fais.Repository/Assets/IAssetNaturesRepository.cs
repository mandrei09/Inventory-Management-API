using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetNaturesRepository : IRepository<AssetNature>
    {
        IEnumerable<AssetNature> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> assetTypeIds);
        int GetCountByFilters(string filter, List<int?> assetTypeIds);
        IEnumerable<AssetNature> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
