using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IPermissionsRepository : IRepository<Permission>
    {
        IEnumerable<Permission> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> permissionTypeIds);
        int GetCountByFilters(string filter, List<int?> permissionTypeIds);
    }
}
