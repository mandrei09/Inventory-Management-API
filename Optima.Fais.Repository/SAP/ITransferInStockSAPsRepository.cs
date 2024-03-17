using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ITransferInStockSAPsRepository : IRepository<TransferInStockSAP>
    {
        int GetCountByFilters(string filter);
        IEnumerable<TransferInStockSAP> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
