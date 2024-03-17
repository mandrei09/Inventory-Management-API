using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IPartnersRepository : IRepository<Partner>
    {
        IEnumerable<Partner> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> countyIds, bool showAll);
        int GetCountByFilters(string filter, List<int> countyIds, bool showAll);
    }
}
