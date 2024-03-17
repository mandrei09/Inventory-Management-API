using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IStoragesRepository : IRepository<Storage>
    {
        int GetCountByFilters(string filter);
        IEnumerable<Storage> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
