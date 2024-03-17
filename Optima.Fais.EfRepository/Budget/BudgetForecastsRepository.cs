using System.Collections.Generic;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using Optima.Fais.Dto;
using System;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Optima.Fais.Model.Utils;
using System.Text;
using Optima.Fais.Model;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using Optima.Fais.Dto.Sync;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace Optima.Fais.EfRepository
{
    public class BudgetForecastsRepository : Repository<Model.BudgetForecast>, IBudgetForecastsRepository
    {

        public BudgetForecastsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.BudgetBase.Code.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.BudgetForecastDetail> GetBuget(BudgetFilter budgetFilter, string filter, string includes, Sorting sorting, Paging paging, out ForecastTotal depTotal)
        {
            IQueryable<Model.BudgetForecast> budgetQuery = null;
            IQueryable<BudgetForecastDetail> query = null;

            budgetQuery = _context.BudgetForecasts.AsNoTracking().AsQueryable();

			int? accMonthId = budgetFilter.AccMonthId;

			if (budgetFilter.MonthYear != null)
			{
				var month = budgetFilter.MonthYear.Value.AddDays(0).Month;
				var year = budgetFilter.MonthYear.Value.AddYears(0).Year;

				accMonthId = _context.Set<Model.AccMonth>().AsNoTracking().Where(a => a.Month == month && a.Year == year && a.IsDeleted == false).Select(a => a.Id).SingleOrDefault();
			}
			else
			{
                accMonthId = _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true && a.IsDeleted == false).Select(a => a.AccMonthBudgetId).SingleOrDefault();
			}

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) 
            {
				budgetQuery = budgetQuery
                    .Where(a => (
                    a.BudgetBase.Code.Contains(budgetFilter.Filter) || 
                    a.BudgetBase.Info.Contains(budgetFilter.Filter) || 
                    a.BudgetBase.Project.Code.Contains(budgetFilter.Filter) || 
                    a.BudgetBase.AdmCenter.Name.Contains(budgetFilter.Filter) || 
                    a.BudgetBase.AssetType.Name.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Activity.Name.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Employee.Email.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Department.Name.Contains(budgetFilter.Filter)));
			}

			if (filter != "" && filter != null)
			{
				budgetQuery = budgetQuery
					.Where(a => (
					a.BudgetBase.Code.Contains(filter) ||
					a.BudgetBase.Info.Contains(filter) ||
					a.BudgetBase.Project.Code.Contains(filter) ||
					a.BudgetBase.AdmCenter.Name.Contains(filter) ||
					a.BudgetBase.AssetType.Name.Contains(filter) ||
					a.BudgetBase.Activity.Name.Contains(filter) ||
					a.BudgetBase.Employee.Email.Contains(filter) ||
					a.BudgetBase.Department.Name.Contains(filter)));
			}

			if (budgetFilter.HasChange != null) budgetQuery = budgetQuery.Where(a => (
                (a.HasChangeApril == budgetFilter.HasChange) ||
                (a.HasChangeMay == budgetFilter.HasChange) ||
                (a.HasChangeJune == budgetFilter.HasChange) ||
                (a.HasChangeJuly == budgetFilter.HasChange) ||
                (a.HasChangeAugust == budgetFilter.HasChange) ||
                (a.HasChangeSeptember == budgetFilter.HasChange) ||
                (a.HasChangeOctomber == budgetFilter.HasChange) ||
                (a.HasChangeNovember == budgetFilter.HasChange) ||
                (a.HasChangeDecember == budgetFilter.HasChange) ||
                (a.HasChangeJanuary == budgetFilter.HasChange) ||
                (a.HasChangeFebruary == budgetFilter.HasChange) ||
                (a.HasChangeMarch == budgetFilter.HasChange)
                ));

            if (budgetFilter.IsFirst != null) budgetQuery = budgetQuery.Where(a => a.IsFirst != budgetFilter.IsFirst);

            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "BudgetForecast":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetForecastDetail { BudgetForecast = budget });

			if (budgetFilter.Role != null && budgetFilter.Role != "")
			{
				if (budgetFilter.Role.ToUpper() == "ADMINISTRATOR")
				{
                    //
				}
				else if (budgetFilter.Role.ToUpper() == "PROCUREMENT")
				{
					List<int?> divisionIds = new List<int?>();
					divisionIds.Add(1482);

					query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.DivisionId != id; }, divisionIds));
				}
				else if (budgetFilter.Role.ToUpper() == "PROC-IT")
				{
					//List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == budgetFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

					//if (divisionIds.Count == 0)
					//{
					//	divisionIds = new List<int?>();
					//	divisionIds.Add(-1);
					//}

					List<int?> divisionIds = new List<int?>();
					divisionIds.Add(1482);

					query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.DivisionId == id; }, divisionIds));
				}
				else
				{
					List<int?> divisionIds = _context.Set<Model.EmployeeDivision>().AsNoTracking().Where(e => e.EmployeeId == budgetFilter.EmployeeId && e.IsDeleted == false).Select(c => (int?)c.DivisionId).ToList();

					if (divisionIds.Count == 0)
					{
						divisionIds = new List<int?>();
						divisionIds.Add(-1);
					}

					query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.DivisionId == id; }, divisionIds));
				}
			}

			if ((budgetFilter.ProjectIds != null) && (budgetFilter.ProjectIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.ProjectId == id; }, budgetFilter.ProjectIds));
            }

            
            if ((budgetFilter.AssetTypeIds != null) && (budgetFilter.AssetTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.AssetTypeId == id; }, budgetFilter.AssetTypeIds));
            }

            
            if ((budgetFilter.ProjectTypeIds != null) && (budgetFilter.ProjectTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.ProjectTypeId == id; }, budgetFilter.ProjectTypeIds));
            }

            
            if ((budgetFilter.AdmCenterIds != null) && (budgetFilter.AdmCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.AdmCenterId == id; }, budgetFilter.AdmCenterIds));
            }

            
            if ((budgetFilter.DivisionIds != null) && (budgetFilter.DivisionIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.DivisionId == id; }, budgetFilter.DivisionIds));
			}
			            

            if ((budgetFilter.ActivityIds != null) && (budgetFilter.ActivityIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.ActivityId == id; }, budgetFilter.ActivityIds));
			}


            if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.EmployeeId == id; }, budgetFilter.EmployeeIds));
            }


            if ((budgetFilter.ProjectId != null) && (budgetFilter.ProjectId > 0))
			{
				query = query.Where(a => a.BudgetForecast.BudgetBase.ProjectId == budgetFilter.ProjectId);
			}


            if (budgetFilter.EmployeeIds.Count > 0)
            {
                query = query.Where(a => budgetFilter.EmployeeIds.Contains(a.BudgetForecast.BudgetBase.EmployeeId));
            }

            //if ((budgetFilter.AccMonthIds != null) && (budgetFilter.AccMonthIds.Count > 0))
            //         {
            //             query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.IsDeleted == false && a.BudgetForecast.AccMonthId == id; }, budgetFilter.AccMonthIds));
            //}
            //else
            //{
            //             query = query.Where(a => a.BudgetForecast.IsDeleted == false && a.BudgetForecast.BudgetBase.IsDeleted == false && a.BudgetForecast.AccMonthId == accMonthId && a.BudgetForecast.IsLast == true);
            //}

            query = query.Where(a => a.BudgetForecast.IsDeleted == false && a.BudgetForecast.BudgetBase.IsDeleted == false && a.BudgetForecast.AccMonthId == accMonthId && a.BudgetForecast.IsLast == true);
            
            depTotal = new ForecastTotal();
            depTotal.Count = query.Count();
            depTotal.April = query.Sum(a => a.BudgetForecast.April);
            depTotal.May = query.Sum(a => a.BudgetForecast.May);
            depTotal.June = query.Sum(a => a.BudgetForecast.June);
            depTotal.July = query.Sum(a => a.BudgetForecast.July);
            depTotal.August = query.Sum(a => a.BudgetForecast.August);
            depTotal.September = query.Sum(a => a.BudgetForecast.September);
            depTotal.Octomber = query.Sum(a => a.BudgetForecast.Octomber);
            depTotal.November = query.Sum(a => a.BudgetForecast.November);
            depTotal.December = query.Sum(a => a.BudgetForecast.December);
            depTotal.January = query.Sum(a => a.BudgetForecast.January);
            depTotal.February = query.Sum(a => a.BudgetForecast.February);
            depTotal.March = query.Sum(a => a.BudgetForecast.March);
            depTotal.Total = query.Sum(a => a.BudgetForecast.Total);
            depTotal.TotalRem = query.Sum(a => a.BudgetForecast.TotalRem);
			depTotal.ImportValueOrder = query.Sum(a => a.BudgetForecast.ImportValueOrder);
			depTotal.ValueAsset = query.Sum(a => a.BudgetForecast.ValueAsset);
			depTotal.ValueAssetYTD = query.Sum(a => a.BudgetForecast.ValueAssetYTD);
			depTotal.ValueAssetYTG = query.Sum(a => a.BudgetForecast.ValueAssetYTG);
			depTotal.ValueOrder = query.Sum(a => a.BudgetForecast.ValueOrder);
			depTotal.ValueOrder = query.Sum(a => a.BudgetForecast.ValOrderPending);
			depTotal.ValueOrder = query.Sum(a => a.BudgetForecast.ValOrderApproved);
			depTotal.ValueOrder = query.Sum(a => a.BudgetForecast.ValRequest);


			

			if (sorting != null)
            {
				//query = sorting.Direction.ToLower() == "asc"
				//	? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column))
				//	: query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetDetail>(sorting.Column));
			}

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            //list = list.GroupBy(item => item.Budget.ProjectId)
            //        .Select(group => new BudgetDetail()
            //        {
            //            Budget = list.First().Budget,
            //              // = group.Key,
            //              //Orders = group.ToList()
            //          })
            //        .ToList();

            return list;
        }

        public IEnumerable<Model.BudgetForecastDetail> GetBugetUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out ForecastTotal depTotal)
        {
            IQueryable<Model.BudgetForecast> budgetQuery = null;
            IQueryable<BudgetForecastDetail> query = null;

            budgetQuery = _context.BudgetForecasts.AsNoTracking().AsQueryable();

            // int accMonthId = _context.Set<Model.AccMonth>().AsNoTracking().Where(a => a.IsActive == true).Select(a => a.Id).SingleOrDefault(); // trebuie modificata conditia !!!important
            //int accMonthId = 51;

			if (budgetFilter.Filter != "" && budgetFilter.Filter != null)
			{
				budgetQuery = budgetQuery
					.Where(a => (
					a.BudgetBase.Code.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Info.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Project.Code.Contains(budgetFilter.Filter) ||
					a.BudgetBase.AdmCenter.Name.Contains(budgetFilter.Filter) ||
					a.BudgetBase.AssetType.Name.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Activity.Name.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Employee.Email.Contains(budgetFilter.Filter) ||
					a.BudgetBase.Department.Name.Contains(budgetFilter.Filter)));
			}
				
				//budgetQuery = budgetQuery.Where(a => (a.BudgetBase.Project.Code.Contains(budgetFilter.Filter) || a.BudgetBase.Code.Contains(budgetFilter.Filter) || a.BudgetBase.Info.Contains(budgetFilter.Filter) ));


            includes = includes ?? string.Empty;

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int dotIndex = includeProperty.IndexOf(".");
                string prefix = string.Empty;
                string property = string.Empty;

                if (dotIndex > 0)
                {
                    prefix = includeProperty.Substring(0, dotIndex);
                    property = includeProperty.Length > dotIndex ? includeProperty.Substring(dotIndex + 1) : string.Empty;
                }
                else
                {
                    prefix = includeProperty;
                }


                switch (prefix)
                {
                    case "BudgetForecast":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetForecastDetail { BudgetForecast = budget });

            if ((budgetFilter.CompanyIds != null) && (budgetFilter.CompanyIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.CompanyId == id; }, budgetFilter.CompanyIds));
            }

            if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.EmployeeId == id; }, budgetFilter.EmployeeIds));
            }

            if ((budgetFilter.AdmCenterIds != null) && (budgetFilter.AdmCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.AdmCenterId == id; }, budgetFilter.AdmCenterIds));
            }

            if ((budgetFilter.AssetTypeIds != null) && (budgetFilter.AssetTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.AssetTypeId == id; }, budgetFilter.AssetTypeIds));
            }

            if ((budgetFilter.ProjectTypeIds != null) && (budgetFilter.ProjectTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.ProjectTypeId == id; }, budgetFilter.ProjectTypeIds));
            }

            if ((budgetFilter.DivisionIds != null) && (budgetFilter.DivisionIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.DivisionId == id; }, budgetFilter.DivisionIds));
            }

			if ((budgetFilter.DepartmentIds != null) && (budgetFilter.DepartmentIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.BudgetBase.Division.DepartmentId == id; }, budgetFilter.DepartmentIds));
			}

			if ((budgetFilter.BudgetForecastIds != null) && (budgetFilter.BudgetForecastIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetForecastDetail, int?>((id) => { return a => a.BudgetForecast.Id == id; }, budgetFilter.BudgetForecastIds));
            }

            query = query.Where(a => a.BudgetForecast.BudgetBase.IsDeleted == false && a.BudgetForecast.BudgetBase.Validated == true && a.BudgetForecast.BudgetBase.IsAccepted == true && a.BudgetForecast.IsDeleted == false && a.BudgetForecast.IsLast == true);
            // query = query.Where(a => a.BudgetForecast.BudgetBase.BudgetMonthBase.FirstOrDefault().IsLast == true);


            depTotal = new ForecastTotal();
            depTotal.Count = query.Count();
            depTotal.April = budgetQuery.Sum(a => a.April);
            depTotal.May = budgetQuery.Sum(a => a.May);
            depTotal.June = budgetQuery.Sum(a => a.June);
            depTotal.July = budgetQuery.Sum(a => a.July);
            depTotal.August = budgetQuery.Sum(a => a.August);
            depTotal.September = budgetQuery.Sum(a => a.September);
            depTotal.Octomber = budgetQuery.Sum(a => a.Octomber);
            depTotal.November = budgetQuery.Sum(a => a.November);
            depTotal.December = budgetQuery.Sum(a => a.December);
            depTotal.January = budgetQuery.Sum(a => a.January);
            depTotal.February = budgetQuery.Sum(a => a.February);
            depTotal.March = budgetQuery.Sum(a => a.March);
            depTotal.Total = budgetQuery.Sum(a => a.Total);

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetForecastDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetForecastDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public Model.BudgetForecast GetDetailsById(int id, string includes)
        {
            IQueryable<Model.BudgetForecast> query = null;
            query = GetBudgetQuery(includes);

            return query.AsNoTracking().Where(a => a.Id == id)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Company)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Employee)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Project)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Country)
                .Include(b => b.AccMonth)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Activity)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Department)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Division)
                .Include(b => b.BudgetBase).ThenInclude(b => b.CostCenter)
                .Include(b => b.BudgetBase).ThenInclude(b => b.AdmCenter)
                .Include(b => b.BudgetBase).ThenInclude(b => b.Region)
                .Include(b => b.BudgetBase).ThenInclude(b => b.ProjectType)
                .Include(b => b.BudgetBase).ThenInclude(b => b.AssetType)
                .Include(b => b.BudgetBase).ThenInclude(b => b.AppState)
                .Include(b => b.BudgetBase).ThenInclude(b => b.StartMonth)
                .SingleOrDefault();
        }

        private IQueryable<Model.BudgetForecast> GetBudgetQuery(string includes)
        {
            IQueryable<Model.BudgetForecast> query = null;
            query = _context.BudgetForecasts.AsNoTracking();

            return query;
        }

        public async Task<RequestResult> UpdateBudgetForecast(BudgetSave budgetDto)
        {
            Model.BudgetBase budgetBase = null;
            Model.BudgetBaseOp budgetBaseOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.BudgetType budgetType = null;
            Model.BudgetForecast budgetForecast = null;
            Model.Inventory inventory = null;

			Model.Project project = null;
			Model.Country country = null;
			Model.ProjectType projectType = null;
			Model.AssetType assetType = null;
			Model.Department department = null;
			Model.Division division = null;

			inventory = await _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleAsync();
            documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "BUDGET_BASE_CHANGE").SingleOrDefaultAsync();

            document = new Model.Document()
            {
                Approved = true,
                CompanyId = null,
                CostCenterId = null,
                CreatedAt = DateTime.Now,
                CreatedBy = budgetDto.UserId,
                CreationDate = DateTime.Now,
                Details = string.Empty,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                DocumentTypeId = documentType.Id,
                Exported = true,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = budgetDto.UserId,
                ParentDocumentId = null,
                PartnerId = null,
                RegisterDate = DateTime.Now,
                ValidationDate = DateTime.Now
            };

            _context.Add(document);


            if (budgetDto.BudgetForecastId > 0)
            {
                budgetForecast = await _context.Set<Model.BudgetForecast>().Where(a => a.Id == budgetDto.BudgetForecastId).SingleAsync();

                // budgetForecast.BudgetBaseId = budgetDto.BudgetBaseNewId;

                budgetForecast.HasChangeApril = budgetForecast.April != budgetDto.AprilForecastNew ? true : false;
                budgetForecast.HasChangeMay = budgetForecast.May != budgetDto.MayForecastNew ? true : false;
                budgetForecast.HasChangeJune = budgetForecast.June != budgetDto.JuneForecastNew ? true : false;
                budgetForecast.HasChangeJuly = budgetForecast.July != budgetDto.JulyForecastNew ? true : false;
                budgetForecast.HasChangeAugust = budgetForecast.August != budgetDto.AugustForecastNew ? true : false;
                budgetForecast.HasChangeSeptember = budgetForecast.September != budgetDto.SeptemberForecastNew ? true : false;
                budgetForecast.HasChangeOctomber = budgetForecast.Octomber != budgetDto.OctomberForecastNew ? true : false;
                budgetForecast.HasChangeNovember = budgetForecast.November != budgetDto.NovemberForecastNew ? true : false;
                budgetForecast.HasChangeDecember = budgetForecast.December != budgetDto.DecemberForecastNew ? true : false;
                budgetForecast.HasChangeJanuary = budgetForecast.January != budgetDto.JanuaryForecastNew ? true : false;
                budgetForecast.HasChangeFebruary = budgetForecast.February != budgetDto.FebruaryForecastNew ? true : false;
                budgetForecast.HasChangeMarch = budgetForecast.March != budgetDto.MarchForecastNew ? true : false;

                budgetForecast.April = budgetDto.AprilForecastNew;
                budgetForecast.May = budgetDto.MayForecastNew;
                budgetForecast.June = budgetDto.JuneForecastNew;
                budgetForecast.July = budgetDto.JulyForecastNew;
                budgetForecast.August = budgetDto.AugustForecastNew;
                budgetForecast.September = budgetDto.SeptemberForecastNew;
                budgetForecast.Octomber = budgetDto.OctomberForecastNew;
                budgetForecast.November = budgetDto.NovemberForecastNew;
                budgetForecast.December = budgetDto.DecemberForecastNew;
                budgetForecast.January = budgetDto.JanuaryForecastNew;
                budgetForecast.February = budgetDto.FebruaryForecastNew;
                budgetForecast.March = budgetDto.MarchForecastNew;

                decimal sumMonthsForecast = budgetDto.AprilForecastNew + budgetDto.MayForecastNew + budgetDto.JuneForecastNew + budgetDto.JulyForecastNew + budgetDto.AugustForecastNew + budgetDto.SeptemberForecastNew +
					budgetDto.OctomberForecastNew + budgetDto.NovemberForecastNew + budgetDto.DecemberForecastNew + budgetDto.JanuaryForecastNew + budgetDto.FebruaryForecastNew + budgetDto.MarchForecastNew;

                budgetForecast.Total = sumMonthsForecast;
                budgetForecast.TotalRem = sumMonthsForecast - budgetForecast.ValueOrder;


				budgetBase = await _context.Set<Model.BudgetBase>().Where(a => a.Id == budgetForecast.BudgetBaseId).SingleAsync();

				country = await _context.Set<Model.Country>().Where(c => c.Id == budgetDto.CountryId).SingleAsync();
				department = await _context.Set<Model.Department>().Where(c => c.Id == budgetDto.DepartmentId).SingleAsync();
				division = await _context.Set<Model.Division>().Where(c => c.Id == budgetDto.DivisionId).SingleAsync();
				projectType = await _context.Set<Model.ProjectType>().Where(c => c.Id == budgetDto.ProjectTypeId).SingleAsync();
				assetType = await _context.Set<Model.AssetType>().Where(c => c.Id == budgetDto.AssetTypeId).SingleAsync();

				project = await _context.Set<Model.Project>().Where(p => p.Code == (country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code) && p.IsDeleted == false).FirstOrDefaultAsync();

				if (project == null)
				{
					project = new Model.Project
					{
						Code = country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code,
						Name = country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code,
						IsDeleted = false,
						ProjectTypeId = projectType.Id
					};

					_context.Set<Model.Project>().Add(project);
				}

				

				budgetBase.DepartmentId = budgetDto.DepartmentId;
				budgetBase.DivisionId = budgetDto.DivisionId;
				budgetBase.AdmCenterId = budgetDto.AdmCenterId;
				budgetBase.EmployeeId = budgetDto.EmployeeId;

				budgetBase.Project = project;
				budgetBase.CountryId = budgetDto.CountryId;
				budgetBase.ActivityId = budgetDto.ActivityId;
				budgetBase.ProjectTypeId = budgetDto.ProjectTypeId;
				budgetBase.AssetTypeId = budgetDto.AssetTypeId;
				budgetBase.AppStateId = budgetDto.AppStateId;
				budgetBase.RegionId = budgetDto.RegionId;
				

				_context.Update(budgetBase);

				_context.Update(budgetForecast);
            }


            if (budgetDto.Id > 0)
            {
				budgetBase.Info = budgetDto.Info;
				budgetBase.Name = budgetDto.Name;
				//budgetBase.Total = budgetDto.AugustForecastNew + budgetDto.MayForecastNew + budgetDto.JuneForecastNew + budgetDto.JulyForecastNew + budgetDto.AugustForecastNew + budgetDto.SeptemberForecastNew +
				//            budgetDto.OctomberForecastNew + budgetDto.NovemberForecastNew + budgetDto.DecemberForecastNew + budgetDto.JanuaryForecastNew + budgetDto.FebruaryForecastNew + budgetDto.MarchForecastNew;
				//budgetBase.ValueIni = budgetDto.AugustForecastNew + budgetDto.MayForecastNew + budgetDto.JuneForecastNew + budgetDto.JulyForecastNew + budgetDto.AugustForecastNew + budgetDto.SeptemberForecastNew +
				//                budgetDto.OctomberForecastNew + budgetDto.NovemberForecastNew + budgetDto.DecemberForecastNew + budgetDto.JanuaryForecastNew + budgetDto.FebruaryForecastNew + budgetDto.MarchForecastNew;

				//budget.StartMonthId = budgetDto.StartAccMonthId;
				//budget.EmployeeId = budgetDto.EmployeeId;
				//budget.ProjectId = budgetDto.ProjectId;
				//budget.CountryId = budgetDto.CountryId;
				//budget.ActivityId = budgetDto.ActivityId;
				//budget.DepartmentId = budgetDto.DepartmentId;
				//budget.AdmCenterId = budgetDto.AdmCenterId;
				//budget.Info = budgetDto.Info;
				//budget.DivisionId = budgetDto.DivisionId;
				//budget.ModifiedAt = DateTime.Now;
				//budget.ModifiedBy = budgetDto.UserId;
				//budget.ProjectTypeId = budgetDto.ProjectTypeId;
				//budget.AssetTypeId = budgetDto.AssetTypeId;

				_context.Set<Model.BudgetBase>().Update(budgetBase);


            }
            //else
            //{
            //    entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefault();

            //    var lastCode = int.Parse(entityType.Name);
            //    var newBudgetCode = entityType.Code + lastCode.ToString();


            //    documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").SingleOrDefault();

            //    document = new Model.Document()
            //    {
            //        Approved = true,
            //        CompanyId = budgetDto.CompanyId,
            //        CostCenterId = budgetDto.CostCenterId,
            //        CreatedAt = DateTime.Now,
            //        CreatedBy = budgetDto.UserId,
            //        CreationDate = DateTime.Now,
            //        Details = budgetDto.Info != null ? budgetDto.Info : string.Empty,
            //        DocNo1 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
            //        DocNo2 = budgetDto.Info != null ? budgetDto.Info : string.Empty,
            //        DocumentDate = DateTime.Now,
            //        DocumentTypeId = documentType.Id,
            //        Exported = true,
            //        IsDeleted = false,
            //        ModifiedAt = DateTime.Now,
            //        ModifiedBy = budgetDto.UserId,
            //        ParentDocumentId = null,
            //        PartnerId = budgetDto.PartnerId,
            //        RegisterDate = DateTime.Now,
            //        ValidationDate = DateTime.Now
            //    };

            //    _context.Add(document);


            //    budget = new Model.Budget()
            //    {
            //        AccMonthId = budgetDto.AccMonthId,
            //        AccountId = budgetDto.AccountId,
            //        AdministrationId = budgetDto.AdministrationId,
            //        AppStateId = 1,
            //        BudgetManagerId = null,
            //        Code = newBudgetCode,
            //        CompanyId = budgetDto.CompanyId,
            //        CostCenterId = budgetDto.CostCenterId,
            //        CreatedAt = DateTime.Now,
            //        CreatedBy = budgetDto.UserId,
            //        EmployeeId = budgetDto.EmployeeId,
            //        EndDate = budgetDto.StartDate,
            //        StartDate = budgetDto.EndDate,
            //        Info = budgetDto.Info,
            //        InterCompanyId = budgetDto.InterCompanyId,
            //        IsAccepted = false,
            //        IsDeleted = false,
            //        ModifiedAt = DateTime.Now,
            //        ModifiedBy = budgetDto.UserId,
            //        // Name = newBudgetCode,
            //        PartnerId = budgetDto.PartnerId,
            //        ProjectId = budgetDto.ProjectId,
            //        Quantity = budgetDto.Quantity,
            //        SubTypeId = budgetDto.SubTypeId,
            //        UserId = budgetDto.UserId,
            //        Validated = true,
            //        ValueFin = budgetDto.ValueFin,
            //        ValueIni = budgetDto.ValueIni,
            //        Guid = Guid.NewGuid()


            //    };
            //    _context.Add(budget);

            budgetBaseOp = new Model.BudgetBaseOp()
            {
                AccMonthId = inventory.AccMonthId,
                AccSystemId = null,
                BudgetManagerId = inventory.BudgetManagerId.Value,
                BudgetTypeId = budgetForecast.BudgetTypeId,
                Document = document,
                //AccountIdInitial = budgetDto.AccountId,
                //AccountIdFinal = budgetDto.AccountId,
                //AdministrationIdInitial = budgetDto.AdministrationId,
                //AdministrationIdFinal = budgetDto.AdministrationId,
                //BudgetBase = budgetBase,
                BudgetForecastId = budgetForecast.Id,
                //BudgetManagerIdInitial = null,
                //BudgetManagerIdFinal = null,
                // BudgetStateId = 1,
                //CompanyIdInitial = budgetDto.CompanyId,
                //CompanyIdFinal = budgetDto.CompanyId,
                //CostCenterIdInitial = budgetDto.CostCenterId,
                //CostCenterIdFinal = budgetDto.CostCenterId,
                CreatedAt = DateTime.Now,
                CreatedBy = budgetDto.UserId,
                DstConfAt = DateTime.Now,
                DstConfBy = budgetDto.UserId,
                //EmployeeIdInitial = budgetDto.EmployeeId,
                //EmployeeIdFinal = budgetDto.EmployeeId,
                InfoIni = budgetDto.Info,
                InfoFin = budgetDto.Info,
                //InterCompanyIdInitial = budgetDto.InterCompanyId,
                //InterCompanyIdFinal = budgetDto.InterCompanyId,
                // IsAccepted = false,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = budgetDto.UserId,
                //PartnerIdInitial = budgetDto.PartnerId,
                //PartnerIdFinal = budgetDto.PartnerId,
                //ProjectIdInitial = budgetDto.ProjectId,
                //ProjectIdFinal = budgetDto.ProjectId,
                //QuantityIni = budgetDto.Quantity,
                //QuantityFin = budgetDto.Quantity,
                //SubTypeIdInitial = budgetDto.SubTypeId,
                //SubTypeIdFinal = budgetDto.SubTypeId,
                // Validated = true,
                //ValueFin1 = budget.ValueFin,
                //ValueIni1 = budget.ValueIni,
                //ValueFin2 = budget.ValueFin,
                //ValueIni2 = budget.ValueIni,
                Guid = Guid.NewGuid(),
                AprilIni = budgetDto.AprilForecast,
                AprilFin = budgetDto.AprilForecastNew,
                MayIni = budgetDto.MayForecast,
                MayFin = budgetDto.MayForecastNew,
                JuneIni = budgetDto.JuneForecast,
                JuneFin = budgetDto.JuneForecastNew,
                JulyIni = budgetDto.JulyForecast,
                JulyFin = budgetDto.JulyForecastNew,
                AugustIni = budgetDto.AugustForecast,
                AugustFin = budgetDto.AugustForecastNew,
                SeptemberIni = budgetDto.SeptemberForecast,
                SeptemberFin = budgetDto.SeptemberForecastNew,
                OctomberIni = budgetDto.OctomberForecast,
                OctomberFin = budgetDto.OctomberForecastNew,
                NovemberIni = budgetDto.NovemberForecast,
                NovemberFin = budgetDto.NovemberForecastNew,
                DecemberIni = budgetDto.DecemberForecast,
                DecemberFin = budgetDto.DecemberForecastNew,
                JanuaryIni = budgetDto.JanuaryForecast,
                JanuaryFin = budgetDto.JanuaryForecastNew,
                FebruaryIni = budgetDto.FebruaryForecast,
                FebruaryFin = budgetDto.FebruaryForecastNew,
                MarchIni = budgetDto.MarchForecast,
                MarchFin = budgetDto.MarchForecastNew,
            };

            _context.Add(budgetBaseOp);

            //entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            //_context.Update(entityType);

            _context.SaveChanges();

            return new RequestResult { Success = true, Message = "Datele au fost actualizate", RequestId = budgetDto.RequestId != null ? budgetDto.RequestId.Value : budgetForecast.Id };

		}

		public async Task<BudgetForecastCorrectionResult> CorrectionBudgetForecast(BudgetCorrectionSave budgetDto)
		{
			Model.BudgetBaseOp budgetBaseOpSrc = null;
			Model.BudgetBaseOp budgetBaseOpDst = null;
			Model.Document documentSrc = null;
			Model.Document documentDst = null;
			Model.DocumentType documentTypeSrc = null;
			Model.DocumentType documentTypeDst = null;
			Model.BudgetForecast budgetForecastSrc = null;
			Model.BudgetForecast budgetForecastDst = null;
			Model.Inventory inventory = null;

			inventory = await _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefaultAsync();
			if (inventory == null) return new BudgetForecastCorrectionResult { Success = false, Message= "Nu exista inventar activ", SourceId = 0, DestinationId = 0 };


			// SRC //

			documentTypeSrc = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "BUDGET_FORECAST_SRC_TRANSFER").SingleOrDefaultAsync();
			if (documentTypeSrc == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista tip document sursa", SourceId = 0, DestinationId = 0 };

			documentSrc = new Model.Document()
			{
				Approved = true,
				CompanyId = null,
				CostCenterId = null,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				CreationDate = DateTime.Now,
				Details = string.Empty,
				DocNo1 = string.Empty,
				DocNo2 = string.Empty,
				DocumentDate = DateTime.Now,
				DocumentTypeId = documentTypeSrc.Id,
				Exported = true,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				ParentDocumentId = null,
				PartnerId = null,
				RegisterDate = DateTime.Now,
				ValidationDate = DateTime.Now
			};

			_context.Add(documentSrc);

			budgetForecastSrc = await _context.Set<Model.BudgetForecast>().Where(a => a.Id == budgetDto.BudgetForecastId).SingleOrDefaultAsync();
			if (budgetForecastSrc == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista buget sursa", SourceId = 0, DestinationId = 0 };

			decimal aprilSrc = budgetForecastSrc.April;
			decimal maySrc = budgetForecastSrc.May;
			decimal juneSrc = budgetForecastSrc.June;
			decimal julySrc = budgetForecastSrc.July;
			decimal augustSrc = budgetForecastSrc.August;
			decimal septemberSrc = budgetForecastSrc.September;
			decimal octomberSrc = budgetForecastSrc.Octomber;
			decimal novemberSrc = budgetForecastSrc.November;
			decimal decemberSrc = budgetForecastSrc.December;
			decimal januarySrc = budgetForecastSrc.January;
			decimal februarySrc = budgetForecastSrc.February;
			decimal marchSrc = budgetForecastSrc.March;

			budgetForecastSrc.HasChangeApril = budgetForecastSrc.April != budgetDto.AprilForecast ? true : false;
			budgetForecastSrc.HasChangeMay = budgetForecastSrc.May != budgetDto.MayForecast ? true : false;
			budgetForecastSrc.HasChangeJune = budgetForecastSrc.June != budgetDto.JuneForecast ? true : false;
			budgetForecastSrc.HasChangeJuly = budgetForecastSrc.July != budgetDto.JulyForecast ? true : false;
			budgetForecastSrc.HasChangeAugust = budgetForecastSrc.August != budgetDto.AugustForecast ? true : false;
			budgetForecastSrc.HasChangeSeptember = budgetForecastSrc.September != budgetDto.SeptemberForecast ? true : false;
			budgetForecastSrc.HasChangeOctomber = budgetForecastSrc.Octomber != budgetDto.OctomberForecast ? true : false;
			budgetForecastSrc.HasChangeNovember = budgetForecastSrc.November != budgetDto.NovemberForecast ? true : false;
			budgetForecastSrc.HasChangeDecember = budgetForecastSrc.December != budgetDto.DecemberForecast ? true : false;
			budgetForecastSrc.HasChangeJanuary = budgetForecastSrc.January != budgetDto.JanuaryForecast ? true : false;
			budgetForecastSrc.HasChangeFebruary = budgetForecastSrc.February != budgetDto.FebruaryForecast ? true : false;
			budgetForecastSrc.HasChangeMarch = budgetForecastSrc.March != budgetDto.MarchForecast ? true : false;

			budgetForecastSrc.April = budgetDto.AprilForecast;
			budgetForecastSrc.May = budgetDto.MayForecast;
			budgetForecastSrc.June = budgetDto.JuneForecast;
			budgetForecastSrc.July = budgetDto.JulyForecast;
			budgetForecastSrc.August = budgetDto.AugustForecast;
			budgetForecastSrc.September = budgetDto.SeptemberForecast;
			budgetForecastSrc.Octomber = budgetDto.OctomberForecast;
			budgetForecastSrc.November = budgetDto.NovemberForecast;
			budgetForecastSrc.December = budgetDto.DecemberForecast;
			budgetForecastSrc.January = budgetDto.JanuaryForecast;
			budgetForecastSrc.February = budgetDto.FebruaryForecast;
			budgetForecastSrc.March = budgetDto.MarchForecast;

			budgetForecastSrc.Total = (budgetDto.AugustForecast + budgetDto.MayForecast + budgetDto.JuneForecast + budgetDto.JulyForecast + budgetDto.AugustForecast + budgetDto.SeptemberForecast +
						budgetDto.OctomberForecast + budgetDto.NovemberForecast + budgetDto.DecemberForecast + budgetDto.JanuaryForecast + budgetDto.FebruaryForecast + budgetDto.MarchForecast);
			//budgetForecastSrc.TotalRem = (budgetDto.AugustForecast + budgetDto.MayForecast + budgetDto.JuneForecast + budgetDto.JulyForecast + budgetDto.AugustForecast + budgetDto.SeptemberForecast +
			//budgetDto.OctomberForecast + budgetDto.NovemberForecast + budgetDto.DecemberForecast + budgetDto.JanuaryForecast + budgetDto.FebruaryForecast + budgetDto.MarchForecast) - budgetForecastSrc.ValueOrder;

			_context.Update(budgetForecastSrc);

			budgetBaseOpSrc = new Model.BudgetBaseOp()
			{
				AccMonthId = inventory.AccMonthId,
				AccSystemId = null,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				BudgetTypeId = budgetForecastSrc.BudgetTypeId,
				Document = documentSrc,
				//AccountIdInitial = budgetDto.AccountId,
				//AccountIdFinal = budgetDto.AccountId,
				//AdministrationIdInitial = budgetDto.AdministrationId,
				//AdministrationIdFinal = budgetDto.AdministrationId,
				//BudgetBase = budgetBase,
				BudgetForecastId = budgetForecastSrc.Id,
				//BudgetManagerIdInitial = null,
				//BudgetManagerIdFinal = null,
				// BudgetStateId = 1,
				//CompanyIdInitial = budgetDto.CompanyId,
				//CompanyIdFinal = budgetDto.CompanyId,
				//CostCenterIdInitial = budgetDto.CostCenterId,
				//CostCenterIdFinal = budgetDto.CostCenterId,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				DstConfAt = DateTime.Now,
				DstConfBy = budgetDto.UserId,
				//EmployeeIdInitial = budgetDto.EmployeeId,
				//EmployeeIdFinal = budgetDto.EmployeeId,
				InfoIni = string.Empty,
				InfoFin = string.Empty,
				//InterCompanyIdInitial = budgetDto.InterCompanyId,
				//InterCompanyIdFinal = budgetDto.InterCompanyId,
				// IsAccepted = false,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				//PartnerIdInitial = budgetDto.PartnerId,
				//PartnerIdFinal = budgetDto.PartnerId,
				//ProjectIdInitial = budgetDto.ProjectId,
				//ProjectIdFinal = budgetDto.ProjectId,
				//QuantityIni = budgetDto.Quantity,
				//QuantityFin = budgetDto.Quantity,
				//SubTypeIdInitial = budgetDto.SubTypeId,
				//SubTypeIdFinal = budgetDto.SubTypeId,
				// Validated = true,
				//ValueFin1 = budget.ValueFin,
				//ValueIni1 = budget.ValueIni,
				//ValueFin2 = budget.ValueFin,
				//ValueIni2 = budget.ValueIni,
				Guid = Guid.NewGuid(),
				AprilIni = aprilSrc,
				AprilFin = budgetDto.AprilForecast,
				MayIni = maySrc,
				MayFin = budgetDto.MayForecast,
				JuneIni = juneSrc,
				JuneFin = budgetDto.JuneForecast,
				JulyIni = julySrc,
				JulyFin = budgetDto.JulyForecast,
				AugustIni = augustSrc,
				AugustFin = budgetDto.AugustForecast,
				SeptemberIni = septemberSrc,
				SeptemberFin = budgetDto.SeptemberForecast,
				OctomberIni = octomberSrc,
				OctomberFin = budgetDto.OctomberForecast,
				NovemberIni = novemberSrc,
				NovemberFin = budgetDto.NovemberForecast,
				DecemberIni = decemberSrc,
				DecemberFin = budgetDto.DecemberForecast,
				JanuaryIni = januarySrc,
				JanuaryFin = budgetDto.JanuaryForecast,
				FebruaryIni = februarySrc,
				FebruaryFin = budgetDto.FebruaryForecast,
				MarchIni = marchSrc,
				MarchFin = budgetDto.MarchForecast,
			};

			_context.Add(budgetBaseOpSrc);

			// SRC //




			// DST //


			documentTypeDst = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "BUDGET_FORECAST_DST_TRANSFER").SingleOrDefaultAsync();
			if (documentTypeDst == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista tip document destinatie", SourceId = 0, DestinationId = 0 };



			documentDst = new Model.Document()
			{
				Approved = true,
				CompanyId = null,
				CostCenterId = null,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				CreationDate = DateTime.Now,
				Details = string.Empty,
				DocNo1 = string.Empty,
				DocNo2 = string.Empty,
				DocumentDate = DateTime.Now,
				DocumentTypeId = documentTypeDst.Id,
				Exported = true,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				ParentDocumentId = null,
				PartnerId = null,
				RegisterDate = DateTime.Now,
				ValidationDate = DateTime.Now
			};

			_context.Add(documentDst);

			budgetForecastDst = await _context.Set<Model.BudgetForecast>().Where(a => a.Id == budgetDto.BudgetForecastDestinationId).SingleOrDefaultAsync();
			if (budgetForecastDst == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista buget destinatie", SourceId= 0, DestinationId = 0 };

			decimal aprilDst = budgetForecastDst.April;
			decimal mayDst = budgetForecastDst.May;
			decimal juneDst = budgetForecastDst.June;
			decimal julyDst = budgetForecastDst.July;
			decimal augustDst = budgetForecastDst.August;
			decimal septemberDst = budgetForecastDst.September;
			decimal octomberDst = budgetForecastDst.Octomber;
			decimal novemberDst = budgetForecastDst.November;
			decimal decemberDst = budgetForecastDst.December;
			decimal januaryDst = budgetForecastDst.January;
			decimal februaryDst = budgetForecastDst.February;
			decimal marchDst = budgetForecastDst.March;


			budgetForecastDst.HasChangeApril = budgetForecastDst.April != budgetDto.AprilForecastDstNew ? true : false;
			budgetForecastDst.HasChangeMay = budgetForecastDst.May != budgetDto.MayForecastDstNew ? true : false;
			budgetForecastDst.HasChangeJune = budgetForecastDst.June != budgetDto.JuneForecastDstNew ? true : false;
			budgetForecastDst.HasChangeJuly = budgetForecastDst.July != budgetDto.JulyForecastDstNew ? true : false;
			budgetForecastDst.HasChangeAugust = budgetForecastDst.August != budgetDto.AugustForecastDstNew ? true : false;
			budgetForecastDst.HasChangeSeptember = budgetForecastDst.September != budgetDto.SeptemberForecastDstNew ? true : false;
			budgetForecastDst.HasChangeOctomber = budgetForecastDst.Octomber != budgetDto.OctomberForecastDstNew ? true : false;
			budgetForecastDst.HasChangeNovember = budgetForecastDst.November != budgetDto.NovemberForecastDstNew ? true : false;
			budgetForecastDst.HasChangeDecember = budgetForecastDst.December != budgetDto.DecemberForecastDstNew ? true : false;
			budgetForecastDst.HasChangeJanuary = budgetForecastDst.January != budgetDto.JanuaryForecastDstNew ? true : false;
			budgetForecastDst.HasChangeFebruary = budgetForecastDst.February != budgetDto.FebruaryForecastDstNew ? true : false;
			budgetForecastDst.HasChangeMarch = budgetForecastDst.March != budgetDto.MarchForecastDstNew ? true : false;

			budgetForecastDst.April = budgetDto.AprilForecastDstNew + aprilDst;
			budgetForecastDst.May = budgetDto.MayForecastDstNew + mayDst;
			budgetForecastDst.June = budgetDto.JuneForecastDstNew + juneDst;
			budgetForecastDst.July = budgetDto.JulyForecastDstNew + julyDst;
			budgetForecastDst.August = budgetDto.AugustForecastDstNew + augustDst;
			budgetForecastDst.September = budgetDto.SeptemberForecastDstNew + septemberDst;
			budgetForecastDst.Octomber = budgetDto.OctomberForecastDstNew + octomberDst;
			budgetForecastDst.November = budgetDto.NovemberForecastDstNew + novemberDst;
			budgetForecastDst.December = budgetDto.DecemberForecastDstNew + decemberDst;
			budgetForecastDst.January = budgetDto.JanuaryForecastDstNew + januaryDst;
			budgetForecastDst.February = budgetDto.FebruaryForecastDstNew + februaryDst;
			budgetForecastDst.March = budgetDto.MarchForecastDstNew + marchDst;

			budgetForecastDst.Total = (budgetDto.AugustForecastDstNew + budgetDto.MayForecastDstNew + budgetDto.JuneForecastDstNew + budgetDto.JulyForecastDstNew + budgetDto.AugustForecastDstNew + budgetDto.SeptemberForecastDstNew +
						budgetDto.OctomberForecastDstNew + budgetDto.NovemberForecastDstNew + budgetDto.DecemberForecastDstNew + budgetDto.JanuaryForecastDstNew + budgetDto.FebruaryForecastDstNew + budgetDto.MarchForecastDstNew +
						aprilDst + mayDst + juneDst + julyDst + augustDst + septemberDst + octomberDst + novemberDst + decemberDst + januaryDst + februaryDst + marchDst);
			//budgetForecastDst.TotalRem = (budgetDto.AugustForecast + budgetDto.MayForecast + budgetDto.JuneForecast + budgetDto.JulyForecast + budgetDto.AugustForecast + budgetDto.SeptemberForecast +
			//budgetDto.OctomberForecast + budgetDto.NovemberForecast + budgetDto.DecemberForecast + budgetDto.JanuaryForecast + budgetDto.FebruaryForecast + budgetDto.MarchForecast) - budgetForecastSrc.ValueOrder;

			_context.Update(budgetForecastDst);

			budgetBaseOpDst = new Model.BudgetBaseOp()
			{
				AccMonthId = inventory.AccMonthId,
				AccSystemId = null,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				BudgetTypeId = budgetForecastDst.BudgetTypeId,
				Document = documentDst,
				//AccountIdInitial = budgetDto.AccountId,
				//AccountIdFinal = budgetDto.AccountId,
				//AdministrationIdInitial = budgetDto.AdministrationId,
				//AdministrationIdFinal = budgetDto.AdministrationId,
				//BudgetBase = budgetBase,
				BudgetForecastId = budgetForecastDst.Id,
				//BudgetManagerIdInitial = null,
				//BudgetManagerIdFinal = null,
				// BudgetStateId = 1,
				//CompanyIdInitial = budgetDto.CompanyId,
				//CompanyIdFinal = budgetDto.CompanyId,
				//CostCenterIdInitial = budgetDto.CostCenterId,
				//CostCenterIdFinal = budgetDto.CostCenterId,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				DstConfAt = DateTime.Now,
				DstConfBy = budgetDto.UserId,
				//EmployeeIdInitial = budgetDto.EmployeeId,
				//EmployeeIdFinal = budgetDto.EmployeeId,
				InfoIni = string.Empty,
				InfoFin = string.Empty,
				//InterCompanyIdInitial = budgetDto.InterCompanyId,
				//InterCompanyIdFinal = budgetDto.InterCompanyId,
				// IsAccepted = false,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				//PartnerIdInitial = budgetDto.PartnerId,
				//PartnerIdFinal = budgetDto.PartnerId,
				//ProjectIdInitial = budgetDto.ProjectId,
				//ProjectIdFinal = budgetDto.ProjectId,
				//QuantityIni = budgetDto.Quantity,
				//QuantityFin = budgetDto.Quantity,
				//SubTypeIdInitial = budgetDto.SubTypeId,
				//SubTypeIdFinal = budgetDto.SubTypeId,
				// Validated = true,
				//ValueFin1 = budget.ValueFin,
				//ValueIni1 = budget.ValueIni,
				//ValueFin2 = budget.ValueFin,
				//ValueIni2 = budget.ValueIni,
				Guid = Guid.NewGuid(),
				AprilIni = aprilDst,
				AprilFin = budgetDto.AprilForecastDstNew + aprilDst,
				MayIni = mayDst,
				MayFin = budgetDto.MayForecastDstNew + mayDst,
				JuneIni = juneDst,
				JuneFin = budgetDto.JuneForecastDstNew + juneDst,
				JulyIni = julyDst,
				JulyFin = budgetDto.JulyForecastDstNew + julyDst,
				AugustIni = augustDst,
				AugustFin = budgetDto.AugustForecastDstNew + augustDst,
				SeptemberIni = septemberDst,
				SeptemberFin = budgetDto.SeptemberForecastDstNew + septemberDst,
				OctomberIni = octomberDst,
				OctomberFin = budgetDto.OctomberForecastDstNew + octomberDst,
				NovemberIni = novemberDst,
				NovemberFin = budgetDto.NovemberForecastDstNew + novemberDst,
				DecemberIni = decemberDst,
				DecemberFin = budgetDto.DecemberForecastDstNew + decemberDst,
				JanuaryIni = januaryDst,
				JanuaryFin = budgetDto.JanuaryForecastDstNew + januaryDst,
				FebruaryIni = februaryDst,
				FebruaryFin = budgetDto.FebruaryForecastDstNew + februaryDst,
				MarchIni = marchDst,
				MarchFin = budgetDto.MarchForecastDstNew + marchDst,
			};

			_context.Add(budgetBaseOpDst);

			// DST //




			_context.SaveChanges();

			return new BudgetForecastCorrectionResult { Success = true, Message = "Datele au fost actualizate", SourceId = budgetForecastSrc.Id, DestinationId = budgetForecastDst.Id };

		}

		public async Task<BudgetForecastCorrectionResult> CorrectionValidateBudgetForecast(BudgetCorrectionValidate budgetDto)
		{
			Model.BudgetBaseOp budgetBaseOpSrc = null;
			Model.BudgetBaseOp budgetBaseOpDst = null;
			Model.Document document = null;
			Model.DocumentType documentType = null;
			Model.BudgetForecast budgetForecastSrc = null;
			Model.BudgetForecast budgetForecastDst = null;
			Model.Inventory inventory = null;
			Model.BudgetBaseOp budgetBaseOp = null;
			Model.AppState appState = null;
			Model.AccMonth accMonth = null;
			Model.Document documentOrder = null;
			Model.DocumentType documentTypeOrder = null;
			Model.Order order = null;

			inventory = await _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefaultAsync();
			if (inventory == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista inventar activ", SourceId = 0, DestinationId = 0 };

			appState = await _context.Set<Model.AppState>().Where(i => i.Code == "ACCEPTED").SingleOrDefaultAsync();
			if (appState == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista stare finala", SourceId = 0, DestinationId = 0 };

			budgetBaseOp = await _context.Set<Model.BudgetBaseOp>().AsNoTracking().Where(d => d.Id == budgetDto.BudgetBaseOpId).SingleOrDefaultAsync();
			if (budgetBaseOp == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista propunere transfer buget", SourceId = 0, DestinationId = 0 };

			accMonth = await _context.Set<Model.BudgetForecast>()
			   .Where(i => i.IsDeleted == false && i.IsLast == true)
			   .AsNoTracking()
			   .Select(a => a.AccMonth)
			   .LastOrDefaultAsync();

			// SRC //

			documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "BGT_F_TRANSFER_VALIDATED").SingleOrDefaultAsync();
			if (documentType == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista tip document sursa", SourceId = 0, DestinationId = 0 };

			document = new Model.Document()
			{
				Approved = true,
				CompanyId = null,
				CostCenterId = null,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				CreationDate = DateTime.Now,
				Details = string.Empty,
				DocNo1 = string.Empty,
				DocNo2 = string.Empty,
				DocumentDate = DateTime.Now,
				DocumentTypeId = documentType.Id,
				Exported = true,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				ParentDocumentId = null,
				PartnerId = null,
				RegisterDate = DateTime.Now,
				ValidationDate = DateTime.Now
			};

			_context.Add(document);

			budgetForecastSrc = await _context.Set<Model.BudgetForecast>().Include(a => a.AccMonth).Where(a => a.Id == budgetBaseOp.BudgetForecastId).SingleOrDefaultAsync();
			if (budgetForecastSrc == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista buget sursa", SourceId = 0, DestinationId = 0 };

			decimal aprilSrc = budgetForecastSrc.April;
			decimal maySrc = budgetForecastSrc.May;
			decimal juneSrc = budgetForecastSrc.June;
			decimal julySrc = budgetForecastSrc.July;
			decimal augustSrc = budgetForecastSrc.August;
			decimal septemberSrc = budgetForecastSrc.September;
			decimal octomberSrc = budgetForecastSrc.Octomber;
			decimal novemberSrc = budgetForecastSrc.November;
			decimal decemberSrc = budgetForecastSrc.December;
			decimal januarySrc = budgetForecastSrc.January;
			decimal februarySrc = budgetForecastSrc.February;
			decimal marchSrc = budgetForecastSrc.March;
			decimal totalSrc = budgetForecastSrc.Total;

			if (budgetForecastSrc.AccMonth.Month == 4)
			{
				budgetForecastSrc.HasChangeApril = budgetForecastSrc.April != budgetBaseOp.AprilIni ? true : false;
				budgetForecastSrc.April = budgetBaseOp.AprilIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 5)
			{
				budgetForecastSrc.HasChangeMay = budgetForecastSrc.May != budgetBaseOp.MayIni ? true : false;
				budgetForecastSrc.May = budgetBaseOp.MayIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 6)
			{
				budgetForecastSrc.HasChangeJune = budgetForecastSrc.June != budgetBaseOp.JuneIni ? true : false;
				budgetForecastSrc.June = budgetBaseOp.JuneIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 7)
			{
				budgetForecastSrc.HasChangeJuly = budgetForecastSrc.July != budgetBaseOp.JulyIni ? true : false;
				budgetForecastSrc.July = budgetBaseOp.JulyIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 8)
			{
				budgetForecastSrc.HasChangeAugust = budgetForecastSrc.August != budgetBaseOp.AugustIni ? true : false;
				budgetForecastSrc.August = budgetBaseOp.AugustIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 9)
			{
				budgetForecastSrc.HasChangeSeptember = budgetForecastSrc.September != budgetBaseOp.SeptemberIni ? true : false;
				budgetForecastSrc.September = budgetBaseOp.SeptemberIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 10)
			{
				budgetForecastSrc.HasChangeOctomber = budgetForecastSrc.Octomber != budgetBaseOp.OctomberIni ? true : false;
				budgetForecastSrc.Octomber = budgetBaseOp.OctomberIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 11)
			{
				budgetForecastSrc.HasChangeNovember = budgetForecastSrc.November != budgetBaseOp.NovemberIni ? true : false;
				budgetForecastSrc.November = budgetBaseOp.NovemberIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 && budgetForecastSrc.AccMonth.Month <= 12)
			{
				budgetForecastSrc.HasChangeDecember = budgetForecastSrc.December != budgetBaseOp.DecemberIni ? true : false;
				budgetForecastSrc.December = budgetBaseOp.DecemberIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 || budgetForecastSrc.AccMonth.Month == 1)
			{
				budgetForecastSrc.HasChangeJanuary = budgetForecastSrc.January != budgetBaseOp.JanuaryIni ? true : false;
				budgetForecastSrc.January = budgetBaseOp.JanuaryIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 || (budgetForecastSrc.AccMonth.Month >= 1 && budgetForecastSrc.AccMonth.Month <=2))
			{
				budgetForecastSrc.HasChangeFebruary = budgetForecastSrc.February != budgetBaseOp.FebruaryIni ? true : false;
				budgetForecastSrc.February = budgetBaseOp.FebruaryIni;
			}

			if (budgetForecastSrc.AccMonth.Month >= 4 || (budgetForecastSrc.AccMonth.Month >= 1 && budgetForecastSrc.AccMonth.Month <= 3))
			{
				budgetForecastSrc.HasChangeMarch = budgetForecastSrc.March != budgetBaseOp.MarchIni ? true : false;
				budgetForecastSrc.March = budgetBaseOp.MarchIni;
			}
			
			

			budgetForecastSrc.Total = 
				(
				//totalSrc -
				(budgetBaseOp.AprilIni +
				budgetBaseOp.MayIni + 
				budgetBaseOp.JuneIni + 
				budgetBaseOp.JulyIni + 
				budgetBaseOp.AugustIni + 
				budgetBaseOp.SeptemberIni +
				budgetBaseOp.OctomberIni + 
				budgetBaseOp.NovemberIni + 
				budgetBaseOp.DecemberIni + 
				budgetBaseOp.JanuaryIni + 
				budgetBaseOp.FebruaryIni + 
				budgetBaseOp.MarchIni
				));

			_context.Update(budgetForecastSrc);

			budgetBaseOpSrc = new Model.BudgetBaseOp()
			{
				AccMonthId = inventory.AccMonthId,
				AccSystemId = null,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				BudgetTypeId = budgetForecastSrc.BudgetTypeId,
				Document = document,
				//AccountIdInitial = budgetDto.AccountId,
				//AccountIdFinal = budgetDto.AccountId,
				//AdministrationIdInitial = budgetDto.AdministrationId,
				//AdministrationIdFinal = budgetDto.AdministrationId,
				//BudgetBase = budgetBase,
				BudgetForecastId = budgetForecastSrc.Id,
				//BudgetManagerIdInitial = null,
				//BudgetManagerIdFinal = null,
				// BudgetStateId = 1,
				//CompanyIdInitial = budgetDto.CompanyId,
				//CompanyIdFinal = budgetDto.CompanyId,
				//CostCenterIdInitial = budgetDto.CostCenterId,
				//CostCenterIdFinal = budgetDto.CostCenterId,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				DstConfAt = DateTime.Now,
				DstConfBy = budgetDto.UserId,
				//EmployeeIdInitial = budgetDto.EmployeeId,
				//EmployeeIdFinal = budgetDto.EmployeeId,
				InfoIni = string.Empty,
				InfoFin = string.Empty,
				//InterCompanyIdInitial = budgetDto.InterCompanyId,
				//InterCompanyIdFinal = budgetDto.InterCompanyId,
				// IsAccepted = false,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				//PartnerIdInitial = budgetDto.PartnerId,
				//PartnerIdFinal = budgetDto.PartnerId,
				//ProjectIdInitial = budgetDto.ProjectId,
				//ProjectIdFinal = budgetDto.ProjectId,
				//QuantityIni = budgetDto.Quantity,
				//QuantityFin = budgetDto.Quantity,
				//SubTypeIdInitial = budgetDto.SubTypeId,
				//SubTypeIdFinal = budgetDto.SubTypeId,
				// Validated = true,
				//ValueFin1 = budget.ValueFin,
				//ValueIni1 = budget.ValueIni,
				//ValueFin2 = budget.ValueFin,
				//ValueIni2 = budget.ValueIni,
				Guid = Guid.NewGuid(),
				AprilIni = aprilSrc,
				AprilFin = budgetBaseOp.AprilIni,
				MayIni = maySrc,
				MayFin = budgetBaseOp.MayIni,
				JuneIni = juneSrc,
				JuneFin = budgetBaseOp.JuneIni,
				JulyIni = julySrc,
				JulyFin = budgetBaseOp.JulyIni,
				AugustIni = augustSrc,
				AugustFin = budgetBaseOp.AugustIni,
				SeptemberIni = septemberSrc,
				SeptemberFin = budgetBaseOp.SeptemberIni,
				OctomberIni = octomberSrc,
				OctomberFin = budgetBaseOp.OctomberIni,
				NovemberIni = novemberSrc,
				NovemberFin = budgetBaseOp.NovemberIni,
				DecemberIni = decemberSrc,
				DecemberFin = budgetBaseOp.DecemberIni,
				JanuaryIni = januarySrc,
				JanuaryFin = budgetBaseOp.JanuaryIni,
				FebruaryIni = februarySrc,
				FebruaryFin = budgetBaseOp.FebruaryIni,
				MarchIni = marchSrc,
				MarchFin = budgetBaseOp.MarchIni,
			};

			_context.Add(budgetBaseOpSrc);

			// SRC //




			// DST //

			budgetForecastDst = await _context.Set<Model.BudgetForecast>().Include(a => a.AccMonth).Where(a => a.Id == budgetBaseOp.BudgetForecastFinId).SingleOrDefaultAsync();
			if (budgetForecastDst == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista buget destinatie", SourceId = 0, DestinationId = 0 };

			decimal aprilDst = budgetForecastDst.April;
			decimal mayDst = budgetForecastDst.May;
			decimal juneDst = budgetForecastDst.June;
			decimal julyDst = budgetForecastDst.July;
			decimal augustDst = budgetForecastDst.August;
			decimal septemberDst = budgetForecastDst.September;
			decimal octomberDst = budgetForecastDst.Octomber;
			decimal novemberDst = budgetForecastDst.November;
			decimal decemberDst = budgetForecastDst.December;
			decimal januaryDst = budgetForecastDst.January;
			decimal februaryDst = budgetForecastDst.February;
			decimal marchDst = budgetForecastDst.March;
			decimal totalDst = budgetForecastDst.Total;


			if (budgetForecastDst.AccMonth.Month == 4)
			{
				budgetForecastDst.HasChangeApril = budgetForecastDst.April != budgetBaseOp.AprilFin ? true : false;
				budgetForecastDst.April = budgetBaseOp.AprilFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 5)
			{
				budgetForecastDst.HasChangeMay = budgetForecastDst.May != budgetBaseOp.MayFin ? true : false;
				budgetForecastDst.May = budgetBaseOp.MayFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 6)
			{
				budgetForecastDst.HasChangeJune = budgetForecastDst.June != budgetBaseOp.JuneFin ? true : false;
				budgetForecastDst.June = budgetBaseOp.JuneFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 7)
			{
				budgetForecastDst.HasChangeJuly = budgetForecastDst.July != budgetBaseOp.JulyFin ? true : false;
				budgetForecastDst.July = budgetBaseOp.JulyFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 8)
			{
				budgetForecastDst.HasChangeAugust = budgetForecastDst.August != budgetBaseOp.AugustFin ? true : false;
				budgetForecastDst.August = budgetBaseOp.AugustFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 9)
			{
				budgetForecastDst.HasChangeSeptember = budgetForecastDst.September != budgetBaseOp.SeptemberFin ? true : false;
				budgetForecastDst.September = budgetBaseOp.SeptemberFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 10)
			{
				budgetForecastDst.HasChangeOctomber = budgetForecastDst.Octomber != budgetBaseOp.OctomberFin ? true : false;
				budgetForecastDst.Octomber = budgetBaseOp.OctomberFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 11)
			{
				budgetForecastDst.HasChangeNovember = budgetForecastDst.November != budgetBaseOp.NovemberFin ? true : false;
				budgetForecastDst.November = budgetBaseOp.NovemberFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 && budgetForecastDst.AccMonth.Month <= 12)
			{
				budgetForecastDst.HasChangeDecember = budgetForecastDst.December != budgetBaseOp.DecemberFin ? true : false;
				budgetForecastDst.December = budgetBaseOp.DecemberFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 || budgetForecastDst.AccMonth.Month == 1)
			{
				budgetForecastDst.HasChangeJanuary = budgetForecastDst.January != budgetBaseOp.JanuaryFin ? true : false;
				budgetForecastDst.January = budgetBaseOp.JanuaryFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 || (budgetForecastDst.AccMonth.Month >= 1 && budgetForecastDst.AccMonth.Month <= 2))
			{
				budgetForecastDst.HasChangeFebruary = budgetForecastDst.February != budgetBaseOp.FebruaryFin ? true : false;
				budgetForecastDst.February = budgetBaseOp.FebruaryFin;
			}

			if (budgetForecastDst.AccMonth.Month >= 4 || (budgetForecastDst.AccMonth.Month >= 1 && budgetForecastDst.AccMonth.Month <= 3))
			{
				budgetForecastDst.HasChangeMarch = budgetForecastDst.March != budgetBaseOp.MarchFin ? true : false;
				budgetForecastDst.March = budgetBaseOp.MarchFin;
			}

			budgetForecastDst.Total = 
				(
				totalDst + 
				budgetBaseOp.AprilFin + 
				budgetBaseOp.MayFin + 
				budgetBaseOp.JuneFin + 
				budgetBaseOp.JulyFin + 
				budgetBaseOp.AugustFin + 
				budgetBaseOp.SeptemberFin +
				budgetBaseOp.OctomberFin + 
				budgetBaseOp.NovemberFin + 
				budgetBaseOp.DecemberFin + 
				budgetBaseOp.JanuaryFin + 
				budgetBaseOp.FebruaryFin + 
				budgetBaseOp.MarchFin
				);

			_context.Update(budgetForecastDst);

			budgetBaseOpDst = new Model.BudgetBaseOp()
			{
				AccMonthId = inventory.AccMonthId,
				AccSystemId = null,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				BudgetTypeId = budgetForecastDst.BudgetTypeId,
				Document = document,
				//AccountIdInitial = budgetDto.AccountId,
				//AccountIdFinal = budgetDto.AccountId,
				//AdministrationIdInitial = budgetDto.AdministrationId,
				//AdministrationIdFinal = budgetDto.AdministrationId,
				//BudgetBase = budgetBase,
				BudgetForecastId = budgetForecastDst.Id,
				//BudgetManagerIdInitial = null,
				//BudgetManagerIdFinal = null,
				// BudgetStateId = 1,
				//CompanyIdInitial = budgetDto.CompanyId,
				//CompanyIdFinal = budgetDto.CompanyId,
				//CostCenterIdInitial = budgetDto.CostCenterId,
				//CostCenterIdFinal = budgetDto.CostCenterId,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				DstConfAt = DateTime.Now,
				DstConfBy = budgetDto.UserId,
				//EmployeeIdInitial = budgetDto.EmployeeId,
				//EmployeeIdFinal = budgetDto.EmployeeId,
				InfoIni = string.Empty,
				InfoFin = string.Empty,
				//InterCompanyIdInitial = budgetDto.InterCompanyId,
				//InterCompanyIdFinal = budgetDto.InterCompanyId,
				// IsAccepted = false,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				//PartnerIdInitial = budgetDto.PartnerId,
				//PartnerIdFinal = budgetDto.PartnerId,
				//ProjectIdInitial = budgetDto.ProjectId,
				//ProjectIdFinal = budgetDto.ProjectId,
				//QuantityIni = budgetDto.Quantity,
				//QuantityFin = budgetDto.Quantity,
				//SubTypeIdInitial = budgetDto.SubTypeId,
				//SubTypeIdFinal = budgetDto.SubTypeId,
				// Validated = true,
				//ValueFin1 = budget.ValueFin,
				//ValueIni1 = budget.ValueIni,
				//ValueFin2 = budget.ValueFin,
				//ValueIni2 = budget.ValueIni,
				Guid = Guid.NewGuid(),
				AprilIni = aprilDst,
				AprilFin = budgetBaseOp.AprilFin,
				MayIni = mayDst,
				MayFin = budgetBaseOp.MayFin,
				JuneIni = juneDst,
				JuneFin = budgetBaseOp.JuneFin,
				JulyIni = julyDst,
				JulyFin = budgetBaseOp.JulyFin,
				AugustIni = augustDst,
				AugustFin = budgetBaseOp.AugustFin,
				SeptemberIni = septemberDst,
				SeptemberFin = budgetBaseOp.SeptemberFin,
				OctomberIni = octomberDst,
				OctomberFin = budgetBaseOp.OctomberFin,
				NovemberIni = novemberDst,
				NovemberFin = budgetBaseOp.NovemberFin,
				DecemberIni = decemberDst,
				DecemberFin = budgetBaseOp.DecemberFin,
				JanuaryIni = januaryDst,
				JanuaryFin = budgetBaseOp.JanuaryFin,
				FebruaryIni = februaryDst,
				FebruaryFin = budgetBaseOp.FebruaryFin,
				MarchIni = marchDst,
				MarchFin = budgetBaseOp.MarchFin,
			};

			_context.Add(budgetBaseOpDst);

			// DST //

		
			budgetBaseOp.BudgetStateIdFinal = appState.Id;

			_context.Update(budgetBaseOp);

			budgetForecastDst.InTransfer = false;
			_context.Update(budgetForecastDst);

			budgetForecastSrc.InTransfer = false;
			_context.Update(budgetForecastSrc);

			_context.SaveChanges();

			return new BudgetForecastCorrectionResult { Success = true, Message = "Datele au fost actualizate", SourceId = budgetForecastSrc.Id, DestinationId = budgetForecastDst.Id };

		}

		public async Task<BudgetForecastCorrectionResult> TransferBudgetForecast(BudgetTransferSave budgetDto)
		{
			Model.BudgetBaseOp budgetBaseOp = null;
			Model.Document document = null;
			Model.DocumentType documentType = null;
			Model.BudgetForecast budgetForecast = null;
			Model.BudgetForecast budgetForecastDst = null;
			Model.Inventory inventory = null;

			inventory = await _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefaultAsync();
			if (inventory == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista inventar activ", SourceId = 0, DestinationId = 0 };

			documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "BUDGET_FORECAST_VALUE_TRANSFER").SingleOrDefaultAsync();
			if (documentType == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista tip document sursa", SourceId = 0, DestinationId = 0 };

			document = new Model.Document()
			{
				Approved = true,
				CompanyId = null,
				CostCenterId = null,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				CreationDate = DateTime.Now,
				Details = string.Empty,
				DocNo1 = string.Empty,
				DocNo2 = string.Empty,
				DocumentDate = DateTime.Now,
				DocumentTypeId = documentType.Id,
				Exported = true,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				ParentDocumentId = null,
				PartnerId = null,
				RegisterDate = DateTime.Now,
				ValidationDate = DateTime.Now
			};

			_context.Add(document);

			budgetForecast = await _context.Set<Model.BudgetForecast>().Where(a => a.Id == budgetDto.BudgetForecastId).SingleOrDefaultAsync();
			if (budgetForecast == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista buget sursa", SourceId = 0, DestinationId = 0 };

			budgetForecastDst = await _context.Set<Model.BudgetForecast>().Where(a => a.Id == budgetDto.BudgetForecastDestinationId).SingleOrDefaultAsync();
			if (budgetForecastDst == null) return new BudgetForecastCorrectionResult { Success = false, Message = "Nu exista buget sursa", SourceId = 0, DestinationId = 0 };

			budgetBaseOp = new Model.BudgetBaseOp()
			{
				AccMonthId = inventory.AccMonthId,
				AccSystemId = null,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				BudgetTypeId = budgetForecast.BudgetTypeId,
				Document = document,
				//AccountIdInitial = budgetDto.AccountId,
				//AccountIdFinal = budgetDto.AccountId,
				//AdministrationIdInitial = budgetDto.AdministrationId,
				//AdministrationIdFinal = budgetDto.AdministrationId,
				//BudgetBase = budgetBase,
				BudgetForecastId = budgetForecast.Id,
				BudgetForecastFinId = budgetForecastDst.Id,
				//BudgetManagerIdInitial = null,
				//BudgetManagerIdFinal = null,
				// BudgetStateId = 1,
				//CompanyIdInitial = budgetDto.CompanyId,
				//CompanyIdFinal = budgetDto.CompanyId,
				//CostCenterIdInitial = budgetDto.CostCenterId,
				//CostCenterIdFinal = budgetDto.CostCenterId,
				CreatedAt = DateTime.Now,
				CreatedBy = budgetDto.UserId,
				DstConfAt = DateTime.Now,
				DstConfBy = budgetDto.UserId,
				//EmployeeIdInitial = budgetDto.EmployeeId,
				//EmployeeIdFinal = budgetDto.EmployeeId,
				InfoIni = string.Empty,
				InfoFin = string.Empty,
				//InterCompanyIdInitial = budgetDto.InterCompanyId,
				//InterCompanyIdFinal = budgetDto.InterCompanyId,
				// IsAccepted = false,
				IsDeleted = false,
				ModifiedAt = DateTime.Now,
				ModifiedBy = budgetDto.UserId,
				//PartnerIdInitial = budgetDto.PartnerId,
				//PartnerIdFinal = budgetDto.PartnerId,
				//ProjectIdInitial = budgetDto.ProjectId,
				//ProjectIdFinal = budgetDto.ProjectId,
				//QuantityIni = budgetDto.Quantity,
				//QuantityFin = budgetDto.Quantity,
				//SubTypeIdInitial = budgetDto.SubTypeId,
				//SubTypeIdFinal = budgetDto.SubTypeId,
				// Validated = true,
				//ValueFin1 = budget.ValueFin,
				//ValueIni1 = budget.ValueIni,
				//ValueFin2 = budget.ValueFin,
				//ValueIni2 = budget.ValueIni,
				Guid = Guid.NewGuid(),
				AprilIni = budgetDto.AprilForecast,
				AprilFin = budgetDto.AprilForecastDstNew,
				MayIni = budgetDto.MayForecast,
				MayFin = budgetDto.MayForecastDstNew,
				JuneIni = budgetDto.JuneForecast,
				JuneFin = budgetDto.JuneForecastDstNew,
				JulyIni = budgetDto.JulyForecast,
				JulyFin = budgetDto.JulyForecastDstNew,
				AugustIni = budgetDto.AugustForecast,
				AugustFin = budgetDto.AugustForecastDstNew,
				SeptemberIni = budgetDto.SeptemberForecast,
				SeptemberFin = budgetDto.SeptemberForecastDstNew,
				OctomberIni = budgetDto.OctomberForecast,
				OctomberFin = budgetDto.OctomberForecastDstNew,
				NovemberIni = budgetDto.NovemberForecast,
				NovemberFin = budgetDto.NovemberForecastDstNew,
				DecemberIni = budgetDto.DecemberForecast,
				DecemberFin = budgetDto.DecemberForecastDstNew,
				JanuaryIni = budgetDto.JanuaryForecast,
				JanuaryFin = budgetDto.JanuaryForecastDstNew,
				FebruaryIni = budgetDto.FebruaryForecast,
				FebruaryFin = budgetDto.FebruaryForecastDstNew,
				MarchIni = budgetDto.MarchForecast,
				MarchFin = budgetDto.MarchForecastDstNew,
				OrderId = budgetDto.OrderId
			};

			_context.Add(budgetBaseOp);

			budgetForecast.InTransfer = true;

			_context.Update(budgetForecast);

			budgetForecastDst.InTransfer = true;

			_context.Update(budgetForecastDst);

			_context.SaveChanges();

			return new BudgetForecastCorrectionResult { Success = true, Message = "Datele au fost actualizate", SourceId = budgetForecast.Id, DestinationId = budgetForecastDst.Id, BudgetBaseOpId = budgetBaseOp.Id };

		}

		public async Task<ImportBudgetResult> BudgetForecastImport(Dto.BudgetForecastImport budgetDto)
        {
            Model.BudgetBase budgetBase = null;
			// Model.BudgetOp budgetOp = null;
			Model.Document document = null;
			Model.DocumentType documentType = null;
			Model.EntityType entityType = null;
			Model.Company company = null;
            Model.Country country = null;
            Model.Project project = null;
            Model.Activity activity = null;
            Model.AdmCenter admCenter = null;
            Model.Region region = null;
            Model.AssetType assetType = null;
            Model.ProjectType projectType = null;
            Model.AppState appState = null;
            Model.Inventory inventory = null;
            //Model.BudgetManager budgetManager = null;
            Model.BudgetType budgetType = null;
            Model.BudgetType budgetTypeNew = null;
            //Model.AccMonth accMonth = null;
            Model.AccMonth startAccMonth = null;
            Model.Department department = null;
            Model.Division division = null;
            Model.Uom uom = null;
            Model.Employee employee = null;
            bool noBGT = false;
			decimal valueMonthSum = 0;

            //Model.BudgetMonthBase budgetMonth1 = null;
            //Model.BudgetMonthBase budgetMonth2 = null;
            //Model.BudgetMonthBase budgetMonth3 = null;
            //Model.BudgetMonthBase budgetMonth4 = null;
            //Model.BudgetMonthBase budgetMonth5 = null;
            //Model.BudgetMonthBase budgetMonth6 = null;
            //Model.BudgetMonthBase budgetMonth7 = null;
            //Model.BudgetMonthBase budgetMonth8 = null;
            //Model.BudgetMonthBase budgetMonth9 = null;
            //Model.BudgetMonthBase budgetMonth10 = null;
            //Model.BudgetMonthBase budgetMonth11 = null;
            //Model.BudgetMonthBase budgetMonth12 = null;

            Model.BudgetForecast budgetForecast = null;
            Model.BudgetMonthBase budgetMonthBase = null;
            //Model.BudgetMonth budgetTotal = null;

            inventory = await _context.Set<Model.Inventory>().Include(i => i.AccMonth).Where(i => i.Active == true).AsNoTracking().FirstOrDefaultAsync();
            if(inventory == null) return new ImportBudgetResult { Success = false, Message = "Lipsa inventar activ", Id = 0 };

            company = await _context.Set<Model.Company>().Where(c => c.Code == "RO10" && c.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
            if (company == null) return new ImportBudgetResult { Success = false, Message = "Lipsa companie RO10", Id = 0 };

            uom = await _context.Set<Model.Uom>().Where(i => i.Code == "RON" && i.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
            if (uom == null) return new ImportBudgetResult { Success = false, Message = "Lipsa moneda RON", Id = 0 };

            if (budgetDto.CurrentIndex == 1)
			{
                budgetType = await _context.Set<Model.BudgetType>()
                .Where(i => i.Name == ((DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + DateTime.Now.Year.ToString()) && i.IsDeleted == false)
                .AsNoTracking()
                .LastOrDefaultAsync();

                if (budgetType == null)
                {
                    budgetTypeNew = new Model.BudgetType
                    {
                        Code = "V1",
                        Name = ((DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + DateTime.Now.Year.ToString()),
                        IsDeleted = false,
                        CompanyId = company.Id
                    };
                    _context.Set<Model.BudgetType>().Add(budgetTypeNew);
                }
                else
                {
                    int bg = int.Parse(budgetType.Code.Substring(1, 1));
                    bg++;
                    budgetTypeNew = new Model.BudgetType
                    {
                        Code = "V" + bg,
                        Name = ((DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + DateTime.Now.Year.ToString()),
                        IsDeleted = false,
                        CompanyId = company.Id
                    };
                    _context.Set<Model.BudgetType>().Add(budgetTypeNew);
                }
			}
			else
			{
                budgetTypeNew = await _context.Set<Model.BudgetType>()
                .Where(i => i.Name == ((DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + DateTime.Now.Year.ToString()) && i.IsDeleted == false)
                .AsNoTracking()
                .LastOrDefaultAsync();

                _context.Set<Model.BudgetType>().Update(budgetTypeNew);
            }

           

            employee = await _context.Set<Model.Employee>().Where(c => c.Email == budgetDto.Employee).AsNoTracking().FirstOrDefaultAsync();
            if (employee == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa owner {budgetDto.Employee}", Id = 0 };

            project = await _context.Set<Model.Project>().Where(c => c.Name == budgetDto.Project && c.IsDeleted == false).FirstOrDefaultAsync();

            if (project == null)
            {
                project = new Model.Project
                {
                    Code = budgetDto.Project,
                    Name = budgetDto.Project.Trim(),
                    IsDeleted = false,
                    CompanyId = company.Id
                };
                _context.Set<Model.Project>().Add(project);
            }

            country = await _context.Set<Model.Country>().Where(c => c.Name == budgetDto.CountryName && c.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
            if (country == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa tara {budgetDto.CountryName}", Id = 0 };

            activity = await _context.Set<Model.Activity>().Where(c => c.Name == budgetDto.Activity && c.IsDeleted == false).FirstOrDefaultAsync();

            if (activity == null)
            {
                activity = new Model.Activity
                {
                    Code = budgetDto.Activity.Trim(),
                    Name = budgetDto.Activity.Trim(),
                    IsDeleted = false,
                    CompanyId = company.Id
                };
                _context.Set<Model.Activity>().Add(activity);
            }

            department = await _context.Set<Model.Department>().Where(c => c.Code == budgetDto.DepartmentCode && c.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
            if (department == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa departament {budgetDto.DepartmentCode}", Id = 0 };

            admCenter = await _context.Set<Model.AdmCenter>().Where(c => c.Name == budgetDto.AdmCenter && c.IsDeleted == false).FirstOrDefaultAsync();

            if (admCenter == null)
            {
                admCenter = new Model.AdmCenter
                {
                    Code = budgetDto.AdmCenter,
                    Name = budgetDto.AdmCenter,
                    IsDeleted = true,
                    CompanyId = company.Id
                };
                _context.Set<Model.AdmCenter>().Add(admCenter);
            }


            region = await _context.Set<Model.Region>().Where(c => c.Name == budgetDto.Region && c.IsDeleted == false).FirstOrDefaultAsync();

            if (region == null)
            {
                region = new Model.Region
                {
                    Code = budgetDto.Region,
                    Name = budgetDto.Region,
                    IsDeleted = true,
                    CompanyId = company.Id
                };
                _context.Set<Model.Region>().Add(region);
            }

            division = await _context.Set<Model.Division>().Where(c => c.Code == budgetDto.DivisionCode && c.DepartmentId == department.Id && c.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
            if (division == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa B.U {budgetDto.DivisionCode}", Id = 0 };
            projectType = await _context.Set<Model.ProjectType>().Where(c => c.Code == budgetDto.ProjectTypeCode && c.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
            if (projectType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa tip {budgetDto.ProjectTypeCode}", Id = 0 };
            assetType = await _context.Set<Model.AssetType>().Where(c => c.Code == budgetDto.AssetTypeCode && c.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
            if (assetType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa tip  {budgetDto.AssetTypeCode}", Id = 0 };
            appState = await _context.Set<Model.AppState>().Where(c => c.Name == budgetDto.AppState && c.IsDeleted == false).FirstOrDefaultAsync();


            if (appState == null)
            {
                appState = new Model.AppState
                {
                    Code = budgetDto.AppState.Trim(),
                    Name = budgetDto.AppState.Trim(),
                    IsDeleted = false,
                    CompanyId = company.Id
                };
                _context.Set<Model.AppState>().Add(appState);
            }

			budgetBase = await _context.Set<Model.BudgetBase>().Where(c => c.IsDeleted == false && c.Name == budgetDto.UniqueCode && c.BudgetManagerId == inventory.BudgetManagerId).FirstOrDefaultAsync();

			if (budgetBase == null)
			{
				entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefaultAsync();
				if (entityType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa entitate", Id = 0 };
				var newBudgetCode = string.Empty;

				var lastCode = int.Parse(entityType.Name);

                string currentYear = DateTime.Now.Year.ToString();
                string code = "";
                int limit = 5 - lastCode.ToString().Length;

                for (int i = 0; i < limit; i++) code += "0";

                newBudgetCode = "B" + currentYear + code + entityType.Name;

                documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").FirstOrDefaultAsync();

				document = new Model.Document()
				{
					Approved = true,
					CompanyId = company.Id,
					CostCenterId = null,
					CreatedAt = DateTime.Now,
					CreatedBy = budgetDto.UserId,
					CreationDate = DateTime.Now,
					Details = string.Empty,
					DocNo1 = string.Empty,
					DocNo2 = string.Empty,
					DocumentDate = DateTime.Now,
					DocumentTypeId = documentType.Id,
					Exported = true,
					IsDeleted = false,
					ModifiedAt = DateTime.Now,
					ModifiedBy = budgetDto.UserId,
					ParentDocumentId = null,
					PartnerId = null,
					RegisterDate = DateTime.Now,
					ValidationDate = DateTime.Now
				};

				_context.Add(document);

                valueMonthSum =
					budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 +
					budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10 + budgetDto.ValueMonth11 + budgetDto.ValueMonth12;

                budgetBase = new Model.BudgetBase()
				{
					AccMonthId = inventory.AccMonthBudgetId,
					EmployeeId = employee.Id,
					Project = project,
					CountryId = country.Id,
					Activity = activity,
					DepartmentId = department.Id,
					AdmCenter = admCenter,
					Region = region,
					DivisionId = division.Id,
					ProjectTypeId = projectType.Id,
					Info = budgetDto.Info,
					AssetTypeId = assetType.Id,
					AppState = appState,
					StartMonth = null,
					DepPeriod = budgetDto.DepPeriod,
					DepPeriodRem = budgetDto.DepPeriodRem,
					Code = newBudgetCode,
					CompanyId = company.Id,
					CreatedAt = DateTime.Now,
					CreatedBy = budgetDto.UserId,
					IsAccepted = true,
					IsDeleted = false,
					ModifiedAt = DateTime.Now,
					ModifiedBy = budgetDto.UserId,
					Name = budgetDto.UniqueCode,
					UserId = budgetDto.UserId,
					Validated = true,
					ValueFin = valueMonthSum,
					ValueIni = valueMonthSum,
					ValueRem = valueMonthSum,
					Total = valueMonthSum,
					UomId = uom.Id,
					//BudgetForecast = budgetForecast,
					//BudgetMonthBase = budgetMonthBase,
					BudgetType = budgetTypeNew,
					BudgetManagerId = inventory.BudgetManagerId,
					IsFirst = false,
					IsLast = true

				};
				_context.Add(budgetBase);

				budgetMonthBase = new Model.BudgetMonthBase()
				{
					AccMonthId = inventory.AccMonthBudgetId,
					BudgetBase = budgetBase,
					BudgetManagerId = inventory.BudgetManagerId.Value,
					BudgetType = budgetTypeNew,
					IsFirst = false,
					IsLast = true,
					April = budgetDto.ValueMonth1,
					May = budgetDto.ValueMonth2,
					June = budgetDto.ValueMonth3,
					July = budgetDto.ValueMonth4,
					August = budgetDto.ValueMonth5,
					September = budgetDto.ValueMonth6,
					Octomber = budgetDto.ValueMonth7,
					November = budgetDto.ValueMonth8,
					December = budgetDto.ValueMonth9,
					January = budgetDto.ValueMonth10,
					February = budgetDto.ValueMonth11,
					March = budgetDto.ValueMonth12,
					Total = valueMonthSum
                };

				_context.Add(budgetMonthBase);
				
				budgetBase.StartMonth = startAccMonth;

				entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
				_context.Update(entityType);

				//if (!noBGT)
				//{
				//                   budgetForecast = _context.Set<Model.BudgetForecast>().Where(c => c.BudgetBaseId == budgetBase.Id && c.BudgetTypeId == budgetType.Id).SingleOrDefault();
				//               }



				//entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefault();

				//var lastCode = int.Parse(entityType.Name);
				//var newBudgetCode = entityType.Code + lastCode.ToString();

				//documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").SingleOrDefault();

				//document = new Model.Document()
				//{
				//    Approved = true,
				//    CompanyId = company.Id,
				//    CostCenterId = null,
				//    CreatedAt = DateTime.Now,
				//    CreatedBy = budgetDto.UserId,
				//    CreationDate = DateTime.Now,
				//    Details = string.Empty,
				//    DocNo1 = string.Empty,
				//    DocNo2 = string.Empty,
				//    DocumentDate = DateTime.Now,
				//    DocumentTypeId = documentType.Id,
				//    Exported = true,
				//    IsDeleted = false,
				//    ModifiedAt = DateTime.Now,
				//    ModifiedBy = budgetDto.UserId,
				//    ParentDocumentId = null,
				//    PartnerId = null,
				//    RegisterDate = DateTime.Now,
				//    ValidationDate = DateTime.Now
				//};

				//_context.Add(document);

				//budget = new Model.BudgetBase()
				//{
				//    AccMonthId = accMonth.Id,
				//    EmployeeId = employee.Id,
				//    Project = project,
				//    Country = country,
				//    Activity = activity,
				//    Department = department,
				//    AdmCenter = admCenter,
				//    Region = region,
				//    Division = division,
				//    ProjectType = projectType,
				//    Info = budgetDto.Info,
				//    AssetType = assetType,
				//    AppState = appState,
				//    StartMonth = null,
				//    DepPeriod = budgetDto.DepPeriod,
				//    DepPeriodRem = budgetDto.DepPeriodRem,
				//    Code = newBudgetCode,
				//    Company = company,
				//    CreatedAt = DateTime.Now,
				//    CreatedBy = budgetDto.UserId,
				//    IsAccepted = true,
				//    IsDeleted = false,
				//    ModifiedAt = DateTime.Now,
				//    ModifiedBy = budgetDto.UserId,
				//    Name = newBudgetCode,
				//    UserId = budgetDto.UserId,
				//    Validated = true,
				//    ValueFin = 0,
				//    ValueIni = 0,
				//    Total = budgetDto.ValueRem,
				//    Uom = uom,
				//    //BudgetForecast = budgetForecast,
				//    //BudgetMonthBase = budgetMonthBase,
				//    BudgetType = budgetType,
				//    BudgetManager = budgetManager

				//};
				//_context.Add(budget);


				//var sumMonth1 = 0;
				//var sumMonth2 = budgetDto.ValueMonth1;
				//var sumMonth3 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2;
				//var sumMonth4 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3;
				//var sumMonth5 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4;
				//var sumMonth6 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5;
				//var sumMonth7 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6;
				//var sumMonth8 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7;
				//var sumMonth9 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8;
				//var sumMonth10 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9;
				//var sumMonth11 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10;
				//var sumMonth12 = budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 + budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10 + budgetDto.ValueMonth11;

				//var startMonth = budgetDto.StartMonth;
				//int month = 0;

				//if (startMonth != null && startMonth != "")
				//{
				//    DateTime date = Convert.ToDateTime(startMonth, CultureInfo.InvariantCulture);

				//    month = date.Month;

				//    if (month == 4)
				//    {
				//        month = 1;

				//    }
				//    else if (month == 5)
				//    {
				//        month = 2;
				//    }
				//    else if (month == 6)
				//    {
				//        month = 3;
				//    }
				//    else if (month == 7)
				//    {
				//        month = 4;
				//    }
				//    else if (month == 8)
				//    {
				//        month = 5;
				//    }
				//    else if (month == 9)
				//    {
				//        month = 6;
				//    }
				//    else if (month == 10)
				//    {
				//        month = 7;
				//    }
				//    else if (month == 11)
				//    {
				//        month = 8;
				//    }
				//    else if (month == 12)
				//    {
				//        month = 9;
				//    }
				//    else if (month == 1)
				//    {
				//        month = 10;
				//    }
				//    else if (month == 2)
				//    {
				//        month = 11;
				//    }
				//    else if (month == 3)
				//    {
				//        month = 12;
				//    }

				//    startAccMonth = _context.Set<Model.AccMonth>().Where(a => a.Month == month && a.Year == 2022).Single();
				//}

				//budgetMonthBase = new Model.BudgetMonthBase()
				//{
				//    BudgetBaseId = budget.Id,
				//    BudgetManagerId = budgetManager.Id,
				//    BudgetType = budgetType,
				//    IsFirst = true,
				//    IsLast = true,
				//    April = budgetDto.ValueMonth1,
				//    May = budgetDto.ValueMonth2,
				//    June = budgetDto.ValueMonth3,
				//    July = budgetDto.ValueMonth4,
				//    August = budgetDto.ValueMonth5,
				//    September = budgetDto.ValueMonth6,
				//    Octomber = budgetDto.ValueMonth7,
				//    November = budgetDto.ValueMonth8,
				//    December = budgetDto.ValueMonth9,
				//    January = budgetDto.ValueMonth10,
				//    February = budgetDto.ValueMonth11,
				//    March = budgetDto.ValueMonth12
				//};

				//_context.Add(budgetMonthBase);

				

				// _context.Update(budgetBase);

				//if (month > 0)
				//{
				//    if (month == 2)
				//    {
				//        budgetForecast.January = 0;

				//    }
				//    else if (month == 3)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;

				//    }
				//    else if (month == 4)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;

				//    }
				//    else if (month == 5)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;

				//    }
				//    else if (month == 6)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;
				//        budgetForecast.May = 0;

				//    }
				//    else if (month == 7)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;
				//        budgetForecast.May = 0;
				//        budgetForecast.June = 0;

				//    }
				//    else if (month == 8)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;
				//        budgetForecast.May = 0;
				//        budgetForecast.June = 0;
				//        budgetForecast.July = 0;

				//    }
				//    else if (month == 9)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;
				//        budgetForecast.May = 0;
				//        budgetForecast.June = 0;
				//        budgetForecast.July = 0;
				//        budgetForecast.August = 0;

				//    }
				//    else if (month == 10)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;
				//        budgetForecast.May = 0;
				//        budgetForecast.June = 0;
				//        budgetForecast.July = 0;
				//        budgetForecast.August = 0;
				//        budgetForecast.September = 0;

				//    }
				//    else if (month == 11)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;
				//        budgetForecast.May = 0;
				//        budgetForecast.June = 0;
				//        budgetForecast.July = 0;
				//        budgetForecast.August = 0;
				//        budgetForecast.September = 0;
				//        budgetForecast.Octomber = 0;

				//    }
				//    else if (month == 12)
				//    {
				//        budgetForecast.January = 0;
				//        budgetForecast.February = 0;
				//        budgetForecast.March = 0;
				//        budgetForecast.April = 0;
				//        budgetForecast.May = 0;
				//        budgetForecast.June = 0;
				//        budgetForecast.July = 0;
				//        budgetForecast.August = 0;
				//        budgetForecast.September = 0;
				//        budgetForecast.Octomber = 0;
				//        budgetForecast.November = 0;

				//    }

				//}
			}



            //budget.StartMonth = startAccMonth;

            //entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            //_context.Update(entityType);

            var sumMonth1 = 0;
            var sumMonth2 = budgetDto.ValueMonth1;
            var sumMonth3 = sumMonth2 + budgetDto.ValueMonth2;
            var sumMonth4 = sumMonth3 + budgetDto.ValueMonth3;
            var sumMonth5 = sumMonth4 + budgetDto.ValueMonth4;
            var sumMonth6 = sumMonth5 + budgetDto.ValueMonth5;
            var sumMonth7 = sumMonth6 + budgetDto.ValueMonth6;
            var sumMonth8 = sumMonth7 + budgetDto.ValueMonth7;
            var sumMonth9 = sumMonth8 + budgetDto.ValueMonth8;
            var sumMonth10 = sumMonth9 + budgetDto.ValueMonth9;
            var sumMonth11 = sumMonth10 + budgetDto.ValueMonth10;
            var sumMonth12 = sumMonth11 + budgetDto.ValueMonth11;

            var startMonth = budgetDto.StartMonth;
			int month = 0;

			if (startMonth != null && startMonth != "")
			{
				DateTime date = Convert.ToDateTime(startMonth, CultureInfo.InvariantCulture);

                month = ((date.Month + 8) % 12) + 1;

                startAccMonth = await _context.Set<Model.AccMonth>().Where(a => a.Month == month && a.Year == inventory.AccMonth.Year).AsNoTracking().FirstOrDefaultAsync();
			}

			budgetForecast = new Model.BudgetForecast()
			{
				BudgetBase = budgetBase,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				BudgetType = budgetTypeNew,
				IsFirst = false,
				IsLast = true,
				April = budgetDto.ValueMonth1,
				May = budgetDto.ValueMonth2,
				June = budgetDto.ValueMonth3,
				July = budgetDto.ValueMonth4,
				August = budgetDto.ValueMonth5,
				September = budgetDto.ValueMonth6,
				Octomber = budgetDto.ValueMonth7,
				November = budgetDto.ValueMonth8,
				December = budgetDto.ValueMonth9,
				January = budgetDto.ValueMonth10,
				February = budgetDto.ValueMonth11,
				March = budgetDto.ValueMonth12,
				Total = (valueMonthSum - budgetDto.ValueOrder),
				TotalRem = (valueMonthSum - budgetDto.ValueOrder),
				AccMonthId = inventory.AccMonthBudgetId,
				DepPeriod = budgetDto.DepPeriod,
				DepPeriodRem = budgetDto.DepPeriodRem,
				ImportValueOrder = budgetDto.ValueOrder
			};

			_context.Add(budgetForecast);

			_context.SaveChanges();

            return new ImportBudgetResult { Success = true, Message = budgetBase.Name, Id = budgetBase.Id };
        }

		public async Task<RequestResult> UpdateAssetBudgetForecast(Dto.BudgetForecastUpdate budgetDto)
		{

            Model.Asset asset = null;
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterialOld = null;
            Model.RequestBFMaterialCostCenter requestBFMaterialCostCenterOld = null;
			List<Model.RequestBFMaterialCostCenter> requestBFMaterialCostCenterOlds = null;


            Model.BudgetForecast budgetForecast = null;
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.Order order = null;
            Model.Inventory inventory = null;
            Model.AppState appState = null;
            Model.AssetState assetState = null;
            Model.OrderMaterial orderMaterial = null;
            Model.Request request = null;
            Model.Offer offer = null;
            Model.Rate rate = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.Tax tax = null;
            //Model.Stock stock = null;
            Model.EmailType emailType = null;
            Model.EntityType entityType = null;
			Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
			Model.RequestBFMaterialCostCenter requestBFMaterialCostCenter = null;
			Model.BudgetBaseAsset budgetBaseAsset = null;
			Model.EmailStatus emailStatus = null;
			Model.AssetOp operation = null;
			Model.Employee employeeIni = null;
			Model.Project project = null;
            Model.AdmCenter admCenter = null;
            Model.CostCenter costCenter = null;
            //Model.Employee employee = null;
            Model.BudgetBase budgetBase = null;
			Model.Document document = null;
			Model.DocumentType documentType = null;

			int documentNumber = 0;
			Guid guid = Guid.NewGuid();
			Guid guidAll = Guid.NewGuid();

			asset = await _context.Set<Model.Asset>().Include(c => c.CostCenter).Where(a => a.Id == budgetDto.AssetId).SingleAsync();

            // OLD //

			requestBudgetForecastMaterialOld = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.Id == asset.ReqBFMaterialId).SingleAsync();
			requestBFMaterialCostCenterOld = await _context.Set<Model.RequestBFMaterialCostCenter>().Where(a => a.Id == asset.ReqBFMCostCenterId).SingleAsync();

			requestBFMaterialCostCenterOlds = await _context.Set<Model.RequestBFMaterialCostCenter>().Where(a => a.RequestBudgetForecastMaterialId == requestBudgetForecastMaterialOld.Id && a.IsDeleted == false).ToListAsync();

            // OLD //

			// NEW //

			budgetForecast = await _context.Set<Model.BudgetForecast>().Include(b => b.BudgetBase).Where(a => a.Id == budgetDto.BudgetForecastId).SingleAsync();

			budgetBase = await _context.Set<Model.BudgetBase>().Where(a => a.Id == budgetForecast.BudgetBaseId).FirstOrDefaultAsync();
			if (budgetBase == null) return new RequestResult { Success = false, Message = $"Cod- ul de buget  nu exista!" };

			admCenter = await _context.Set<Model.AdmCenter>().Where(a => a.Id == asset.CostCenter.AdmCenterId).FirstOrDefaultAsync();
			if (admCenter == null) return new RequestResult { Success = false, Message = $"Profit Center - ul nu exista!" };

			project = await _context.Set<Model.Project>().Where(a => a.Id == budgetForecast.BudgetBase.ProjectId).FirstOrDefaultAsync();
			if (project == null) return new RequestResult { Success = false, Message = $"Cod - ul WBS nu exista!" };

			costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == asset.CostCenterId).FirstOrDefaultAsync();
			if (costCenter == null) return new RequestResult { Success = false, Message = $"Centru de cost nu exista!" };

			//employee = await _context.Set<Model.Employee>().Where(a => a.Id == asset.EmployeeId).FirstOrDefaultAsync();
			//if (employee == null) return new RequestResult { Success = false, Message = $"Marca  nu exista!" };

			//if (employee.Email == null) return new RequestResult { Success = false, Message = $"Marca nu are o adresa de email!" };

			//if(employee.CostCenter == null) return new ImportITMFXResult { Success = false, Message = $"Marca {import.InternalCode} nu are un centru de cost!" };

			//if (employee.CostCenter.Code != costCenter.Code) return new ImportITMFXResult { Success = false, Message = $"Centru - ul de cost al angajatului {employee.Email} este diferit de centru -ul de cost {costCenter.Code} din fisier!" };
			//if (employee.Email.Substring(0, employee.Email.IndexOf('@')) != import.Email) return new RequestResult { Success = false, Message = $"Userul - ul angajatului {employee.Email} este diferit de user -ul {import.Email} din fisier!" };

			order = await _context.Set<Model.Order>().Include(o => o.Offer).ThenInclude(o => o.OfferType).Where(a => a.Id == asset.OrderId).FirstOrDefaultAsync();

			inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).FirstOrDefaultAsync();
			if (inventory == null) return new RequestResult { Success = false, Message = $"Nu exista niciun inventar activ!" };

			tax = await _context.Set<Model.Tax>().Where(t => t.Code == "W1").FirstOrDefaultAsync();
			if (tax == null) return new Model.RequestResult { Success = false, Message = "Lipsa TAX" };

			//stock = await _context.Set<Model.Stock>().Where(t => t.Id == asset.StockId).FirstOrDefaultAsync();
			//if (stock == null) return new Model.RequestResult { Success = false, Message = "Lipsa Produs Stock" };

			emailType = await _context.Set<Model.EmailType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
			if (emailType == null) return new RequestResult { Success = false, Message = $"Nu exista tip email TRANSFER!" };

			entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
			if (entityType == null) return new RequestResult { Success = false, Message = $"Nu exista entitate de tip TRANSFER!" };

			appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
			if (appState == null) return new RequestResult { Success = false, Message = $"Nu exista stare tip FINAL_EMPLOYEE_VALIDATE!" };

			assetState = await _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
			if (assetState == null) return new RequestResult { Success = false, Message = $"Nu exista stare tip FINAL_EMPLOYEE_VALIDATE!" };

			orderMaterial = await _context.Set<Model.OrderMaterial>().Include(o => o.OfferMaterial).Where(a => a.OrderId == order.Id && a.MaterialId == asset.MaterialId && a.IsDeleted == false).FirstOrDefaultAsync();
			if (orderMaterial == null) return new RequestResult { Success = false, Message = $"Nu exista order material pentru numarul de inventar {asset.InvNo} !" };

			request = await _context.Set<Model.Request>().Where(a => a.Id == order.Offer.RequestId && a.IsDeleted == false).FirstOrDefaultAsync();
			if (request == null) return new RequestResult { Success = false, Message = $"Nu exista P.R. pentru numarul de inventar {asset.InvNo} !" };

			offer = await _context.Set<Model.Offer>().Where(a => a.Id == order.OfferId && a.IsDeleted == false).FirstOrDefaultAsync();
			if (offer == null) return new RequestResult { Success = false, Message = $"Nu exista oferta pentru numarul de inventar {asset.InvNo} !" };

			rate = await _context.Set<Model.Rate>().Where(t => t.Id == order.RateId).FirstOrDefaultAsync();
			if (rate == null) return new Model.RequestResult { Success = false, Message = "Lipsa CURS BNR" };

			documentType = await _context.Set<Model.DocumentType>().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
			if (documentType == null) return new RequestResult { Success = false, Message = $"Tip - ul de document transfer nu exista!" };

			assetAdmMD = await _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == asset.Id && a.AccMonthId == inventory.AccMonthId.Value).FirstOrDefaultAsync();

			requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
								   .Where(e => e.BudgetForecastId == budgetForecast.Id && e.RequestId == order.Offer.RequestId && e.ContractId == order.ContractId && e.OfferTypeId == order.Offer.OfferTypeId && e.IsDeleted == false)
								   .FirstOrDefaultAsync();

			if (requestBudgetForecast == null)
            {
				requestBudgetForecast = new Model.RequestBudgetForecast()
				{
					AccMonthId = inventory.AccMonthId.Value,
					AppStateId = appState.Id,
					BudgetForecastId = budgetForecast.Id,
					BudgetManagerId = inventory.BudgetManagerId.Value,
					ContractId = order.ContractId,
					CreatedAt = DateTime.Now,
					CreatedBy = _context.UserId,
					Guid = orderMaterial.OfferMaterial.Guid,
					IsDeleted = false,
					Materials = "",
					MaxQuantity = 1,
					MaxValue = asset.ValueInv,
					MaxValueRon = asset.ValueInvRon,
					ModifiedAt = DateTime.Now,
					ModifiedBy = _context.UserId,
					NeedBudget = false,
					NeedContract = false,
                    Price = asset.ValueInv,
                    PriceRon = asset.ValueInvRon,
					Quantity = 1,
					RequestId = order.Offer.RequestId,
					TotalOrderQuantity = 1,
					TotalOrderValue = asset.ValueInv,
					TotalOrderValueRon = asset.ValueInv,
					Value = asset.ValueInv,
					ValueRon = asset.ValueInvRon,
					NeedBudgetValue = 0,
					NeedContractValue = 0,
					OfferTypeId = order.Offer.OfferTypeId
                };

				_context.Add(requestBudgetForecast);
			}
			else
			{
				requestBudgetForecast.MaxQuantity++;
                requestBudgetForecast.MaxValue += asset.ValueInv;
                requestBudgetForecast.MaxValueRon += asset.ValueInvRon;
                requestBudgetForecast.Quantity++;
				requestBudgetForecast.TotalOrderQuantity++;
				requestBudgetForecast.TotalOrderValue += asset.ValueInv;
				requestBudgetForecast.TotalOrderValueRon += asset.ValueInvRon;
				requestBudgetForecast.Value += asset.ValueInv;
				requestBudgetForecast.ValueRon += asset.ValueInvRon;

				requestBudgetForecast.AppStateId = appState.Id;

				_context.Update(requestBudgetForecast);
			}


			//if (requestBudgetForecast == null) return new ImportITMFXResult { Success = false, Message = $"Nu exista P.R.pentru numarul de inventar {import.ERPCode} !" };

			if (budgetForecast.TotalRem < orderMaterial.PriceRon)
			{
				return new RequestResult { Success = false, Message = $"Nu exista buget disponibil pentru numarul de inventar {asset.InvNo} !" };
			}

			//requestBudgetForecast.BudgetForecastId = budgetForecast.Id;
			//orderMaterial.BudgetForecastId = budgetForecast.Id;
			//request.BudgetForecastId = budgetForecast.Id;
			//offer.BudgetForecastId = budgetForecast.Id;
			//order.BudgetForecastId = budgetForecast.Id;

			//requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
			//    .Where(u => u.RequestBudgetForecastId == requestBudgetForecast.Id && u.MaterialId == asset.MaterialId && u.IsDeleted == false)
			//    .FirstOrDefaultAsync();
			//if (requestBudgetForecastMaterial == null) return new ImportITMFXResult { Success = false, Message = $"Nu exista request budget forecast material pentru numarul de inventar {import.ERPCode} !" };
			requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
								  .Where(e => e.RequestBudgetForecastId == requestBudgetForecast.Id && e.OrderId == order.Id && e.MaterialId == asset.MaterialId && e.OfferTypeId == order.Offer.OfferTypeId && e.IsDeleted == false)
								  .FirstOrDefaultAsync();

			if (requestBudgetForecastMaterial == null)
			{
				requestBudgetForecastMaterial = new Model.RequestBudgetForecastMaterial()
				{
                    AppStateId = appState.Id,
                    BudgetForecastTimeStamp = 0,
                    BudgetValueNeed = 0,
                    CreatedAt = DateTime.Now,
					CreatedBy = _context.UserId,
					Guid = orderMaterial.OfferMaterial.Guid,
                    IsDeleted = false,
					MaterialId = (int)(asset.MaterialId !=null ? asset.MaterialId.Value : (int?)null),
					OfferMaterial = orderMaterial.OfferMaterial,
					OrderId = order.Id,
					OrderMaterial = orderMaterial,
					PreAmount = 0,
                    PreAmountRon = 0,
					Price = asset.ValueInv,
					PriceRon = asset.ValueInvRon,
					Quantity = 1,
					QuantityRem = 0,
					RequestBudgetForecast = requestBudgetForecast,
					Value = asset.ValueInv,
					ValueRon = asset.ValueInvRon,
					ValueRem = 0,
					ValueRemRon = 0,
					MaxQuantity = 1,
                    MaxValue = asset.ValueInv,
                    MaxValueRon = asset.ValueInvRon,
                    ModifiedAt = DateTime.Now,
					ModifiedBy = _context.UserId,
					OfferTypeId = order.Offer.OfferTypeId

				};

				_context.Add(requestBudgetForecastMaterial);
			}
			else
			{
				requestBudgetForecastMaterial.MaxQuantity++;
				requestBudgetForecastMaterial.MaxValue += asset.ValueInv;
				requestBudgetForecastMaterial.MaxValueRon += asset.ValueInvRon;
				requestBudgetForecastMaterial.Quantity++;
				//requestBudgetForecastMaterial.QuantityRem++;
				//requestBudgetForecastMaterial.ValueRemRon += stock.Value;
				//requestBudgetForecastMaterial.ValueRem += stock.Value;
				requestBudgetForecastMaterial.Value += asset.ValueInv;
				requestBudgetForecastMaterial.ValueRon += asset.ValueInvRon;


				requestBudgetForecastMaterial.AppStateId = appState.Id;
				//requestBudgetForecastMaterial.QuantityRem--;
				//requestBudgetForecastMaterial.ValueRem -= requestBudgetForecastMaterial.Price;
				//requestBudgetForecastMaterial.ValueRemRon -= requestBudgetForecastMaterial.PriceRon;

				requestBudgetForecastMaterial.TotalCostCenterQuantity++;
				requestBudgetForecastMaterial.TotalCostCenterValue = requestBudgetForecastMaterial.Price;
				requestBudgetForecastMaterial.TotalCostCenterValueRon = requestBudgetForecastMaterial.PriceRon;

				requestBudgetForecastMaterial.QuantityRem = 0;
				requestBudgetForecastMaterial.ValueRem = 0;
				requestBudgetForecastMaterial.ValueRemRon = 0;


				_context.Update(requestBudgetForecastMaterial);
			}

			List<Model.RequestBFMaterialCostCenter> requestBFMaterialCostCenters = await _context.Set<Model.RequestBFMaterialCostCenter>().
				Where(a => a.RequestBudgetForecastMaterialId == requestBudgetForecastMaterial.Id && a.OrderMaterialId == orderMaterial.Id && a.OfferMaterialId == orderMaterial.OfferMaterialId
				&& a.OrderId == order.Id).ToListAsync();

			for (int r = 0; r < requestBFMaterialCostCenters.Count; r++)
			{
				requestBFMaterialCostCenters[r].MaxQuantity++;
				requestBFMaterialCostCenters[r].MaxValue += asset.ValueInv;
				requestBFMaterialCostCenters[r].MaxValueRon += asset.ValueInvRon;

				_context.Update(requestBFMaterialCostCenters[r]);
			}

			requestBFMaterialCostCenter = new Model.RequestBFMaterialCostCenter()
			{
				RequestBudgetForecastMaterial = requestBudgetForecastMaterial,
				CostCenterId = costCenter.Id,
				Price = requestBudgetForecastMaterial.Price,
				PriceRon = requestBudgetForecastMaterial.PriceRon,
				Quantity = 1,
				QuantityRem = 0,
				Value = requestBudgetForecastMaterial.Price,
				ValueRon = requestBudgetForecastMaterial.PriceRon,
				ValueRem = 0,
				ValueRemRon = 0,
				Guid = requestBudgetForecastMaterial.Guid,
				AppStateId = appState.Id,
				OfferTypeId = requestBudgetForecastMaterial.OfferTypeId,
				MaxQuantity = requestBudgetForecastMaterial.Quantity,
				MaxValueRon = requestBudgetForecastMaterial.ValueRon,
				MaxValue = requestBudgetForecastMaterial.Value,
				OfferMaterialId = requestBudgetForecastMaterial.OfferMaterialId,
				OrderId = requestBudgetForecastMaterial.OrderId,
				OrderMaterialId = requestBudgetForecastMaterial.OrderMaterialId,
				ReceptionsPrice = requestBudgetForecastMaterial.Price,
				ReceptionsPriceRon = requestBudgetForecastMaterial.PriceRon,
				ReceptionsQuantity = 1,
				ReceptionsValue = requestBudgetForecastMaterial.Price,
				ReceptionsValueRon = requestBudgetForecastMaterial.PriceRon
			};

			_context.Add(requestBFMaterialCostCenter);

			budgetBaseAsset = new BudgetBaseAsset()
			{
				BudgetBaseId = budgetBase.Id,
				BudgetTypeId = 1,
                BudgetManagerId = inventory.BudgetManagerId.Value,
				AccMonthId = inventory.AccMonthId.Value,
                AppStateId = appState.Id,
                AssetId = asset.Id,
                IsLast = true,
                IsFirst = true,
            };

			_context.Add(budgetBaseAsset);

			documentNumber = int.Parse(entityType.Name);
			documentNumber++;

			document = new Model.Document();

			document.DocumentTypeId = documentType.Id;
			document.DocNo1 = string.Empty;
			document.DocNo2 = string.Empty;
			document.DocumentDate = DateTime.Now;
			document.Approved = true;
			document.Exported = false;
			document.CreationDate = DateTime.Now;
			document.CreatedAt = DateTime.Now;
			document.RegisterDate = DateTime.Now;
			document.ValidationDate = DateTime.Now;

			_context.Add(document);

			asset.Guid = guid;
			asset.AppStateId = appState.Id;
			asset.AssetStateId = assetState.Id;
			asset.IsInTransfer = true;
			//asset.EmployeeTransferId = employee.Id;
			asset.ModifiedAt = DateTime.Now;
			asset.ModifiedBy = _context.UserId;
			asset.BudgetForecastId = budgetForecast.Id;
			asset.ReqBFMCostCenter = requestBFMaterialCostCenter;
			asset.ReqBFMaterialId = requestBudgetForecastMaterial.Id;
			asset.RequestId = offer.RequestId;
			asset.BudgetBaseId = budgetBase.Id;
			asset.OfferMaterialId = orderMaterial.OfferMaterialId;
			asset.OrderMaterialId = orderMaterial.Id;
			asset.TaxId = tax.Id;
			asset.RateId = rate.Id;
			asset.ContractId = order.ContractId;
			asset.ProjectTypeId = budgetBase.ProjectTypeId;
			asset.AllowLabel = true;
			asset.ProjectId = budgetBase.ProjectId;
			//asset.BrandId = a.BrandId;
			asset.UomId = rate.UomId;

			asset.ValueInv = requestBudgetForecastMaterial.Price;
			asset.ValueInvRon = requestBudgetForecastMaterial.PriceRon;
			asset.ValueRem = requestBudgetForecastMaterial.Price;
			asset.ValueRemRon = requestBudgetForecastMaterial.PriceRon;
			asset.TaxAmount = (requestBudgetForecastMaterial.Price * tax.Value) / 100;
			asset.TaxAmountRon = (requestBudgetForecastMaterial.PriceRon * tax.Value) / 100;
			asset.NetAmount = requestBudgetForecastMaterial.Price;
			asset.NetAmountRon = requestBudgetForecastMaterial.PriceRon;
			asset.TotalAmount = (requestBudgetForecastMaterial.Price + (requestBudgetForecastMaterial.Price * tax.Value) / 100);
			asset.TotalAmountRon = (requestBudgetForecastMaterial.Price + (requestBudgetForecastMaterial.Price * tax.Value) / 100);
			asset.ReceptionPrice = requestBudgetForecastMaterial.PriceRon;

			_context.Update(asset);

			if (assetAdmMD != null)
			{
				assetAdmMD.ProjectId = budgetBase.ProjectId;
				assetAdmMD.AssetStateId = assetState.Id;
				_context.Update(assetAdmMD);

			}


			operation = new Model.AssetOp
			{
				AssetOpStateId = appState.Id,
				InvStateIdInitial = asset.InvStateId,
				InvStateIdFinal = asset.InvStateId,
				AssetStateIdInitial = asset.AssetStateId,
				AssetStateIdFinal = assetState.Id
			};

			operation.AccSystemId = 3;
			operation.AdministrationIdInitial = asset.AdministrationId;
			operation.AdministrationIdFinal = asset.AdministrationId;
			operation.AssetCategoryIdInitial = asset.AssetCategoryId;
			operation.AssetCategoryIdFinal = asset.AssetCategoryId;
			operation.AssetId = asset.Id;
			operation.CostCenterIdInitial = asset.CostCenterId;
			operation.CostCenterIdFinal = costCenter.Id;
			operation.CreatedAt = DateTime.Now;
			operation.CreatedBy = _context.UserId;
			operation.DepartmentIdInitial = asset.DepartmentId;
			operation.DepartmentIdFinal = asset.DepartmentId;
			operation.DocumentId = document.Id;
			operation.EmployeeIdInitial = asset.EmployeeId;
			//operation.EmployeeIdFinal = employee.Id;
			operation.IsDeleted = false;
			operation.ModifiedAt = DateTime.Now;
			operation.ModifiedBy = _context.UserId;
			operation.RoomIdInitial = asset.RoomId;
			operation.RoomIdFinal = asset.RoomId;
			operation.SrcConfAt = DateTime.Now;
			operation.SrcConfBy = _context.UserId;
			operation.AllowLabel = asset.AllowLabel != null ? (bool)asset.AllowLabel : false;
			operation.AssetTypeIdInitial = asset.AssetTypeId;
			operation.AssetTypeIdFinal = asset.AssetTypeId;
			operation.InvName = asset.Name;
			operation.Quantity = asset.Quantity;
			operation.SerialNumber = asset.SerialNumber;
			operation.AssetNatureIdInitial = asset.AssetNatureId;
			operation.AssetNatureIdFinal = asset.AssetNatureId;
			operation.BudgetManagerIdInitial = asset.BudgetManagerId;
			operation.BudgetManagerIdFinal = asset.BudgetManagerId;
			operation.DimensionIdInitial = asset.DimensionId;
			operation.DimensionIdFinal = asset.DimensionId;
			operation.ProjectIdInitial = asset.ProjectId;
			operation.ProjectIdFinal = asset.ProjectId;
			operation.IsMinus = false;
			operation.IsPlus = false;
			operation.CompanyId = asset.CompanyId;
			operation.InsuranceCategoryId = asset.InsuranceCategoryId;
			operation.InterCompanyId = asset.InterCompanyId;
			operation.UomId = asset.UomId;
			operation.TaxId = asset.TaxId;
			operation.ValueAdd = documentNumber;
			operation.Guid = guid;

			emailStatus = new Model.EmailStatus()
			{
				AppStateId = appState.Id,
				AssetId = asset.Id,
				AssetOp = operation,
				BudgetBaseId = asset.BudgetBaseId,
				CompanyId = asset.CompanyId,
				Completed = false,
				CostCenterIdFinal = costCenter.Id,
				CostCenterIdInitial = asset.CostCenterId,
				CreatedAt = DateTime.Now,
				CreatedBy = _context.UserId,
				DocumentNumber = documentNumber,
				DstEmployeeEmailSend = false,
				DstEmployeeValidateAt = null,
				DstEmployeeValidateBy = null,
				DstManagerEmailSend = false,
				DstManagerValidateAt = null,
				DstManagerValidateBy = null,
				EmailSend = false,
				EmailTypeId = emailType.Id,
				//EmployeeIdFinal = employee.Id,
				EmployeeIdInitial = asset.EmployeeId,
				ErrorId = null,
				Exported = false,
				FinalValidateAt = null,
				FinalValidateBy = null,
				Guid = guid,
				GuidAll = guidAll,
				Info = string.Empty,
				IsAccepted = false,
				IsDeleted = true,
				ModifiedAt = DateTime.Now,
				ModifiedBy = _context.UserId,
				NotCompletedSync = false,
				NotDstEmployeeSync = true,
				NotDstManagerSync = false,
				NotSrcEmployeeSync = false,
				NotSrcManagerSync = false,
				NotSync = false,
				OfferId = order.OfferId,
				OrderId = asset.OrderId,
				PartnerId = order.PartnerId,
				RequestId = asset.RequestId,
				//SameEmployee = asset.EmployeeId == employee.Id ? true : false,
				SameManager = false,
				Skip = false,
				SkipDstEmployee = false,
				SkipDstManager = false,
				SkipSrcEmployee = false,
				SkipSrcManager = false,
				SrcEmployeeEmailSend = false,
				SrcEmployeeValidateAt = DateTime.Now,
				SrcEmployeeValidateBy = _context.UserId,
				SrcManagerEmailSend = false,
				SrcManagerValidateAt = DateTime.Now,
				SrcManagerValidateBy = _context.UserId,
				StockId = asset.StockId,
				SyncCompletedErrorCount = 0,
				SyncDstEmployeeErrorCount = 0,
				SyncDstManagerErrorCount = 0,
				SyncErrorCount = 0,
				SyncSrcEmployeeErrorCount = 0,
				SyncSrcManagerErrorCount = 0

			};

			entityType.Name = documentNumber.ToString();

			_context.Add(emailStatus);
			_context.Add(operation);
			_context.Update(asset);
			_context.Update(entityType);
			_context.SaveChanges();

			var count = await _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToListAsync();
			var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
			var countOrd = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
			var countOrdMaterial = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();
			var countContract = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
			var countContractAmount = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();
			var countBudget = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
			var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
			var countOfferMaterials2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials2").ToList();
			var UpdateAllRequestBudgetForecastMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBudgetForecastMaterials").ToList();
			var UpdateAllRequestBFMaterialCostCenters = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBFMaterialCostCenters").ToList();

			// NEW //

			_context.SaveChanges();

			return new RequestResult { Success = true, Message = "", RequestId = 0 };
		}

		//public async Task<RequestResult> UpdateAssetBudgetForecast(Dto.BudgetForecastUpdate budgetDto)
		//{

		//	Model.Asset asset = null;
		//	Model.RequestBudgetForecastMaterial requestBudgetForecastMaterialOld = null;
		//	Model.RequestBFMaterialCostCenter requestBFMaterialCostCenterOld = null;
		//	List<Model.RequestBFMaterialCostCenter> requestBFMaterialCostCenterOlds = null;


		//	Model.BudgetForecast budgetForecast = null;
		//	Model.RequestBudgetForecast requestBudgetForecast = null;
		//	Model.Order order = null;
		//	Model.Inventory inventory = null;
		//	Model.AppState appState = null;
		//	Model.AssetState assetState = null;
		//	Model.OrderMaterial orderMaterial = null;
		//	Model.Request request = null;
		//	Model.Offer offer = null;
		//	Model.Rate rate = null;
		//	Model.AssetAdmMD assetAdmMD = null;
		//	Model.Tax tax = null;
		//	Model.Stock stock = null;
		//	Model.EmailType emailType = null;
		//	Model.EntityType entityType = null;
		//	Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
		//	Model.RequestBFMaterialCostCenter requestBFMaterialCostCenter = null;
		//	Model.BudgetBaseAsset budgetBaseAsset = null;
		//	Model.EmailStatus emailStatus = null;
		//	Model.AssetOp operation = null;
		//	Model.Employee employeeIni = null;
		//	Model.Project project = null;
		//	Model.AdmCenter admCenter = null;
		//	Model.CostCenter costCenter = null;
		//	//Model.Employee employee = null;
		//	Model.BudgetBase budgetBase = null;
		//	Model.Document document = null;
		//	Model.DocumentType documentType = null;

		//	int documentNumber = 0;
		//	Guid guid = Guid.NewGuid();
		//	Guid guidAll = Guid.NewGuid();

		//	asset = await _context.Set<Model.Asset>().Include(c => c.CostCenter).Where(a => a.Id == budgetDto.AssetId).SingleAsync();

		//	// OLD //

		//	requestBudgetForecastMaterialOld = await _context.Set<Model.RequestBudgetForecastMaterial>().Where(a => a.Id == asset.ReqBFMaterialId).SingleAsync();
		//	requestBFMaterialCostCenterOld = await _context.Set<Model.RequestBFMaterialCostCenter>().Where(a => a.Id == asset.ReqBFMCostCenterId).SingleAsync();

		//	requestBFMaterialCostCenterOlds = await _context.Set<Model.RequestBFMaterialCostCenter>().Where(a => a.RequestBudgetForecastMaterialId == requestBudgetForecastMaterialOld.Id && a.IsDeleted == false).ToListAsync();

		//	// OLD //

		//	// NEW //

		//	budgetForecast = await _context.Set<Model.BudgetForecast>().Include(b => b.BudgetBase).Where(a => a.Id == budgetDto.BudgetForecastId).SingleAsync();

		//	budgetBase = await _context.Set<Model.BudgetBase>().Where(a => a.Id == budgetForecast.BudgetBaseId).FirstOrDefaultAsync();
		//	if (budgetBase == null) return new RequestResult { Success = false, Message = $"Cod- ul de buget  nu exista!" };

		//	admCenter = await _context.Set<Model.AdmCenter>().Where(a => a.Id == asset.CostCenter.AdmCenterId).FirstOrDefaultAsync();
		//	if (admCenter == null) return new RequestResult { Success = false, Message = $"Profit Center - ul nu exista!" };

		//	project = await _context.Set<Model.Project>().Where(a => a.Id == budgetForecast.BudgetBase.ProjectId).FirstOrDefaultAsync();
		//	if (project == null) return new RequestResult { Success = false, Message = $"Cod - ul WBS nu exista!" };

		//	costCenter = await _context.Set<Model.CostCenter>().Where(a => a.Id == asset.CostCenterId).FirstOrDefaultAsync();
		//	if (costCenter == null) return new RequestResult { Success = false, Message = $"Centru de cost nu exista!" };

		//	//employee = await _context.Set<Model.Employee>().Where(a => a.Id == asset.EmployeeId).FirstOrDefaultAsync();
		//	//if (employee == null) return new RequestResult { Success = false, Message = $"Marca  nu exista!" };

		//	//if (employee.Email == null) return new RequestResult { Success = false, Message = $"Marca nu are o adresa de email!" };

		//	//if(employee.CostCenter == null) return new ImportITMFXResult { Success = false, Message = $"Marca {import.InternalCode} nu are un centru de cost!" };

		//	//if (employee.CostCenter.Code != costCenter.Code) return new ImportITMFXResult { Success = false, Message = $"Centru - ul de cost al angajatului {employee.Email} este diferit de centru -ul de cost {costCenter.Code} din fisier!" };
		//	//if (employee.Email.Substring(0, employee.Email.IndexOf('@')) != import.Email) return new RequestResult { Success = false, Message = $"Userul - ul angajatului {employee.Email} este diferit de user -ul {import.Email} din fisier!" };

		//	order = await _context.Set<Model.Order>().Include(o => o.Offer).ThenInclude(o => o.OfferType).Where(a => a.Id == asset.OrderId).FirstOrDefaultAsync();

		//	inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).FirstOrDefaultAsync();
		//	if (inventory == null) return new RequestResult { Success = false, Message = $"Nu exista niciun inventar activ!" };

		//	tax = await _context.Set<Model.Tax>().Where(t => t.Code == "W1").FirstOrDefaultAsync();
		//	if (tax == null) return new Model.RequestResult { Success = false, Message = "Lipsa TAX" };

		//	stock = await _context.Set<Model.Stock>().Where(t => t.Id == asset.StockId).FirstOrDefaultAsync();
		//	if (stock == null) return new Model.RequestResult { Success = false, Message = "Lipsa Produs Stock" };

		//	emailType = await _context.Set<Model.EmailType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
		//	if (emailType == null) return new RequestResult { Success = false, Message = $"Nu exista tip email TRANSFER!" };

		//	entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
		//	if (entityType == null) return new RequestResult { Success = false, Message = $"Nu exista entitate de tip TRANSFER!" };

		//	appState = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
		//	if (appState == null) return new RequestResult { Success = false, Message = $"Nu exista stare tip FINAL_EMPLOYEE_VALIDATE!" };

		//	assetState = await _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
		//	if (assetState == null) return new RequestResult { Success = false, Message = $"Nu exista stare tip FINAL_EMPLOYEE_VALIDATE!" };

		//	orderMaterial = await _context.Set<Model.OrderMaterial>().Include(o => o.OfferMaterial).Where(a => a.OrderId == order.Id && a.MaterialId == asset.MaterialId && a.IsDeleted == false).FirstOrDefaultAsync();
		//	if (orderMaterial == null) return new RequestResult { Success = false, Message = $"Nu exista order material pentru numarul de inventar {asset.InvNo} !" };

		//	request = await _context.Set<Model.Request>().Where(a => a.Id == order.Offer.RequestId && a.IsDeleted == false).FirstOrDefaultAsync();
		//	if (request == null) return new RequestResult { Success = false, Message = $"Nu exista P.R. pentru numarul de inventar {asset.InvNo} !" };

		//	offer = await _context.Set<Model.Offer>().Where(a => a.Id == order.OfferId && a.IsDeleted == false).FirstOrDefaultAsync();
		//	if (offer == null) return new RequestResult { Success = false, Message = $"Nu exista oferta pentru numarul de inventar {asset.InvNo} !" };

		//	rate = await _context.Set<Model.Rate>().Where(t => t.Id == order.RateId).FirstOrDefaultAsync();
		//	if (rate == null) return new Model.RequestResult { Success = false, Message = "Lipsa CURS BNR" };

		//	documentType = await _context.Set<Model.DocumentType>().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
		//	if (documentType == null) return new RequestResult { Success = false, Message = $"Tip - ul de document transfer nu exista!" };

		//	assetAdmMD = await _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == asset.Id && a.AccMonthId == inventory.AccMonthId.Value).FirstOrDefaultAsync();

		//	requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
		//						   .Where(e => e.BudgetForecastId == budgetForecast.Id && e.RequestId == order.Offer.RequestId && e.ContractId == order.ContractId && e.OfferTypeId == order.Offer.OfferTypeId && e.IsDeleted == false)
		//						   .FirstOrDefaultAsync();

		//	if (requestBudgetForecast == null)
		//	{
		//		requestBudgetForecast = new Model.RequestBudgetForecast()
		//		{
		//			AccMonthId = inventory.AccMonthId.Value,
		//			AppStateId = appState.Id,
		//			BudgetForecastId = budgetForecast.Id,
		//			BudgetManagerId = inventory.BudgetManagerId.Value,
		//			ContractId = order.ContractId,
		//			CreatedAt = DateTime.Now,
		//			CreatedBy = _context.UserId,
		//			Guid = orderMaterial.OfferMaterial.Guid,
		//			IsDeleted = false,
		//			Materials = "",
		//			MaxQuantity = 1,
		//			MaxValue = stock.Value,
		//			MaxValueRon = stock.Value,
		//			ModifiedAt = DateTime.Now,
		//			ModifiedBy = _context.UserId,
		//			NeedBudget = false,
		//			NeedContract = false,
		//			Price = stock.Value,
		//			PriceRon = stock.Value,
		//			Quantity = 1,
		//			RequestId = order.Offer.RequestId,
		//			TotalOrderQuantity = 1,
		//			TotalOrderValue = stock.Value,
		//			TotalOrderValueRon = stock.Value,
		//			Value = stock.Value,
		//			ValueRon = stock.Value,
		//			NeedBudgetValue = 0,
		//			NeedContractValue = 0,
		//			OfferTypeId = order.Offer.OfferTypeId
		//		};

		//		_context.Add(requestBudgetForecast);
		//	}
		//	else
		//	{
		//		requestBudgetForecast.MaxQuantity++;
		//		requestBudgetForecast.MaxValue += stock.Value;
		//		requestBudgetForecast.MaxValueRon += stock.Value;
		//		requestBudgetForecast.Quantity++;
		//		requestBudgetForecast.TotalOrderQuantity++;
		//		requestBudgetForecast.TotalOrderValue += stock.Value;
		//		requestBudgetForecast.TotalOrderValueRon += stock.Value;
		//		requestBudgetForecast.Value += stock.Value;
		//		requestBudgetForecast.ValueRon += stock.Value;

		//		requestBudgetForecast.AppStateId = appState.Id;

		//		_context.Update(requestBudgetForecast);
		//	}


		//	//if (requestBudgetForecast == null) return new ImportITMFXResult { Success = false, Message = $"Nu exista P.R.pentru numarul de inventar {import.ERPCode} !" };

		//	if (budgetForecast.TotalRem < orderMaterial.PriceRon)
		//	{
		//		return new RequestResult { Success = false, Message = $"Nu exista buget disponibil pentru numarul de inventar {asset.InvNo} !" };
		//	}

		//	//requestBudgetForecast.BudgetForecastId = budgetForecast.Id;
		//	//orderMaterial.BudgetForecastId = budgetForecast.Id;
		//	//request.BudgetForecastId = budgetForecast.Id;
		//	//offer.BudgetForecastId = budgetForecast.Id;
		//	//order.BudgetForecastId = budgetForecast.Id;

		//	//requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
		//	//    .Where(u => u.RequestBudgetForecastId == requestBudgetForecast.Id && u.MaterialId == asset.MaterialId && u.IsDeleted == false)
		//	//    .FirstOrDefaultAsync();
		//	//if (requestBudgetForecastMaterial == null) return new ImportITMFXResult { Success = false, Message = $"Nu exista request budget forecast material pentru numarul de inventar {import.ERPCode} !" };
		//	requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
		//						  .Where(e => e.RequestBudgetForecastId == requestBudgetForecast.Id && e.OrderId == order.Id && e.MaterialId == asset.MaterialId && e.OfferTypeId == order.Offer.OfferTypeId && e.IsDeleted == false)
		//						  .FirstOrDefaultAsync();

		//	if (requestBudgetForecastMaterial == null)
		//	{
		//		requestBudgetForecastMaterial = new Model.RequestBudgetForecastMaterial()
		//		{
		//			AppStateId = appState.Id,
		//			BudgetForecastTimeStamp = 0,
		//			BudgetValueNeed = 0,
		//			CreatedAt = DateTime.Now,
		//			CreatedBy = _context.UserId,
		//			Guid = orderMaterial.OfferMaterial.Guid,
		//			IsDeleted = false,
		//			MaterialId = stock.MaterialId != null ? stock.MaterialId.Value : 627,
		//			OfferMaterial = orderMaterial.OfferMaterial,
		//			OrderId = order.Id,
		//			OrderMaterial = orderMaterial,
		//			PreAmount = 0,
		//			PreAmountRon = 0,
		//			Price = stock.Value,
		//			PriceRon = stock.Value,
		//			Quantity = 1,
		//			QuantityRem = 0,
		//			RequestBudgetForecast = requestBudgetForecast,
		//			Value = stock.Value,
		//			ValueRon = stock.Value,
		//			ValueRem = 0,
		//			ValueRemRon = 0,
		//			MaxQuantity = 1,
		//			MaxValue = stock.Value,
		//			MaxValueRon = stock.Value,
		//			ModifiedAt = DateTime.Now,
		//			ModifiedBy = _context.UserId,
		//			OfferTypeId = order.Offer.OfferTypeId

		//		};

		//		_context.Add(requestBudgetForecastMaterial);
		//	}
		//	else
		//	{
		//		requestBudgetForecastMaterial.MaxQuantity++;
		//		requestBudgetForecastMaterial.MaxValue += stock.Value;
		//		requestBudgetForecastMaterial.MaxValueRon += stock.Value;
		//		requestBudgetForecastMaterial.Quantity++;
		//		//requestBudgetForecastMaterial.QuantityRem++;
		//		//requestBudgetForecastMaterial.ValueRemRon += stock.Value;
		//		//requestBudgetForecastMaterial.ValueRem += stock.Value;
		//		requestBudgetForecastMaterial.Value += stock.Value;
		//		requestBudgetForecastMaterial.ValueRon += stock.Value;


		//		requestBudgetForecastMaterial.AppStateId = appState.Id;
		//		//requestBudgetForecastMaterial.QuantityRem--;
		//		//requestBudgetForecastMaterial.ValueRem -= requestBudgetForecastMaterial.Price;
		//		//requestBudgetForecastMaterial.ValueRemRon -= requestBudgetForecastMaterial.PriceRon;

		//		requestBudgetForecastMaterial.TotalCostCenterQuantity++;
		//		requestBudgetForecastMaterial.TotalCostCenterValue = requestBudgetForecastMaterial.Price;
		//		requestBudgetForecastMaterial.TotalCostCenterValueRon = requestBudgetForecastMaterial.PriceRon;

		//		requestBudgetForecastMaterial.QuantityRem = 0;
		//		requestBudgetForecastMaterial.ValueRem = 0;
		//		requestBudgetForecastMaterial.ValueRemRon = 0;


		//		_context.Update(requestBudgetForecastMaterial);
		//	}

		//	List<Model.RequestBFMaterialCostCenter> requestBFMaterialCostCenters = await _context.Set<Model.RequestBFMaterialCostCenter>().
		//		Where(a => a.RequestBudgetForecastMaterialId == requestBudgetForecastMaterial.Id && a.OrderMaterialId == orderMaterial.Id && a.OfferMaterialId == orderMaterial.OfferMaterialId
		//		&& a.OrderId == order.Id).ToListAsync();

		//	for (int r = 0; r < requestBFMaterialCostCenters.Count; r++)
		//	{
		//		requestBFMaterialCostCenters[r].MaxQuantity++;
		//		requestBFMaterialCostCenters[r].MaxValue += stock.Value;
		//		requestBFMaterialCostCenters[r].MaxValueRon += stock.Value;

		//		_context.Update(requestBFMaterialCostCenters[r]);
		//	}

		//	requestBFMaterialCostCenter = new Model.RequestBFMaterialCostCenter()
		//	{
		//		RequestBudgetForecastMaterial = requestBudgetForecastMaterial,
		//		CostCenterId = costCenter.Id,
		//		Price = requestBudgetForecastMaterial.Price,
		//		PriceRon = requestBudgetForecastMaterial.PriceRon,
		//		Quantity = 1,
		//		QuantityRem = 0,
		//		Value = requestBudgetForecastMaterial.Price,
		//		ValueRon = requestBudgetForecastMaterial.PriceRon,
		//		ValueRem = 0,
		//		ValueRemRon = 0,
		//		Guid = requestBudgetForecastMaterial.Guid,
		//		AppStateId = appState.Id,
		//		OfferTypeId = requestBudgetForecastMaterial.OfferTypeId,
		//		MaxQuantity = requestBudgetForecastMaterial.Quantity,
		//		MaxValueRon = requestBudgetForecastMaterial.ValueRon,
		//		MaxValue = requestBudgetForecastMaterial.Value,
		//		OfferMaterialId = requestBudgetForecastMaterial.OfferMaterialId,
		//		OrderId = requestBudgetForecastMaterial.OrderId,
		//		OrderMaterialId = requestBudgetForecastMaterial.OrderMaterialId,
		//		ReceptionsPrice = requestBudgetForecastMaterial.Price,
		//		ReceptionsPriceRon = requestBudgetForecastMaterial.PriceRon,
		//		ReceptionsQuantity = 1,
		//		ReceptionsValue = requestBudgetForecastMaterial.Price,
		//		ReceptionsValueRon = requestBudgetForecastMaterial.PriceRon
		//	};

		//	_context.Add(requestBFMaterialCostCenter);




		//	budgetBaseAsset = new BudgetBaseAsset()
		//	{
		//		BudgetBaseId = budgetBase.Id,
		//		BudgetTypeId = 1,
		//		BudgetManagerId = inventory.BudgetManagerId.Value,
		//		AccMonthId = inventory.AccMonthId.Value,
		//		AppStateId = appState.Id,
		//		AssetId = asset.Id,
		//		IsLast = true,
		//		IsFirst = true,
		//	};

		//	_context.Add(budgetBaseAsset);

		//	documentNumber = int.Parse(entityType.Name);
		//	documentNumber++;

		//	document = new Model.Document();

		//	document.DocumentTypeId = documentType.Id;
		//	document.DocNo1 = string.Empty;
		//	document.DocNo2 = string.Empty;
		//	document.DocumentDate = DateTime.Now;
		//	document.Approved = true;
		//	document.Exported = false;
		//	document.CreationDate = DateTime.Now;
		//	document.CreatedAt = DateTime.Now;
		//	document.RegisterDate = DateTime.Now;
		//	document.ValidationDate = DateTime.Now;

		//	_context.Add(document);

		//	asset.Guid = guid;
		//	asset.AppStateId = appState.Id;
		//	asset.AssetStateId = assetState.Id;
		//	asset.IsInTransfer = true;
		//	//asset.EmployeeTransferId = employee.Id;
		//	asset.ModifiedAt = DateTime.Now;
		//	asset.ModifiedBy = _context.UserId;
		//	asset.BudgetForecastId = budgetForecast.Id;
		//	asset.ReqBFMCostCenter = requestBFMaterialCostCenter;
		//	asset.ReqBFMaterialId = requestBudgetForecastMaterial.Id;
		//	asset.RequestId = offer.RequestId;
		//	asset.BudgetBaseId = budgetBase.Id;
		//	asset.OfferMaterialId = orderMaterial.OfferMaterialId;
		//	asset.OrderMaterialId = orderMaterial.Id;
		//	asset.TaxId = tax.Id;
		//	asset.RateId = rate.Id;
		//	asset.ContractId = order.ContractId;
		//	asset.ProjectTypeId = budgetBase.ProjectTypeId;
		//	asset.AllowLabel = true;
		//	asset.ProjectId = budgetBase.ProjectId;
		//	asset.BrandId = stock.BrandId;
		//	asset.UomId = rate.UomId;

		//	asset.ValueInv = requestBudgetForecastMaterial.Price;
		//	asset.ValueInvRon = requestBudgetForecastMaterial.PriceRon;
		//	asset.ValueRem = requestBudgetForecastMaterial.Price;
		//	asset.ValueRemRon = requestBudgetForecastMaterial.PriceRon;
		//	asset.TaxAmount = (requestBudgetForecastMaterial.Price * tax.Value) / 100;
		//	asset.TaxAmountRon = (requestBudgetForecastMaterial.PriceRon * tax.Value) / 100;
		//	asset.NetAmount = requestBudgetForecastMaterial.Price;
		//	asset.NetAmountRon = requestBudgetForecastMaterial.PriceRon;
		//	asset.TotalAmount = (requestBudgetForecastMaterial.Price + (requestBudgetForecastMaterial.Price * tax.Value) / 100);
		//	asset.TotalAmountRon = (requestBudgetForecastMaterial.Price + (requestBudgetForecastMaterial.Price * tax.Value) / 100);
		//	asset.ReceptionPrice = requestBudgetForecastMaterial.PriceRon;

		//	_context.Update(asset);

		//	if (assetAdmMD != null)
		//	{
		//		assetAdmMD.ProjectId = budgetBase.ProjectId;
		//		assetAdmMD.AssetStateId = assetState.Id;
		//		_context.Update(assetAdmMD);

		//	}


		//	operation = new Model.AssetOp
		//	{
		//		AssetOpStateId = appState.Id,
		//		InvStateIdInitial = asset.InvStateId,
		//		InvStateIdFinal = asset.InvStateId,
		//		AssetStateIdInitial = asset.AssetStateId,
		//		AssetStateIdFinal = assetState.Id
		//	};

		//	operation.AccSystemId = 3;
		//	operation.AdministrationIdInitial = asset.AdministrationId;
		//	operation.AdministrationIdFinal = asset.AdministrationId;
		//	operation.AssetCategoryIdInitial = asset.AssetCategoryId;
		//	operation.AssetCategoryIdFinal = asset.AssetCategoryId;
		//	operation.AssetId = asset.Id;
		//	operation.CostCenterIdInitial = asset.CostCenterId;
		//	operation.CostCenterIdFinal = costCenter.Id;
		//	operation.CreatedAt = DateTime.Now;
		//	operation.CreatedBy = _context.UserId;
		//	operation.DepartmentIdInitial = asset.DepartmentId;
		//	operation.DepartmentIdFinal = asset.DepartmentId;
		//	operation.DocumentId = document.Id;
		//	operation.EmployeeIdInitial = asset.EmployeeId;
		//	//operation.EmployeeIdFinal = employee.Id;
		//	operation.IsDeleted = false;
		//	operation.ModifiedAt = DateTime.Now;
		//	operation.ModifiedBy = _context.UserId;
		//	operation.RoomIdInitial = asset.RoomId;
		//	operation.RoomIdFinal = asset.RoomId;
		//	operation.SrcConfAt = DateTime.Now;
		//	operation.SrcConfBy = _context.UserId;
		//	operation.AllowLabel = asset.AllowLabel != null ? (bool)asset.AllowLabel : false;
		//	operation.AssetTypeIdInitial = asset.AssetTypeId;
		//	operation.AssetTypeIdFinal = asset.AssetTypeId;
		//	operation.InvName = asset.Name;
		//	operation.Quantity = asset.Quantity;
		//	operation.SerialNumber = asset.SerialNumber;
		//	operation.AssetNatureIdInitial = asset.AssetNatureId;
		//	operation.AssetNatureIdFinal = asset.AssetNatureId;
		//	operation.BudgetManagerIdInitial = asset.BudgetManagerId;
		//	operation.BudgetManagerIdFinal = asset.BudgetManagerId;
		//	operation.DimensionIdInitial = asset.DimensionId;
		//	operation.DimensionIdFinal = asset.DimensionId;
		//	operation.ProjectIdInitial = asset.ProjectId;
		//	operation.ProjectIdFinal = asset.ProjectId;
		//	operation.IsMinus = false;
		//	operation.IsPlus = false;
		//	operation.CompanyId = asset.CompanyId;
		//	operation.InsuranceCategoryId = asset.InsuranceCategoryId;
		//	operation.InterCompanyId = asset.InterCompanyId;
		//	operation.UomId = asset.UomId;
		//	operation.TaxId = asset.TaxId;
		//	operation.ValueAdd = documentNumber;
		//	operation.Guid = guid;

		//	emailStatus = new Model.EmailStatus()
		//	{
		//		AppStateId = appState.Id,
		//		AssetId = asset.Id,
		//		AssetOp = operation,
		//		BudgetBaseId = asset.BudgetBaseId,
		//		CompanyId = asset.CompanyId,
		//		Completed = false,
		//		CostCenterIdFinal = costCenter.Id,
		//		CostCenterIdInitial = asset.CostCenterId,
		//		CreatedAt = DateTime.Now,
		//		CreatedBy = _context.UserId,
		//		DocumentNumber = documentNumber,
		//		DstEmployeeEmailSend = false,
		//		DstEmployeeValidateAt = null,
		//		DstEmployeeValidateBy = null,
		//		DstManagerEmailSend = false,
		//		DstManagerValidateAt = null,
		//		DstManagerValidateBy = null,
		//		EmailSend = false,
		//		EmailTypeId = emailType.Id,
		//		//EmployeeIdFinal = employee.Id,
		//		EmployeeIdInitial = asset.EmployeeId,
		//		ErrorId = null,
		//		Exported = false,
		//		FinalValidateAt = null,
		//		FinalValidateBy = null,
		//		Guid = guid,
		//		GuidAll = guidAll,
		//		Info = string.Empty,
		//		IsAccepted = false,
		//		IsDeleted = true,
		//		ModifiedAt = DateTime.Now,
		//		ModifiedBy = _context.UserId,
		//		NotCompletedSync = false,
		//		NotDstEmployeeSync = true,
		//		NotDstManagerSync = false,
		//		NotSrcEmployeeSync = false,
		//		NotSrcManagerSync = false,
		//		NotSync = false,
		//		OfferId = order.OfferId,
		//		OrderId = asset.OrderId,
		//		PartnerId = order.PartnerId,
		//		RequestId = asset.RequestId,
		//		//SameEmployee = asset.EmployeeId == employee.Id ? true : false,
		//		SameManager = false,
		//		Skip = false,
		//		SkipDstEmployee = false,
		//		SkipDstManager = false,
		//		SkipSrcEmployee = false,
		//		SkipSrcManager = false,
		//		SrcEmployeeEmailSend = false,
		//		SrcEmployeeValidateAt = DateTime.Now,
		//		SrcEmployeeValidateBy = _context.UserId,
		//		SrcManagerEmailSend = false,
		//		SrcManagerValidateAt = DateTime.Now,
		//		SrcManagerValidateBy = _context.UserId,
		//		StockId = asset.StockId,
		//		SyncCompletedErrorCount = 0,
		//		SyncDstEmployeeErrorCount = 0,
		//		SyncDstManagerErrorCount = 0,
		//		SyncErrorCount = 0,
		//		SyncSrcEmployeeErrorCount = 0,
		//		SyncSrcManagerErrorCount = 0

		//	};

		//	entityType.Name = documentNumber.ToString();

		//	_context.Add(emailStatus);
		//	_context.Add(operation);
		//	_context.Update(asset);
		//	_context.Update(entityType);
		//	_context.SaveChanges();

		//	var count = await _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToListAsync();
		//	var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
		//	var countOrd = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
		//	var countOrdMaterial = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();
		//	var countContract = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
		//	var countContractAmount = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();
		//	var countBudget = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
		//	var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
		//	var countOfferMaterials2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials2").ToList();
		//	var UpdateAllRequestBudgetForecastMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBudgetForecastMaterials").ToList();
		//	var UpdateAllRequestBFMaterialCostCenters = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBFMaterialCostCenters").ToList();

		//	// NEW //

		//	_context.SaveChanges();

		//	return new RequestResult { Success = true, Message = "", RequestId = 0 };
		//}

	}
}
