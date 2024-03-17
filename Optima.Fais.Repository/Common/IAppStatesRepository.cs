using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IAppStatesRepository : IRepository<AppState>
    {
        IEnumerable<AppState> GetAppStatesByFilters(string parentCode);
        IEnumerable<AppState> GetAppStatesByParentCode(string parentCode);
        int GetCountByFilters(string filter, string parentCode);
        IEnumerable<AppState> GetByFilters(string filter, string parentCode, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        Task<AppState> GetByCodeAsync(string code);
    }
}
