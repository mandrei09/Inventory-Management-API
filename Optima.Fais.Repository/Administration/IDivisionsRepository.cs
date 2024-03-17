using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IDivisionsRepository : IRepository<Division>
    {
        IEnumerable<Division> GetByFilters(string filter, List<int?> departmentIds, List<int?> divisionIds, List<int?> locationIds, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        int GetCountByFilters(string filter, List<int?> departmentIds, List<int?> divisionIds, List<int?> locationIds);
        IEnumerable<Model.Division> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
