using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IPermissionRolesRepository : IRepository<PermissionRole>
    {
        IEnumerable<PermissionRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> permissionTypeIds, List<int?> permissionIds, List<string> roleIds);
        int GetCountByFilters(string filter, List<int?> permissionTypeIds, List<int?> permissionIds, List<string> roleIds);
        Task<List<PermissionRole>> GetPermissionByRoleAsync(string role);
    }
}
