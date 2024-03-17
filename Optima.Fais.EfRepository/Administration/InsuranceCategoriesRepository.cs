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
	public class InsuranceCategoriesRepository : Repository<InsuranceCategory>, IInsuranceCategoriesRepository
	{
		public InsuranceCategoriesRepository(ApplicationDbContext context)
			: base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
		{ }

		private Expression<Func<Model.InsuranceCategory, bool>> GetFiltersPredicate(string filter)
		{
			Expression<Func<Model.InsuranceCategory, bool>> predicate = null;
			if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			return predicate;
		}

		public IEnumerable<Model.InsuranceCategory> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
		{
			var predicate = GetFiltersPredicate(filter);

			return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
		}

		public int GetCountByFilters(string filter)
		{
			var predicate = GetFiltersPredicate(filter);

			return GetQueryable(predicate).Count();
		}

		public IEnumerable<Model.InsuranceCategory> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
		{
			var query = _context.InsuranceCategories.AsNoTracking();

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
