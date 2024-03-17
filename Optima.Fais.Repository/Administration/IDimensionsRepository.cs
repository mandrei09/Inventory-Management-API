using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IDimensionsRepository : IRepository<Dimension>
    {
        IEnumerable<Dimension> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> assetCategoryIds);
        int GetCountByFilters(string filter, List<int?> assetCategoryIds);
        IEnumerable<Model.Dimension> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
