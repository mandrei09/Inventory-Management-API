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
    public class AssetNaturesRepository : Repository<Model.AssetNature>, IAssetNaturesRepository
    {
        public AssetNaturesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.AssetNature, bool>> GetFiltersPredicate(string filter, List<int?> assetTypeIds)
        {
            Expression<Func<Model.AssetNature, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((assetTypeIds != null) && (assetTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.AssetNature>(predicate, r => assetTypeIds.Contains(r.AssetTypeId))
                    : r => assetTypeIds.Contains(r.AssetTypeId);
            }



            return predicate;
        }

        public IEnumerable<Model.AssetNature> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> assetTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, assetTypeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> assetTypeIds)
        {
            var predicate = GetFiltersPredicate(filter, assetTypeIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.AssetNature> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.AssetNatures.AsNoTracking();

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
