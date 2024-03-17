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
    public class BudgetMonthBasesRepository : Repository<Model.BudgetMonthBase>, IBudgetMonthBasesRepository
    {

        public BudgetMonthBasesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.BudgetBase.Code.Contains(filter)); })
        {
           
        }

        public IEnumerable<Model.BudgetMonthBaseDetail> GetBuget(BudgetFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal)
        {
            IQueryable<Model.BudgetMonthBase> budgetQuery = null;
            IQueryable<BudgetMonthBaseDetail> query = null;

            budgetQuery = _context.BudgetMonthBases.AsNoTracking().AsQueryable();
			int? accMonthId = _context.Set<Model.AccMonth>().AsNoTracking().Where(a => a.IsActive == true).Select(a => a.Id).SingleOrDefault();
			budgetFilter.AccMonthId = 60;

			if (budgetFilter.Filter != "" && budgetFilter.Filter != null) budgetQuery = budgetQuery.Where(a => (a.BudgetBase.Name.Contains(budgetFilter.Filter) || a.BudgetBase.Code.Contains(budgetFilter.Filter)));


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
                    case "BudgetMonthBase":
                        if (property.Length > 0) budgetQuery = budgetQuery.Include(property);
                        break;
                    default:
                        break;
                }
            }

            query = budgetQuery.Select(budget => new BudgetMonthBaseDetail { BudgetMonthBase = budget });

			//if ((budgetFilter.CompanyIds != null) && (budgetFilter.CompanyIds.Count > 0))
			//{
			//    query = query.Where(a => budgetFilter.CompanyIds.Contains(a.BudgetMonthBase.CompanyId));
			//}

			//if ((budgetFilter.EmployeeIds != null) && (budgetFilter.EmployeeIds.Count > 0))
			//{
			//    query = query.Where(a => budgetFilter.EmployeeIds.Contains(a.BudgetMonthBase.EmployeeId));
			//}


			if ((budgetFilter.AccMonthId != null) && (budgetFilter.AccMonthId > 0))
			{
				query = query.Where(a => a.BudgetMonthBase.AccMonthId == budgetFilter.AccMonthId);
			}
			else
			{
				query = query.Where(a => a.BudgetMonthBase.AccMonthId == accMonthId);
			}

			if ((budgetFilter.ProjectIds != null) && (budgetFilter.ProjectIds.Count > 0))
            {
                query = query.Where(ExpressionHelper.GetInListPredicate<Model.BudgetMonthBaseDetail, int?>((id) => { return a => a.BudgetMonthBase.BudgetBase.ProjectId == id; }, budgetFilter.ProjectIds));
            }

            query = query.Where(a => a.BudgetMonthBase.IsDeleted == false && a.BudgetMonthBase.BudgetBase.IsDeleted == false && a.BudgetMonthBase.IsFirst == true && a.BudgetMonthBase.BudgetType.Code == "V1");

            //query = query.GroupBy(item => item.Budget.ProjectId)
            //        .Select(group => new BudgetDetail()
            //        {
            //            Budget = query.First().Budget,
            //              //= group.Key,
            //              //Orders = group.ToList()
            //          })
            //        .AsQueryable();


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

    }
}
