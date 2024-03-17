using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class SubTypesRepository : Repository<SubType>, ISubTypesRepository
    {
        public SubTypesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.SubType, bool>> GetFiltersPredicate(string filter, List<int?> typeIds)
        {
            Expression<Func<Model.SubType, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((typeIds != null) && (typeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.SubType>(predicate, r => typeIds.Contains(r.TypeId))
                    : r => typeIds.Contains(r.TypeId);
            }

          

            return predicate;
        }

        public IEnumerable<Model.SubType> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> typeIds)
        {
            var predicate = GetFiltersPredicate(filter, typeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> typeIds)
        {
            var predicate = GetFiltersPredicate(filter, typeIds);

            return GetQueryable(predicate).Count();
        }
    }
}
