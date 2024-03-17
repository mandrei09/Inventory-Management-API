using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ITransferAssetSAPsRepository : IRepository<TransferAssetSAP>
    {
        int GetCountByFilters(string filter);
        IEnumerable<TransferAssetSAP> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
