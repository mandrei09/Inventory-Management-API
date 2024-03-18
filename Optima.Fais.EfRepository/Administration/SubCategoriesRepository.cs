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
	public class SubCategoriesRepository : Repository<SubCategory>, ISubCategoriesRepository
    {
		public SubCategoriesRepository(ApplicationDbContext context)
			: base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
		{ }

        private Expression<Func<Model.SubCategory, bool>> GetFiltersPredicate(string filter, List<int> categoryIds, List<int> assetTypeIds, List<int> subIds)
        {
            Expression<Func<Model.SubCategory, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((categoryIds != null) && (categoryIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.SubCategory>(predicate, r => categoryIds.Contains(r.CategoryId.Value))
                    : r => categoryIds.Contains(r.CategoryId.Value);
            }

            if ((subIds != null) && (subIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.SubCategory>(predicate, r => subIds.Contains(r.Id))
                    : r => subIds.Contains(r.Id);
            }

            return predicate;
        }

        public IEnumerable<Model.SubCategory> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> categoryIds, List<int> assetTypeIds, List<int> subIds)
        {
            var predicate = GetFiltersPredicate(filter, categoryIds, assetTypeIds, subIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> categoryIds, List<int> assetTypeIds, List<int> subIds)
        {
            var predicate = GetFiltersPredicate(filter, categoryIds, assetTypeIds, subIds);

            return GetQueryable(predicate).Count();
        }
	}
}
