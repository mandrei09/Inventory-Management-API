﻿using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
	public class TaxsRepository : Repository<Tax>, ITaxsRepository
	{
		public TaxsRepository(ApplicationDbContext context)
			: base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
		{ }

		private Expression<Func<Model.Tax, bool>> GetFiltersPredicate(string filter)
		{
			Expression<Func<Model.Tax, bool>> predicate = null;
			if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			return predicate;
		}

		public IEnumerable<Model.Tax> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
		{
			var predicate = GetFiltersPredicate(filter);

			return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
		}

		public int GetCountByFilters(string filter)
		{
			var predicate = GetFiltersPredicate(filter);

			return GetQueryable(predicate).Count();
		}
	}
}
