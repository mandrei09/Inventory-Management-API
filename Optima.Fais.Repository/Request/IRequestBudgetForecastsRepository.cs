using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IRequestBudgetForecastsRepository : IRepository<RequestBudgetForecast>
    {
        IEnumerable<RequestBudgetForecast> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> requestIds, List<int?> budgetForecastIds, bool needBudget);
        int GetCountByFilters(string filter, List<int?> requestIds, List<int?> budgetForecastIds, bool needBudget);
        Task<List<RequestBudgetForecast>> GetAllIncludingChildrensAsync();
        Task<List<RequestBudgetForecast>> GetAllRequestBFByRequestId(int? requestId);
    }
}
