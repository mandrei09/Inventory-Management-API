using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class ColumnDefinitionsRepository : Repository<ColumnDefinition>, IColumnDefinitionsRepository
    {
        public ColumnDefinitionsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.HeaderCode.Contains(filter) || a.Property.Contains(filter) || a.SortBy.Contains(filter) || a.Include.Contains(filter)); })
        { }

        private Expression<Func<Model.ColumnDefinition, bool>> GetFiltersPredicate(string filter, List<int> tableDefinitionIds, List<string> roleIds, string roleName)
        {
            Expression<Func<Model.ColumnDefinition, bool>> predicate = null;

            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);
			

			if ((tableDefinitionIds != null) && (tableDefinitionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<ColumnDefinition>(predicate, cd => tableDefinitionIds.Contains(cd.TableDefinitionId))
                    : cd => tableDefinitionIds.Contains(cd.TableDefinitionId);
            }

			if ((roleIds != null) && (roleIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<ColumnDefinition>(predicate, cd => roleIds.Contains(cd.RoleId))
					: cd => roleIds.Contains(cd.RoleId);
			}


			if ((roleName != null) && (roleName != ""))
			{
				predicate = predicate != null
					? ExpressionHelper.And<ColumnDefinition>(predicate, cd => roleName.Contains(cd.AspNetRole.Name))
					: cd => roleName.Contains(cd.AspNetRole.Name);
			}

			return predicate;
        }

        public IEnumerable<Model.ColumnDefinition> GetByFilters(string filter, List<int> tableDefinitionIds, List<string> roleIds, string roleName, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter, tableDefinitionIds, roleIds, roleName);

            return GetQueryable(predicate, "TableDefinition,AspNetRole,ColumnFilter", sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> tableDefinitionIds, List<string> roleIds, string roleName)
        {
            var predicate = GetFiltersPredicate(filter, tableDefinitionIds, roleIds, roleName);

            return GetQueryable(predicate).Count();
        }
    }
}
