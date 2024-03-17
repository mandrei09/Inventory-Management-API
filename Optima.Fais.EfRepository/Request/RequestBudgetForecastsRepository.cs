using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class RequestBudgetForecastsRepository : Repository<Model.RequestBudgetForecast>, IRequestBudgetForecastsRepository
    {
        public RequestBudgetForecastsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Request.Code.Contains(filter) || a.BudgetForecast.BudgetBase.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.RequestBudgetForecast, bool>> GetFiltersPredicate(string filter, List<int?> requestIds, List<int?> budgetForecastIds, bool needBudget)
        {
            Expression<Func<Model.RequestBudgetForecast, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((requestIds != null) && (requestIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBudgetForecast>(predicate, r => requestIds.Contains(r.RequestId))
                    : r => requestIds.Contains(r.RequestId);
            }

            if ((budgetForecastIds != null) && (budgetForecastIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBudgetForecast>(predicate, r => budgetForecastIds.Contains(r.BudgetForecastId))
                    : r => budgetForecastIds.Contains(r.BudgetForecastId);
            }

            if (needBudget)
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBudgetForecast>(predicate, r => r.NeedBudget == true && r.Request.AppStateId == 78)
                    : r => r.NeedBudget == true && r.Request.AppStateId == 78;
            }

            return predicate;
        }

        public IEnumerable<Model.RequestBudgetForecast> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> requestIds, List<int?> budgetForecastIds, bool needBudget)
        {
            var predicate = GetFiltersPredicate(filter, requestIds, budgetForecastIds, needBudget);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> requestIds, List<int?> budgetForecastIds, bool needBudget)
        {
            var predicate = GetFiltersPredicate(filter, requestIds, budgetForecastIds, needBudget);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<Model.RequestBudgetForecast>> GetAllIncludingChildrensAsync()
        {
            return await _context.Set<Model.RequestBudgetForecast>()
                .Include(b => b.Request)
                .Include(b => b.BudgetForecast).ThenInclude(b => b.BudgetBase)
                .Include(i => i.RequestBudgetForecastMaterials)
                    .ThenInclude(b => b.Material)
                .Include(i => i.RequestBudgetForecastMaterials)
                    .ThenInclude(b => b.RequestBudgetForecast)

                .Where(r => r.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Model.RequestBudgetForecast>> GetAllRequestBFByRequestId(int? requestId)
        {
             return await _context.Set<Model.RequestBudgetForecast>().Where(a => a.RequestId == requestId).ToListAsync();
        }

    }
}
