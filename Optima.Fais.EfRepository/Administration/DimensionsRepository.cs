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
    public class DimensionsRepository : Repository<Dimension>, IDimensionsRepository
    {
        public DimensionsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Length.Contains(filter) || a.Width.Contains(filter) || a.Height.Contains(filter)); })
        { }

        private Expression<Func<Model.Dimension, bool>> GetFiltersPredicate(string filter, List<int?> assetCategoryIds)
        {
            Expression<Func<Model.Dimension, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((assetCategoryIds != null) && (assetCategoryIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Dimension>(predicate, r => assetCategoryIds.Contains(r.AssetCategoryId))
                    : r => assetCategoryIds.Contains(r.AssetCategoryId);
            }

            return predicate;
        }

        public IEnumerable<Model.Dimension> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> assetCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, assetCategoryIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> assetCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, assetCategoryIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.Dimension> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Dimensions.AsNoTracking();

            if (lastId.HasValue)
            {
                query = query
                    .Where(r => (((r.ModifiedAt == lastModifiedAt) && (r.Id > lastId)) || (r.ModifiedAt > lastModifiedAt)));
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }
            else
            {
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }

            return query.ToList();
        }
    }
}
