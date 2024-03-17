using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class ConfigValuesRepository : Repository<ConfigValue>, IConfigValuesRepository
    {
        public ConfigValuesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Description.Contains(filter)); })
        { }

        private Expression<Func<Model.ConfigValue, bool>> GetFiltersPredicate(string filter, List<string> roleIds, string roleName)
        {
            Expression<Func<Model.ConfigValue, bool>> predicate = null;

            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((roleIds != null) && (roleIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<ConfigValue>(predicate, cd => roleIds.Contains(cd.RoleId))
                    : cd => roleIds.Contains(cd.RoleId);
            }


            if ((roleName != null) && (roleName != ""))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<ConfigValue>(predicate, cd => roleName.Contains(cd.AspNetRole.Name))
                    : cd => roleName.Contains(cd.AspNetRole.Name);
            }

            return predicate;
        }

        public IEnumerable<Model.ConfigValue> GetByFilters(string filter, string includes, List<string> roleIds, string roleName, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter, roleIds, roleName);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<string> roleIds, string roleName)
        {
            var predicate = GetFiltersPredicate(filter, roleIds, roleName);

            return GetQueryable(predicate).Count();
        }
    }
}
