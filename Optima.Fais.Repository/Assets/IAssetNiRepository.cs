using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Repository
{
    public interface IAssetNiRepository : IRepository<Model.AssetNi>
    {
        IQueryable<Dto.AssetNiInvDet> GetAssetNiInvDetQuery(int inventoryId);

        IEnumerable<Dto.AssetNiInvDet> GetAssetNiInvDetByFilters(int inventoryId, string filter, string reportType, bool? custody,
            List<int> assetCategoryIds, List<int> assetTypeIds, List<int> partnerIds,
            List<int> costCenterIds, List<int> admCenterIds, List<int> departmentIds, List<int> employeeIds, List<int> locationIds, List<int> roomIds,
            string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

        IEnumerable<Dto.AssetNi> GetAssetNiByFilters(
            //string filter,
            //int? wordCount, int? letterCount, string searchType, string conditionType, 
            List<string> filters, string conditionType,
            int? inventoryId, int? assetId,
            List<int> departmentIds, List<int> employeeIds, List<int> locationIds, List<int> roomIds, List<int> regionIds, List<int> admCenterIds,
            string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
            int RecoverAssetNi(int assetId, int inventoryId);
    }
}
