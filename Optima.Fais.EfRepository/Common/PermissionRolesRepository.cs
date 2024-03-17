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
    public class PermissionRolesRepository : Repository<PermissionRole>, IPermissionRolesRepository
    {
        public PermissionRolesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.PermissionRole, bool>> GetFiltersPredicate(string filter, List<int?> permissionTypeIds, List<int?> permissionIds, List<string> roleIds)
        {
            Expression<Func<Model.PermissionRole, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((permissionTypeIds != null) && (permissionTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.PermissionRole>(predicate, r => permissionTypeIds.Contains(r.PermissionTypeId))
                    : r => permissionTypeIds.Contains(r.PermissionTypeId);
            }

            if ((permissionIds != null) && (permissionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.PermissionRole>(predicate, r => permissionIds.Contains(r.PermissionId))
                    : r => permissionIds.Contains(r.PermissionId);
            }

            if ((roleIds != null) && (roleIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.PermissionRole>(predicate, r => roleIds.Contains(r.RoleId))
                    : r => roleIds.Contains(r.RoleId);
            }


            return predicate;
        }

        public IEnumerable<Model.PermissionRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> permissionTypeIds, List<int?> permissionIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, permissionTypeIds, permissionIds, roleIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> permissionTypeIds, List<int?> permissionIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, permissionTypeIds, permissionIds, roleIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<PermissionRole>> GetPermissionByRoleAsync(string role)
        {
            return await _context.Set<PermissionRole>().Include(i => i.Permission).Include(p => p.PermissionType).Include(i => i.Role).Where(i => i.Role.Name == role && i.IsDeleted == false && i.PermissionType.IsDeleted == false).ToListAsync();
        }
    }
}
