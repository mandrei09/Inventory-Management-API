using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAreasRepository : IRepository<Area>
    {
        int GetCountByFilters(string filter);
        IEnumerable<Area> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
