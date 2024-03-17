using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class AdministrationsRepository : Repository<Administration>, IAdministrationsRepository
    {
        public AdministrationsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.Administration, bool>> GetFiltersPredicate(string filter, List<int?> divisionIds)
        {
            Expression<Func<Model.Administration, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((divisionIds != null) && (divisionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Administration>(predicate, r => divisionIds.Contains(r.DivisionId))
                    : r => divisionIds.Contains(r.DivisionId);
            }

            return predicate;
        }

        public IEnumerable<Model.Administration> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> divisionIds)
        {
            var predicate = GetFiltersPredicate(filter, divisionIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> locationIds)
        {
            var predicate = GetFiltersPredicate(filter, locationIds);

            return GetQueryable(predicate).Count();
        }
    }
}
