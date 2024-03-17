using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface ITypesRepository : IRepository<Type>
    {
        IEnumerable<Type> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> masterTypeIds);
        int GetCountByFilters(string filter, List<int?> masterTypeIds);
    }
}
