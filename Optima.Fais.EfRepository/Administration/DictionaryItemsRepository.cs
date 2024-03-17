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
    public class DictionaryItemsRepository : Repository<DictionaryItem>, IDictionaryItemsRepository
    {
        public DictionaryItemsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.DictionaryItem, bool>> GetFiltersPredicate(string filter, List<int?> dictionaryTypeIds, List<int?> assetCategoryIds, bool showWFH)
        {
            Expression<Func<Model.DictionaryItem, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((dictionaryTypeIds != null) && (dictionaryTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.DictionaryItem>(predicate, r => dictionaryTypeIds.Contains(r.DictionaryTypeId))
                    : r => dictionaryTypeIds.Contains(r.DictionaryTypeId);
            }

            if ((assetCategoryIds != null) && (assetCategoryIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.DictionaryItem>(predicate, r => assetCategoryIds.Contains(r.AssetCategoryId))
                    : r => assetCategoryIds.Contains(r.AssetCategoryId);
            }

			if (showWFH)
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.DictionaryItem>(predicate, r => r.DictionaryType.Code == "WFH")
					: r => r.DictionaryType.Code == "WFH";
			}

			return predicate;
        }

        public IEnumerable<Model.DictionaryItem> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> dictionaryTypeIds, List<int?> assetCategoryIds, bool showWFH)
        {
            var predicate = GetFiltersPredicate(filter, dictionaryTypeIds, assetCategoryIds, showWFH);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> dictionaryTypeIds, List<int?> assetCategoryIds, bool showWFH)
        {
            var predicate = GetFiltersPredicate(filter, dictionaryTypeIds, assetCategoryIds, showWFH);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.DictionaryItem> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.DictionaryItems.AsNoTracking();

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
