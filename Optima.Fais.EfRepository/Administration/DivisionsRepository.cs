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
    public class DivisionsRepository : Repository<Division>, IDivisionsRepository
    {
        public DivisionsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)) || a.Department.Name.Contains(filter); })
        { }

        private Expression<Func<Division, bool>> GetFiltersPredicate(string filter, List<int?> departmentIds, List<int?> divisionIds, List<int?> locationIds)
        {
            Expression<Func<Division, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((departmentIds != null) && (departmentIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Division>(predicate, r => departmentIds.Contains(r.DepartmentId))
                    : r => departmentIds.Contains(r.DepartmentId);
            }

            if ((locationIds != null) && (locationIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Division>(predicate, r => locationIds.Contains(r.LocationId))
                    : r => locationIds.Contains(r.LocationId);
            }

            if ((divisionIds != null) && (divisionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Division>(predicate, r => divisionIds.Contains(r.Id))
                    : r => divisionIds.Contains(r.Id);
            }

            return predicate;
        }

        public IEnumerable<Division> GetByFilters(string filter, List<int?> departmentIds, List<int?> divisionIds, List<int?> locationIds, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter, departmentIds, divisionIds, locationIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filters, List<int?> departmentIds, List<int?> divisionIds, List<int?> locationIds)
        {
            var predicate = GetFiltersPredicate(filters, departmentIds, divisionIds, locationIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.Division> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.Divisions.AsNoTracking();

            if (lastId.HasValue)
            {
                query = query
                    .Where(r => (((r.ModifiedAt == lastModifiedAt) && (r.Id > lastId)) || (r.ModifiedAt > lastModifiedAt)));
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }
            else
            {
                totalItems = query.Count();
                query = query
                    .OrderBy(a => a.ModifiedAt)
                    .ThenBy(a => a.Id)
                    .Take(pageSize);
            }

            return query.ToList();
        }
    }
}
