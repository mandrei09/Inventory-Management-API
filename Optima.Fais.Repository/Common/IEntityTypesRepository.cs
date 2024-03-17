using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IEntityTypesRepository : IRepository<EntityType>
    {
        IEnumerable<EntityType> GetByFilters(string filter, string code, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        int GetCountByFilters(string filter, string code);
    }
}
