using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IContractOpsRepository : IRepository<Model.ContractOp>
    {
        IEnumerable<Model.ContractOp> GetFiltered(ContractFilter assetFilter, string includes, int? assetId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
       
    }
}