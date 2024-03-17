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
    public class EmployeeStoragesRepository : Repository<EmployeeStorage>, IEmployeeStoragesRepository
    {
        public EmployeeStoragesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Storage.Code.Contains(filter) || a.Storage.Name.Contains(filter) || a.Employee.InternalCode.Contains(filter) || a.Employee.FirstName.Contains(filter) || a.Employee.LastName.Contains(filter)); })
        { }

        private Expression<Func<Model.EmployeeStorage, bool>> GetFiltersPredicate(string filter, List<int?> storageIds, List<int?> employeeIds)
        {
            Expression<Func<Model.EmployeeStorage, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((storageIds != null) && (storageIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeStorage>(predicate, r => storageIds.Contains(r.StorageId))
                    : r => storageIds.Contains(r.StorageId);
            }

            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeStorage>(predicate, r => employeeIds.Contains(r.EmployeeId))
                    : r => employeeIds.Contains(r.EmployeeId);
            }

			return predicate;
        }

        public IEnumerable<Model.EmployeeStorage> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> storageIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, storageIds, employeeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> storageIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, storageIds, employeeIds);

            return GetQueryable(predicate).Count();
        }
    }
}
