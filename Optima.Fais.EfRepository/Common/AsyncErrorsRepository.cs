using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class AsyncErrorsRepository : Repository<AssetSyncError>, IAsyncErrorsRepository
    {
        public AsyncErrorsRepository(ApplicationDbContext context)
             : base(context)
        {
        }

        private Expression<Func<AssetSyncError, bool>> GetFiltersPredicate(string filter, List<int> infoTypeIds)
        {
            Expression<Func<AssetSyncError, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((infoTypeIds != null) && (infoTypeIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.AssetSyncError, int>((id) => { return c => c.AssetId == id; }, infoTypeIds);
                inListPredicate = ExpressionHelper.Or<Model.AssetSyncError>(inListPredicate, c => c.AssetId > 0);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetSyncError>(predicate, inListPredicate)
                    : inListPredicate;
            }

            return predicate;
        }
        public IEnumerable<AssetSyncError> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> infoTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, infoTypeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> errorTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, errorTypeIds);

            return GetQueryable(predicate).Count();
        }

    }
}
