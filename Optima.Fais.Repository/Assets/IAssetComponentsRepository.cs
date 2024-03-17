using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetComponentsRepository : IRepository<AssetComponent>
    {
        int GetCountByFilters(string filter, List<int> assetIds, List<int> employeeIds, List<int> subTypeIds);
        IEnumerable<AssetComponent> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> assetIds, List<int> employeeIds, List<int> subTypeIds);
        IEnumerable<Model.AssetComponent> GetFiltered(string includes, int? assetId, int? employeeId, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
        IEnumerable<Model.AssetComponent> GetFilteredDetailUI(string includes, int? assetId, int? employeeId, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
    }
}
