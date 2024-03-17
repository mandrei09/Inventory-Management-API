using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAssetComponentOpsRepository : IRepository<Model.AssetComponentOp>
    {
      
        IEnumerable<Model.AssetComponentOp> GetFiltered(AssetFilter assetFilter, string includes, int? employeeId, string documentTypeCode, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
       
    }
}