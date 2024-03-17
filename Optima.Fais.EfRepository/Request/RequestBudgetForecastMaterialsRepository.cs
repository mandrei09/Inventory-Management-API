using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class RequestBudgetForecastMaterialsRepository : Repository<RequestBudgetForecastMaterial>, IRequestBudgetForecastMaterialsRepository
    {
        public RequestBudgetForecastMaterialsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Material.Code.Contains(filter) || a.RequestBudgetForecast.BudgetForecast.BudgetBase.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.RequestBudgetForecastMaterial, bool>> GetFiltersPredicate(RequestFilter budgetFilter, string filter, List<int?> materialIds, int? orderId, List<int?> requestBudgetForecastIds, bool showAll)
        {
            Expression<Func<Model.RequestBudgetForecastMaterial, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((materialIds != null) && (materialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBudgetForecastMaterial>(predicate, r => materialIds.Contains(r.MaterialId))
                    : r => materialIds.Contains(r.MaterialId);
            }

            if ((requestBudgetForecastIds != null) && (requestBudgetForecastIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBudgetForecastMaterial>(predicate, r => requestBudgetForecastIds.Contains(r.RequestBudgetForecastId))
                    : r => requestBudgetForecastIds.Contains(r.RequestBudgetForecastId);
            }

            if ((budgetFilter.RequestBudgetForecastIds != null) && (budgetFilter.RequestBudgetForecastIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBudgetForecastMaterial>(predicate, r => budgetFilter.RequestBudgetForecastIds.Contains(r.RequestBudgetForecastId))
                    : r => budgetFilter.RequestBudgetForecastIds.Contains(r.RequestBudgetForecastId);
            }

            if (showAll)
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBudgetForecastMaterial>(predicate, r => r.OrderId != null)
                    : r => r.OrderId != null;
            }
			else
			{
                predicate = predicate != null
                   ? ExpressionHelper.And<Model.RequestBudgetForecastMaterial>(predicate, r => r.OrderId == null)
                   : r => r.OrderId == null;
            }

			if (orderId != null && orderId > 0)
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.RequestBudgetForecastMaterial>(predicate, r => r.OrderId == orderId)
					: r => r.OrderId == orderId;
			}



			return predicate;
        }

        public IEnumerable<Model.RequestBudgetForecastMaterial> GetByFilters(RequestFilter budgetFilter, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> materialIds, int? orderId, List<int?> requestBudgetForecastIds, bool showAll)
        {
            var predicate = GetFiltersPredicate(budgetFilter, filter, materialIds, orderId, requestBudgetForecastIds, showAll);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(RequestFilter budgetFilter, string filter, List<int?> materialIds, int? orderId, List<int?> requestBudgetForecastIds, bool showAll)
        {
            var predicate = GetFiltersPredicate(budgetFilter, filter, materialIds, orderId, requestBudgetForecastIds, showAll);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<Model.RequestBudgetForecastMaterial>> GetAllRequestBFMaterialByRequestId(int? requestId)
        {
            return await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.RequestBudgetForecastId == requestId).ToListAsync();
        }
    }
}
