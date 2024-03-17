using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class RequestBFMaterialCostCentersRepository : Repository<RequestBFMaterialCostCenter>, IRequestBFMaterialCostCentersRepository
    {
        public RequestBFMaterialCostCentersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.CostCenter.Code.Contains(filter) || a.RequestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecast.BudgetBase.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.RequestBFMaterialCostCenter, bool>> GetFiltersPredicate(string filter, List<int?> costCenterIds, List<int?> requestBudgetForecastMaterialIds, int? orderId, bool reception)
        {
            Expression<Func<Model.RequestBFMaterialCostCenter, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((costCenterIds != null) && (costCenterIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBFMaterialCostCenter>(predicate, r => costCenterIds.Contains(r.CostCenterId))
                    : r => costCenterIds.Contains(r.CostCenterId);
            }

            if (reception)
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBFMaterialCostCenter>(predicate, r => r.QuantityRem > 0)
                    : r => r.QuantityRem > 0;
            }


            if ((requestBudgetForecastMaterialIds != null) && (requestBudgetForecastMaterialIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBFMaterialCostCenter>(predicate, r => requestBudgetForecastMaterialIds.Contains(r.RequestBudgetForecastMaterialId))
                    : r => requestBudgetForecastMaterialIds.Contains(r.RequestBudgetForecastMaterialId);
            }

			if (orderId != null && orderId > 0)
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.RequestBFMaterialCostCenter>(predicate, r => r.OrderId == orderId)
					: r => r.OrderId == orderId;
			}

			return predicate;
        }

        public IEnumerable<Model.RequestBFMaterialCostCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> costCenterIds, List<int?> requestBudgetForecastMaterialIds, int? orderId, bool reception)
        {
            var predicate = GetFiltersPredicate(filter, costCenterIds, requestBudgetForecastMaterialIds, orderId, reception);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> costCenterIds, List<int?> requestBudgetForecastMaterialIds, int? orderId, bool reception)
        {
            var predicate = GetFiltersPredicate(filter, costCenterIds, requestBudgetForecastMaterialIds, orderId, reception);

            return GetQueryable(predicate).Count();
        }

        private Expression<Func<Model.RequestBFMaterialCostCenter, bool>> GetReceptionFiltersPredicate(string filter, List<int?> orderIds, bool reception)
        {
            Expression<Func<Model.RequestBFMaterialCostCenter, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((orderIds != null) && (orderIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBFMaterialCostCenter>(predicate, r => orderIds.Contains(r.OrderId))
                    : r => orderIds.Contains(r.OrderId);
            }

            if (reception)
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RequestBFMaterialCostCenter>(predicate, r => r.QuantityRem > 0)
                    : r => r.QuantityRem > 0;
            }

            return predicate;
        }

        public IEnumerable<Model.RequestBFMaterialCostCenter> GetByReceptionFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> orderIds, bool reception)
        {
            var predicate = GetReceptionFiltersPredicate(filter, orderIds, reception);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByReceptionFilters(string filter, List<int?> orderIds, bool reception)
        {
            var predicate = GetReceptionFiltersPredicate(filter, orderIds, reception);

            return GetQueryable(predicate).Count();
        }
    }
}
