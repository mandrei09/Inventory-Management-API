using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface ILevelsRepository : IRepository<Level>
    {
        int GetCountByFilters(string filter);
        IEnumerable<Level> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        Task<Level> GetByCodeAsync(string code);
        Level GetByCode(string code);
    }
}
