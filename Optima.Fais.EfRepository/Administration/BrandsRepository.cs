using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
	public class BrandsRepository : Repository<Brand>, IBrandsRepository
	{
		public BrandsRepository(ApplicationDbContext context)
			: base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
		{ }

		private Expression<Func<Model.Brand, bool>> GetFiltersPredicate(string filter, List<int> dictionaryItemIds)
		{
			Expression<Func<Model.Brand, bool>> predicate = null;
			if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			if ((dictionaryItemIds != null) && (dictionaryItemIds.Count > 0))
			{
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Brand>(predicate, r => dictionaryItemIds.Contains(r.DictionaryItemId.Value) || r.Code == "00")
                    : r => dictionaryItemIds.Contains(r.DictionaryItemId.Value) || r.Code == "00";
            }

			return predicate;
		}

		public IEnumerable<Model.Brand> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> dictionaryItemIds)
		{
			var predicate = GetFiltersPredicate(filter, dictionaryItemIds);

			return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
		}

		public int GetCountByFilters(string filter, List<int> dictionaryItemIds)
		{
			var predicate = GetFiltersPredicate(filter, dictionaryItemIds);

			return GetQueryable(predicate).Count();
		}
	}
}
