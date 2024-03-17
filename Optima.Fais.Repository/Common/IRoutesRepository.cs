using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IRoutesRepository : IRepository<Route>
    {
        IEnumerable<Route> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<string> roleIds);
        int GetCountByFilters(string filter, List<string> roleIds);
        Task<List<Route>> GetAllIncludingRouteChildrensAsync(string role);
    }
}
