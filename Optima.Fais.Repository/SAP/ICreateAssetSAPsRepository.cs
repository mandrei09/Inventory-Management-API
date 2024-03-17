using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ICreateAssetSAPsRepository : IRepository<CreateAssetSAP>
    {
        int GetCountByFilters(string filter);
        IEnumerable<CreateAssetSAP> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
