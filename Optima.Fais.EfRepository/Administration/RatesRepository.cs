using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class RatesRepository : Repository<Rate>, IRatesRepository
    {
        public RatesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter) || a.Uom.Code.Contains(filter) || a.Uom.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.Rate, bool>> GetFiltersPredicate(string filter, List<int?> uomIds, string date, bool showLast)
        {
            Expression<Func<Model.Rate, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((uomIds != null) && (uomIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Rate>(predicate, r => uomIds.Contains(r.UomId))
                    : r => uomIds.Contains(r.UomId);
            }

            if (showLast)
            {
				predicate = predicate != null
				 ? ExpressionHelper.And<Model.Rate>(predicate, r => r.IsLast == true)
				 : r => r.IsLast == true;
			}

            if (date != null && date != "")
            {
                predicate = predicate != null
                 ? ExpressionHelper.And<Model.Rate>(predicate, r => DateTime.Parse(r.Code) == DateTime.Parse(date).AddDays(-1))
                 : r => DateTime.Parse(r.Code) == DateTime.Parse(date).AddDays(-1);
            }

            //predicate = predicate != null
            // ? ExpressionHelper.And<Model.Rate>(predicate, r => r.IsLast == true)
            // : r => r.IsLast == true;

            return predicate;
        }

        public IEnumerable<Model.Rate> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> uomIds, string date, bool showLast)
        {
            var predicate = GetFiltersPredicate(filter, uomIds, date, showLast);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> uomIds, string date, bool showLast)
        {
            var predicate = GetFiltersPredicate(filter, uomIds, date, showLast);

            return GetQueryable(predicate).Count();
        }
    }
}
