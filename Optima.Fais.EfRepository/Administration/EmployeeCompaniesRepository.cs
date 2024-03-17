using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class EmployeeCompaniesRepository : Repository<EmployeeCompany>, IEmployeeCompaniesRepository
    {
        public EmployeeCompaniesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Company.Code.Contains(filter) || a.Company.Name.Contains(filter) || a.Employee.InternalCode.Contains(filter) || a.Employee.FirstName.Contains(filter) || a.Employee.LastName.Contains(filter)); })
        { }

        private Expression<Func<Model.EmployeeCompany, bool>> GetFiltersPredicate(string filter, List<int?> companyIds, List<int?> employeeIds)
        {
            Expression<Func<Model.EmployeeCompany, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((companyIds != null) && (companyIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeCompany>(predicate, r => companyIds.Contains(r.CompanyId))
                    : r => companyIds.Contains(r.CompanyId);
            }

            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeCompany>(predicate, r => employeeIds.Contains(r.EmployeeId))
                    : r => employeeIds.Contains(r.EmployeeId);
            }

			return predicate;
        }

        public IEnumerable<Model.EmployeeCompany> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> companyIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, companyIds, employeeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> companyIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, companyIds, employeeIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.EmployeeCompany> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.EmployeeCompanies.AsNoTracking();

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
