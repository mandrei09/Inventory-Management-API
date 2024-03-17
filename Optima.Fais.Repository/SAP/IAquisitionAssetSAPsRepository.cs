using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAquisitionAssetSAPsRepository : IRepository<AcquisitionAssetSAP>
    {
        int GetCountByFilters(AssetReceptionFilter assetFilter, string filter, bool isTesting);
        IEnumerable<AcquisitionAssetSAP> GetByFilters(AssetReceptionFilter assetFilter, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, bool isTesting);
    }
}
