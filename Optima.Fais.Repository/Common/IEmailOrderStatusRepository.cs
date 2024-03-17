using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IEmailOrderStatusRepository : IRepository<EmailOrderStatus>
    {
        Task<List<EmailOrderStatus>> GetAllEmailOrderStatusesByRequestBFId(int? requestId);
        IEnumerable<EmailOrderStatus> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> emailTypeIds, List<int?> appStateIds);
        int GetCountByFilters(string filter, List<int?> emailTypeIds, List<int?> appStateIds);
    }
}
