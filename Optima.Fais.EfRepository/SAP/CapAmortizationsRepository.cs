using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class CapAmortizationsRepository : Repository<AssetDepMDCapSync>, ICapAmortizationsRepository
    {
        public CapAmortizationsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.BUKRSH.Contains(filter) || a.KOSTL.Contains(filter)); })
        { }


        private Expression<Func<AssetDepMDCapSync, bool>> GetFiltersPredicate(string filter)
        {
            Expression<Func<AssetDepMDCapSync, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            return predicate;
        }

        public IEnumerable<AssetDepMDCapSync> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate).Count();
        }
    }
}
