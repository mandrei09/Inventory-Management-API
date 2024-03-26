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
    public class MatrixLevelsRepository : Repository<MatrixLevel>, IMatrixLevelsRepository
    {
        public MatrixLevelsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => ( a.Uom.Name.Contains(filter) || a.Level.Code.Contains(filter)); })
        { }

        private Expression<Func<Model.MatrixLevel, bool>> GetFiltersPredicate(string filter, List<int?> matrixIds, List<int?> costCenterIds, List<int?> subCategoryIds)
        {
            Expression<Func<Model.MatrixLevel, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			if ((matrixIds != null) && (matrixIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.MatrixLevel>(predicate, r => matrixIds.Contains(r.MatrixId))
					: r => matrixIds.Contains(r.MatrixId);
			}

			return predicate;
        }

        public IEnumerable<Model.MatrixLevel> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> matrixIds, List<int?> costCenterIds, List<int?> subCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, matrixIds, costCenterIds, subCategoryIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> matrixIds, List<int?> costCenterIds, List<int?> subCategoryIds)
        {
            var predicate = GetFiltersPredicate(filter, matrixIds, costCenterIds, subCategoryIds);

            return GetQueryable(predicate).Count();
        }
    }
}
