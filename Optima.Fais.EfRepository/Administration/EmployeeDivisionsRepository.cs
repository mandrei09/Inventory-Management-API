using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class EmployeeDivisionsRepository : Repository<EmployeeDivision>, IEmployeeDivisionsRepository
    {
        public EmployeeDivisionsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Division.Code.Contains(filter) || a.Division.Name.Contains(filter) || a.Employee.InternalCode.Contains(filter) || a.Employee.FirstName.Contains(filter) || a.Employee.LastName.Contains(filter)); })
        { }

        private Expression<Func<Model.EmployeeDivision, bool>> GetFiltersPredicate(string filter, List<int?> divisionIds, List<int?> employeeIds)
        {
            Expression<Func<Model.EmployeeDivision, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((divisionIds != null) && (divisionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeDivision>(predicate, r => divisionIds.Contains(r.DivisionId))
                    : r => divisionIds.Contains(r.DivisionId);
            }

            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeDivision>(predicate, r => employeeIds.Contains(r.EmployeeId))
                    : r => employeeIds.Contains(r.EmployeeId);
            }

            return predicate;
        }

        public IEnumerable<Model.EmployeeDivision> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> divisionIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, divisionIds, employeeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> divisionIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, divisionIds, employeeIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.EmployeeDivision> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.EmployeeDivisions.AsNoTracking();

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
        public IEnumerable<EmployeeDivision> GetCustomQuery(int employeeId, string SortColumn, string sortDirection)
        {
            var query = _context.EmployeeDivisions
                        .Where(ed => ed.EmployeeId == employeeId && !ed.IsDeleted)
                        .Include(ed => ed.Division).ThenInclude(dv => dv.Department)
                        .Include(ed => ed.Employee)
                        .AsNoTracking().Select(ed => new EmployeeDivision
                        {
                            Id = ed.Id,
                            EmployeeId = ed.EmployeeId,
                            DivisionId = ed.DivisionId,
                            DepartmentId = ed.DepartmentId,
                            Division = ed.Division,
                            Department = ed.Department,
                            Employee = ed.Employee,
                        });

            if ((SortColumn != null) && (SortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<EmployeeDivision>(SortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<EmployeeDivision>(SortColumn));
            }


            return query.ToList();
        }
    }
} 