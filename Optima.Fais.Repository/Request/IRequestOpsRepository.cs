using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IRequestOpsRepository : IRepository<Model.RequestOp>
    {
        IEnumerable<Model.RequestOp> GetFiltered(RequestFilter assetFilter, string includes, int? requestId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
       
    }
}