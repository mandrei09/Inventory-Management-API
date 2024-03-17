using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IRouteRolesRepository : IRepository<RouteRole>
    {
        IEnumerable<RouteRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> routeIds, List<string> roleIds);
        int GetCountByFilters(string filter, List<int?> routeIds, List<string> roleIds);
        Task<List<RouteRole>> RouteByRoleAsync(string role);
        Task<List<Route>> GetAllIncludingRouteChildrensAsync(List<string> roleIds);
    }
}
