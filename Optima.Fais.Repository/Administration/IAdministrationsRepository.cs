using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IAdministrationsRepository : IRepository<Administration>
    {
        IEnumerable<Administration> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> divisionIds);
        int GetCountByFilters(string filter, List<int?> locationIds);
    }
}
