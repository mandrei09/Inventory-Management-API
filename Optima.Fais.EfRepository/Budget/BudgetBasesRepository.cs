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

namespace Optima.Fais.EfRepository
{
    public class BudgetBasesRepository : Repository<Model.BudgetBase>, IBudgetBasesRepository
    {

        public BudgetBasesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Name.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.BudgetBaseDetail> GetBuget(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.BudgetBase> budgetQuery = null;
            IQueryable<Model.BudgetBaseDetail> query = null;

            int? accMonthId = _context.Set<Model.AccMonth>().AsNoTracking().Where(a => a.IsActive == true).Select(a => a.Id).SingleOrDefault();
            budgetFilter.AccMonthId = 60;

            budgetQuery = _context.BudgetBases.AsNoTracking().AsQueryable();

            /*if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter) || a.Project.Name.Contains(budgetFilter.Filter) || a.Project.Code.Contains(budgetFilter.Filter)));
*/
            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Name.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)) || a.BudgetManager.Name.Contains(budgetFilter.Filter) || a.Employee.Email.Contains(budgetFilter.Filter) || a.Country.Name.Contains(budgetFilter.Filter) || a.Country.Code.Contains(budgetFilter.Filter) || a.Department.Name.Contains(budgetFilter.Filter) || a.Department.Code.Contains(budgetFilter.Filter) || a.AdmCenter.Name.Contains(budgetFilter.Filter) || a.Division.Name.Contains(budgetFilter.Filter) || a.Division.Code.Contains(budgetFilter.Filter) || a.ProjectType.Name.Contains(budgetFilter.Filter) || a.AssetType.Name.Contains(budgetFilter.Filter) || a.AssetType.Code.Contains(budgetFilter.Filter) || a.AppState.Name.Contains(budgetFilter.Filter) || a.Info.Contains(budgetFilter.Filter) || a.Project.Name.Contains(budgetFilter.Filter) || a.Project.Code.Contains(budgetFilter.Filter));

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
                    case "BudgetBase":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetBaseDetail { BudgetBase = budget });

            if ((budgetFilter.AccMonthId != null) && (budgetFilter.AccMonthId > 0))
            {
                query = query.Where(a => a.BudgetBase.AccMonthId == budgetFilter.AccMonthId);
			}
			else
			{
                query = query.Where(a => a.BudgetBase.AccMonthId == accMonthId);
            }

            if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.EmployeeId == id; }, budgetFilter.EmployeeIds));
            }

            if ((budgetFilter.BudgetManagerIds != null) && (budgetFilter.BudgetManagerIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.BudgetManagerId == id; }, budgetFilter.BudgetManagerIds));
            }

            if ((budgetFilter.ProjectIds != null) && (budgetFilter.ProjectIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.ProjectId == id; }, budgetFilter.ProjectIds));
            }

            if ((budgetFilter.ActivityIds != null) && (budgetFilter.ActivityIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.ActivityId == id; }, budgetFilter.ActivityIds));
            }

            if ((budgetFilter.CostCenterIds != null) && (budgetFilter.CostCenterIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.CostCenterId == id; }, budgetFilter.CostCenterIds));
            }

            if ((budgetFilter.DivisionIds != null) && (budgetFilter.DivisionIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.DivisionId == id; }, budgetFilter.DivisionIds));
            }

            if ((budgetFilter.DepartmentIds != null) && (budgetFilter.DepartmentIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.DepartmentId == id; }, budgetFilter.DepartmentIds));
            }

            if ((budgetFilter.AssetTypeIds != null) && (budgetFilter.AssetTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.AssetTypeId == id; }, budgetFilter.AssetTypeIds));
            }

			if ((budgetFilter.ProjectTypeIds != null) && (budgetFilter.ProjectTypeIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.ProjectTypeId == id; }, budgetFilter.ProjectTypeIds));
			}

			query = query.Where(a => a.BudgetBase.IsDeleted == false && a.BudgetBase.Validated == true);

            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();
            //catTotal.AssetCategoryDeskPhone = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 42 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryMonitor = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 72 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();
            //catTotal.AssetCategoryThinClient = _context.AssetAdmMDs.Include(a => a.Asset).Where(a => a.AssetCategoryId == 1035 && a.EmployeeId == 7228 && a.AccMonthId == budgetFilter.AccMonthId && a.Asset.IsDeleted == false && a.Asset.Validated == true).Count();

            if (sorting != null)
            {
               /* query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetBaseDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetBaseDetail>(sorting.Column));*/
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
        public IEnumerable<Model.BudgetBaseDetail> GetBugetUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.BudgetBase> budgetQuery = null;
            IQueryable<BudgetBaseDetail> query = null;

            budgetQuery = _context.BudgetBases.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Project.Code.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)));


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
                    case "BudgetBase":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetBaseDetail { BudgetBase = budget });

            if ((budgetFilter.CompanyIds != null) && (budgetFilter.CompanyIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.CompanyId == id; }, budgetFilter.CompanyIds));
            }

            if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.EmployeeId == id; }, budgetFilter.EmployeeIds));
            }

			if ((budgetFilter.AdmCenterIds != null) && (budgetFilter.AdmCenterIds.Count > 0))
			{
				query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.AdmCenterId == id; }, budgetFilter.AdmCenterIds));
			}

			if ((budgetFilter.AssetTypeIds != null) && (budgetFilter.AssetTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.AssetTypeId == id; }, budgetFilter.AssetTypeIds));
            }

            if ((budgetFilter.ProjectTypeIds != null) && (budgetFilter.ProjectTypeIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.ProjectTypeId == id; }, budgetFilter.ProjectTypeIds));
            }

            if ((budgetFilter.DivisionIds != null) && (budgetFilter.DivisionIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.DivisionId == id; }, budgetFilter.DivisionIds));
            }

            if ((budgetFilter.BudgetBaseIds != null) && (budgetFilter.BudgetBaseIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetBaseDetail, int?>((id) => { return a => a.BudgetBase.Id == id; }, budgetFilter.BudgetBaseIds));
            }

            query = query.Where(a => a.BudgetBase.IsDeleted == false && a.BudgetBase.Validated == true && a.BudgetBase.IsAccepted == true);
            query = query.Where(a => a.BudgetBase.BudgetMonthBase.FirstOrDefault().IsLast == true);


            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetBaseDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetBaseDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public IEnumerable<Model.BudgetBaseDetail> GetBugetFreezeUI(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.BudgetBase> budgetQuery = null;
            IQueryable<BudgetBaseDetail> query = null;

            budgetQuery = _context.BudgetBases.AsNoTracking().AsQueryable();

            if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.Project.Code.Contains(budgetFilter.Filter) || a.Code.Contains(budgetFilter.Filter)));


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
                    case "BudgetBase":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetBaseDetail { BudgetBase = budget });

            query = query.Where(a => a.BudgetBase.IsDeleted == false && a.BudgetBase.Validated == true && a.BudgetBase.IsAccepted == true && a.BudgetBase.IsFirst == true && a.BudgetBase.BudgetType.Code == "V1");


            depTotal = new AssetDepTotal();
            depTotal.Count = query.Count();
            //depTotal.ValueInv = budgetQuery.Sum(a => a.ValueIni);
            //depTotal.ValueRem = budgetQuery.Sum(a => a.ValueFin);

            catTotal = new AssetCategoryTotal();

            if (sorting != null)
            {
                query = sorting.Direction.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetBaseDetail>(sorting.Column))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Model.BudgetBaseDetail>(sorting.Column));
            }

            if (paging != null)
                query = query.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);

            var list = query.ToList();

            return list;
        }

        public Model.BudgetBase GetDetailsById(int id, string includes)
        {
            IQueryable<Model.BudgetBase> query = null;
            query = GetBudgetQuery(includes);

            return query.AsNoTracking().Where(a => a.Id == id)
                .Include(b => b.Company)
                .Include(b => b.Employee)
                .Include(b => b.Project)
                .Include(b => b.Country)
                .Include(b => b.AccMonth)
                .Include(b => b.Activity)
                .Include(b => b.Department)
                .Include(b => b.Division)
                .Include(b => b.CostCenter)
                .Include(b => b.AdmCenter)
                .Include(b => b.Region)
                .Include(b => b.ProjectType)
                .Include(b => b.AssetType)
                .Include(b => b.AppState)
                .Include(b => b.StartMonth)
                .Include(b => b.BudgetMonthBase).ThenInclude(b => b.BudgetManager)
                .Include(b => b.BudgetMonthBase).ThenInclude(b => b.BudgetType)
                .Include(b => b.BudgetForecast).ThenInclude(b => b.BudgetManager)
                .Include(b => b.BudgetForecast).ThenInclude(b => b.BudgetType)
                .SingleOrDefault();
        }

        private IQueryable<Model.BudgetBase> GetBudgetQuery(string includes)
        {
            IQueryable<Model.BudgetBase> query = null;
            query = _context.BudgetBases.AsNoTracking();

            return query;
        }

        public async Task<ImportBudgetResult> BudgetBaseImport(BudgetBaseImport budgetDto)
        {
            Model.BudgetBase budget = null;
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
            Model.BudgetType budgetTypeTotal = null;
            //Model.AccMonth accMonth = null;
            Model.AccMonth startAccMonth = null;
            Model.Department department = null;
            Model.Division division = null;
            Model.Uom uom = null;
            Model.Employee employee = null;

            Model.BudgetMonthBase budgetMonth1 = null;
            Model.BudgetMonthBase budgetMonth2 = null;
            Model.BudgetMonthBase budgetMonth3 = null;
            Model.BudgetMonthBase budgetMonth4 = null;
            Model.BudgetMonthBase budgetMonth5 = null;
            Model.BudgetMonthBase budgetMonth6 = null;
            Model.BudgetMonthBase budgetMonth7 = null;
            Model.BudgetMonthBase budgetMonth8 = null;
            Model.BudgetMonthBase budgetMonth9 = null;
            Model.BudgetMonthBase budgetMonth10 = null;
            Model.BudgetMonthBase budgetMonth11 = null;
            Model.BudgetMonthBase budgetMonth12 = null;

            // Model.BudgetForecast budgetForecast = null;
            Model.BudgetMonthBase budgetMonthBase = null;
            //Model.BudgetMonth budgetTotal = null;

            _context.UserId = budgetDto.UserId;

			inventory = await _context.Set<Model.Inventory>().Where(i => i.Active == true).FirstOrDefaultAsync();
			if (inventory == null) return new ImportBudgetResult { Success = false, Message = "Lipsa inventar activ", Id = 0 };
			//accMonth = await _context.Set<Model.AccMonth>().Where(i => i.Id == 60).FirstOrDefaultAsync();

			//budgetType = _context.Set<Model.BudgetType>().Where(i => i.Code == "V1" && i.IsDeleted == false).SingleOrDefault();

			//budgetManager = await _context.Set<Model.BudgetManager>().Where(i => i.Name == "2024" && i.IsDeleted == false).FirstOrDefaultAsync();

			uom = await _context.Set<Model.Uom>().Where(i => i.Code == "RON" && i.IsDeleted == false).FirstOrDefaultAsync();
			if (uom == null) return new ImportBudgetResult { Success = false, Message = "Lipsa moneda RON", Id = 0 };

			if (budgetType == null)
            {
                budgetType = new Model.BudgetType
                {
                    Code = "V1".Trim(),
                    Name = "042024".Trim(),
                    IsDeleted = false
                };
                _context.Set<Model.BudgetType>().Add(budgetType);
            }
            //budgetTypeTotal = _context.Set<Model.BudgetType>().Where(i => i.Code == "B" && i.IsDeleted == false).SingleOrDefault();

   //         if(budgetDto.StartMonth != "" && budgetDto.StartMonth != null)
			//{
   //             startMonth = _context.Set<Model.AccMonth>().Where(i => i.GetHashCode == 37).SingleOrDefault();
   //         }
            
            company = await _context.Set<Model.Company>().Where(c => c.Code == "RO10" && c.IsDeleted == false).FirstOrDefaultAsync();
			if (company == null) return new ImportBudgetResult { Success = false, Message = "Lipsa companie RO10", Id = 0 };
			//if (company == null)
			//{
			//    company = new Model.Company
			//    {
			//        Code = budgetDto.Company.Trim(),
			//        Name = budgetDto.Company.Trim(),
			//        IsDeleted = false
			//    };
			//    _context.Set<Model.Company>().Add(company);
			//}

			employee = await _context.Set<Model.Employee>().Where(c => c.Email == budgetDto.Employee).FirstOrDefaultAsync();
			if (employee == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa owner {budgetDto.Employee}", Id = 0 };
			//if (employee == null)
			//{
			//    employee = new Model.Employee
			//    {
			//        FirstName = budgetDto.Project,
			//        Name = budgetDto.Project.Trim(),
			//        IsDeleted = false
			//    };
			//    _context.Set<Model.Employee>().Add(employee);
			//}

			project = await _context.Set<Model.Project>().Where(c => c.Name == budgetDto.Project && c.IsDeleted == false).FirstOrDefaultAsync();

			if (project == null)
			{
				project = new Model.Project
				{
					Code = budgetDto.Project,
					Name = budgetDto.Project.Trim(),
					IsDeleted = false
				};
				_context.Set<Model.Project>().Add(project);
			}

			country = await _context.Set<Model.Country>().Where(c => c.Name == budgetDto.CountryName && c.IsDeleted == false).FirstOrDefaultAsync();
			if (country == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa country {budgetDto.CountryCode}", Id = 0 };
			//if (country == null)
			//{
			//    country = new Model.Country
			//    {
			//        Code = budgetDto.CountryCode.Trim(),
			//        Name = budgetDto.CountryName.Trim(),
			//        IsDeleted = false
			//    };
			//    _context.Set<Model.Country>().Add(country);
			//}



			activity = await _context.Set<Model.Activity>().Where(c => c.Name == budgetDto.Activity && c.IsDeleted == false).FirstOrDefaultAsync();

            if (activity == null)
            {
                activity = new Model.Activity
                {
                    Code = budgetDto.Activity.Trim(),
                    Name = budgetDto.Activity.Trim(),
                    IsDeleted = false
                };
                _context.Set<Model.Activity>().Add(activity);
            }

            department = await _context.Set<Model.Department>().Where(c => c.Code == budgetDto.DepartmentCode && c.IsDeleted == false).FirstOrDefaultAsync();
			if (department == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa B.U. {budgetDto.DepartmentCode}", Id = 0 };

			//if (department == null)
			//{
			//    department = new Model.Department
			//    {
			//        Code = budgetDto.DepartmentCode.Trim(),
			//        Name = budgetDto.DepartmentName.Trim(),
			//        IsDeleted = false
			//    };
			//    _context.Set<Model.Department>().Add(department);


			//}

			admCenter = await _context.Set<Model.AdmCenter>().Where(c => c.Name == budgetDto.AdmCenter && c.IsDeleted == false).FirstOrDefaultAsync();

			if (admCenter == null)
			{
				admCenter = new Model.AdmCenter
				{
					Code = budgetDto.AdmCenter,
					Name = budgetDto.AdmCenter,
					IsDeleted = true
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
					IsDeleted = true
				};
				_context.Set<Model.Region>().Add(region);
			}

			division = await _context.Set<Model.Division>().Where(c => c.Code == budgetDto.DivisionCode && c.DepartmentId == department.Id && c.IsDeleted == false).FirstOrDefaultAsync();
			if (division == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa departament {budgetDto.DivisionCode}", Id = 0 };
			//if (division == null)
			//{
			//    division = new Model.Division
			//    {
			//        Code = budgetDto.DivisionCode.Trim(),
			//        Name = budgetDto.DivisionName.Trim(),
			//        IsDeleted = false
			//    };
			//    _context.Set<Model.Division>().Add(division);


			//}


			projectType = await _context.Set<Model.ProjectType>().Where(c => c.Code == budgetDto.ProjectTypeCode && c.IsDeleted == false).FirstOrDefaultAsync();
			if (projectType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa tip {budgetDto.ProjectTypeCode}", Id = 0 };
			//if (projectType == null)
			//{
			//    projectType = new Model.ProjectType
			//    {
			//        Code = budgetDto.ProjectTypeCode.Trim(),
			//        Name = budgetDto.ProjectTypeName.Trim(),
			//        IsDeleted = false
			//    };
			//    _context.Set<Model.ProjectType>().Add(projectType);
			//}


			assetType = await _context.Set<Model.AssetType>().Where(c => c.Code == budgetDto.AssetTypeCode && c.IsDeleted == false).FirstOrDefaultAsync();
			if (assetType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa type {budgetDto.AssetTypeCode}", Id = 0 };
			//if (assetType == null)
			//{
			//    assetType = new Model.AssetType
			//    {
			//        Code = budgetDto.AssetTypeCode.Trim(),
			//        Name = budgetDto.AssetTypeName.Trim(),
			//        IsDeleted = false
			//    };
			//    _context.Set<Model.AssetType>().Add(assetType);
			//}


			appState = await _context.Set<Model.AppState>().Where(c => c.Name == budgetDto.AppState && c.IsDeleted == false).FirstOrDefaultAsync();


            if (appState == null)
            {
                appState = new Model.AppState
                {
                    Code = budgetDto.AppState.Trim(),
                    Name = budgetDto.AppState.Trim(),
                    IsDeleted = false
                };
                _context.Set<Model.AppState>().Add(appState);
            }

			entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGETBASE").FirstOrDefaultAsync();
			if (entityType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa entityType", Id = 0 };
			var newBudgetCode = string.Empty;

			var lastCode = int.Parse(entityType.Name);

            string currentYear = DateTime.Now.Year.ToString();
            string code = "";
            int limit = 5 - lastCode.ToString().Length;

            for (int i = 0; i < limit; i++) code += "0";

            newBudgetCode = "B" + currentYear + code + entityType.Name;

			documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").FirstOrDefaultAsync();
			if (documentType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa documentType", Id = 0 };

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

            decimal valueMonthSum =
                budgetDto.ValueMonth1 + budgetDto.ValueMonth2 + budgetDto.ValueMonth3 + budgetDto.ValueMonth4 + budgetDto.ValueMonth5 + budgetDto.ValueMonth6 +
                budgetDto.ValueMonth7 + budgetDto.ValueMonth8 + budgetDto.ValueMonth9 + budgetDto.ValueMonth10 + budgetDto.ValueMonth11 + budgetDto.ValueMonth12;

            budget = new Model.BudgetBase()
			{
				AccMonthId = inventory.AccMonthBudgetId,
				EmployeeId = employee.Id,
				Project = project,
				Country = country,
				Activity = activity,
				Department = department,
				AdmCenter = admCenter,
				Region = region,
				Division = division,
				ProjectType = projectType,
				Info = budgetDto.Info,
				AssetType = assetType,
				AppState = appState,
				StartMonth = null,
				DepPeriod = budgetDto.DepPeriod,
				DepPeriodRem = budgetDto.DepPeriodRem,
				Code = newBudgetCode,
				Company = company,
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
				Uom = uom,
				//BudgetForecast = budgetForecast,
				//BudgetMonthBase = budgetMonthBase,
				BudgetType = budgetType,
				BudgetManagerId = inventory.BudgetManagerId,
				IsFirst = true

			};
			_context.Add(budget);

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

                startAccMonth = await _context.Set<Model.AccMonth>().Where(a => a.Month == month && a.Year == 2023).FirstOrDefaultAsync();
			}

			budgetMonthBase = new Model.BudgetMonthBase()
			{
				AccMonthId = inventory.AccMonthBudgetId,
				BudgetBaseId = budget.Id,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				BudgetType = budgetType,
				IsFirst = true,
				IsLast = false,
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

			budget.StartMonth = startAccMonth;

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            _context.SaveChanges();

            return new ImportBudgetResult { Success = true, Message = "", Id = budget.Id };
        }

        public int CreateBudget(BudgetSave budgetDto)
        {
            Model.BudgetBase budget = null;
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
            // Model.Inventory inventory = null;
            Model.BudgetManager budgetManager = null;
            Model.BudgetType budgetType = null;
            Model.BudgetType lastBudgetType = null;
            Model.BudgetType budgetTypeTotal = null;
            Model.AccMonth accMonth = null;
            Model.AccMonth startAccMonth = null;
            Model.Department department = null;
            Model.Division division = null;
            Model.Uom uom = null;
            Model.Employee employee = null;

            Model.BudgetMonthBase budgetMonth1 = null;
            Model.BudgetMonthBase budgetMonth2 = null;
            Model.BudgetMonthBase budgetMonth3 = null;
            Model.BudgetMonthBase budgetMonth4 = null;
            Model.BudgetMonthBase budgetMonth5 = null;
            Model.BudgetMonthBase budgetMonth6 = null;
            Model.BudgetMonthBase budgetMonth7 = null;
            Model.BudgetMonthBase budgetMonth8 = null;
            Model.BudgetMonthBase budgetMonth9 = null;
            Model.BudgetMonthBase budgetMonth10 = null;
            Model.BudgetMonthBase budgetMonth11 = null;
            Model.BudgetMonthBase budgetMonth12 = null;

            Model.BudgetForecast budgetForecast = null;
            Model.BudgetMonthBase budgetMonthBase = null;
            //Model.BudgetMonth budgetTotal = null;

            _context.UserId = budgetDto.UserId;
            int lastBgtType = 0;

            accMonth = _context.Set<Model.AccMonth>().Where(i => i.IsActive == true).SingleOrDefault();
            lastBudgetType = _context.Set<Model.BudgetType>().Where(i => i.Name == ((accMonth.Month.ToString().Length == 1 ? "0" + accMonth.Month.ToString() : accMonth.Month.ToString()) + (accMonth.Year.ToString()))).LastOrDefault();

            budgetManager = _context.Set<Model.BudgetManager>().Where(i => i.Name == accMonth.Year.ToString() && i.IsDeleted == false).SingleOrDefault();

            uom = _context.Set<Model.Uom>().Where(i => i.Code == "RON" && i.IsDeleted == false).SingleOrDefault();

            var ddd = lastBudgetType.Code.Substring(1, 1);

            lastBgtType = int.Parse(ddd);
            lastBgtType++;

            budgetType = new Model.BudgetType
            {
                Code = "V" + lastBgtType,
                Name = ((accMonth.Month.ToString().Length == 1 ? "0" + accMonth.Month.ToString() : accMonth.Month.ToString()) + (accMonth.Year.ToString())),
                IsDeleted = false
            };

            _context.Set<Model.BudgetType>().Add(budgetType);

            company = _context.Set<Model.Company>().Where(c => c.Code == "RO10" && c.IsDeleted == false).Single();
            employee = _context.Set<Model.Employee>().Where(c => c.Id == budgetDto.EmployeeId).Single();

          

            country = _context.Set<Model.Country>().Where(c => c.Id == budgetDto.CountryId).Single();
            activity = _context.Set<Model.Activity>().Where(c => c.Id == budgetDto.ActivityId).Single();
            department = _context.Set<Model.Department>().Where(c => c.Id == budgetDto.DepartmentId).Single();
            admCenter = _context.Set<Model.AdmCenter>().Where(c => c.Id == budgetDto.AdmCenterId).Single();
            //region = _context.Set<Model.Region>().Where(c => c.Id == budgetDto.RegionId).Single();
            division = _context.Set<Model.Division>().Where(c => c.Id == budgetDto.DivisionId).Single();
            projectType = _context.Set<Model.ProjectType>().Where(c => c.Id == budgetDto.ProjectTypeId).Single();
            assetType = _context.Set<Model.AssetType>().Where(c => c.Id == budgetDto.AssetTypeId).Single();
            appState = _context.Set<Model.AppState>().Where(c => c.Id == budgetDto.AppStateId).Single();
            startAccMonth = _context.Set<Model.AccMonth>().Where(i => i.Id == budgetDto.StartAccMonthId).SingleOrDefault();

            if (budgetDto.ProjectId != null)
            {
                project = _context.Set<Model.Project>().Where(c => c.Id == budgetDto.ProjectId).FirstOrDefault();
            }
            else
            {

				project = _context.Set<Model.Project>().Where(p => p.Code == (country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code) && p.IsDeleted == false).FirstOrDefault();

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


            }

            entityType = _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefault();
            var newBudgetCode = string.Empty;

            var lastCode = int.Parse(entityType.Name);

            if (lastCode.ToString().Length == 1)
            {
                newBudgetCode = "BGT000000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 2)
            {
                newBudgetCode = "BGT00000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 3)
            {
                newBudgetCode = "BGT0000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 4)
            {
                newBudgetCode = "BGT000" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 5)
            {
                newBudgetCode = "BGT00" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 6)
            {
                newBudgetCode = "BGT0" + entityType.Name;
            }
            else if (lastCode.ToString().Length == 7)
            {
                newBudgetCode = "BGT" + entityType.Name;
            }

            documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").SingleOrDefault();

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

            budget = new Model.BudgetBase()
            {
                AccMonthId = accMonth.Id,
                EmployeeId = employee.Id,
                Project = project,
                Country = country,
                Activity = activity,
                Department = department,
                AdmCenter = admCenter,
                Region = region,
                Division = division,
                ProjectType = projectType,
                Info = budgetDto.Info,
                AssetType = assetType,
                AppState = appState,
                StartMonth = null,
                DepPeriod = budgetDto.DepPeriod,
                DepPeriodRem = budgetDto.DepPeriodRem,
                Code = newBudgetCode,
                Company = company,
                CreatedAt = DateTime.Now,
                CreatedBy = budgetDto.UserId,
                IsAccepted = true,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = budgetDto.UserId,
                Name = newBudgetCode,
                UserId = budgetDto.UserId,
                Validated = true,
                ValueFin = 0,
                ValueIni = 0,
                Total = 0,
                Uom = uom,
                //BudgetForecast = budgetForecast,
                //BudgetMonthBase = budgetMonthBase,
                BudgetType = budgetType,
                BudgetManager = budgetManager

            };
            _context.Add(budget);


            var sumMonth1 = 0;
            var sumMonth2 = budgetDto.April;
            var sumMonth3 = budgetDto.April + budgetDto.May;
            var sumMonth4 = budgetDto.April + budgetDto.May + budgetDto.June;
            var sumMonth5 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July;
            var sumMonth6 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July + budgetDto.August;
            var sumMonth7 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July + budgetDto.August + budgetDto.September;
            var sumMonth8 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July + budgetDto.August + budgetDto.September + budgetDto.Octomber;
            var sumMonth9 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July + budgetDto.August + budgetDto.September + budgetDto.Octomber + budgetDto.November;
            var sumMonth10 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July + budgetDto.August + budgetDto.September + budgetDto.Octomber + budgetDto.November + budgetDto.December;
            var sumMonth11 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July + budgetDto.August + budgetDto.September + budgetDto.Octomber + budgetDto.November + budgetDto.December + budgetDto.January;
            var sumMonth12 = budgetDto.April + budgetDto.May + budgetDto.June + budgetDto.July + budgetDto.August + budgetDto.September + budgetDto.Octomber + budgetDto.November + budgetDto.December + budgetDto.January + budgetDto.February;

            var startMonth = 0;

            if(startAccMonth != null)
			{
                startMonth = startAccMonth.Month;
            }
            int month = 0;

            if (startMonth > 0 )
            {
                DateTime date = Convert.ToDateTime(startMonth, CultureInfo.InvariantCulture);

                month = date.Month;

                if (month == 4)
                {
                    month = 1;

                }
                else if (month == 5)
                {
                    month = 2;
                }
                else if (month == 6)
                {
                    month = 3;
                }
                else if (month == 7)
                {
                    month = 4;
                }
                else if (month == 8)
                {
                    month = 5;
                }
                else if (month == 9)
                {
                    month = 6;
                }
                else if (month == 10)
                {
                    month = 7;
                }
                else if (month == 11)
                {
                    month = 8;
                }
                else if (month == 12)
                {
                    month = 9;
                }
                else if (month == 1)
                {
                    month = 10;
                }
                else if (month == 2)
                {
                    month = 11;
                }
                else if (month == 3)
                {
                    month = 12;
                }

                startAccMonth = _context.Set<Model.AccMonth>().Where(a => a.Month == month && a.Year == 2022).Single();
            }

            budgetMonthBase = new Model.BudgetMonthBase()
            {
                BudgetBaseId = budget.Id,
                BudgetManagerId = budgetManager.Id,
                BudgetType = budgetType,
                IsFirst = true,
                IsLast = true,
                April = budgetDto.April,
                May = budgetDto.May,
                June = budgetDto.June,
                July = budgetDto.July,
                August = budgetDto.August,
                September = budgetDto.September,
                Octomber = budgetDto.Octomber,
                November = budgetDto.November,
                December = budgetDto.December,
                January = budgetDto.January,
                February = budgetDto.February,
                March = budgetDto.March
            };

            _context.Add(budgetMonthBase);


            budgetForecast = new Model.BudgetForecast()
            {
                BudgetBaseId = budget.Id,
                BudgetManagerId = budgetManager.Id,
                BudgetType = budgetType,
                IsFirst = true,
                IsLast = true,
                April =
                    budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 ? budgetDto.April :
                    budgetDto.DepPeriodRem > 0 && sumMonth1 > 0 ? sumMonth1 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth1 > 0 ? sumMonth1 / budgetDto.DepPeriod : 0,
                May = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 ? budgetDto.May :
                    budgetDto.DepPeriodRem > 0 && sumMonth2 > 0 ? sumMonth2 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth2 > 0 ? sumMonth2 / budgetDto.DepPeriod : 0,
                June = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.June :
                    budgetDto.DepPeriodRem > 0 && sumMonth3 > 0 ? sumMonth3 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth3 > 0 ? sumMonth3 / budgetDto.DepPeriod : 0,
                July = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.July :
                    budgetDto.DepPeriodRem > 0 && sumMonth4 > 0 ? sumMonth4 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth4 > 0 ? sumMonth4 / budgetDto.DepPeriod : 0,
                August = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.August :
                    budgetDto.DepPeriodRem > 0 && sumMonth5 > 0 ? sumMonth5 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth5 > 0 ? sumMonth5 / budgetDto.DepPeriod : 0,
                September = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.September :
                    budgetDto.DepPeriodRem > 0 && sumMonth6 > 0 ? sumMonth6 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth6 > 0 ? sumMonth6 / budgetDto.DepPeriod : 0,
                Octomber = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.Octomber :
                    budgetDto.DepPeriodRem > 0 && sumMonth7 > 0 ? sumMonth7 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth7 > 0 ? sumMonth7 / budgetDto.DepPeriod : 0,
                November = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.November :
                    budgetDto.DepPeriodRem > 0 && sumMonth8 > 0 ? sumMonth8 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth8 > 0 ? sumMonth8 / budgetDto.DepPeriod : 0,
                December = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.December :
                    budgetDto.DepPeriodRem > 0 && sumMonth9 > 0 ? sumMonth9 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth9 > 0 ? sumMonth9 / budgetDto.DepPeriod : 0,
                January = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.January :
                    budgetDto.DepPeriodRem > 0 && sumMonth10 > 0 ? sumMonth10 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth10 > 0 ? sumMonth10 / budgetDto.DepPeriod : 0,
                February = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.February :
                    budgetDto.DepPeriodRem > 0 && sumMonth11 > 0 ? sumMonth11 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth11 > 0 ? sumMonth11 / budgetDto.DepPeriod : 0,
                March = budgetDto.ValueRem > 0 && budgetDto.DepPeriod > 0 ? budgetDto.ValueRem / budgetDto.DepPeriod :
                    budgetDto.DepPeriodRem == 1 || budgetDto.DepPeriod == 1 ? budgetDto.March :
                    budgetDto.DepPeriodRem > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriodRem :
                    budgetDto.DepPeriod > 0 && sumMonth12 > 0 ? sumMonth12 / budgetDto.DepPeriod : 0
            };

            _context.Add(budgetForecast);


            if (month > 0)
            {
                if (month == 2)
                {
                    budgetForecast.January = 0;

                }
                else if (month == 3)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;

                }
                else if (month == 4)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;

                }
                else if (month == 5)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;

                }
                else if (month == 6)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;
                    budgetForecast.May = 0;

                }
                else if (month == 7)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;
                    budgetForecast.May = 0;
                    budgetForecast.June = 0;

                }
                else if (month == 8)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;
                    budgetForecast.May = 0;
                    budgetForecast.June = 0;
                    budgetForecast.July = 0;

                }
                else if (month == 9)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;
                    budgetForecast.May = 0;
                    budgetForecast.June = 0;
                    budgetForecast.July = 0;
                    budgetForecast.August = 0;

                }
                else if (month == 10)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;
                    budgetForecast.May = 0;
                    budgetForecast.June = 0;
                    budgetForecast.July = 0;
                    budgetForecast.August = 0;
                    budgetForecast.September = 0;

                }
                else if (month == 11)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;
                    budgetForecast.May = 0;
                    budgetForecast.June = 0;
                    budgetForecast.July = 0;
                    budgetForecast.August = 0;
                    budgetForecast.September = 0;
                    budgetForecast.Octomber = 0;

                }
                else if (month == 12)
                {
                    budgetForecast.January = 0;
                    budgetForecast.February = 0;
                    budgetForecast.March = 0;
                    budgetForecast.April = 0;
                    budgetForecast.May = 0;
                    budgetForecast.June = 0;
                    budgetForecast.July = 0;
                    budgetForecast.August = 0;
                    budgetForecast.September = 0;
                    budgetForecast.Octomber = 0;
                    budgetForecast.November = 0;

                }

            }

            budget.StartMonth = startAccMonth;

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            _context.SaveChanges();

            return budget.Id;
        }

        public async Task<ImportBudgetResult> CreateRequestBudget(BudgetAddSave budgetDto)
        {
            Model.BudgetBase budgetBase = null;
            // Model.BudgetOp budgetOp = null;
            //Model.Document document = null;
            //Model.DocumentType documentType = null;
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
            Model.BudgetManager budgetManager = null;
            //Model.BudgetType budgetType = null;
            Model.BudgetType lastBudgetType = null;
            Model.BudgetType budgetTypeTotal = null;
            Model.AccMonth startAccMonth = null;
            Model.Department department = null;
            Model.Division division = null;
            Model.Uom uom = null;
            Model.Employee employee = null;
            Model.Request request = null;

            Model.BudgetMonthBase budgetMonth1 = null;
            Model.BudgetMonthBase budgetMonth2 = null;
            Model.BudgetMonthBase budgetMonth3 = null;
            Model.BudgetMonthBase budgetMonth4 = null;
            Model.BudgetMonthBase budgetMonth5 = null;
            Model.BudgetMonthBase budgetMonth6 = null;
            Model.BudgetMonthBase budgetMonth7 = null;
            Model.BudgetMonthBase budgetMonth8 = null;
            Model.BudgetMonthBase budgetMonth9 = null;
            Model.BudgetMonthBase budgetMonth10 = null;
            Model.BudgetMonthBase budgetMonth11 = null;
            Model.BudgetMonthBase budgetMonth12 = null;

            Model.BudgetForecast budgetForecast = null;
            Model.BudgetMonthBase budgetMonthBase = null;
            //Model.BudgetMonth budgetTotal = null;

            Model.RequestBudgetForecast requestBudgetForecast = null;


			_context.UserId = budgetDto.UserId;
            int lastBgtType = 0;
            string ddd = string.Empty;

			var guid = Guid.NewGuid();

			inventory = await _context.Set<Model.Inventory>().Include(a => a.AccMonth).Where(i => i.Active == true).AsNoTracking().SingleAsync();
			if (inventory == null) return new ImportBudgetResult { Success = false, Message = "Lipsa inventar activ", Id = 0 };
			
            //lastBudgetType = _context.Set<Model.BudgetType>().Where(i => i.Name == ((accMonth.Month.ToString().Length == 1 ? "0" + accMonth.Month.ToString() : accMonth.Month.ToString()) + (accMonth.Year.ToString()))).LastOrDefault();

            budgetManager = _context.Set<Model.BudgetManager>().Where(i => i.Id == inventory.BudgetManagerId).SingleOrDefault();

            uom = _context.Set<Model.Uom>().Where(i => i.Code == "RON" && i.IsDeleted == false).SingleOrDefault();


			//         if(lastBudgetType != null)
			//{
			//             ddd = lastBudgetType.Code.Substring(1, 1);

			//             lastBgtType = int.Parse(ddd);
			//             lastBgtType++;

			//             budgetType = new Model.BudgetType
			//             {
			//                 Code = "V" + lastBgtType,
			//                 Name = ((accMonth.Month.ToString().Length == 1 ? "0" + accMonth.Month.ToString() : accMonth.Month.ToString()) + (accMonth.Year.ToString())),
			//                 IsDeleted = false
			//             };

			//             _context.Set<Model.BudgetType>().Add(budgetType);
			//}
			//else
			//{
			//             budgetType = _context.Set<Model.BudgetType>().Where(i => i.Code == "V2" && i.IsDeleted == false).LastOrDefault();
			//         }

			lastBudgetType = await _context.Set<Model.BudgetForecast>()
				.Where(i => i.IsDeleted == false && i.IsLast == true)
                .Select(a => a.BudgetType)
				.AsNoTracking()
				.LastOrDefaultAsync();


			company = await _context.Set<Model.Company>().Where(c => c.Code == "RO10" && c.IsDeleted == false).SingleOrDefaultAsync();
			if (company == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa companie", Id = 0 };
			employee = await _context.Set<Model.Employee>().Where(c => c.Id == budgetDto.EmployeeId).SingleOrDefaultAsync();
			if (employee == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa owner", Id = 0 };
			country = await _context.Set<Model.Country>().Where(c => c.Id == budgetDto.CountryId).SingleOrDefaultAsync();
			if (country == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa tara", Id = 0 };
			activity = await _context.Set<Model.Activity>().Where(c => c.Id == budgetDto.ActivityId).SingleOrDefaultAsync();
			if (activity == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa activitate", Id = 0 };
			department = await _context.Set<Model.Department>().Where(c => c.Id == budgetDto.DepartmentId).SingleOrDefaultAsync();
			if (department == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa departament", Id = 0 };
			admCenter = await _context.Set<Model.AdmCenter>().Where(c => c.Id == budgetDto.AdmCenterId).SingleOrDefaultAsync();
			if (admCenter == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa PC", Id = 0 };
			region = await _context.Set<Model.Region>().Where(c => c.Id == budgetDto.RegionId).SingleOrDefaultAsync();
			if (region == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa PC DET", Id = 0 };
			division = await _context.Set<Model.Division>().Where(c => c.Id == budgetDto.DivisionId).SingleOrDefaultAsync();
			if (division == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa divizie", Id = 0 };
			projectType = await _context.Set<Model.ProjectType>().Where(c => c.Id == budgetDto.ProjectTypeId).SingleOrDefaultAsync();
			if (projectType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa tip proiect", Id = 0 };
			assetType = await _context.Set<Model.AssetType>().Where(c => c.Id == budgetDto.AssetTypeId).SingleOrDefaultAsync();
			if (assetType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa cost type", Id = 0 };
			appState = await _context.Set<Model.AppState>().Where(c => c.Id == budgetDto.AppStateId).SingleOrDefaultAsync();
			if (appState == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa stare", Id = 0 };
			startAccMonth = await _context.Set<Model.AccMonth>().Where(i => i.Id == budgetDto.StartAccMonthId).SingleOrDefaultAsync();

            request = await _context.Set<Model.Request>().Where(i => i.Id == budgetDto.RequestId).SingleOrDefaultAsync();
			if (request == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa PR selectat", Id = 0 };

			if (budgetDto.ProjectId != null)
            {
                project = await _context.Set<Model.Project>().Where(c => c.Id == budgetDto.ProjectId).FirstOrDefaultAsync();
            }
            else
            {

				project = await _context.Set<Model.Project>().Where(p => p.Code == (country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code) && p.IsDeleted == false).SingleOrDefaultAsync();

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
			}

            entityType = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefaultAsync();
			if (entityType == null) return new ImportBudgetResult { Success = false, Message = $"Lipsa entitate", Id = 0 };

			var newBudgetCode = string.Empty;

            var lastCode = int.Parse(entityType.Name);

			if (lastCode.ToString().Length == 1)
			{
				newBudgetCode = "B20240000" + entityType.Name;
			}
			else if (lastCode.ToString().Length == 2)
			{
				newBudgetCode = "B2024000" + entityType.Name;
			}
			else if (lastCode.ToString().Length == 3)
			{
				newBudgetCode = "B202400" + entityType.Name;
			}
			else if (lastCode.ToString().Length == 4)
			{
				newBudgetCode = "B20240" + entityType.Name;
			}
			else if (lastCode.ToString().Length == 5)
			{
				newBudgetCode = "B2024" + entityType.Name;
			}

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

			budgetBase = new Model.BudgetBase()
            {
                AccMonthId = inventory.AccMonthId.Value,
                EmployeeId = employee.Id,
                Project = project,
                Country = country,
                Activity = activity,
                Department = department,
                AdmCenter = admCenter,
                Region = region,
                Division = division,
                ProjectType = projectType,
                Info = budgetDto.Info,
                AssetType = assetType,
                AppState = appState,
                StartMonth = null,
                DepPeriod = budgetDto.DepPeriod,
                DepPeriodRem = budgetDto.DepPeriodRem,
                Code = newBudgetCode,
                Company = company,
                CreatedAt = DateTime.Now,
                CreatedBy = budgetDto.UserId,
                IsAccepted = true,
                IsDeleted = false,
                ModifiedAt = DateTime.Now,
                ModifiedBy = budgetDto.UserId,
                Name = newBudgetCode,
                UserId = budgetDto.UserId,
                Validated = true,
                ValueFin = (budgetDto.AprilForecastNew +
                budgetDto.MayForecastNew +
                budgetDto.JuneForecastNew +
                budgetDto.JulyForecastNew +
                budgetDto.AugustForecastNew +
                budgetDto.SeptemberForecastNew +
                budgetDto.OctomberForecastNew +
                budgetDto.NovemberForecastNew +
                budgetDto.DecemberForecastNew +
                budgetDto.JanuaryForecastNew +
                budgetDto.FebruaryForecastNew +
                budgetDto.MarchForecastNew),
                ValueIni = (budgetDto.AprilForecastNew +
                budgetDto.MayForecastNew +
                budgetDto.JuneForecastNew +
                budgetDto.JulyForecastNew +
                budgetDto.AugustForecastNew +
                budgetDto.SeptemberForecastNew +
                budgetDto.OctomberForecastNew +
                budgetDto.NovemberForecastNew +
                budgetDto.DecemberForecastNew +
                budgetDto.JanuaryForecastNew +
                budgetDto.FebruaryForecastNew +
                budgetDto.MarchForecastNew),
                ValueRem = (budgetDto.AprilForecastNew +
                budgetDto.MayForecastNew +
                budgetDto.JuneForecastNew +
                budgetDto.JulyForecastNew +
                budgetDto.AugustForecastNew +
                budgetDto.SeptemberForecastNew +
                budgetDto.OctomberForecastNew +
                budgetDto.NovemberForecastNew +
                budgetDto.DecemberForecastNew +
                budgetDto.JanuaryForecastNew +
                budgetDto.FebruaryForecastNew +
                budgetDto.MarchForecastNew),
                Total = (budgetDto.AprilForecastNew +
                budgetDto.MayForecastNew +
                budgetDto.JuneForecastNew +
                budgetDto.JulyForecastNew +
                budgetDto.AugustForecastNew +
                budgetDto.SeptemberForecastNew +
                budgetDto.OctomberForecastNew +
                budgetDto.NovemberForecastNew +
                budgetDto.DecemberForecastNew +
                budgetDto.JanuaryForecastNew +
                budgetDto.FebruaryForecastNew +
                budgetDto.MarchForecastNew),
                Uom = uom,
                //BudgetForecast = budgetForecast,
                //BudgetMonthBase = budgetMonthBase,
                BudgetTypeId = lastBudgetType.Id,
                BudgetManager = budgetManager,
                IsFirst = false,
                IsLast = true,

            };
            _context.Add(budgetBase);


            var startMonth = 0;

            if (startAccMonth != null)
            {
                startMonth = startAccMonth.Month;
            }
            int month = 0;

            //if (startMonth > 0)
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

            //    startAccMonth = _context.Set<Model.AccMonth>().Where(a => a.Month == month && a.Year == 2023).Single();
            //}

            budgetMonthBase = new Model.BudgetMonthBase()
            {
                BudgetBaseId = budgetBase.Id,
                BudgetManagerId = budgetManager.Id,
                BudgetTypeId = lastBudgetType.Id,
                IsFirst = false,
                IsLast = true,
                April = budgetDto.AprilForecastNew,
                May = budgetDto.MayForecastNew,
                June = budgetDto.JuneForecastNew,
                July = budgetDto.JulyForecastNew,
                August = budgetDto.AugustForecastNew,
                September = budgetDto.SeptemberForecastNew,
                Octomber = budgetDto.OctomberForecastNew,
                November = budgetDto.NovemberForecastNew,
                December = budgetDto.DecemberForecastNew,
                January = budgetDto.JanuaryForecastNew,
                February = budgetDto.FebruaryForecastNew,
                March = budgetDto.MarchForecastNew,
                Total = (budgetDto.AprilForecastNew + 
                budgetDto.MayForecastNew +
                budgetDto.JuneForecastNew +
                budgetDto.JulyForecastNew +
                budgetDto.AugustForecastNew +
                budgetDto.SeptemberForecastNew +
                budgetDto.OctomberForecastNew +
                budgetDto.NovemberForecastNew +
                budgetDto.DecemberForecastNew +
                budgetDto.JanuaryForecastNew +
                budgetDto.FebruaryForecastNew +
                budgetDto.MarchForecastNew),
                AccMonthId = inventory.AccMonthId
            };

            _context.Add(budgetMonthBase);


            budgetForecast = new Model.BudgetForecast()
            {
                BudgetBase = budgetBase,
                BudgetManagerId = budgetManager.Id,
                BudgetTypeId = lastBudgetType.Id,
                IsFirst = false,
                IsLast = true,
                April = budgetDto.AprilForecastNew,
                May = budgetDto.MayForecastNew,
                June = budgetDto.JuneForecastNew,
                July = budgetDto.JulyForecastNew,
                August = budgetDto.AugustForecastNew,
                September = budgetDto.SeptemberForecastNew,
                Octomber = budgetDto.OctomberForecastNew,
                November = budgetDto.NovemberForecastNew,
                December = budgetDto.DecemberForecastNew,
                January = budgetDto.JanuaryForecastNew,
                February = budgetDto.FebruaryForecastNew,
                March = budgetDto.MarchForecastNew,
                Total = (budgetDto.AprilForecastNew +
                budgetDto.MayForecastNew +
                budgetDto.JuneForecastNew +
                budgetDto.JulyForecastNew +
                budgetDto.AugustForecastNew +
                budgetDto.SeptemberForecastNew +
                budgetDto.OctomberForecastNew +
                budgetDto.NovemberForecastNew +
                budgetDto.DecemberForecastNew +
                budgetDto.JanuaryForecastNew +
                budgetDto.FebruaryForecastNew +
                budgetDto.MarchForecastNew),
                AccMonthId = inventory.AccMonthId,
                ValueAsset = 0,
                ValueOrder = 0,
                TotalRem = (budgetDto.AprilForecastNew +
                budgetDto.MayForecastNew +
                budgetDto.JuneForecastNew +
                budgetDto.JulyForecastNew +
                budgetDto.AugustForecastNew +
                budgetDto.SeptemberForecastNew +
                budgetDto.OctomberForecastNew +
                budgetDto.NovemberForecastNew +
                budgetDto.DecemberForecastNew +
                budgetDto.JanuaryForecastNew +
                budgetDto.FebruaryForecastNew +
                budgetDto.MarchForecastNew)
            };

            _context.Add(budgetForecast);

            budgetBase.StartMonth = startAccMonth;

            entityType.Name = (int.Parse(entityType.Name) + 1).ToString();
            _context.Update(entityType);

            if(request != null)
			{
                request.Project = project;
                _context.Update(request);
            }

			requestBudgetForecast = new Model.RequestBudgetForecast()
			{
				RequestId = request.Id,
				BudgetForecastId = budgetForecast.Id,
				AccMonthId = inventory.AccMonthId.Value,
				BudgetManagerId = inventory.BudgetManagerId.Value,
				Guid = guid
			};

			_context.Add(requestBudgetForecast);

			_context.SaveChanges();

			return new ImportBudgetResult { Success = true, Message = budgetBase.Code, Id = budgetForecast.Id };
		}

        public int UpdateBudget(BudgetSave budgetDto)
        {
            Model.BudgetBase budgetBase = null;
            Model.BudgetBaseOp budgetBaseOp = null;
            Model.Document document = null;
            Model.DocumentType documentType = null;
            Model.BudgetType budgetType = null;
            Model.BudgetForecast budgetForecast = null;
            Model.BudgetManager budgetManager = null;
            Model.AccMonth accMonth = null;

            accMonth = _context.Set<Model.AccMonth>().Where(i => i.IsActive == true).SingleOrDefault();
            budgetManager = _context.Set<Model.BudgetManager>().Where(i => i.Name == accMonth.Year.ToString() && i.IsDeleted == false).SingleOrDefault();

            documentType = _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "BUDGET_BASE_CHANGE").SingleOrDefault();

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
                budgetForecast = _context.Set<Model.BudgetForecast>().Where(a => a.Id == budgetDto.BudgetForecastId).Single();

                // budgetForecast.BudgetBaseId = budgetDto.BudgetBaseNewId;

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
                budgetForecast.Total = budgetDto.AugustForecastNew + budgetDto.MayForecastNew + budgetDto.JuneForecastNew + budgetDto.JulyForecastNew + budgetDto.AugustForecastNew + budgetDto.SeptemberForecastNew +
                            budgetDto.OctomberForecastNew + budgetDto.NovemberForecastNew + budgetDto.DecemberForecastNew + budgetDto.JanuaryForecastNew + budgetDto.FebruaryForecastNew + budgetDto.MarchForecastNew;

                _context.Update(budgetForecast);
            }


            if (budgetDto.Id > 0)
            {
                budgetBase = _context.Set<Model.BudgetBase>().Where(a => a.Id == budgetDto.Id).Single();
                budgetBase.Total = budgetDto.AugustForecastNew + budgetDto.MayForecastNew + budgetDto.JuneForecastNew + budgetDto.JulyForecastNew + budgetDto.AugustForecastNew + budgetDto.SeptemberForecastNew +
                            budgetDto.OctomberForecastNew + budgetDto.NovemberForecastNew + budgetDto.DecemberForecastNew + budgetDto.JanuaryForecastNew + budgetDto.FebruaryForecastNew + budgetDto.MarchForecastNew;
                budgetBase.ValueIni = budgetDto.AugustForecastNew + budgetDto.MayForecastNew + budgetDto.JuneForecastNew + budgetDto.JulyForecastNew + budgetDto.AugustForecastNew + budgetDto.SeptemberForecastNew +
                                budgetDto.OctomberForecastNew + budgetDto.NovemberForecastNew + budgetDto.DecemberForecastNew + budgetDto.JanuaryForecastNew + budgetDto.FebruaryForecastNew + budgetDto.MarchForecastNew;

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
				AccMonthId = budgetBase.AccMonthId,
				AccSystemId = null,
                BudgetManagerId = budgetManager.Id,
                BudgetTypeId = budgetForecast.BudgetTypeId,
                Document = document,
                //AccountIdInitial = budgetDto.AccountId,
                //AccountIdFinal = budgetDto.AccountId,
                //AdministrationIdInitial = budgetDto.AdministrationId,
                //AdministrationIdFinal = budgetDto.AdministrationId,
                BudgetBase = budgetBase,
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

            return budgetBase.Id;
        }

    }
}
