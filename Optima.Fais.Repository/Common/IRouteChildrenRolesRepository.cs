using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IRouteChildrenRolesRepository : IRepository<RouteChildrenRole>
    {
        IEnumerable<RouteChildrenRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> routeChildrenIds, List<string> roleIds);
        int GetCountByFilters(string filter, List<int?> routeChildrenIds, List<string> roleIds);
        Task<List<RouteChildrenRole>> RouteChildrenByRoleAsync(string role);
    }
}
