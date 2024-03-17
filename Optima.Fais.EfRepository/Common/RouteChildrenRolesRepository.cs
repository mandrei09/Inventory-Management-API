using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class RouteChildrenRolesRepository : Repository<RouteChildrenRole>, IRouteChildrenRolesRepository
    {
        public RouteChildrenRolesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.RouteChildren.Name.Contains(filter) || a.Role.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.RouteChildrenRole, bool>> GetFiltersPredicate(string filter, List<int?> routeChildrenIds, List<string> roleIds)
        {
            Expression<Func<Model.RouteChildrenRole, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((routeChildrenIds != null) && (routeChildrenIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RouteChildrenRole>(predicate, r => routeChildrenIds.Contains(r.RouteChildrenId))
                    : r => routeChildrenIds.Contains(r.RouteChildrenId);
            }

            if ((roleIds != null) && (roleIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RouteChildrenRole>(predicate, r => roleIds.Contains(r.RoleId))
                    : r => roleIds.Contains(r.RoleId);
            }


            return predicate;
        }

        public IEnumerable<Model.RouteChildrenRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> routeChildrenIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, routeChildrenIds, roleIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> routeChildrenIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, routeChildrenIds, roleIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<RouteChildrenRole>> RouteChildrenByRoleAsync(string role)
        {
            return await _context.Set<RouteChildrenRole>()
                .Include(i => i.RouteChildren)
                .Include(i => i.Role)
                .Where(i => i.Role.Name == role && i.IsDeleted == false && i.RouteChildren.IsDeleted == false).ToListAsync();
        }
    }
}
