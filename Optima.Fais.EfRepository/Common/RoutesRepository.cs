using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace Optima.Fais.EfRepository
{
	public class RoutesRepository : Repository<Route>, IRoutesRepository
	{
		public RoutesRepository(ApplicationDbContext context)
			: base(context, (filter) => { return (a) => (a.Url.Contains(filter) || a.Name.Contains(filter) || a.Icon.Contains(filter)); })
		{ }

		private Expression<Func<Model.Route, bool>> GetFiltersPredicate(string filter, List<string> roleIds)
		{
			Expression<Func<Model.Route, bool>> predicate = null;
			if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			if ((roleIds != null) && (roleIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Route>(predicate, r => roleIds.Contains(r.RoleId))
					: r => roleIds.Contains(r.RoleId);
			}

			return predicate;
		}

		public IEnumerable<Model.Route> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<string> roleIds)
		{
			var predicate = GetFiltersPredicate(filter, roleIds);

			return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
		}

		public int GetCountByFilters(string filter, List<string> roleIds)
		{
			var predicate = GetFiltersPredicate(filter, roleIds);

			return GetQueryable(predicate).Count();
		}

		public async Task<List<Route>> GetAllIncludingRouteChildrensAsync(string role)
		{
			return await _context.Set<Route>()
				.Include(b => b.Badge)
				.Include(i => i.RouteChildrens)
					.ThenInclude(b => b.Badge)
				.Include(r => r.Role)
				
				.Where(r => r.Role.NormalizedName == role.ToUpper() && r.IsDeleted == false && r.Active == true).OrderBy(r => r.Position).ToListAsync();
		}
	}
}
