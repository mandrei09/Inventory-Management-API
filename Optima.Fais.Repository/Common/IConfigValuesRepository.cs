using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IConfigValuesRepository : IRepository<Model.ConfigValue>
    {
        int GetCountByFilters(string filter, List<string> roleIds, string roleName);
        IEnumerable<Model.ConfigValue> GetByFilters(string filter, string includes, List<string> roleIds, string roleName, string sortColumn, string sortDirection, int? page, int? pageSize);
    }
}
