using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class ProjectTypeDivisionsRepository : Repository<ProjectTypeDivision>, IProjectTypeDivisionsRepository
    {
        public ProjectTypeDivisionsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.ProjectType.Code.Contains(filter) || a.ProjectType.Name.Contains(filter) || a.Division.Code.Contains(filter) || a.Division.Name.Contains(filter) || a.Division.Department.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.ProjectTypeDivision, bool>> GetFiltersPredicate(string filter, List<int?> projectTypeIds, List<int> divisionIds, List<int?> divIds)
        {
            Expression<Func<Model.ProjectTypeDivision, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((projectTypeIds != null) && (projectTypeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.ProjectTypeDivision>(predicate, r => projectTypeIds.Contains(r.ProjectTypeId))
                    : r => projectTypeIds.Contains(r.ProjectTypeId);
            }

			if ((divisionIds != null) && (divisionIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.ProjectTypeDivision>(predicate, r => divisionIds.Contains(r.DivisionId))
					: r => divisionIds.Contains(r.DivisionId);
			}

			if ((divIds != null) && (divIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.ProjectTypeDivision>(predicate, r => divIds.Contains(r.DivisionId))
					: r => divIds.Contains(r.DivisionId);
			}

			return predicate;
        }

        public IEnumerable<Model.ProjectTypeDivision> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> projectTypeIds, List<int> divisionIds, List<int?> divIds)
        {
            var predicate = GetFiltersPredicate(filter, projectTypeIds, divisionIds, divIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> projectTypeIds, List<int> divisionIds, List<int?> divIds)
        {
            var predicate = GetFiltersPredicate(filter, projectTypeIds, divisionIds, divIds);

            return GetQueryable(predicate).Count();
        }
    }
}
