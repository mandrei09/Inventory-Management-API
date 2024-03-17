using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface ITableDefinitionsRepository : IRepository<TableDefinition>
    {
        Task<List<TableDefinition>> GetAllIncludingColumnDefinitionsAsync();
        int GetCountByFilters(string filter);
        IEnumerable<TableDefinition> GetByFilters(string filter, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
