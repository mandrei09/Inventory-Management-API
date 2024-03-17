using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class RouteChildrensRepository : Repository<RouteChildren>, IRouteChildrensRepository
    {
        public RouteChildrensRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Name.Contains(filter) || a.Url.Contains(filter) || a.Icon.Contains(filter)); })
        { }

        private Expression<Func<Model.RouteChildren, bool>> GetFiltersPredicate(string filter, List<int> routeIds, List<string> roleIds, string roleName)
        {
            Expression<Func<Model.RouteChildren, bool>> predicate = null;

            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((routeIds != null) && (routeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<RouteChildren>(predicate, cd => routeIds.Contains(cd.RouteId.Value))
                    : cd => routeIds.Contains(cd.RouteId.Value);
            }

            if ((roleIds != null) && (roleIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<RouteChildren>(predicate, cd => roleIds.Contains(cd.RoleId))
                    : cd => roleIds.Contains(cd.RoleId);
            }


            if ((roleName != null) && (roleName != ""))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<RouteChildren>(predicate, cd => roleName.Contains(cd.Role.Name))
                    : cd => roleName.Contains(cd.Role.Name);
            }

            return predicate;
        }

        public IEnumerable<Model.RouteChildren> GetByFilters(string filter, string includes, List<int> routeIds, List<string> roleIds, string roleName, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter, routeIds, roleIds, roleName);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> routeIds, List<string> roleIds, string roleName)
        {
            var predicate = GetFiltersPredicate(filter, routeIds, roleIds, roleName);

            return GetQueryable(predicate).Count();
        }
    }
}
