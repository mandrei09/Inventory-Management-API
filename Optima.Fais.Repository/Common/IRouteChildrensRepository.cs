using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IRouteChildrensRepository : IRepository<RouteChildren>
    {
        int GetCountByFilters(string filter, List<int> routeIds, List<string> roleIds, string roleName);
        IEnumerable<RouteChildren> GetByFilters(string filter, string includes, List<int> routeIds, List<string> roleIds, string roleName, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
