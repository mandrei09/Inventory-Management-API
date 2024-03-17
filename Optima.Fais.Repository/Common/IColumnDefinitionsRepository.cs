using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IColumnDefinitionsRepository : IRepository<ColumnDefinition>
    {
        int GetCountByFilters(string filter, List<int> tableDefinitionIds, List<string> roleIds, string roleName);
        IEnumerable<ColumnDefinition> GetByFilters(string filter, List<int> tableDefinitionIds, List<string> roleIds, string roleName, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
