using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IColumnDefinitionRolesRepository : IRepository<ColumnDefinitionRole>
    {
        IEnumerable<ColumnDefinitionRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> columnDefinitionIds, List<string> roleIds);
        int GetCountByFilters(string filter, List<int?> columnDefinitionIds, List<string> roleIds);
        Task<List<ColumnDefinitionRole>> GetColumnDefinitionByRoleAsync(string role);
    }
}
