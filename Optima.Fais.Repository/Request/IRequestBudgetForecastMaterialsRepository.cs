using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IRequestBudgetForecastMaterialsRepository : IRepository<RequestBudgetForecastMaterial>
    {
        Task<List<RequestBudgetForecastMaterial>> GetAllRequestBFMaterialByRequestId(int? requestId);
        IEnumerable<RequestBudgetForecastMaterial> GetByFilters(RequestFilter budgetFilter, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> materialIds, int? orderId, List<int?> budgetForecastIds, bool showAll);
        int GetCountByFilters(RequestFilter budgetFilter, string filter, List<int?> materialIds, int? orderId, List<int?> budgetForecastIds, bool showAll);
    }
}
