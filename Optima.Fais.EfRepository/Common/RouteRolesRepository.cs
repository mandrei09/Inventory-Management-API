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
    public class RouteRolesRepository : Repository<RouteRole>, IRouteRolesRepository
    {
        public RouteRolesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Route.Name.Contains(filter) || a.Role.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.RouteRole, bool>> GetFiltersPredicate(string filter, List<int?> routeIds, List<string> roleIds)
        {
            Expression<Func<Model.RouteRole, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((routeIds != null) && (routeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RouteRole>(predicate, r => routeIds.Contains(r.RouteId))
                    : r => routeIds.Contains(r.RouteId);
            }

            if ((roleIds != null) && (roleIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.RouteRole>(predicate, r => roleIds.Contains(r.RoleId))
                    : r => roleIds.Contains(r.RoleId);
            }


            return predicate;
        }

        public IEnumerable<Model.RouteRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> routeIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, routeIds, roleIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> routeIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, routeIds, roleIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<RouteRole>> RouteByRoleAsync(string role)
        {
            return await _context.Set<RouteRole>()
                .Include(i => i.Route)
                .Include(i => i.Role)
                .Where(i => i.Role.Name == role && i.IsDeleted == false && i.Route.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Route>> GetAllIncludingRouteChildrensAsync(List<string> roleIds)
        {
            return await _context.Set<RouteRole>()
                .Include(r => r.Route).ThenInclude(b => b.Badge)
                .Include(r => r.Route).ThenInclude(i => i.RouteChildrens).ThenInclude(b => b.Badge)
                .Include(r => r.Role)
                .Where(r => roleIds.Contains(r.RoleId) && r.IsDeleted == false && r.Route.Active == true).OrderBy(r => r.Route.Position)
                .Select(a => new Model.Route {
                    Url = a.Route.Url,
                    Name = a.Route.Name,
                    Icon = a.Route.Icon,
                    Class = a.Route.Class,
                    Variant = a.Route.Variant,
                    Active = a.Route.Active,
                    Title = a.Route.Title,
                    Divider = a.Route.Divider,
                    Badge = new Badge { Variant = a.Route.Badge.Variant, Text = a.Route.Badge.Text },
                    RouteChildrens = a.Route.RouteChildrens
                })
                .ToListAsync();
        }
    }
}
