using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IEmailStatusRepository : IRepository<EmailStatus>
    {
        IEnumerable<EmailStatus> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds);
        int GetCountByFilters(string filter, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds);
    }
}
