using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IPlantsRepository : IRepository<Plant>
    {
        int GetCountByFilters(string filter);
        IEnumerable<Plant> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
