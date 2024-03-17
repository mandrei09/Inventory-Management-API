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
	public class CompaniesRepository : Repository<Company>, ICompaniesRepository
	{
		public CompaniesRepository(ApplicationDbContext context)
			: base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
		{ }

		private Expression<Func<Model.Company, bool>> GetFiltersPredicate(string filter, List<int?> exceptCompanyIds)
		{
			Expression<Func<Model.Company, bool>> predicate = null;
			if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			if ((exceptCompanyIds != null) && (exceptCompanyIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Company>(predicate, r => !exceptCompanyIds.Contains(r.Id))
					: r => !exceptCompanyIds.Contains(r.Id);
			}

			return predicate;
		}

		public IEnumerable<Model.Company> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> exceptCompanyIds)
		{
			var predicate = GetFiltersPredicate(filter, exceptCompanyIds);

			return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
		}

		public int GetCountByFilters(string filter, List<int?> exceptCompanyIds)
		{
			var predicate = GetFiltersPredicate(filter, exceptCompanyIds);

			return GetQueryable(predicate).Count();
		}

		public IEnumerable<Model.Company> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
		{
			var query = _context.Companies.AsNoTracking();

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
