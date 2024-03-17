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
    public class ColumnDefinitionRolesRepository : Repository<ColumnDefinitionRole>, IColumnDefinitionRolesRepository
    {
        public ColumnDefinitionRolesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.ColumnDefinition.HeaderCode.Contains(filter) || a.Role.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.ColumnDefinitionRole, bool>> GetFiltersPredicate(string filter, List<int?> columnDefinitionIds, List<string> roleIds)
        {
            Expression<Func<Model.ColumnDefinitionRole, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            if ((columnDefinitionIds != null) && (columnDefinitionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.ColumnDefinitionRole>(predicate, r => columnDefinitionIds.Contains(r.ColumnDefinitionId))
                    : r => columnDefinitionIds.Contains(r.ColumnDefinitionId);
            }

            if ((roleIds != null) && (roleIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.ColumnDefinitionRole>(predicate, r => roleIds.Contains(r.RoleId))
                    : r => roleIds.Contains(r.RoleId);
            }


            return predicate;
        }

        public IEnumerable<Model.ColumnDefinitionRole> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> columnDefinitionIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, columnDefinitionIds, roleIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> columnDefinitionIds, List<string> roleIds)
        {
            var predicate = GetFiltersPredicate(filter, columnDefinitionIds, roleIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<List<ColumnDefinitionRole>> GetColumnDefinitionByRoleAsync(string role)
        {
            return await _context.Set<ColumnDefinitionRole>()
                .Include(i => i.ColumnDefinition)
                .Include(i => i.Role)
                .Where(i => i.Role.Name == role && i.IsDeleted == false && i.ColumnDefinition.IsDeleted == false).ToListAsync();
        }
    }
}
