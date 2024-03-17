using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IMaterialsRepository : IRepository<Material>
    {
        int GetCountByFilters(string filter, List<int> countyIds, List<int?> materialIds, List<int?> exceptEmployeeIds, bool hasSubCategory);
        IEnumerable<Material> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> countyIds, List<int?> materialIds, List<int?> exceptEmployeeIds, bool hasSubCategory);
        IEnumerable<Model.Material> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
