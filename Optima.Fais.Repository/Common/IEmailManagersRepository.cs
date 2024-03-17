using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IEmailManagersRepository : IRepository<EmailManager>
    {
        IEnumerable<EmailManager> GetByFilters(string employeeId, string role, bool inInventory, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds, List<int> divisionIds, string type);
        int GetCountByFilters(string employeeId, string role, bool inInventory, string filter, List<int?> emailTypeIds, List<int?> appStateIds, List<int?> assetCategoryIds, List<int> divisionIds, string type);
    }
}
