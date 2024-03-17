using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IBudgetOpsRepository : IRepository<Model.BudgetOp>
    {
        IEnumerable<Model.BudgetOp> GetFiltered(AssetFilter assetFilter, string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
       
    }
}