using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class CostCentersRepository : Repository<CostCenter>, ICostCentersRepository
    {
        public CostCentersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (
                c.Code.Contains(filter) || 
                c.Name.Contains(filter) || 
                c.Division.Name.Contains(filter) || 
                c.Division.Department.Name.Contains(filter) || 
                c.Region.Name.Contains(filter) ||
                c.Administration.Name.Contains(filter) ||
                c.Location.Name.Contains(filter) ||
                c.AdmCenter.Name.Contains(filter)); })
        { }

        private Expression<Func<Model.CostCenter, bool>> GetFiltersPredicate(string filter, List<int> administrationIds, List<int> admCenterIds, List<int?> divisionIds, List<int?> departmentIds, List<int> locationIds, List<int> costCenterIds, List<int?> exceptCostCenterIds, bool fromStock)
        {
            Expression<Func<Model.CostCenter, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((admCenterIds != null) && (admCenterIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.CostCenter, int>((id) => { return c => c.AdmCenterId == id; }, admCenterIds);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.CostCenter>(predicate, inListPredicate)
                    : inListPredicate;
            }

			if ((administrationIds != null) && (administrationIds.Count > 0))
			{
				var inListPredicate = ExpressionHelper.GetInListPredicate<Model.CostCenter, int>((id) => { return c => c.AdministrationId == id; }, administrationIds);

				predicate = predicate != null
					? ExpressionHelper.And<Model.CostCenter>(predicate, inListPredicate)
					: inListPredicate;
			}

			//if ((divisionIds != null) && (divisionIds.Count > 0))
			//{
			//    var inListPredicate = ExpressionHelper.GetInListPredicate<Model.CostCenter, int>((id) => { return c => c.DivisionId == id; }, divisionIds);

			//    predicate = predicate != null
			//        ? ExpressionHelper.And<Model.CostCenter>(predicate, inListPredicate)
			//        : inListPredicate;
			//}

			if ((divisionIds != null) && (divisionIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.CostCenter>(predicate, r => divisionIds.Contains(r.DivisionId))
                    : r => divisionIds.Contains(r.DivisionId);
            }

            if ((departmentIds != null) && (departmentIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.CostCenter>(predicate, r => departmentIds.Contains(r.Division.DepartmentId))
                    : r => departmentIds.Contains(r.Division.DepartmentId);
            }



            //if ((departmentIds != null) && (departmentIds.Count > 0))
            //{
            //    var inListPredicate = ExpressionHelper.GetInListPredicate<Model.CostCenter, int>((id) => { return c => c.Division.DepartmentId == id; }, departmentIds);

            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.CostCenter>(predicate, inListPredicate)
            //        : inListPredicate;
            //}

            if ((locationIds != null) && (locationIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.CostCenter, int>((id) => { return c => c.LocationId == id; }, locationIds);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.CostCenter>(predicate, inListPredicate)
                    : inListPredicate;
            }


            if ((costCenterIds != null) && (costCenterIds.Count > 0))
            {
                var inListPredicate = ExpressionHelper.GetInListPredicate<Model.CostCenter, int>((id) => { return c => c.Id == id; }, costCenterIds);

                predicate = predicate != null
                    ? ExpressionHelper.And<Model.CostCenter>(predicate, inListPredicate)
                    : inListPredicate;
            }

            if ((exceptCostCenterIds != null) && (exceptCostCenterIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.CostCenter>(predicate, r => !exceptCostCenterIds.Contains(r.Id))
                    : r => !exceptCostCenterIds.Contains(r.Id);
            }

            //if (fromStock)
            //{
            //    predicate = predicate != null
            //        ? ExpressionHelper.And<Model.CostCenter>(predicate, r => r.Storage != null)
            //        : r => r.Storage != null;
            //}

            return predicate;
        }

        public IEnumerable<Model.CostCenter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> administrationIds, List<int> admCenterIds, List<int?> divisionIds, List<int?> departmentIds, List<int> locationIds, List<int> costCenterIds, List<int?> exceptCostCenterIds, bool fromStock)
        {
            var predicate = GetFiltersPredicate(filter, administrationIds, admCenterIds, divisionIds, departmentIds, locationIds, costCenterIds, exceptCostCenterIds, fromStock);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int> administrationIds, List<int> admCenterIds, List<int?> divisionIds, List<int?> departmentIds, List<int> locationIds, List<int> costCenterIds, List<int?> exceptCostCenterIds, bool fromStock)
        {
            var predicate = GetFiltersPredicate(filter, administrationIds, admCenterIds, divisionIds, departmentIds, locationIds, costCenterIds, exceptCostCenterIds, fromStock);

            return GetQueryable(predicate).Count();
        }

        public IEnumerable<Model.CostCenter> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems)
        {
            var query = _context.CostCenters.AsNoTracking();

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

		public async Task<ImportCostCenterResult> Import(Dto.CostCenterImport importData)
		{
			Model.CostCenter costCenter = null;
			Model.AdmCenter admCenter = null;
			Model.Region region = null;
			Model.Company company = null;

			company = await _context.Set<Model.Company>().Where(c => c.Code == importData.CompanyCode).FirstOrDefaultAsync();
			admCenter = await _context.Set<Model.AdmCenter>().Where(i => i.Code == importData.AdmCenterCode).FirstOrDefaultAsync();
			region = await _context.Set<Model.Region>().Where(i => i.Code == importData.AdmCenterCode).FirstOrDefaultAsync();
			costCenter = await _context.Set<Model.CostCenter>().Where(i => i.Code == importData.CostCenterCode).FirstOrDefaultAsync();

			if (company == null)
			{
				company = new Model.Company
				{
					Code = importData.CompanyCode,
					Name = importData.CompanyCode,
					IsDeleted = false,
				};
				_context.Set<Model.Company>().Add(company);
            }
            else
            {
                company.IsDeleted = false;
                company.ModifiedAt = DateTime.Now;

                _context.Update(company);
            }

			if (admCenter == null)
			{
				admCenter = new Model.AdmCenter
				{
					Code = importData.AdmCenterCode,
					Name = importData.AdmCenterCode,
					IsDeleted = false,
					CompanyId = company.Id
				};
				_context.Set<Model.AdmCenter>().Add(admCenter);
			}
			else
			{
				admCenter.IsDeleted = false;
				admCenter.ModifiedAt = DateTime.Now;

				_context.Update(admCenter);
			}

			if (region == null)
			{
				region = new Model.Region
				{
					Code = importData.AdmCenterCode,
					Name = importData.AdmCenterCode,
					IsDeleted = false,
					CompanyId = company.Id
				};
				_context.Set<Model.Region>().Add(region);
			}
			else
			{
				region.IsDeleted = false;
				region.ModifiedAt = DateTime.Now;

				_context.Update(region);
			}


			if (costCenter == null)
			{
				costCenter = new Model.CostCenter
				{
					Code = importData.CostCenterCode,
					Name = importData.CostCenterName,
                    AdmCenter = admCenter,
                    Region = region,
					IsDeleted = false,
					CompanyId = company.Id
				};
				_context.Set<Model.CostCenter>().Add(costCenter);
			}
			else
			{
				costCenter.IsDeleted = false;
                costCenter.Name = importData.CostCenterName;
                costCenter.AdmCenter = admCenter;
                costCenter.Region = region;
				costCenter.ModifiedAt = DateTime.Now;

				_context.Update(costCenter);
			}

			_context.SaveChanges();

			return new ImportCostCenterResult { Success = true, Message = costCenter.Name, Id = costCenter.Id };
		}
	}
}
