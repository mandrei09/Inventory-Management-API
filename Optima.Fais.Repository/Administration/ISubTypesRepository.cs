using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ISubTypesRepository : IRepository<SubType>
    {
        IEnumerable<SubType> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> typeIds);
        int GetCountByFilters(string filter, List<int?> typeIds);
    }
}
