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
    public class EmployeeCostCentersRepository : Repository<EmployeeCostCenter>, IEmployeeCostCentersRepository
    {
        public EmployeeCostCentersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.CostCenter.Code.Contains(filter) || a.CostCenter.Name.Contains(filter) || a.Employee.InternalCode.Contains(filter) || a.Employee.FirstName.Contains(filter) || a.Employee.LastName.Contains(filter)); })
        { }

        private Expression<Func<Model.EmployeeCostCenter, bool>> GetFiltersPredicate(string filter, List<int?> costCenterIds, List<int?> employeeIds)
        {
            Expression<Func<Model.EmployeeCostCenter, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((costCenterIds != null) && (costCenterIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeCostCenter>(predicate, r => costCenterIds.Contains(r.CostCenterId))
                    : r => costCenterIds.Contains(r.CostCenterId);
            }

            if ((employeeIds != null) && (employeeIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.EmployeeCostCenter>(predicate, r => employeeIds.Contains(r.EmployeeId))
                    : r => employeeIds.Contains(r.EmployeeId);
            }

			return predicate;
        }

        public IEnumerable<Model.EmployeeCostCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> costCenterIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, costCenterIds, employeeIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> costCenterIds, List<int?> employeeIds)
        {
            var predicate = GetFiltersPredicate(filter, costCenterIds, employeeIds);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.EmployeeCostCenter> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.EmployeeCostCenters.AsNoTracking();

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

		public async Task<List<Model.EmployeeCostCenter>> GetAllByCostCenter(ReportFilter reportFilter)
		{
			IQueryable<Model.EmployeeCostCenter> query = null;
			List<Model.EmployeeCostCenter> listInventoryAssets = null;
			query = _context.EmployeeCostCenters.AsNoTracking();

            query = query
                   .Include(i => i.Employee)
                   .Include(i => i.CostCenter).ThenInclude(d => d.Division);

			if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
			{
				query = query.Where(i => i.IsDeleted == false && reportFilter.CostCenterIds.Contains(i.CostCenterId) && i.Employee.IsDeleted == false);
			}

			if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
			{
				query = query.Where(i => i.IsDeleted == false && reportFilter.DivisionIds.Contains(i.CostCenter.DivisionId) && i.Employee.IsDeleted == false);
			}

			if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
			{
				query = query.Where(i => i.IsDeleted == false && reportFilter.DepartmentIds.Contains(i.CostCenter.Division.DepartmentId) && i.Employee.IsDeleted == false);
			}

			if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
			{
				query = query.Where(i => i.IsDeleted == false && reportFilter.AdministrationIds.Contains(i.CostCenter.AdministrationId) && i.Employee.IsDeleted == false);
			}

			listInventoryAssets = query.Select(p => new Model.EmployeeCostCenter
			{
				Employee = p.Employee,
                CostCenter = p.CostCenter,

			}).Distinct().ToList();

			return listInventoryAssets;
		}
	}
}
