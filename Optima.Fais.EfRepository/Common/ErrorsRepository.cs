using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class ErrorsRepository : Repository<Error>, IErrorsRepository
    {
        public ErrorsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Error, bool>> GetFiltersPredicate(string filter, List<int> errorTypeIds)
        {
            Expression<Func<Error, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((errorTypeIds != null) && (errorTypeIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.Error, int>((id) => { return c => c.ErrorTypeId == id; }, errorTypeIds);
                inListPredicate = ExpressionHelper.Or<Model.Error>(inListPredicate, c => c.ErrorTypeId == null);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Error>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;
        }

        public IEnumerable<Error> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> errorTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, errorTypeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> errorTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, errorTypeIds);

            return GetQueryable(predicate).Count();
        }
    }
}
