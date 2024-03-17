using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class BudgetManagersRepository : Repository<Model.BudgetManager>, IBudgetManagersRepository
    {
        public BudgetManagersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.BudgetManager, bool>> GetFiltersPredicate(string filter, List<int?> uomIds)
        {
            Expression<Func<Model.BudgetManager, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((uomIds != null) && (uomIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.BudgetManager>(predicate, r => uomIds.Contains(r.UomId))
                    : r => uomIds.Contains(r.UomId);
            }



            return predicate;
        }

        public IEnumerable<Model.BudgetManager> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> uomIds)
        {
            var predicate = GetFiltersPredicate(filter, uomIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> uomIds)
        {
            var predicate = GetFiltersPredicate(filter, uomIds);

            return GetQueryable(predicate).Count();
        }
    }
}
