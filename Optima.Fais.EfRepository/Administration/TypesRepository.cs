using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class TypesRepository : Repository<Model.Type>, ITypesRepository
    {
        public TypesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.Type, bool>> GetFiltersPredicate(string filter, List<int?> masterTypeIds)
        {
            Expression<Func<Model.Type, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((masterTypeIds != null) && (masterTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Type>(predicate, r => masterTypeIds.Contains(r.MasterTypeId))
                    : r => masterTypeIds.Contains(r.MasterTypeId);
            }

          

            return predicate;
        }

        public IEnumerable<Model.Type> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> masterTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, masterTypeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> masterTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, masterTypeIds);

            return GetQueryable(predicate).Count();
        }
    }
}
